using Assets.Logic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public int TileSize;
    public int HorizontalTiles;
    public int VerticalTiles;
    public GameObject firePrefab;

    private const float WaitTimeBetweenSteps = 0.1f;
    private GameObject player;
    private GameObject[] chicken;
    private GameObject[] strawBales;
    private IList<GameObject> fires;
    private GameObject exit;
    private Level level;

    private readonly IDictionary<GameEntity, GameObject> entityMapping = new Dictionary<GameEntity, GameObject>();

    void Start()
    {
        player = GameObject.FindWithTag(Level.PlayerName);
        chicken = GameObject.FindGameObjectsWithTag(Level.ChickenName);
        strawBales = GameObject.FindGameObjectsWithTag(Level.StrawBaleName);
        fires = new List<GameObject>();
        exit = GameObject.FindWithTag(Level.ExitName);

        level = new Level(HorizontalTiles, VerticalTiles);
        PlaceEntities(player);
        PlaceEntities(chicken);
        PlaceEntities(strawBales);
        PlaceEntities(exit);

        level.PrintTiles();
    }

    void Update()
    {
        // TODO ensure that previous "level.Next"/Update is done
        var direction = GetMoveDirection();

        if (direction != null)
        {
            StartCoroutine(DoLevelStep(direction));
        }
    }

    private IEnumerator DoLevelStep(Position direction)
    {
        var steps = level.Next(direction);

        foreach (var step in steps)
        {
            if (!step.HasPlayerMoved)
            {
                yield break;
            }
            VisualizeLevelChanges(step);
            yield return new WaitForSeconds(WaitTimeBetweenSteps);
        }
    }

    private void VisualizeLevelChanges(LevelStepResult stepResult)
    {
        for (int x = 0; x < level.xSize; x++)
        {
            for (int y = 0; y < level.ySize; y++)
            {
                var entity = level.GetEntity(x, y);

                if (!entity.IsEmpty())
                {
                    if (entity.IsBurning && entity.Name != Level.PlayerName)
                    {
                        ShowFlames(entity);
                    }
                    MoveGameObjectOfEntity(entity);
                    ChangeChickenColor(entity);
                }
            }
        }
        RemoveGameObjects(stepResult);
        level.PrintTiles();
    }

    private void RemoveGameObjects(LevelStepResult stepResult)
    {
        foreach (var removed in stepResult.RemovedEntities)
        {
            var unityObject = entityMapping[removed];
            Destroy(unityObject);
        }
    }

    private void ChangeChickenColor(GameEntity entity)
    {
        if (entity.Name == Level.ChickenName)
        {
            var chicken = entityMapping[entity];
            var sprite = chicken.GetComponentInChildren<SpriteRenderer>();
            if (entity.RoundsNextToFire >= 1)
            {
                sprite.color = new Color(1, 0.78f, 0.63f);
            }
            else
            {
                sprite.color = Color.white;
            }
        }
    }

    private Position GetMoveDirection()
    {
        var moveUp = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        var moveLeft = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        var moveDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        var moveRight = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        Position direction = null;

        if (moveUp)
        {
            direction = new Position(0, 1);
        }
        else if (moveLeft)
        {
            direction = new Position(-1, 0);
        }
        else if (moveDown)
        {
            direction = new Position(0, -1);
        }
        else if (moveRight)
        {
            direction = new Position(1, 0);
        }
        return direction;
    }

    private void MoveGameObjectOfEntity(GameEntity entity)
    {
        var newX = ConvertIndexToTransformPosition(entity.Position.X);
        var newY = ConvertIndexToTransformPosition(entity.Position.Y);
        var unityObject = entityMapping[entity];
        unityObject.transform.position = new Vector2(newX, newY); // TODO animate movement
    }

    private void ShowFlames(GameEntity entity)
    {
        var position = new Vector3
        {
            x = ConvertIndexToTransformPosition(entity.Position.X),
            y = ConvertIndexToTransformPosition(entity.Position.Y)
        };
        var hasFireYet = fires.Any(f => f.transform.position == position);
        if (!hasFireYet)
        {
            var newUnityObject = Instantiate(firePrefab, position, Quaternion.identity);
            fires.Add(newUnityObject);
        }
    }

    private void PlaceEntities(params GameObject[] unityObjects)
    {
        foreach (var unityObject in unityObjects)
        {
            int xIndex = ConvertTransformPositionToIndex(unityObject.transform.position.x);
            int yIndex = ConvertTransformPositionToIndex(unityObject.transform.position.y);
            var gameEntity = new GameEntity() { Name = unityObject.tag };
            level.PlaceEntity(xIndex, yIndex, gameEntity);
            entityMapping.Add(gameEntity, unityObject);
        }
    }

    private int ConvertTransformPositionToIndex(float position)
    {
        return (int)(position / TileSize);
    }

    private int ConvertIndexToTransformPosition(int index)
    {
        return index * TileSize;
    }
}
