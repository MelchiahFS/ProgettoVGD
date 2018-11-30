using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    public Vector2Int gridPos; //la posizione della prima tile della stanza (in basso a sinistra)
    public bool bossRoom = false, shopRoom = false, startRoom = false;
    public bool doorTop, doorBot, doorLeft, doorRight; //indica la presenza di porte nella stanza 
    public bool openUp = false, openDown = false, openLeft = false, openRight = false; //indica se la porta è aperta o no
    public bool visited = false, locked = false;
    public List<GameObject> roomTiles = null;
    public List<GameObject>[] passageTiles = null;
    public List<GameObject> passageLeftTiles = null;
    public List<GameObject> passageRightTiles = null;
    public List<GameObject> passageUpTiles = null;
    public List<GameObject> passageDownTiles = null;
    public List<Vector2> spawnPoints = null;
    public List<Vector2> freePositions = null;
    public GameObject doorSpriteUp, doorSpriteDown, doorSpriteLeft, doorSpriteRight;
    public GameObject actualMapSprite, visitedMapSprite, actualBossMapSprite, visitedBossMapSprite, actualShopMapSprite, visitedShopMapSprite, unknownMapSprite;
    public List<GameObject> enemies = null;
    public List<GameObject> toSort = null;

    public int[,] obsLayout;
    public int obsNumber;
    public int enemyCounter;



    public Room(Vector2Int _gridPos)
    {

        //obsLayout = ObstacleLayout.GetRandomLayout();
        obsLayout = ObstacleLayout.GetLayoutZero();

        passageLeftTiles = new List<GameObject>();
        passageRightTiles = new List<GameObject>();
        passageUpTiles = new List<GameObject>();
        passageDownTiles = new List<GameObject>();
        roomTiles = new List<GameObject>();
        spawnPoints = new List<Vector2>();
        freePositions = new List<Vector2>();
        enemies = new List<GameObject>();
        toSort = new List<GameObject>();
        gridPos = _gridPos;
        enemyCounter = 2;
    }

}