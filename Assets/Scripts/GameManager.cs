using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager manager = null;
    public LevelManager lvlManager;
    private Room actualRoom = null;


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

    void InitGame() {
        lvlManager = GetComponent<LevelManager>();
        lvlManager.DrawMap();
        ActualRoom = lvlManager.InstantiatePlayer();
    }

    //Esegue OnSceneLoaded dopo che viene caricata la scena
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
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

    public void UpdateActualRoom(char room)
    {
        if (room == 'u')
        {
            lvlManager.ActualPos += new Vector2(0, 1);  
        }
        else if (room == 'd')
        {
            lvlManager.ActualPos += new Vector2(0, -1);
        }
        else if (room == 'l')
        {
            lvlManager.ActualPos += new Vector2(-1, 0);
        }
        else if (room == 'r')
        {
            lvlManager.ActualPos += new Vector2(1, 0);
        }
        ActualRoom = lvlManager.map[(int)lvlManager.ActualPos.x, (int)lvlManager.ActualPos.y];
    }

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

