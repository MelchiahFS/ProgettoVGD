using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomFactory : MonoBehaviour
{
    private int numberOfRooms;
    private Room room;
    public int dimX, dimY;
    public GameObject tileToRend;
    private Vector2 drawPos;

    private void Start()
    {
        room = new Room(new Vector2(0, 0), dimX, dimY); //stanza di prova, la dimensione viene impostata da interfaccia unity
        drawPos = room.gridPos;
        for (int i = 0; i < room.roomDimY; i++)
        {
            for (int j = 0; j < room.roomDimX ; j++)
            {
                TileSpriteSelector mapper = Object.Instantiate(tileToRend, drawPos, Quaternion.identity).GetComponent<TileSpriteSelector>();
                if (i == 0)
                {
                    mapper.up = false;
                    mapper.down = true;
                }
                else if (i == room.roomDimY - 1)
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
                else if (j == room.roomDimX - 1)
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
}
