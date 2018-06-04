using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager manager;
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
        lvlManager = GetComponent<LevelManager>();
        InitGame();
    }

    void InitGame() {
        lvlManager.DrawMap();
        lvlManager.InstantiatePlayer();
    }
	
    

    void Save()
    {

    }

    void Load()
    {

    }
}

