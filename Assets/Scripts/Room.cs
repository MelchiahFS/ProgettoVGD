using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{

    public Vector2Int gridPos; //la posizione della prima tile della stanza (in basso a sinistra)
    public bool bossRoom = false, shopRoom = false, startRoom = false;
    public bool doorTop, doorBot, doorLeft, doorRight; //indica la presenza di porte nella stanza 

    public Room(Vector2Int _gridPos)
    {
        gridPos = _gridPos;
    }
}