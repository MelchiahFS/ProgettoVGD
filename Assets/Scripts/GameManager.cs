using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	
    public static GameManager manager = null;
    public LevelManager lvlManager;
    private Room actualRoom = null;
    public Vector2Int actualPos;

	//flag relativi allo stato delle varie schermate (pausa, inventario, gameOver...)
    public bool gamePause = false, inventoryActive = false, inventoryMenuActive = false, pauseMenuActive = false, signboardActive = false;

	//flag relativi allo stato del player o del livello attuale
    public bool dead = false, isDying = false, ending = false, startingLevel = false;
	public bool signboardContact = false;
	public GameObject playerReference;
	public GameObject signBoard;

	private void Awake()
	{
		if (manager == null)
		{
			manager = this;
		}
		else if (manager != this)
		{
			Destroy(gameObject);
		}

		lvlManager = GetComponent<LevelManager>();
		lvlManager.DrawMap(); //disegna la mappa
		actualPos = lvlManager.ActualPos; //recupera la posizione attuale nella mappa
		ActualRoom = lvlManager.InstantiatePlayer(); //istanzia il player e restituisce la stanza in cui si trova
		GetComponent<MiniMapController>().enabled = false;
		playerReference = GameObject.Find("Player"); //salvo il riferimento al player per l'accesso da parte degli altri script
	}

    //Carica il nuovo livello
    private void NewLevel()
    {
		//Carica la schermata di caricamento
		StartCoroutine(GameManager.manager.lvlManager.FadeOffToNewScene(1f, "LoadingScreen"));
	}

    //viene aggiornata la posizione della stanza attuale nella griglia delle stanze
    public void UpdateActualRoom(char room)
    {
        if (room == 'u')
        {
            actualPos += new Vector2Int(0, 1);
        }
        else if (room == 'd')
        {
            actualPos += new Vector2Int(0, -1);
        }
        else if (room == 'l')
        {
            actualPos += new Vector2Int(-1, 0);
        }
        else if (room == 'r')
        {
            actualPos += new Vector2Int(1, 0);
        }
        ActualRoom = lvlManager.map[actualPos.x, actualPos.y]; //setto la nuova stanza attuale
    }

    //imposta o restituisce la stanza attuale
    public Room ActualRoom
    {
        get
        {
            return actualRoom;
        }

        set
        {
            actualRoom = value;
        }
    }

    //restituisce la stanza adiacente a quella attuale nella direzione richiesta
    public Room GetAdiacentRoom(char dir)
    {
		//ottengo la dimensione in x e y della mappa
        Vector2 mapSize = lvlManager.lvlGen.GetMapSize();

		//se esiste una stanza sopra quella attuale, la restituisco
		if (dir == 'u')
        {
            if (actualPos.y + 1 < mapSize.y)
            {
                return lvlManager.map[actualPos.x, actualPos.y + 1];
            }
            else
            {
                return null;
            }
        }
        else if (dir == 'd')
        {
            if (actualPos.y - 1 >= 0)
            {
                return lvlManager.map[actualPos.x, actualPos.y - 1];
            }
            else
            {
                return null;
            }
        }
        else if (dir == 'l')
        {
            if (actualPos.x - 1 >= 0)
            {
                return lvlManager.map[actualPos.x - 1, actualPos.y];
            }
            else
            {
                return null;
            }
        }
        else if (dir == 'r')
        {
            if (actualPos.x + 1 < mapSize.x)
            {
                return lvlManager.map[actualPos.x + 1, actualPos.y];
            }
            else
            {
                return null;
            }
        }
        return null;
    }


}

