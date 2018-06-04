using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    LevelGenerator lvlGen;
    public int roomSizeX, roomSizeY;
    private int passSizeX = 3, passSizeY = 5;
    private BoxCollider2D wallCollider;
    public GameObject tileToRend;
    public GameObject player;
    public Room[,] map;

    public void DrawMap()
    {
        lvlGen = new LevelGenerator(roomSizeX, roomSizeY);
        map = lvlGen.Rooms;
        wallCollider = tileToRend.GetComponent<BoxCollider2D>();
        foreach (Room room in map) //per ogni stanza nella griglia delle stanze
        {
            if (room != null) //salto le posizioni inoccupate della griglia
            {
                DrawRoom(room);
                DrawWalls(room);
                LinkRooms(room);
            }
        }
    }

    public void InstantiatePlayer()
    {
        foreach (Room room in map)
        {
            if (room != null && room.startRoom)
            {
                Instantiate(player, new Vector2(room.gridPos.x + (float)(roomSizeX / 2), room.gridPos.y + (float)(roomSizeY / 2)), Quaternion.identity);
                break;
            }
        }
    }

    void DrawRoom(Room room)
    {
        Vector2 drawPos = room.gridPos;
        for (int i = 0; i < roomSizeY; i++)
        {
            for (int j = 0; j < roomSizeX; j++)
            {
                wallCollider.enabled = false;
                TileSpriteSelector mapper = Object.Instantiate(tileToRend, drawPos, Quaternion.identity).GetComponent<TileSpriteSelector>();
                //mi permette di impostare le tile relative al pavimento
                mapper.floor = true;
                if (i == 0 && j == (roomSizeX / 2) && room.doorBot)
                {
                    mapper.doorDown = true;
                }
                else if (i == (roomSizeY - 1) && j == (roomSizeX / 2) && room.doorTop)
                {
                    mapper.doorUp = true;
                }
                else if (i == (roomSizeY / 2) && j == 0 && room.doorLeft)
                {
                    mapper.doorLeft = true;
                }
                else if (i == (roomSizeY / 2) && j == (roomSizeX - 1) && room.doorRight)
                {
                    mapper.doorRight = true;
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
                    wallCollider.enabled = true;

                    //se è la stanza del boss
                    if (room.bossRoom && i == (roomSizeY + 1) && j == (roomSizeX / 2) + 1)
                    {
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
                    TileSpriteSelector mapper = Object.Instantiate(tileToRend, drawPos, Quaternion.identity).GetComponent<TileSpriteSelector>();

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
    void LinkRooms(Room room)
    {
        Vector2 drawPos;
        //disegno il passaggio a destra
        if (room.doorRight)
        {
            drawPos = new Vector2(room.gridPos.x + roomSizeX, room.gridPos.y + (roomSizeY / 2) - 1);
            for (int i = 0; i < passSizeY; i++)
            {
                for (int j = 0; j < passSizeX; j++)
                {
                    if (i == 3)
                    {
                        continue;
                    }

                    if (i != 1)
                    {
                        wallCollider.enabled = true;
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
                    }

                    TileSpriteSelector mapper = Object.Instantiate(tileToRend, drawPos, Quaternion.identity).GetComponent<TileSpriteSelector>();

                    mapper.passageHor = true;

                    if (i == 0)
                    {

                        mapper.wall = true;
                        mapper.down = true;
                        mapper.up = false;
                    }
                    else if (i == passSizeY - 1)
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
                    else if (j == passSizeX - 1)
                    {
                        mapper.right = true;
                        mapper.left = false;
                    }
                    else
                    {
                        mapper.right = false;
                        mapper.left = false;
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
            for (int i = 0; i < passSizeY; i++)
            {
                for (int j = 0; j < passSizeX; j++)
                {
                    if (i == 1 && j != 1)
                    {
                        drawPos.x++;
                        continue;
                    }

                    if (j != 1)
                    {
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
                        wallCollider.enabled = false;
                    }

                    //TileSpriteSelector mapper = Object.Instantiate(tileToRend, drawPos, Quaternion.identity).GetComponent<TileSpriteSelector>();
                    TileSpriteSelector mapper = Object.Instantiate(tileToRend, drawPos, Quaternion.identity).GetComponent<TileSpriteSelector>();
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
                    else if (i == passSizeY - 1)
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
                    else if (j == passSizeX - 1)
                    {
                        mapper.right = true;
                        mapper.left = false;
                    }
                    else
                    {
                        mapper.left = false;
                        mapper.right = false;
                    }
                    drawPos.x++;
                }
                drawPos.x = room.gridPos.x + (roomSizeX / 2) - 1;
                drawPos.y++;
            }
        }
    }
}
