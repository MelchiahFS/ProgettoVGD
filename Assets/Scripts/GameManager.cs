using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager manager = null;
    public LevelManager lvlManager;
    private Room actualRoom = null;
    private Vector2Int actualPos;



    private void Awake()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        else if (manager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        
    }

    void InitGame() 
    {
        lvlManager = GetComponent<LevelManager>();
        lvlManager.DrawMap();
        ActualRoom = lvlManager.InstantiatePlayer();
        actualPos = lvlManager.ActualPos;
    }



    //Esegue OnSceneLoaded dopo che viene caricata la scena
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //Prepara la scena appena caricata
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        manager.InitGame();
    }

    void Save()
    {

    }

    void Load()
    {

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
        ActualRoom = lvlManager.map[(int)actualPos.x, (int)actualPos.y]; //setto la nuova stanza attuale
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
            actualRoom.visited = true;
        }
    }

    
}

