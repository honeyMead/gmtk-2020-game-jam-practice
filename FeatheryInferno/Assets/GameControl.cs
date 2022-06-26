using Assets.Logic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public int TileSize;
    public int HorizontalTiles;
    public int VerticalTiles;

    private const string PlayerTag = "Player";
    private const string ChickenTag = "Chicken";
    private const string TileTag = "Tile";
    private const string ExitTag = "Finish";

    private GameObject player;
    private GameObject[] chicken;
    private GameObject[] tiles;
    private GameObject exit;
    private Level level;

    void Start()
    {
        player = GameObject.FindWithTag(PlayerTag);
        chicken = GameObject.FindGameObjectsWithTag(ChickenTag);
        tiles = GameObject.FindGameObjectsWithTag(TileTag);
        exit = GameObject.FindWithTag(ExitTag);

        level = new Level(HorizontalTiles, VerticalTiles);
    }

    void Update()
    {

    }
}
