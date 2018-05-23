using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    Vector2Int worldSize;
    Room[,] rooms;
    List<Vector2Int> takenPositions = new List<Vector2Int>();
    int gridSizeX, gridSizeY;
    public int roomSizeX = 13, roomSizeY = 13;
    private int distRoomX = 3, distRoomY = 5;
    private int passSizeX = 3, passSizeY = 5;
    private int numberOfRooms;
    public GameObject tileToRend;
    public GameObject player;
    private BoxCollider2D wallCollider;
    private Rigidbody2D rb2d;


    // Use this for initialization
    void Start()
    {
        numberOfRooms = Random.Range(10, 20);

        //NOTA: la dimensione della griglia delle stanze sarà il doppio in x e y 
        //in modo che la prima stanza stia al centro della griglia
        worldSize = new Vector2Int(Random.Range(5, 7), Random.Range(5, 7));
        wallCollider = tileToRend.GetComponent<BoxCollider2D>();
        wallCollider.size = new Vector2(1, 1);
        wallCollider.enabled = false;
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
        Instantiate(player, new Vector2((float)(roomSizeX / 2), (float)(roomSizeY / 2)), Quaternion.identity);
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
            
            //posiziono la stanza nella griglia delle stanze
            rooms[(checkPos.x / (roomSizeX + distRoomX)) + gridSizeX, (checkPos.y / (roomSizeY + distRoomY)) + gridSizeY] = new Room(checkPos);
            
            
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
                    y += roomSizeY + distRoomY;
                else
                    y -= roomSizeY + distRoomY;
            }
            else //determino se sarà sopra o sotto di quella attuale
            {
                if (positive)
                    x += roomSizeX + distRoomX;
                else
                    x -= roomSizeX + distRoomX;
            }
            checkingPos = new Vector2Int(x, y);
        }//ripeto finché la nuova stanza non sarà dentro la griglia o la nuova posizione non sia diversa da una già occupata
        while (takenPositions.Contains(checkingPos) || (x / (roomSizeX + distRoomX)) >= gridSizeX || (x / (roomSizeX + distRoomX)) < -gridSizeX || (y / (roomSizeY + distRoomY)) >= gridSizeY || (y / (roomSizeY + distRoomY)) < -gridSizeY); 
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
                    y += roomSizeY + distRoomY;
                else
                    y -= roomSizeY + distRoomY;
            }
            else
            {
                if (positive)
                    x += roomSizeX + distRoomX;
                else
                    x -= roomSizeX + distRoomX;
            }
            //sceglie dove mettere la nuova stanza rispetto a quella attuale
            checkingPos = new Vector2Int(x, y); 
        }//ripeto finché la nuova stanza non sarà dentro la griglia o la nuova posizione non sia diversa da una già occupata
        while (takenPositions.Contains(checkingPos) || (x / (roomSizeX + distRoomX)) >= gridSizeX || (x / (roomSizeX + distRoomX)) < -gridSizeX || (y / (roomSizeY + distRoomY)) >= gridSizeY || (y / (roomSizeY + distRoomY)) < -gridSizeY);
        if (inc >= 100)
            print("Error: could not find position with only one neighbor");
        return checkingPos;
    }

    //controlla il numero di stanze adiacenti a quella corrente e restituisce il numero;
    //viene usata per la scelta della posizione delle stanze da generare
    int NumberOfNeighbors(Vector2Int checkingPos, List<Vector2Int> usedPositions)
    {
        
        int ret = 0;
        if (usedPositions.Contains(new Vector2Int(checkingPos.x + (roomSizeX + distRoomX), checkingPos.y)))
            ret++;
        if (usedPositions.Contains(new Vector2Int(checkingPos.x - (roomSizeX + distRoomX), checkingPos.y)))
            ret++;
        if (usedPositions.Contains(new Vector2Int(checkingPos.x, checkingPos.y + (roomSizeY + distRoomY))))
            ret++;
        if (usedPositions.Contains(new Vector2Int(checkingPos.x, checkingPos.y - (roomSizeY + distRoomY))))
            ret++;
        return ret;
    }


    //imposta le variabili booleane corrispondenti alla presenza delle porte per ogni stanza
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
            if (room != null) //salto le posizioni inoccupate della griglia
            {
                DrawRoom(room);
                DrawWalls(room);
                LinkRooms(room);
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
                //tileToRend.layer = 0;
                if (i == 0 || j == 0 || i == (roomSizeY + 3) || i == (roomSizeY + 1) || (j == roomSizeX + 1))
                {
                    wallCollider.enabled = true;
                    if (i == (roomSizeY + 1) && j > 0 && j <= roomSizeX)
                    {
                        wallCollider.size = new Vector2(1, 2);
                        wallCollider.offset = new Vector2(0, 0.5f);
                    }
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
                    else if ((j == 1 && i == roomSizeY + 1) | (i == roomSizeY + 1 && j == ((roomSizeX + 1) / 2) + 1 && room.doorTop))
                    {
                        mapper.innerWall = true;
                        mapper.left = true;
                        mapper.right = false;

                    }
                    else if ((j == roomSizeX && i == roomSizeY + 1) | (i == roomSizeY + 1 && j == (roomSizeX / 2) && room.doorTop))
                    {
                        mapper.innerWall = true;
                        mapper.right = true;
                        mapper.left = false;
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

                    if (i == 0 || i== 2)
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