using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{

    public Vector2 gridPos;
    public bool doorTop, doorBot, doorLeft, doorRight; //indica la presenza di porte nella stanza 
    public int roomDimX, roomDimY;

    public Room(Vector2 _gridPos, int _roomDimX, int _roomDimY)
    {
        gridPos = _gridPos;
        roomDimX = _roomDimX;
        roomDimY = _roomDimY;
    }
}