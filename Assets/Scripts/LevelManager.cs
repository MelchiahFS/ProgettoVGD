using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    LevelGenerator lvlGen = null;
    public int roomSizeX, roomSizeY;
    private int horPassSizeX, horPassSizeY, verPassSizeX, verPassSizeY;
    private BoxCollider2D wallCollider;
    private BoxCollider2D doorCollider;
    public GameObject tileToRend;
    public GameObject player;
    public Room[,] map;
    public Vector2 mapSize;
    public Vector2 actualPos;

    public void DrawMap()
    {
        lvlGen = new LevelGenerator(roomSizeX, roomSizeY);
        map = lvlGen.Rooms;
        mapSize = lvlGen.GetMapSize();


        for (int i = 0; i < mapSize.x; i++) //per ogni stanza nella griglia delle stanze
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if (map[i,j] != null) //salto le posizioni inoccupate della griglia
                {

                    DrawRoom(map[i, j]);
                    DrawDoors(map[i, j]);
                    DrawWalls(map[i, j]);
                    LinkRooms(new Vector2Int(i,j));
                }
            }
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

                    Instantiate(player, new Vector2(map[i, j].gridPos.x + (float)(roomSizeX / 2), map[i, j].gridPos.y + (float)(roomSizeY / 2)), Quaternion.identity);
                    actualPos = new Vector2(i, j);
                    return map[i, j];
                }
            }
        }
        return null;
    }

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

    void DrawDoors(Room room)
    {
        Vector2 drawPos;
        if (room.doorTop)
        {
            drawPos = new Vector2(room.gridPos.x + roomSizeX / 2, room.gridPos.y + roomSizeY);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doors.Add(doorSprite);
            doorSprite.tag = "DoorUp";
            doorSprite.layer = LayerMask.NameToLayer("Level");
            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            doorSprite.GetComponent<SpriteRenderer>().sprite = mapper.closedDoorUp;
            doorSprite.GetComponent<SpriteRenderer>().sortingOrder = 1;
            doorCollider = doorSprite.GetComponent<BoxCollider2D>();
            doorCollider.size = new Vector2(1, 2);
            doorCollider.offset = new Vector2(0, 0.5f);
            doorCollider.enabled = true;

        }
        if (room.doorBot)
        {
            drawPos = new Vector2(room.gridPos.x + roomSizeX / 2, room.gridPos.y - 2);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doors.Add(doorSprite);
            doorSprite.tag = "DoorDown";
            doorSprite.layer = LayerMask.NameToLayer("Level");
            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            doorSprite.GetComponent<SpriteRenderer>().sprite = mapper.closedDoorDown;
            doorSprite.GetComponent<SpriteRenderer>().sortingOrder = 1;
            doorCollider = doorSprite.GetComponent<BoxCollider2D>();
            doorCollider.size = new Vector2(1, 2);
            doorCollider.offset = new Vector2(0, 0.5f);
            doorCollider.enabled = true;
        }
        if (room.doorLeft)
        {
            drawPos = new Vector2(room.gridPos.x, room.gridPos.y + roomSizeY / 2 + 1);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doors.Add(doorSprite);
            doorSprite.tag = "DoorLeft";
            doorSprite.layer = LayerMask.NameToLayer("Level");
            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            doorSprite.GetComponent<SpriteRenderer>().sprite = mapper.closedDoorLeft;
            doorSprite.GetComponent<SpriteRenderer>().sortingOrder = 1;
            doorCollider = doorSprite.GetComponent<BoxCollider2D>();
            doorCollider.size = new Vector2(0.01f, 1);
            doorCollider.offset = new Vector2(-0.5f, -1);
            doorCollider.enabled = true;
        }
        if (room.doorRight)
        {
            drawPos = new Vector2(room.gridPos.x + roomSizeX - 1, room.gridPos.y + roomSizeY / 2 + 1);
            GameObject doorSprite = Instantiate(tileToRend, drawPos, Quaternion.identity) as GameObject;
            room.doors.Add(doorSprite);
            doorSprite.tag = "DoorRight";
            doorSprite.layer = LayerMask.NameToLayer("Level");
            TileSpriteSelector mapper = doorSprite.GetComponent<TileSpriteSelector>();
            doorSprite.GetComponent<SpriteRenderer>().sprite = mapper.closedDoorRight;
            doorSprite.GetComponent<SpriteRenderer>().sortingOrder = 1;
            doorCollider = doorSprite.GetComponent<BoxCollider2D>();
            doorCollider.size = new Vector2(0.01f, 1);
            doorCollider.offset = new Vector2(0.5f, -1);
            doorCollider.enabled = true;
        }
    }

    //disegna i muri delle stanze
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
    void LinkRooms(Vector2Int roomPos)
    {
        horPassSizeX = lvlGen.distRoomX;
        horPassSizeY = 5;
        verPassSizeY = lvlGen.distRoomY;
        verPassSizeX = 3;
        Room room = map[roomPos.x, roomPos.y];
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
                    room.AddToList(Room.Passage.right, passTile);
                    map[roomPos.x + 1, roomPos.y].AddToList(Room.Passage.left, passTile);
                    
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
                        doorTrigger.size = new Vector2(2, 1);
                        doorTrigger.offset = new Vector2(1, 0);
                        doorTrigger.isTrigger = true;
                        doorTrigger.enabled = true;
                    }
                    if (i == 1 && j == horPassSizeX - 1)
                    {
                        //trigger per la porta di destra del corridoio
                        BoxCollider2D doorTrigger = passTile.GetComponent<BoxCollider2D>();
                        passTile.tag = "outerDoorLeft";
                        doorTrigger.size = new Vector2(2, 1);
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
                    room.AddToList(Room.Passage.up, passTile);
                    map[roomPos.x, roomPos.y + 1].AddToList(Room.Passage.left, passTile);


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
                        doorTrigger.size = new Vector2(1, 2.8f);
                        doorTrigger.offset = new Vector2(0, -1);
                        passTile.tag = "outerDoorUp";
                        doorTrigger.isTrigger = true;
                        doorTrigger.enabled = true;
                    }
                    if (i == 3 && j == 1)
                    {
                        //trigger per la porta di sopra del corridoio
                        BoxCollider2D doorTrigger = passTile.GetComponent<BoxCollider2D>();
                        doorTrigger.size = new Vector2(1, 2.8f);
                        doorTrigger.offset = new Vector2(0, 1);
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

    public Vector2 ActualPos
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
