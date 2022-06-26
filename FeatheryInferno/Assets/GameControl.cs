using Assets.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public int TileSize;
    public int HorizontalTiles;
    public int VerticalTiles;

    private GameObject player;
    private GameObject[] chicken;
    private GameObject[] strawBales;
    private GameObject exit;
    private Level level;

    private readonly IDictionary<GameEntity, GameObject> entityMapping = new Dictionary<GameEntity, GameObject>();

    void Start()
    {
        player = GameObject.FindWithTag(Level.PlayerName);
        chicken = GameObject.FindGameObjectsWithTag(Level.ChickenName);
        strawBales = GameObject.FindGameObjectsWithTag(Level.StrawBaleName);
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
        // TODO ensure that previous "level.Next" is done
        var up = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        var left = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
        var down = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        var right = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        var hasMoved = false;

        if (up)
        {
            hasMoved = level.Next(new Position(0, 1));
        }
        else if (left)
        {
            hasMoved = level.Next(new Position(-1, 0));
        }
        else if (down)
        {
            hasMoved = level.Next(new Position(0, -1));
        }
        else if (right)
        {
            hasMoved = level.Next(new Position(1, 0));
        }

        if (hasMoved)
        {
            foreach (var m in entityMapping)
            {
                var entity = m.Key;
                var unityObject = m.Value;

                if (entity.Name == Level.PlayerName)
                {
                    var newX = ConvertIndexToTransformPosition(entity.Position.X);
                    var newY = ConvertIndexToTransformPosition(entity.Position.Y);
                    unityObject.transform.position = new Vector2(newX, newY); // TODO animate movement
                }
            }
            level.PrintTiles();
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
