using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerator
{
    Vector2Int worldSize;
    private Room[,] rooms;
    List<Vector2Int> takenPositions = new List<Vector2Int>(); //lista delle posizioni occupate dalle stanze generate
    int gridSizeX, gridSizeY;
    private int roomSizeX, roomSizeY; //dimensione in x e y delle stanze
    public int distRoomX = 6, distRoomY = 6; //distanza tra le stanze (utile alla generazione dei corridoi)
    private int numberOfRooms;
    private bool bossIsSet = false, shopIsSet = false; //indica se shop e boss room sono già state generate
	
	//genera la mappa del livello
    public LevelGenerator(int sizeX, int sizeY)
    {
        this.roomSizeX = sizeX;
        this.roomSizeY = sizeY;
	
		//genera un numero di stanze casuale
		numberOfRooms = UnityEngine.Random.Range(10, 16);

        //NOTA: la dimensione della griglia delle stanze sarà il doppio in x e y 
        //in modo che la prima stanza stia al centro della griglia (0, 0)
        worldSize = new Vector2Int(UnityEngine.Random.Range(4, 5), UnityEngine.Random.Range(4, 5));
        
        //se il numero delle stanze eccede il numero delle celle nella griglia allora lo eguaglio a tale numero
        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2))
        {
            numberOfRooms = (worldSize.x * 2) * (worldSize.y * 2);
        }
        gridSizeX = worldSize.x;
        gridSizeY = worldSize.y;

		//bug fix per forzare la generazione di un livello con le stanze boss e shop
		while (!bossIsSet || !shopIsSet)
		{
			CreateRooms();
			SetRoomDoors();
			SetBossAndShop();
		}
	}

	//crea la matrice delle stanze generando effettivamente una mappa
    private void CreateRooms()
    {
        //crea una matrice Room che rappresenta la griglia delle stanze
        rooms = new Room[gridSizeX * 2, gridSizeY * 2];

        //crea una stanza al centro della griglia e la imposta come stanza iniziale
        rooms[gridSizeX, gridSizeY] = new Room(Vector2Int.zero);
        rooms[gridSizeX, gridSizeY].startRoom = true;

        //inserisce nella lista delle posizioni visitate la posizione della stanza attuale
        takenPositions.Insert(0, Vector2Int.zero);
        Vector2Int checkPos = Vector2Int.zero;

        float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.1f;

        //tolgo una stanza perché ho già creato la stanza iniziale
        for (int i = 0; i < numberOfRooms - 1; i++)
        {
            //restituisce un valore inizialmente piccolo, che cresce a ogni iterazione
            float randomPerc = ((float)i) / ((float)numberOfRooms - 1);
            //ottengo un valore casuale dipendente dal valore precedentemente calcolato
            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);

			//seleziono una possibile posizione valida per una nuova stanza
			//(ossia che non sia già occupata e che sia nei limiti della mappa)
            checkPos = NewPosition();

			//se la nuova stanza avrà più di una stanza adiacente
			if (NumberOfNeighbors(checkPos, takenPositions) > 1)
			{
				//se ottengo un valore casuale maggiore del risultato dell'interpolazione
				if (UnityEngine.Random.value > randomCompare)
				{
					int iterations = 0;
					do
					{
						//cerco una nuova posizione per la nuova stanza
						checkPos = SelectiveNewPosition();
						iterations++;
					}//ripeto finché non trovo una stanza che abbia al massimo una stanza adiacente
					while (NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < numberOfRooms);
				}
			}

			//posiziono la stanza nella griglia delle stanze
			rooms[(checkPos.x / (roomSizeX + distRoomX)) + gridSizeX, (checkPos.y / (roomSizeY + distRoomY)) + gridSizeY] = new Room(checkPos);
			
            //inserisco la nuova stanza nella lista delle stanze visitate
            takenPositions.Insert(0, checkPos);
        }
    }

	//seleziona una possibile posizione per una nuova stanza rispetto a una già generata
    Vector2Int NewPosition()
    {
        int x = 0, y = 0;
        Vector2Int checkingPos = Vector2Int.zero;
        do
        {   //ottengo un indice casuale compreso nel range delle stanze già generate
            int index = Mathf.RoundToInt(UnityEngine.Random.value * (takenPositions.Count - 1));
            x = takenPositions[index].x;
            y = takenPositions[index].y;

			//determino casualmente dove posizionare la nuova stanza rispetto a quella selezionata
            bool upDown = (UnityEngine.Random.value < 0.5f);
            bool positive = (UnityEngine.Random.value < 0.5f);

			//aggiorno le coordinate della nuova stanza
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
            checkingPos = new Vector2Int(x, y);

        }//ripeto finché la nuova stanza non sarà dentro la griglia o la nuova posizione non sia diversa da una già occupata
        while (takenPositions.Contains(checkingPos) || (x / (roomSizeX + distRoomX)) >= gridSizeX || (x / (roomSizeX + distRoomX)) < -gridSizeX 
				|| (y / (roomSizeY + distRoomY)) >= gridSizeY || (y / (roomSizeY + distRoomY)) < -gridSizeY);
        
        return checkingPos;
    }

	//metodo simile a NewPosition, differisce per il fatto che rende quasi certamente una posizione con una sola stanza adiacente
	//(utile per creare delle ramificazioni nella mappa)
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
				//ottengo un indice casuale compreso nel range delle stanze già generate
				index = Mathf.RoundToInt(UnityEngine.Random.value * (takenPositions.Count - 1));
				x = takenPositions[index].x;
				y = takenPositions[index].y;
				inc++;

            }//controlla finché non trova una stanza che ha una sola stanza adiacente
            while (NumberOfNeighbors(takenPositions[index], takenPositions) > 1 && inc < numberOfRooms);

			//determino casualmente dove posizionare la nuova stanza rispetto a quella selezionata
			bool upDown = (UnityEngine.Random.value < 0.5f);
            bool positive = (UnityEngine.Random.value < 0.5f);

			//aggiorno le coordinate della nuova stanza
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
            checkingPos = new Vector2Int(x, y);

        }//ripeto finché la nuova stanza non sarà dentro la griglia o la nuova posizione non sia diversa da una già occupata
        while (takenPositions.Contains(checkingPos) || (x / (roomSizeX + distRoomX)) >= gridSizeX || (x / (roomSizeX + distRoomX)) < -gridSizeX || (y / (roomSizeY + distRoomY)) >= gridSizeY || (y / (roomSizeY + distRoomY)) < -gridSizeY);
        
        return checkingPos;
    }

    //controlla il numero di stanze adiacenti a quella corrente e restituisce il numero;
    //viene usata per la scelta della posizione delle stanze da generare
    int NumberOfNeighbors(Vector2Int checkingPos, List<Vector2Int> usedPositions)
    {

        int ret = 0;
		//controllo se ci sono stanze a destra
        if (usedPositions.Contains(new Vector2Int(checkingPos.x + (roomSizeX + distRoomX), checkingPos.y)))
            ret++;
		//controllo se ci sono stanze a sinistra
		if (usedPositions.Contains(new Vector2Int(checkingPos.x - (roomSizeX + distRoomX), checkingPos.y)))
            ret++;
		//controllo se ci sono stanze sopra
		if (usedPositions.Contains(new Vector2Int(checkingPos.x, checkingPos.y + (roomSizeY + distRoomY))))
            ret++;
		//controllo se ci sono stanze sotto
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

	//si occupa di cercare tra le stanze generate delle candidate boss e shop room
    void SetBossAndShop()
    {
        foreach (Room room in rooms)
        {
            if (!bossIsSet && room != null && !room.startRoom && !room.shopRoom && !room.doorTop)
            {
                if (AdiacentRooms(room) == 1)
                {
                    room.bossRoom = true;
                    bossIsSet = true;

                }
            }
            else if (!shopIsSet && room != null && !room.startRoom && !room.bossRoom)
            {
                if (AdiacentRooms(room) == 1)
                {
                    room.shopRoom = true;
                    shopIsSet = true;

                }
            }
        }
    }

	//controlla la presenza di stanze adiacenti a una stanza data
    int AdiacentRooms(Room room)
    {
        int n = 0;
        if (room.doorTop)
            n++;
        if (room.doorBot)
            n++;
        if (room.doorLeft)
            n++;
        if (room.doorRight)
            n++;
        return n;
    }

	//restituisce la mappa generata
    public Room[,] Rooms
    {
        get
        {
            return rooms;
        }
    }

	//restituisce la dimensione della mappa
    public Vector2Int GetMapSize()
    {
        return new Vector2Int(gridSizeX * 2, gridSizeY * 2);
    }

	//restituisce il numero delle stanze
    public int GetRoomNumber()
    {
        return numberOfRooms;
    }
}
