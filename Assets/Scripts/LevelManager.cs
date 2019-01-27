using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class LevelManager : MonoBehaviour {

    public LevelGenerator lvlGen = null;
    private static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
    public int roomSizeX, roomSizeY;
    private int horPassSizeX, horPassSizeY, verPassSizeX, verPassSizeY;
    private BoxCollider2D wallCollider;
    private BoxCollider2D doorCollider;
    private BoxCollider2D obsCollider;
    private BoxCollider2D groundTrigger;
    public GameObject tileToRend;
    public GameObject playerPrefab;
    public List<GameObject> enemyPrefabs;
    public Room[,] map;
    public Vector2 mapSize;
    private Vector2Int actualPos;
    public float dim = 0.5f, full = 1, off = 0, alpha;
    private Color c;

    float fadeTime = 0.8f;

    public int roomNumber;
    public GameObject exit, hideExit;

    private MiniMapController minimap;

    //Si occupa di disegnare la mappa di gioco
    public void DrawMap()
    {
        lvlGen = new LevelGenerator(roomSizeX, roomSizeY);
        map = lvlGen.Rooms;
        mapSize = lvlGen.GetMapSize();
        roomNumber = lvlGen.GetRoomNumber();
        ActualPos = new Vector2Int((int)mapSize.x / 2, (int)mapSize.y / 2);
        minimap = GetComponent<MiniMapController>();

        for (int i = 0; i < mapSize.x; i++) //per ogni stanza nella griglia delle stanze
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (map[i,j] != null) //salto le posizioni inoccupate della griglia
                {

                    DrawRoom(map[i, j]);
                    DrawWalls(map[i, j]);
                    LinkRooms(i,j);
                    DrawDoors(i, j);
                    DrawObstacles(i, j);
                    CreateGridGraphs(map[i,j]);
                    DrawMinimapSprites(map[i,j]);
                    SetEnemyNumber(i, j);
                }
            }
        }
        //esegue una scansione di tutti i GridGraph
        AstarPath.active.Scan();
    }

    //Istanzia gli ostacoli nelle stanze
    void DrawObstacles(int x, int y)
    {
        //ObstacleLayout.GetLayoutZero()
        Room room = map[x, y];
        Vector2 drawPos = room.gridPos;

        if (room.startRoom)
            room.obsLayout = ObstacleLayout.GetLayoutZero();
        else if (room.shopRoom)
            room.obsLayout = ObstacleLayout.GetShopLayout();
        else if (room.bossRoom)
            room.obsLayout = ObstacleLayout.GetBossLayout();
        else
            room.obsLayout = ObstacleLayout.GetRandomLayout();
        //if (room.shopRoom)
        //    room.obsLayout = ObstacleLayout.GetShopLayout();
        //else
        //    room.obsLayout = ObstacleLayout.GetLayoutZero();

        for (int i = 0; i < roomSizeY; i++)
        {
            for (int j = 0; j < roomSizeX; j++)
            {                
                
                if (room.obsLayout[i, j] == 1)
                {
                    GameObject obsSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
                    obsSprite.tag = "Obstacle";

                    //imposto questo layer per il sistema di pathfinding
                    obsSprite.layer = LayerMask.NameToLayer("ObstacleLayer");

                    TileSpriteSelector mapper = obsSprite.GetComponent<TileSpriteSelector>();
                    SpriteRenderer rend = obsSprite.GetComponent<SpriteRenderer>();
                    rend.sortingLayerName = "Obstacles";
                    rend.sprite = mapper.obstacles;
                    obsSprite.AddComponent(typeof(BoxCollider2D));
                    room.roomTiles.Add(obsSprite);
                }
                else
                {
                    //aggiorno la lista delle posizioni prive di ostacoli
                    if (room.shopRoom)
                    {
                        //se la stanza è lo shop, le posizioni libere saranno i punti in cui spawnare gli item da comprare
                        if (room.obsLayout[i, j] == 2)
                            room.freePositions.Add(drawPos);

                    }
                    else
                        room.freePositions.Add(drawPos);  
                }
                drawPos.x++;
            }
            drawPos.x = room.gridPos.x;
            drawPos.y++;
        }
    }

    //Istanzia i nemici nelle stanze
    public void InstantiateEnemies(int x, int y)
    {
        Room room = map[x, y];
        Vector2 drawPos = room.gridPos;
        int enemyPosition, enemyType;

        for (int i = 0; i < roomSizeY; i++)
        {
            for (int j = 0; j < roomSizeX; j++)
            {
                //aggiorno la lista degli spawn point
                if (room.obsLayout[i, j] == 2)
                {
                    room.spawnPoints.Add(drawPos);
                }
                
                drawPos.x++;
            }
            drawPos.x = room.gridPos.x;
            drawPos.y++;
        }

        for (int c = 0; c < room.enemyCounter; c++)
        {
            //scelgo casualmente uno tra gli spawn point disponibili (sceglie da 0 a X - 1)
            enemyPosition = rnd.Next(0, room.spawnPoints.Count);

            //scelgo casualmente il tipo di nemico da istanziare
            enemyType = rnd.Next(0, enemyPrefabs.Count);

            GameObject enemy = Instantiate(enemyPrefabs[enemyType], room.spawnPoints[enemyPosition], Quaternion.identity) as GameObject;
            enemy.name = enemyPrefabs[enemyType].name;

            //imposto il sorting layer dei nemici
            SpriteRenderer enemyRenderer = enemy.GetComponent<SpriteRenderer>();
            enemyRenderer.sortingLayerName = "Characters";

            //imposto i nemici come trasparenti, per poi fare un effetto di fade-in quando verranno attivati 
            Color color = enemyRenderer.color;
            color.a = 0;
            enemyRenderer.color = color;

            //aggiungo il nemico alla lista dei nemici per stanza attuale
            room.enemies.Add(enemy);

            //aggiungo il nemico alla lista degli oggetti attivi da ordinare per sorting order per stanza attuale
            room.toSort.Add(enemy);

            //rimuovo lo spawn point dalla lista di quelli disponinili per la stanza attuale
            room.spawnPoints.RemoveAt(enemyPosition);

            StartCoroutine(GameManager.manager.lvlManager.FadeIn(enemy.GetComponent<SpriteRenderer>(), fadeTime));
        }
        room.enemyNumber = room.enemyCounter;
        room.enemyWaves--;
        
    }

    //Istanzia il player nella stanza di partenza
    public Room InstantiatePlayer()
    {
        for (int i = 0; i < mapSize.x; i++) 
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (map[i, j] != null && map[i, j].startRoom) 
                {

                    LightUpRoom(map[i, j], true);
                    GameObject player = Instantiate(playerPrefab, new Vector2(map[i, j].gridPos.x + (float)(roomSizeX / 2), map[i, j].gridPos.y + (float)(roomSizeY / 2)), Quaternion.identity) as GameObject;
                    player.name = playerPrefab.name;
                    playerPrefab.GetComponent<SpriteRenderer>().sortingLayerName = "Characters";
                    minimap.SetEnterRoom(map[i, j]);
                    return map[i, j];
                }
            }
        }
        return null;
    }

    //Disegna il pavimento delle stanze
    void DrawRoom(Room room)
    {
        Vector2 drawPos = room.gridPos;
        for (int i = 0; i < roomSizeY; i++)
        {
            for (int j = 0; j < roomSizeX; j++)
            {
                GameObject roomTile = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;

                roomTile.tag = "Floor";
                roomTile.layer = LayerMask.NameToLayer("Ground");
                TileSpriteSelector mapper = roomTile.GetComponent<TileSpriteSelector>();
                SpriteRenderer rend = roomTile.GetComponent<SpriteRenderer>();
                rend.sortingLayerName = "Ground";
                room.roomTiles.Add(roomTile);

                if (i == 0 && j == (roomSizeX / 2) && room.doorBot)
                {
                    roomTile.tag = "innerDoorDown";
                    roomTile.AddComponent(typeof(TileTrigger));
                    groundTrigger = roomTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
                    groundTrigger.size = new Vector2(1, 1);
                    groundTrigger.isTrigger = true;

                    rend.sprite = mapper.doorFloorDown;
                }
                else if (i == (roomSizeY - 1) && j == (roomSizeX / 2) && room.doorTop)
                {
                    roomTile.tag = "innerDoorUp";
                    roomTile.AddComponent(typeof(TileTrigger));
                    groundTrigger = roomTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
                    groundTrigger.size = new Vector2(1, 1);
                    groundTrigger.isTrigger = true;

                    rend.sprite = mapper.doorFloorUp;
                }
                else if (i == (roomSizeY / 2) && j == 0 && room.doorLeft)
                {
                    roomTile.tag = "innerDoorLeft";
                    roomTile.AddComponent(typeof(TileTrigger));
                    groundTrigger = roomTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
                    groundTrigger.size = new Vector2(1, 1);
                    groundTrigger.isTrigger = true;
                    
                    rend.sprite = mapper.doorFloorLeft;
                }
                else if (i == (roomSizeY / 2) && j == (roomSizeX - 1) && room.doorRight)
                {
                    roomTile.tag = "innerDoorRight";
                    roomTile.AddComponent(typeof(TileTrigger));
                    groundTrigger = roomTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
                    groundTrigger.size = new Vector2(1, 1);
                    groundTrigger.isTrigger = true;

                    rend.sprite = mapper.doorFloorRight;
                }
                else if (i == 0)
                {
                    if (j == 0)
                        rend.sprite = mapper.outerDownLeftCorner;
                    else if (j == roomSizeX - 1)
                        rend.sprite = mapper.outerDownRightCorner;
                    else
                        rend.sprite = mapper.downFloor;
                }
                else if (i == roomSizeY - 1)
                {
                    if (j == 0)
                        rend.sprite = mapper.outerUpLeftCorner;
                    else if (j == roomSizeX - 1)
                        rend.sprite = mapper.outerUpRightCorner;
                    else
                        rend.sprite = mapper.upFloor;
                }
                else
                {
                    if (j == 0)
                        rend.sprite = mapper.leftFloor;
                    else if (j == roomSizeX - 1)
                        rend.sprite = mapper.rightFloor;
                    else
                        rend.sprite = mapper.floorTile;
                }
                drawPos.x++;

            }
            drawPos.x = room.gridPos.x;
            drawPos.y++;
        }
    }

    //Disegna i muri delle stanze e ne imposta i collider
    void DrawWalls(Room room)
    {
        Vector2 drawPos = new Vector2(room.gridPos.x - 1, room.gridPos.y - 1);
        for (int i = 0; i < roomSizeY + 4; i++) //altezza stanza + muro basso (1) + muro alto (3)
        {
            for (int j = 0; j < roomSizeX + 2; j++) //lunghezza stanza + muro sinistra (1) + muro destra (1)
            {
                //preparo lo spazio vuoto tra le stanze da collegare (del collegamento si occuperà un'altro metodo)
                if (
                    (((i == 0 && room.doorBot) || ((i == roomSizeY + 1 || i == roomSizeY + 3) && room.doorTop)) && j >= (roomSizeX / 2) && j <= (roomSizeX / 2) + 2) ||
                    (((j == 0 && room.doorLeft) || (j == roomSizeX + 1 && room.doorRight)) && i >= (roomSizeY / 2) && i <= (roomSizeY / 2) + 4)
                   )
                {
                    drawPos.x++;
                    continue;
                }

                //se l'indice è relativo a uno dei muri istanzio la tile
                if (i == 0 || j == 0 || i == (roomSizeY + 3) || i == (roomSizeY + 1) || (j == roomSizeX + 1))
                {
                    

                    GameObject wallTile = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
                    wallTile.tag = "Wall";
                    wallTile.layer = LayerMask.NameToLayer("Walls");

                    //rendo tangibile il muro
                    wallCollider = wallTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

                    TileSpriteSelector mapper = wallTile.GetComponent<TileSpriteSelector>();
                    SpriteRenderer rend = wallTile.GetComponent<SpriteRenderer>();
                    rend.sortingLayerName = "Ground";
                    room.roomTiles.Add(wallTile);

                    //se è la stanza del boss creo l'uscita
                    if (room.bossRoom && i == (roomSizeY + 1) && j == (roomSizeX / 2) + 1)
                    {
                        wallTile.tag = "Exit";
                        rend.sprite = mapper.stairs;
                        wallCollider.size = new Vector2(1, 1);
                        wallCollider.offset = new Vector2(0, 1);
                        exit = wallTile;
                        room.roomTiles.Add(wallTile);



                        hideExit = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
                        hideExit.tag = "Wall";
                        hideExit.layer = LayerMask.NameToLayer("InnerWalls");

                        BoxCollider2D hideExitCollider = hideExit.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
                        hideExitCollider.size = new Vector2(1, 2);
                        hideExitCollider.offset = new Vector2(0, 0.5f);

                        TileSpriteSelector HEmapper = hideExit.GetComponent<TileSpriteSelector>();
                        SpriteRenderer HErend = hideExit.GetComponent<SpriteRenderer>();
                        HErend.sortingLayerName = "Ground";
                        HErend.sprite = HEmapper.innerWallCenter;
                        room.roomTiles.Add(hideExit);
                    }
                    //se il muro è quello frontale
                    else if (i == roomSizeY + 1)
                    {
                        if (j > 0 && j <= roomSizeX)
                        {
                            if (j > 1 && j < roomSizeX)
                            {
                                wallTile.layer = LayerMask.NameToLayer("InnerWalls");
                                rend.sprite = mapper.innerWallCenter;
                            }                                
                            else if (j == 1)
                            {
                                wallTile.layer = LayerMask.NameToLayer("InnerWalls");
                                rend.sprite = mapper.innerWallLeft;
                            }                                
                            else if (j == roomSizeX)
                            {
                                wallTile.layer = LayerMask.NameToLayer("InnerWalls");
                                rend.sprite = mapper.innerWallRight;
                            }    

                            wallCollider.size = new Vector2(1, 2);
                            wallCollider.offset = new Vector2(0, 0.5f);
                        }
                        else
                        {
                            if (j == 0)
                                rend.sprite = mapper.leftWall;
                            else if (j == roomSizeX + 1)
                                rend.sprite = mapper.rightWall;
                        
                            wallCollider.size = new Vector2(1, 1);
                        }
                    }
                        
                    else if (i == roomSizeY + 3)
                    {
                        if (j > 0 && j <= roomSizeX)
                            rend.sprite = mapper.upWall;
                        else if (j == 0)
                            rend.sprite = mapper.upLeftCorner;
                        else if (j == roomSizeX + 1)
                            rend.sprite = mapper.upRightCorner;

                        if (j >= 0 && j <= roomSizeX + 1)
                        {
                            wallCollider.size = new Vector2(1, 1);
                        }
                    }
                    else if (i == 0)
                    {
                        if (j > 0 && j <= roomSizeX)
                            rend.sprite = mapper.downWall;
                        else if (j == 0)
                            rend.sprite = mapper.downLeftCorner;
                        else if (j == roomSizeX + 1)
                            rend.sprite = mapper.downRightCorner;

                        if (j >= 0 && j <= roomSizeX + 1)
                        {
                            wallCollider.size = new Vector2(1, 1);
                        }
                    }
                    else if (j == 0)
                    {
                        rend.sprite = mapper.leftWall;
                        wallCollider.size = new Vector2(1, 1);
                    }
                    else if (j == roomSizeX + 1)
                    {
                        rend.sprite = mapper.rightWall;
                        wallCollider.size = new Vector2(1, 1);
                    }                    
                }
                drawPos.x++;
            }
            drawPos.x = room.gridPos.x - 1;
            drawPos.y++;
        }
    }

    //Per ogni stanza della mappa, controlla se ha una porta a destra e sopra, e in caso affermativo disegna il passaggio
    void LinkRooms(int x, int y)
    {
        horPassSizeX = lvlGen.distRoomX;
        horPassSizeY = 5;
        verPassSizeY = lvlGen.distRoomY;
        verPassSizeX = 3;
        Room room = map[x, y];
        Vector2 drawPos;
        
        //disegno il passaggio a destra
        if (room.doorRight)
        {
            drawPos = new Vector2(room.gridPos.x + roomSizeX, room.gridPos.y + (roomSizeY / 2) - 1);
            for (int i = 0; i < horPassSizeY; i++)
            {
                for (int j = 0; j < horPassSizeX; j++)
                {
                    //la tile del muro di sopra è il doppio in altezza delle altre: per disegnarlo viene considerata solo la riga sotto (i == 2)
                    if (i == 3)
                    {
                        continue;
                    }

                    GameObject passTile = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;

                    TileSpriteSelector mapper = passTile.GetComponent<TileSpriteSelector>();
                    SpriteRenderer rend = passTile.GetComponent<SpriteRenderer>();

                    //aggiorno le liste delle tiles del passaggio delle due stanze che lo condividono
                    if (j == 0)
                        room.roomTiles.Add(passTile);
                    else if (j == horPassSizeX - 1)
                        map[x + 1, y].roomTiles.Add(passTile);
                    else
                    {
                        room.passageRightTiles.Add(passTile);
                        map[x + 1, y].passageLeftTiles.Add(passTile);
                    }


                    if (i != 1)
                    {
                        wallCollider = passTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

                        passTile.tag = "Wall";
                        passTile.layer = LayerMask.NameToLayer("Walls");

                        if (i == 0)
                        {
                            if (j > 0 && j < horPassSizeX - 1)
                                rend.sprite = mapper.downWall;
                            else if (j == 0)
                                rend.sprite = mapper.innerLeftUpWallCorner;
                            else if (j == horPassSizeX - 1)
                                rend.sprite = mapper.innerRightUpWallCorner;

                            wallCollider.size = new Vector2(1, 1);
                        }
                        else if (i == 2)
                        {
                            if (j > 0 && j < horPassSizeX - 1)
                                rend.sprite = mapper.innerWallCenter;
                            else if (j == 0)
                                rend.sprite = mapper.innerWallLeft;
                            else if (j == horPassSizeX - 1)
                                rend.sprite = mapper.innerWallRight;

                            wallCollider.size = new Vector2(1, 2);
                            wallCollider.offset = new Vector2(0, 0.5f);
                        }
                        else if (i == horPassSizeY - 1)
                        {
                            if (j > 0 && j < horPassSizeX - 1)
                                rend.sprite = mapper.upWall;
                            else if (j == 0)
                                rend.sprite = mapper.innerLeftDownWallCorner;
                            else if (j == horPassSizeX - 1)
                                rend.sprite = mapper.innerRightDownWallCorner;

                            wallCollider.size = new Vector2(1, 1);
                        }
                    }
                    //imposto i trigger per gli ingressi al corridoio
                    else
                    {
                        
                        passTile.layer = LayerMask.NameToLayer("Ground");
                        rend.sprite = mapper.horizontalPass;

                        if (j == 0)
                        { 
                            //trigger per la porta di sinistra del corridoio
                            passTile.tag = "outerDoorRight";
                            passTile.AddComponent(typeof(TileTrigger));
                            wallCollider = passTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
                            wallCollider.isTrigger = true;
                            wallCollider.size = new Vector2(1, 1);
                            wallCollider.offset = new Vector2(1.1f, 0);
                        }
                        else if (j == horPassSizeX - 1)
                        {
                            //trigger per la porta di destra del corridoio
                            passTile.tag = "outerDoorLeft";
                            passTile.AddComponent(typeof(TileTrigger));
                            wallCollider = passTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;
                            wallCollider.isTrigger = true;
                            wallCollider.size = new Vector2(1, 1);
                            wallCollider.offset = new Vector2(-1.1f, 0);
                        }
                        else
                        {
                            passTile.tag = "Floor";
                        }
                    }
                    rend.sortingLayerName = "Ground";
                    drawPos.x++;
                }
                drawPos.x = room.gridPos.x + roomSizeX;
                drawPos.y++;
            }
        }
        //disegno il passaggio sopra
        if (room.doorTop)
        {
            drawPos = new Vector2(room.gridPos.x + (roomSizeX / 2) - 1, room.gridPos.y + roomSizeY);
            for (int i = 0; i < verPassSizeY; i++)
            {
                for (int j = 0; j < verPassSizeX; j++)
                {
                    //il muro ha altezza doppia, perciò per disegnarlo considero solo la prima riga (i == 0)
                    if (i == 1 && j != 1)
                    {
                        drawPos.x++;
                        continue;
                    }

                    GameObject passTile = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;

                    TileSpriteSelector mapper = passTile.GetComponent<TileSpriteSelector>();
                    SpriteRenderer rend = passTile.GetComponent<SpriteRenderer>();

                    //aggiorno le liste delle tiles del passaggio delle due stanze che lo condividono
                    if (i == 0 || (i == 2 && j != 1))
                        room.roomTiles.Add(passTile);
                    else if (i == verPassSizeY - 1)
                        map[x, y + 1].roomTiles.Add(passTile);
                    else
                    {
                        room.passageUpTiles.Add(passTile);
                        map[x, y + 1].passageDownTiles.Add(passTile);
                    }

                    if (j != 1)
                    {
                        wallCollider = passTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

                        passTile.tag = "Wall";
                        passTile.layer = LayerMask.NameToLayer("Walls");
                        if (j == 0)
                        {
                            if (i > 2 && i < verPassSizeY - 1)
                            {
                                rend.sprite = mapper.leftWall;
                                wallCollider.size = new Vector2(1, 1);
                            }
                            else if (i == 0)
                            {
                                passTile.layer = LayerMask.NameToLayer("InnerWalls");
                                rend.sprite = mapper.innerWallRight;
                                wallCollider.size = new Vector2(1, 2);
                                wallCollider.offset = new Vector2(0, 0.5f);
                            }
                            else if (i == 2)
                            {
                                rend.sprite = mapper.innerRightDownWallCorner;
                                wallCollider.size = new Vector2(1, 1);
                            }
                            else if (i == verPassSizeY - 1)
                            {
                                rend.sprite = mapper.innerRightUpWallCorner;
                                wallCollider.size = new Vector2(1, 1);
                            }

                        }
                        else
                        {
                            if (i > 2 && i < verPassSizeY - 1)
                            {
                                rend.sprite = mapper.rightWall;
                                wallCollider.size = new Vector2(1, 1);
                            }
                            else if (i == 0)
                            {
                                passTile.layer = LayerMask.NameToLayer("InnerWalls");
                                rend.sprite = mapper.innerWallLeft;
                                wallCollider.size = new Vector2(1, 2);
                                wallCollider.offset = new Vector2(0, 0.5f);
                            }
                            else if (i == 2)
                            {
                                rend.sprite = mapper.innerLeftDownWallCorner;
                                wallCollider.size = new Vector2(1, 1);
                            }
                            else if (i == verPassSizeY - 1)
                            {
                                rend.sprite = mapper.innerLeftUpWallCorner;
                                wallCollider.size = new Vector2(1, 1);
                            }
                        }    
                    }
                    else
                    {
                        passTile.layer = LayerMask.NameToLayer("Ground");
                        rend.sprite = mapper.verticalPass;

                        if (i == 2)
                        {
                            //trigger per la porta di sotto del corridoio
                            wallCollider = passTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

                            wallCollider.size = new Vector2(1, 1);
                            wallCollider.offset = new Vector2(0, -0.5f);
                            passTile.tag = "outerDoorUp";
                            passTile.AddComponent(typeof(TileTrigger));
                            wallCollider.isTrigger = true;
                        }
                        else if (i == 3)
                        {
                            //trigger per la porta di sopra del corridoio
                            wallCollider = passTile.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

                            wallCollider.size = new Vector2(1, 1);
                            wallCollider.offset = new Vector2(0, 0.5f);
                            passTile.tag = "outerDoorDown";
                            passTile.AddComponent(typeof(TileTrigger));
                            wallCollider.isTrigger = true;
                        }
                        else
                        {
                            passTile.tag = "Floor";
                        }
                        
                    }
                    rend.sortingLayerName = "Ground";
                    drawPos.x++;
                }
                drawPos.x = room.gridPos.x + (roomSizeX / 2) - 1;
                drawPos.y++;
            }
        }

    }

    //Disegna le porte delle stanze e ne imposta i collider
    void DrawDoors(int x, int y)
    {
        Vector2 drawPos;
        Room room = map[x, y];
        if (room.doorTop)
        {
            drawPos = new Vector2(room.gridPos.x + roomSizeX / 2, room.gridPos.y + roomSizeY);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doorSpriteUp = doorSprite;
            doorSprite.tag = "DoorUp";

            doorSprite.layer = LayerMask.NameToLayer("Doors");

            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            SpriteRenderer rend = doorSprite.GetComponent<SpriteRenderer>();
            rend.sortingLayerName = "Doors";
            rend.sprite = mapper.closedDoorUp;
            rend.sortingOrder = 1;
            
            doorCollider = doorSprite.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

            doorCollider.size = new Vector2(1, 2);
            doorCollider.offset = new Vector2(0, 0.5f);
        }
        if (room.doorBot)
        {
            drawPos = new Vector2(room.gridPos.x + roomSizeX / 2, room.gridPos.y - 2);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doorSpriteDown = doorSprite;
            doorSprite.tag = "DoorDown";

            doorSprite.layer = LayerMask.NameToLayer("Doors");

            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            SpriteRenderer rend = doorSprite.GetComponent<SpriteRenderer>();
            rend.sortingLayerName = "Doors";
            rend.sprite = mapper.closedDoorDown;
            rend.sortingOrder = 1;

            doorCollider = doorSprite.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

            doorCollider.size = new Vector2(1, 2);
            doorCollider.offset = new Vector2(0, 0.5f);
        }
        if (room.doorLeft)
        {
            drawPos = new Vector2(room.gridPos.x - 1, room.gridPos.y + roomSizeY / 2 + 1);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doorSpriteLeft = doorSprite;
            doorSprite.tag = "DoorLeft";

            doorSprite.layer = LayerMask.NameToLayer("Doors");

            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            SpriteRenderer rend = doorSprite.GetComponent<SpriteRenderer>();
            rend.sortingLayerName = "Doors";
            rend.sprite = mapper.closedDoorLeft;
            rend.sortingOrder = 1;

            doorCollider = doorSprite.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

            doorCollider.size = new Vector2(0.01f, 1);
            doorCollider.offset = new Vector2(-0.5f, -1);
        }
        if (room.doorRight)
        {
            drawPos = new Vector2(room.gridPos.x + roomSizeX, room.gridPos.y + roomSizeY / 2 + 1);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doorSpriteRight = doorSprite;
            doorSprite.tag = "DoorRight";

            doorSprite.layer = LayerMask.NameToLayer("Doors");

            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            SpriteRenderer rend = doorSprite.GetComponent<SpriteRenderer>();
            rend.sortingLayerName = "Doors";
            rend.sprite = mapper.closedDoorRight;
            rend.sortingOrder = 1;

            doorCollider = doorSprite.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

            doorCollider.size = new Vector2(0.01f, 1);
            doorCollider.offset = new Vector2(0.5f, -1);
        }
    }

    //Illumina le stanze al loro accesso
    public void LightUpRoom(Room actualRoom, bool light)
    {
        if (light)
        {
            alpha = full;
        }
        else
        {
            alpha = dim;
        }
        foreach (GameObject g in actualRoom.roomTiles)
        {
            if (!ReferenceEquals(g,exit)) //se i due riferimenti rappresentano la stessa istanza
            {
                SpriteRenderer s = g.GetComponent<SpriteRenderer>();
                StartCoroutine(FadeIn(s, fadeTime));
            }
            
        }
        if (actualRoom.doorSpriteUp != null)
        {
            SpriteRenderer s = actualRoom.doorSpriteUp.GetComponent<SpriteRenderer>();
            StartCoroutine(FadeIn(s, fadeTime));
        }
        if (actualRoom.doorSpriteDown != null)
        {
            SpriteRenderer s = actualRoom.doorSpriteDown.GetComponent<SpriteRenderer>();
            StartCoroutine(FadeIn(s, fadeTime));
        }
        if (actualRoom.doorSpriteLeft != null)
        {
            SpriteRenderer s = actualRoom.doorSpriteLeft.GetComponent<SpriteRenderer>();
            StartCoroutine(FadeIn(s, fadeTime));
        }
        if (actualRoom.doorSpriteRight != null)
        {
            SpriteRenderer s = actualRoom.doorSpriteRight.GetComponent<SpriteRenderer>();
            StartCoroutine(FadeIn(s, fadeTime));
        }

        actualRoom.visited = true;
    }

    //Illumina i passaggi tra le stanze al loro accesso
    public void LightUpPassage(GameObject door, List<GameObject> passage, bool light)
    {
        SpriteRenderer s = null;
        if (light)
        {
            alpha = full;
        }
        else
        {
            alpha = dim;
        }
        foreach (GameObject g in passage)
        {
            s = g.GetComponent<SpriteRenderer>();
            StartCoroutine(FadeIn(s, fadeTime));
        }
        s = door.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeIn(s, fadeTime));

    }

    //Istanzia le sprite usate dalla minimappa per rappresentare la mappa di gioco
    public void DrawMinimapSprites(Room room)
    {
        int posX = room.gridPos.x + roomSizeX / 2;
        int posY = room.gridPos.y + roomSizeY / 2;
        
        if (room.bossRoom)
        {
            room.actualBossMapSprite = Instantiate(minimap.actualBossRoom, new Vector2(posX, posY), Quaternion.identity);
            room.visitedBossMapSprite = Instantiate(minimap.visitedBossRoom, new Vector2(posX, posY), Quaternion.identity);

            room.actualBossMapSprite.SetActive(false);
            room.visitedBossMapSprite.SetActive(false);
        }
        else if (room.shopRoom)
        {
            room.actualShopMapSprite = Instantiate(minimap.actualShopRoom, new Vector2(posX, posY), Quaternion.identity);
            room.visitedShopMapSprite = Instantiate(minimap.visitedShopRoom, new Vector2(posX, posY), Quaternion.identity);

            room.actualShopMapSprite.SetActive(false);
            room.visitedShopMapSprite.SetActive(false);
        }
        else
        {
            room.actualMapSprite = Instantiate(minimap.actualRoom, new Vector2(posX, posY), Quaternion.identity);
            room.visitedMapSprite = Instantiate(minimap.visitedRoom, new Vector2(posX, posY), Quaternion.identity);
            room.unknownMapSprite = Instantiate(minimap.unknownRoom, new Vector2(posX, posY), Quaternion.identity);

            room.unknownMapSprite.SetActive(false);
            room.actualMapSprite.SetActive(false);
            room.visitedMapSprite.SetActive(false);
        }
    }

    //Tiene la posizione attuale nella mappa
    public Vector2Int ActualPos
    {
        get
        {
            return actualPos;
        }
        set
        {
            actualPos = value;
        }
    }

    //Crea i GridGraph necessari alla IA dei nemici
    private void CreateGridGraphs(Room room)
    {
        AstarData data = AstarPath.active.data;
        // This creates a Grid Graph
        GridGraph gg = data.AddGraph(typeof(GridGraph)) as GridGraph;

        // Setup a grid graph with some values
        int width = (roomSizeX + 2) * 4;
        int depth = (roomSizeY + 4) * 4;
        float nodeSize = 0.25f;

        gg.center = new Vector3(room.gridPos.x + roomSizeX / 2, room.gridPos.y + roomSizeY / 2 + 1, 0);
        gg.rotation = new Vector3(-90, 0, 0);
        gg.collision.diameter = 1f;
        gg.collision.mask = LayerMask.GetMask("ObstacleLayer");
        gg.collision.use2D = true;
        gg.cutCorners = false;

        // Updates internal size from the above values
        gg.SetDimensions(width, depth, nodeSize);
    }

    //Crea un effetto di FadeIn; usato da LightUpRoom e LightUpPassage
    public IEnumerator FadeIn(SpriteRenderer s, float fadeTime)
    {
        Color c = s.color;
        float rate = 1 / fadeTime;
        while (c.a < 1)
        {
            //è necessario controllare che non sia null altrimenti Unity dà errore
            if (s != null)
            {
                c.a += Time.deltaTime * rate;
                s.color = c;
                yield return 0;
            }
            else
                yield break;
        }
        yield break;
    }

    //Crea un effetto di FadeIn; usato da LightUpRoom e LightUpPassage
    public IEnumerator FadeOff(SpriteRenderer s, float fadeTime)
    {
        Color c = s.color;
        float rate = 1 / fadeTime;
        while (c.a > 0)
        {
            //è necessario controllare che non sia null altrimenti Unity dà errore
            if (s != null)
            {
                c.a -= Time.deltaTime * rate;
                s.color = c;
                yield return 0;
            }
            else
                yield break;
        }
        yield break;
    }

    private void SetEnemyNumber(int x, int y)
    {
        Room room = map[x, y];
        if (room.shopRoom || room.startRoom)
        {
            room.enemyCounter = 0;
            room.enemyWaves = 0;
        }
        else if (room.bossRoom)
        {
            //room.enemyCounter = rnd.Next(4, 8);
            //room.enemyWaves = 3;
            room.enemyCounter = 0;
            room.enemyWaves = 0;
        }
        else
        {
			room.enemyCounter = rnd.Next(3, 6);
			room.enemyWaves = 1;
			//room.enemyCounter = 0;
			//room.enemyWaves = 0;
		}
    }

    public void ShowExit()
    {
        StartCoroutine(FadeOff(hideExit.GetComponent<SpriteRenderer>(), 1));
        hideExit.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(FadeIn(exit.GetComponent<SpriteRenderer>(), 1));
    }
}
