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
    public bool visited = false, locked = false; //indicano se la stanza è stata già visitata e se è sigillata (nemici presenti)

	//liste relative alle tile della stanza (utilizzati per le illuminazioni)
	public List<GameObject> roomTiles = null;
    public List<GameObject>[] passageTiles = null;
    public List<GameObject> passageLeftTiles = null;
    public List<GameObject> passageRightTiles = null;
    public List<GameObject> passageUpTiles = null;
    public List<GameObject> passageDownTiles = null;

    public List<Vector2> spawnPoints = null; //lista delle posizioni di spawn dei nemici
    public List<Vector2> freePositions = null; //lista delle posizioni di spawn delle ricompense

    public GameObject doorSpriteUp, doorSpriteDown, doorSpriteLeft, doorSpriteRight; //gameObjects delle porte della stanza
    public GameObject actualMapSprite, visitedMapSprite, actualBossMapSprite, actualShopMapSprite, unknownMapSprite; //gameObjects delle sprite per le stanze nella minimappa
    public List<GameObject> enemies = null; //lista dei nemici della stanza
    public List<GameObject> toSort = null; //lista dei gameObject di cui riordinare il sortingOrder nello spriteRenderer

    public int[,] obsLayout; //layout degli ostacoli per la stanza
    public int enemyCounter, enemyNumber; //numero attuale e iniziale dei nemici
    public int enemyWaves; //numero di ondate di nemici

    public bool hasGenReward = false; //indica se è stata già generata una ricompensa per la stanza 


    public Room(Vector2Int _gridPos)
    {
        passageLeftTiles = new List<GameObject>();
        passageRightTiles = new List<GameObject>();
        passageUpTiles = new List<GameObject>();
        passageDownTiles = new List<GameObject>();
        roomTiles = new List<GameObject>();
        spawnPoints = new List<Vector2>();
        freePositions = new List<Vector2>();
        enemies = new List<GameObject>();
        toSort = new List<GameObject>();
        gridPos = _gridPos; //la posizione nella scena della stanza
    }

}