using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{

    public Vector2Int gridPos;
    public bool doorTop, doorBot, doorLeft, doorRight; //indica la presenza di porte nella stanza 

    public Room(Vector2Int _gridPos)
    {
        gridPos = _gridPos;
    }
}