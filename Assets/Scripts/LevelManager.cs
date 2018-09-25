using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public LevelGenerator lvlGen = null;
    private static System.Random rnd = new System.Random();
    public int roomSizeX, roomSizeY;
    private int horPassSizeX, horPassSizeY, verPassSizeX, verPassSizeY;
    private BoxCollider2D wallCollider;
    private BoxCollider2D doorCollider;
    private BoxCollider2D obsCollider;
    public GameObject tileToRend;
    public GameObject player;
    public GameObject enemy;
    public Room[,] map;
    public Vector2 mapSize;
    private Vector2Int actualPos;
    public float dim = 0.5f, full = 1, off = 0, alpha;
    private Color c;

    private MiniMapController minimap;


    public void DrawMap()
    {
        lvlGen = new LevelGenerator(roomSizeX, roomSizeY);
        map = lvlGen.Rooms;
        mapSize = lvlGen.GetMapSize();
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
                    if (!map[i,j].bossRoom && !map[i,j].shopRoom && !map[i,j].startRoom)
                    {
                        DrawObstacles(i, j);
                        InstantiateEnemies(i, j);
                    }

                    DrawMinimapSprites(map[i,j]);
                }
            }
        }
    }

    void DrawObstacles(int x, int y)
    {
        Room room = map[x, y];
        Vector2 drawPos = room.gridPos;
        for (int i = 0; i < roomSizeY; i++)
        {
            for (int j = 0; j < roomSizeX; j++)
            {
                if (room.obsLayout[i, j] == 1)
                {
                    GameObject obsSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
                    obsSprite.tag = "Obstacle";
                    obsSprite.layer = LayerMask.NameToLayer("ObstacleLayer");
                    TileSpriteSelector mapper = obsSprite.GetComponent<TileSpriteSelector>();
                    SpriteRenderer rend = obsSprite.GetComponent<SpriteRenderer>();
                    rend.sprite = mapper.obstacles;
                    rend.sortingOrder = 1;
                    obsCollider = obsSprite.GetComponent<BoxCollider2D>();
                    obsCollider.enabled = true;
                    room.roomTiles.Add(obsSprite);
                }
                drawPos.x++;
            }
            drawPos.x = room.gridPos.x;
            drawPos.y++;
        }
    }

    void InstantiateEnemies(int x, int y)
    {
        Room room = map[x, y];
        Vector2 drawPos = room.gridPos;
        int enemyPosition;
        for (int i = 0; i < roomSizeY; i++)
        {
            for (int j = 0; j < roomSizeX; j++)
            {
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
            enemyPosition = rnd.Next(0, room.spawnPoints.Count - 1);
            Instantiate(enemy, room.spawnPoints[enemyPosition], Quaternion.identity);
            room.spawnPoints.RemoveAt(enemyPosition);
        }
    }

    public Room InstantiatePlayer()
    {
        for (int i = 0; i < mapSize.x; i++) 
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (map[i, j] != null && map[i, j].startRoom) 
                {

                    LightUpRoom(map[i, j], true);
                    Instantiate(player, new Vector2(map[i, j].gridPos.x + (float)(roomSizeX / 2), map[i, j].gridPos.y + (float)(roomSizeY / 2)), Quaternion.identity);
                    minimap.SetEnterRoom(map[i, j]);
                    return map[i, j];
                }
            }
        }
        return null;
    }

    //disegna il pavimento delle stanze
    void DrawRoom(Room room)
    {
        Vector2 drawPos = room.gridPos;
        for (int i = 0; i < roomSizeY; i++)
        {
            for (int j = 0; j < roomSizeX; j++)
            {
                wallCollider = tileToRend.GetComponent<BoxCollider2D>();
                wallCollider.enabled = false;
                wallCollider.gameObject.tag = "Floor";
                
                GameObject roomTile = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
                TileSpriteSelector mapper = roomTile.GetComponent<TileSpriteSelector>();
                room.roomTiles.Add(roomTile);

                //mi permette di impostare le tile relative al pavimento
                mapper.floor = true;
                if (i == 0 && j == (roomSizeX / 2) && room.doorBot)
                {
                    mapper.doorDown = true;
                    BoxCollider2D doorTrigger = roomTile.GetComponent<BoxCollider2D>();
                    roomTile.tag = "innerDoorDown";
                    doorTrigger.isTrigger = true;
                    doorTrigger.enabled = true;
                }
                else if (i == (roomSizeY - 1) && j == (roomSizeX / 2) && room.doorTop)
                {
                    mapper.doorUp = true;
                    BoxCollider2D doorTrigger = roomTile.GetComponent<BoxCollider2D>();
                    roomTile.tag = "innerDoorUp";
                    doorTrigger.isTrigger = true;
                    doorTrigger.enabled = true;
                }
                else if (i == (roomSizeY / 2) && j == 0 && room.doorLeft)
                {
                    mapper.doorLeft = true;
                    BoxCollider2D doorTrigger = roomTile.GetComponent<BoxCollider2D>();
                    roomTile.tag = "innerDoorLeft";
                    doorTrigger.isTrigger = true;
                    doorTrigger.enabled = true;
                }
                else if (i == (roomSizeY / 2) && j == (roomSizeX - 1) && room.doorRight)
                {
                    mapper.doorRight = true;
                    BoxCollider2D doorTrigger = roomTile.GetComponent<BoxCollider2D>();
                    roomTile.tag = "innerDoorRight";
                    doorTrigger.isTrigger = true;
                    doorTrigger.enabled = true;
                }
                else if (i == 0)
                {
                    mapper.up = false;
                    mapper.down = true;
                }
                else if (i == roomSizeY - 1)
                {
                    mapper.up = true;
                    mapper.down = false;
                }
                else
                {
                    mapper.up = false;
                    mapper.down = false;
                }

                if (j == 0)
                {
                    mapper.left = true;
                    mapper.right = false;
                }
                else if (j == roomSizeX - 1)
                {
                    mapper.left = false;
                    mapper.right = true;
                }
                else
                {
                    mapper.left = false;
                    mapper.right = false;
                }
                drawPos.x++;
            }
            drawPos.x = room.gridPos.x;
            drawPos.y++;
        }

    }

    //disegna i muri delle stanze e ne imposta i collider
    void DrawWalls(Room room)
    {
        Vector2 drawPos = new Vector2(room.gridPos.x - 1, room.gridPos.y - 1);
        for (int i = 0; i <= roomSizeY + 3; i++)
        {
            for (int j = 0; j <= roomSizeX + 1; j++)
            {
                //preparo lo spazio vuoto tra le stanze da collegare (del collegamento si occuperà un'altro metodo
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
                    //rendo tangibile il muro
                    wallCollider = tileToRend.GetComponent<BoxCollider2D>();
                    wallCollider.enabled = true;
                    wallCollider.gameObject.tag = "Wall";
                    //se è la stanza del boss
                    if (room.bossRoom && i == (roomSizeY + 1) && j == (roomSizeX / 2) + 1)
                    {
                        wallCollider.gameObject.tag = "Exit";
                        wallCollider.size = new Vector2(1, 1);
                        wallCollider.offset = new Vector2(0, 1);
                    }
                    //se il muro è quello frontale
                    else if (i == (roomSizeY + 1) && j > 0 && j <= roomSizeX)
                    {
                        wallCollider.size = new Vector2(1, 2);
                        wallCollider.offset = new Vector2(0, 0.5f);
                    }
                    //se è il muro esterno/laterale
                    else
                    {
                        wallCollider.size = new Vector2(1, 1);
                        wallCollider.offset = new Vector2(0, 0);
                    }

                    GameObject roomTile = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
                    TileSpriteSelector mapper = roomTile.GetComponent<TileSpriteSelector>();
                    room.roomTiles.Add(roomTile);

                    //mi permette di impostare le tile relative al pavimento
                    mapper.wall = true;

                    if (i == 0)
                    {
                        mapper.down = true;
                        mapper.up = false;
                    }
                    else if (i == (roomSizeY + 3))
                    {
                        mapper.up = true;
                        mapper.down = false;
                    }
                    else
                    {
                        mapper.up = false;
                        mapper.down = false;
                    }

                    if (j == 0)
                    {
                        mapper.left = true;
                        mapper.right = false;
                    }
                    else if ((j == 1 && i == roomSizeY + 1) || (i == roomSizeY + 1 && j == ((roomSizeX + 1) / 2) + 1 && room.doorTop))
                    {
                        mapper.innerWall = true;
                        mapper.left = true;
                        mapper.right = false;

                    }
                    else if ((j == roomSizeX && i == roomSizeY + 1) || (i == roomSizeY + 1 && j == (roomSizeX / 2) && room.doorTop))
                    {
                        mapper.innerWall = true;
                        mapper.right = true;
                        mapper.left = false;
                    }
                    else if (room.bossRoom && i == roomSizeY + 1 && j == ((roomSizeX + 1) / 2))
                    {
                        mapper.innerWall = true;
                        mapper.exit = true;
                    }
                    else if (j < roomSizeX && i == roomSizeY + 1)
                    {
                        mapper.innerWall = true;
                        mapper.left = false;
                        mapper.right = false;
                    }
                    else if (j == roomSizeX + 1)
                    {
                        mapper.right = true;
                        mapper.left = false;
                    }
                    else
                    {
                        mapper.left = false;
                        mapper.right = false;
                    }
                }
                drawPos.x++;
            }
            drawPos.x = room.gridPos.x - 1;
            drawPos.y++;
        }
    }

    //per ogni stanza della griglia delle stanze, controllo se ha una porta a destra e sopra
    //e in caso affermativo disegno il passaggio
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
                    if (i == 3)
                    {
                        continue;
                    }

                    wallCollider = tileToRend.GetComponent<BoxCollider2D>();
                    if (i != 1)
                    {
                        wallCollider.enabled = true;
                        wallCollider.gameObject.tag = "Wall";
                        if (i == 2)
                        {
                            wallCollider.size = new Vector2(1, 2);
                            wallCollider.offset = new Vector2(0, 0.5f);
                        }
                        else
                        {
                            wallCollider.size = new Vector2(1, 1);
                            wallCollider.offset = new Vector2(0, 0);
                        }
                    }
                    else
                    {
                        wallCollider.enabled = false;
                        wallCollider.gameObject.tag = "Floor";
                    }

                    GameObject passTile = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
                    TileSpriteSelector mapper = passTile.GetComponent<TileSpriteSelector>();
                    if (j == 0)
                        room.roomTiles.Add(passTile);
                    else if (j == horPassSizeX - 1)
                        map[x + 1, y].roomTiles.Add(passTile);
                    else
                    {
                        room.passageRightTiles.Add(passTile);
                        map[x + 1, y].passageLeftTiles.Add(passTile);
                    }
                        
                    
                    mapper.passageHor = true;

                    if (i == 0)
                    {

                        mapper.wall = true;
                        mapper.down = true;
                        mapper.up = false;
                    }
                    else if (i == horPassSizeY - 1)
                    {
                        mapper.wall = true;
                        mapper.up = true;
                        mapper.down = false;
                    }
                    else if (i == 2)
                    {
                        mapper.innerWall = true;
                    }
                    else
                    {
                        mapper.up = false;
                        mapper.down = false;
                    }

                    if (j == 0)
                    {
                        mapper.left = true;
                        mapper.right = false;
                    }
                    else if (j == horPassSizeX - 1)
                    {
                        mapper.right = true;
                        mapper.left = false;
                    }
                    else
                    {
                        mapper.right = false;
                        mapper.left = false;
                    }

                    if (i == 1 && j == 0)
                    {
                        //trigger per la porta di sinistra del corridoio
                        BoxCollider2D doorTrigger = passTile.GetComponent<BoxCollider2D>();
                        passTile.tag = "outerDoorRight";
                        doorTrigger.size = new Vector2(1.3f, 1);
                        doorTrigger.offset = new Vector2(1, 0);
                        doorTrigger.isTrigger = true;
                        doorTrigger.enabled = true;
                    }
                    if (i == 1 && j == horPassSizeX - 1)
                    {
                        //trigger per la porta di destra del corridoio
                        BoxCollider2D doorTrigger = passTile.GetComponent<BoxCollider2D>();
                        passTile.tag = "outerDoorLeft";
                        doorTrigger.size = new Vector2(1.3f, 1);
                        doorTrigger.offset = new Vector2(-1, 0);
                        doorTrigger.isTrigger = true;
                        doorTrigger.enabled = true;
                    }
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
                    if (i == 1 && j != 1)
                    {
                        drawPos.x++;
                        continue;
                    }

                    wallCollider = tileToRend.GetComponent<BoxCollider2D>();
                    if (j != 1)
                    {
                        wallCollider.gameObject.tag = "Wall";
                        if (i == 0)
                        {
                            wallCollider.size = new Vector2(1, 2);
                            wallCollider.offset = new Vector2(0, 0.5f);
                        }
                        else
                        {
                            wallCollider.size = new Vector2(1, 1);
                            wallCollider.offset = new Vector2(0, 0);
                        }
                        wallCollider.enabled = true;
                    }
                    else
                    {
                        wallCollider.gameObject.tag = "Floor";
                        wallCollider.enabled = false;
                    }

                    GameObject passTile = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
                    TileSpriteSelector mapper = passTile.GetComponent<TileSpriteSelector>();
                    if (i == 0 || (i == 2 && j != 1))
                    {
                        room.roomTiles.Add(passTile);
                    }
                    else if (i == verPassSizeY - 1)
                    {
                        map[x, y + 1].roomTiles.Add(passTile);
                    }
                    else
                    {
                        room.passageUpTiles.Add(passTile);
                        map[x, y + 1].passageDownTiles.Add(passTile);
                    }



                    mapper.passageVer = true;

                    if (j != 1)
                    {
                        if (i == 0)
                        {
                            mapper.innerWall = true;
                        }
                        else
                        {
                            mapper.wall = true;
                        }
                    }

                    if (i == 0 || i == 2)
                    {
                        mapper.down = true;
                        mapper.up = false;
                    }
                    else if (i == verPassSizeY - 1)
                    {
                        mapper.down = false;
                        mapper.up = true;
                    }
                    else
                    {
                        mapper.down = false;
                        mapper.up = false;
                    }

                    if (j == 0)
                    {
                        mapper.left = true;
                        mapper.right = false;
                    }
                    else if (j == verPassSizeX - 1)
                    {
                        mapper.right = true;
                        mapper.left = false;
                    }
                    else
                    {
                        mapper.left = false;
                        mapper.right = false;
                    }

                    if (i == 2 && j == 1)
                    {
                        //trigger per la porta di sotto del corridoio
                        BoxCollider2D doorTrigger = passTile.GetComponent<BoxCollider2D>();
                        doorTrigger.size = new Vector2(1, 1);
                        doorTrigger.offset = new Vector2(0, -0.5f);
                        passTile.tag = "outerDoorUp";
                        doorTrigger.isTrigger = true;
                        doorTrigger.enabled = true;
                    }
                    if (i == 3 && j == 1)
                    {
                        //trigger per la porta di sopra del corridoio
                        BoxCollider2D doorTrigger = passTile.GetComponent<BoxCollider2D>();
                        doorTrigger.size = new Vector2(1, 1);
                        doorTrigger.offset = new Vector2(0, 0.5f);
                        passTile.tag = "outerDoorDown";
                        doorTrigger.isTrigger = true;
                        doorTrigger.enabled = true;
                    }

                    drawPos.x++;
                }
                drawPos.x = room.gridPos.x + (roomSizeX / 2) - 1;
                drawPos.y++;
            }
        }
    }

    //disegna le porte delle stanze e ne imposta i collider
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
            doorSprite.layer = LayerMask.NameToLayer("Level");
            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            SpriteRenderer rend = doorSprite.GetComponent<SpriteRenderer>();
            rend.sprite = mapper.closedDoorUp;
            rend.sortingOrder = 1;
            doorCollider = doorSprite.GetComponent<BoxCollider2D>();
            doorCollider.size = new Vector2(1, 2);
            doorCollider.offset = new Vector2(0, 0.5f);
            doorCollider.enabled = true;
        }
        if (room.doorBot)
        {
            drawPos = new Vector2(room.gridPos.x + roomSizeX / 2, room.gridPos.y - 2);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doorSpriteDown = doorSprite;
            doorSprite.tag = "DoorDown";
            doorSprite.layer = LayerMask.NameToLayer("Level");
            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            SpriteRenderer rend = doorSprite.GetComponent<SpriteRenderer>();
            rend.sprite = mapper.closedDoorDown;
            rend.sortingOrder = 1;
            doorCollider = doorSprite.GetComponent<BoxCollider2D>();
            doorCollider.size = new Vector2(1, 2);
            doorCollider.offset = new Vector2(0, 0.5f);
            doorCollider.enabled = true;
        }
        if (room.doorLeft)
        {
            drawPos = new Vector2(room.gridPos.x - 1, room.gridPos.y + roomSizeY / 2 + 1);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doorSpriteLeft = doorSprite;
            doorSprite.tag = "DoorLeft";
            doorSprite.layer = LayerMask.NameToLayer("Level");
            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            SpriteRenderer rend = doorSprite.GetComponent<SpriteRenderer>();
            rend.sprite = mapper.closedDoorLeft;
            rend.sortingOrder = 1;
            doorCollider = doorSprite.GetComponent<BoxCollider2D>();
            doorCollider.size = new Vector2(0.01f, 1);
            doorCollider.offset = new Vector2(-0.5f, -1);
            doorCollider.enabled = true;
        }
        if (room.doorRight)
        {
            drawPos = new Vector2(room.gridPos.x + roomSizeX, room.gridPos.y + roomSizeY / 2 + 1);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doorSpriteRight = doorSprite;
            doorSprite.tag = "DoorRight";
            doorSprite.layer = LayerMask.NameToLayer("Level");
            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            SpriteRenderer rend = doorSprite.GetComponent<SpriteRenderer>();
            rend.sprite = mapper.closedDoorRight;
            rend.sortingOrder = 1;
            doorCollider = doorSprite.GetComponent<BoxCollider2D>();
            doorCollider.size = new Vector2(0.01f, 1);
            doorCollider.offset = new Vector2(0.5f, -1);
            doorCollider.enabled = true;
        }
    }

    //illumina le stanze
    public void LightUpRoom(Room actualRoom, bool light)
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
        foreach (GameObject g in actualRoom.roomTiles)
        {
            s = g.GetComponent<SpriteRenderer>();
            c = s.color;
            c.a = alpha;
            s.color = c;
        }
        if (actualRoom.doorSpriteUp != null)
        {
            s = actualRoom.doorSpriteUp.GetComponent<SpriteRenderer>();
            c = s.color;
            c.a = alpha;
            s.color = c;
        }
        if (actualRoom.doorSpriteDown != null)
        {
            s = actualRoom.doorSpriteDown.GetComponent<SpriteRenderer>();
            c = s.color;
            c.a = alpha;
            s.color = c;
        }
        if (actualRoom.doorSpriteLeft != null)
        {
            s = actualRoom.doorSpriteLeft.GetComponent<SpriteRenderer>();
            c = s.color;
            c.a = alpha;
            s.color = c;
        }
        if (actualRoom.doorSpriteRight != null)
        {
            s = actualRoom.doorSpriteRight.GetComponent<SpriteRenderer>();
            c = s.color;
            c.a = alpha;
            s.color = c;
        }

        actualRoom.visited = true;
        /*foreach (List<GameObject> l in actualRoom.passageTiles)
        {
            if (l != null)
            {
                foreach (GameObject g in l)
                {
                    SpriteRenderer s = g.GetComponent<SpriteRenderer>();
                    c = s.color;
                    c.a = full;
                    s.color = c;
                }
            }
        }*/

    }

    //illumina i passaggi tra le stanze
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
            c = s.color;
            c.a = alpha;
            s.color = c;
        }
        s = door.GetComponent<SpriteRenderer>();
        c = s.color;
        c.a = alpha;
        s.color = c;
    }

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
}
