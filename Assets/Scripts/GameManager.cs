using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager manager = null;
    private LevelManager lvlManager;

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
        
        //InitGame();
    }

    void InitGame() {
        lvlManager = GetComponent<LevelManager>();
        lvlManager.DrawMap();
        lvlManager.InstantiatePlayer();
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
}

