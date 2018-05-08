﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    Vector2Int worldSize;
    Room[,] rooms;
    List<Vector2Int> takenPositions = new List<Vector2Int>();
    int gridSizeX, gridSizeY;
    public int roomSizeX = 13, roomSizeY = 13;
    private int numberOfRooms;
    public GameObject tileToRend;

    // Use this for initialization
    void Start()
    {
        numberOfRooms = Random.Range(10, 30);

        //NOTA: la dimensione della griglia delle stanze sarà il doppio in x e y 
        //in modo che la prima stanza stia al centro della griglia
        worldSize = new Vector2Int(Random.Range(3, 6), Random.Range(3, 6)); 
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2))
        {
            //se il numero delle stanze eccede il numero delle celle nella griglia allora lo eguaglio a tale numero
            numberOfRooms = (worldSize.x * 2) * (worldSize.y * 2); 
        }
        gridSizeX = worldSize.x;
        gridSizeY = worldSize.y;
        CreateRooms();
        SetRoomDoors();
        DrawMap();
    }

    void CreateRooms()
    {
        //crea una matrice Room che rappresenta la griglia delle stanze
        rooms = new Room[gridSizeX * 2, gridSizeY * 2];
        //crea una stanza al centro della griglia
        rooms[gridSizeX, gridSizeY] = new Room(Vector2Int.zero);
        //inserisce nella lista delle posizioni visitate la posizione della stanza attuale
        takenPositions.Insert(0, Vector2Int.zero); 
        Vector2Int checkPos = Vector2Int.zero;

        float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.1f;

        for (int i = 0; i < numberOfRooms - 1; i++)
        {
            //restituisce un valore inizialmente piccolo, che cresce a ogni iterazione
            float randomPerc = ((float)i) / ((float)numberOfRooms - 1);
            //random compare è risultato di una interpolazione lineare usando il valore precedente
            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);

            checkPos = NewPosition();
            //se la nuova stanza avrà più di una stanza adiacente e ottengo un valore 
            //casuale maggiore del risultato dell'interpolazione...
            if (NumberOfNeighbors(checkPos, takenPositions) > 1 && Random.value > randomCompare)
            {
                int iterations = 0;
                do
                {
                    //cerco una nuova posizione per la nuova stanza
                    checkPos = SelectiveNewPosition();
                    iterations++;
                }//ripeto finché non trovo una stanza che abbia al massimo una stanza adiacente
                while (NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);
            }
            
            //NOTA: controllare corrispondenza griglia - posizione stanza effettiva
            //posiziono la stanza nella griglia delle stanze
            rooms[(checkPos.x / roomSizeX) + gridSizeX, (checkPos.y / roomSizeY) + gridSizeY] = new Room(checkPos);
            
            
            //inserisco la nuova stanza nella lista delle stanze visitate
            takenPositions.Insert(0, checkPos); 
        }
    }

    Vector2Int NewPosition()
    {
        int x = 0, y = 0;
        Vector2Int checkingPos = Vector2Int.zero;
        do
        {   //random.value restituisce un valore tra 0 e 1 compresi
            int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1)); 
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;
            bool upDown = (Random.value < 0.5f); 
            bool positive = (Random.value < 0.5f); 
            if (upDown) //determino se la nuova stanza andrà a destra o a sinistra di quella attuale
            {
                if (positive)
                    y += roomSizeY;
                else
                    y -= roomSizeY;
            }
            else //determino se sarà sopra o sotto di quella attuale
            {
                if (positive)
                    x += roomSizeX;
                else
                    x -= roomSizeX;
            }
            checkingPos = new Vector2Int(x, y);
        }//ripeto finché la nuova stanza non sarà dentro la griglia o la nuova posizione non sia diversa da una già occupata
        while (takenPositions.Contains(checkingPos) || (x / roomSizeX) >= gridSizeX || (x / roomSizeX) < -gridSizeX || (y / roomSizeY) >= gridSizeY || (y / roomSizeY) < -gridSizeY); 
        return checkingPos;
    }

    Vector2Int SelectiveNewPosition()
    {
        int index = 0, inc = 0;
        int x = 0, y = 0;
        Vector2Int checkingPos = Vector2Int.zero;
        do
        {
            inc = 0;
            do
            {
                index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
                inc++;
            }//controlla finché non trova una stanza che ha una sola stanza adiacente
            while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && inc < 100);
            x = (int)takenPositions[index].x;
            y = (int)takenPositions[index].y;
            bool upDown = (Random.value < 0.5f);
            bool positive = (Random.value < 0.5f);
            if (upDown)
            {
                if (positive)
                    y += roomSizeY;
                else
                    y -= roomSizeY;
            }
            else
            {
                if (positive)
                    x += roomSizeX;
                else
                    x -= roomSizeX;
            }
            //sceglie dove mettere la nuova stanza rispetto a quella attuale
            checkingPos = new Vector2Int(x, y); 
        }//ripeto finché la nuova stanza non sarà dentro la griglia o la nuova posizione non sia diversa da una già occupata
        while (takenPositions.Contains(checkingPos) || (x / roomSizeX) >= gridSizeX || (x / roomSizeX) < -gridSizeX || (y / roomSizeY) >= gridSizeY || (y / roomSizeY) < -gridSizeY);
        if (inc >= 100)
            print("Error: could not find position with only one neighbor");
        return checkingPos;
    }

    //controlla il numero di stanze adiacenti a quella corrente e restituisce il numero
    int NumberOfNeighbors(Vector2Int checkingPos, List<Vector2Int> usedPositions)
    {
        
        int ret = 0;
        if (usedPositions.Contains(new Vector2Int(checkingPos.x + roomSizeX, checkingPos.y)))
            ret++;
        if (usedPositions.Contains(new Vector2Int(checkingPos.x - roomSizeX, checkingPos.y)))
            ret++;
        if (usedPositions.Contains(new Vector2Int(checkingPos.x, checkingPos.y + roomSizeY)))
            ret++;
        if (usedPositions.Contains(new Vector2Int(checkingPos.x, checkingPos.y - roomSizeY)))
            ret++;
        return ret;
    }


    //NOTA: controllare il ciclo di controllo della griglia e l'effettiva corrispondenza con la posizione della stanza
    void SetRoomDoors()
    {
        for (int x = 0; x < (gridSizeX * 2); x++)
        {
            for (int y = 0; y < (gridSizeY * 2); y++)
            {
                if (rooms[x, y] == null)
                    continue;
                if (y - 1 < 0)
                    rooms[x, y].doorBot = false; // non possono esserci stanze sotto perché fuori dalla griglia
                else
                    rooms[x, y].doorBot = (rooms[x, y - 1] != null); //controllo se ci sono stanze sotto

                if (y + 1 >= gridSizeY * 2)
                    rooms[x, y].doorTop = false; // non possono esserci stanze sotto perché fuori dalla griglia
                else
                    rooms[x, y].doorTop = (rooms[x, y + 1] != null); // controllo se ci sono stanze sopra

                if (x - 1 < 0)
                    rooms[x, y].doorLeft = false; // non possono esserci stanze sotto perché fuori dalla griglia
                else
                    rooms[x, y].doorLeft = (rooms[x - 1, y] != null); // controllo se ci sono stanze a sinistra

                if (x + 1 >= gridSizeX * 2)
                    rooms[x, y].doorRight = false; // non possono esserci stanze sotto perché fuori dalla griglia
                else
                    rooms[x, y].doorRight = (rooms[x + 1, y] != null); // controllo se ci sono stanze a destra
            }
        }
    }

    void DrawMap()
    {
        foreach (Room room in rooms) //per ogni stanza nella griglia delle stanze
        {
            if (room == null) //salto le posizioni inoccupate della griglia
            {
                continue;
            }
            DrawRoom(room);
        }
    }

    //accoglierà RoomFactory
    void DrawRoom(Room room)
    {
        Vector2 drawPos = room.gridPos;
        for (int i = 0; i < roomSizeY; i++)
        {
            for (int j = 0; j < roomSizeX; j++)
            {
                TileSpriteSelector mapper = Object.Instantiate(tileToRend, drawPos, Quaternion.identity).GetComponent<TileSpriteSelector>();
                if (i == 0 && j == (roomSizeX / 2) && room.doorBot)
                {
                    mapper.door = true;
                }
                else if (i == (roomSizeY - 1) && j == (roomSizeX / 2) && room.doorTop)
                {
                    mapper.door = true;
                }
                else if (i == (roomSizeY / 2) && j == 0 && room.doorLeft)
                {
                    mapper.door = true;
                }
                else if (i == (roomSizeY / 2) && j == (roomSizeX - 1) && room.doorRight)
                {
                    mapper.door = true;
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
        //Instantiate(player, new Vector2((float)(roomSizeX / 2), (float)(roomSizeY / 2)), Quaternion.identity);
    }
}