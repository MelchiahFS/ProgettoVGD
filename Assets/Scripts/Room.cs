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
    public bool visited = false;
    public List<GameObject> roomTiles = null;
    public List<GameObject>[] passageTiles = null;

    public enum Passage { up, down, left, right};

    public Room(Vector2Int _gridPos)
    {
        passageTiles = new List<GameObject>[4];
        roomTiles = new List<GameObject>();
        gridPos = _gridPos;
    }

    public void AddToList(Passage p, GameObject g)
    {
        if (passageTiles[(int)p] == null)
            passageTiles[(int)p] = new List<GameObject>();
        passageTiles[(int)p].Add(g);
    }

    public List<GameObject> GetPassage(Passage p)
    {
        if (passageTiles[(int)p] != null)
            return passageTiles[(int)p];
        else
            return null;
    }
}