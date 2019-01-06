using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour {

	public static bool GameIsPaused = false;

	public GameObject pauseMenuUI;
    public GameObject button, lastButton, lastInventoryButton;
    public GameObject inventoryMenu;


    void Update ()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (GameIsPaused)
				Resume();
			else
				Pause();
		}

        else if (GameIsPaused)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastButton);
            }
            else
            {
                lastButton = EventSystem.current.currentSelectedGameObject;
            }
        }
        
    }

	public void Resume()
	{ 
		pauseMenuUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        
        GameIsPaused = false;
        GameManager.manager.pauseMenuActive = false;

        if (!GameManager.manager.inventoryActive)
        {
            
            Time.timeScale = 1f;
            GameManager.manager.gamePause = false;
            
        }
        else
        {
            foreach (Button b in inventoryMenu.GetComponentsInChildren<Button>())
            {
                b.enabled = true;
            }
            EventSystem.current.SetSelectedGameObject(lastInventoryButton);
        }
    }

    public void Pause()
    {
        if (GameManager.manager.inventoryActive)
        {
            lastInventoryButton = EventSystem.current.currentSelectedGameObject;

            foreach (Button b in inventoryMenu.GetComponentsInChildren<Button>())
            {
                b.enabled = false;
            }
        }

        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(button);
        Time.timeScale = 0f;
        
        GameIsPaused = true;
        GameManager.manager.gamePause = true;
        GameManager.manager.pauseMenuActive = true;
    }


    public void QuitGame()
	{
		Application.Quit();
	}
}
