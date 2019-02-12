using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public static bool GameIsPaused = false;

	public GameObject pauseMenuUI;
    public GameObject button, lastButton, lastInventoryButton;
    public GameObject inventory, inventoryMenu, signboard;
	public GameObject hidingPanel;

    public AudioClip move, select, enter, exit;
    private AudioSource source;

    private CanvasGroup inventoryG, inventoryMenuG;

    void Start()
    {
        source = GetComponent<AudioSource>();
        inventoryG = inventory.GetComponent<CanvasGroup>();
        inventoryMenuG = inventoryMenu.GetComponent<CanvasGroup>();
    }

    void Update ()
    {
		if (!GameManager.manager.ending)
		{
			if (!GameManager.manager.isDying)
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

					if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
					{
						source.PlayOneShot(move);
					}
				}

			}
		}

	}

	public void Resume()
	{
        source.PlayOneShot(exit);
		MusicManager.mm.DimMusic(false);
		pauseMenuUI.SetActive(false);
		hidingPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(null);
        
        GameIsPaused = false;
        GameManager.manager.pauseMenuActive = false;

        if (!GameManager.manager.inventoryActive && !GameManager.manager.signboardActive)
        {
            Time.timeScale = 1f;
            GameManager.manager.gamePause = false;
        }
        else
        {
            inventoryG.interactable = true;

            if (GameManager.manager.inventoryMenuActive)
            {
                inventoryMenuG.interactable = true;
            }
            EventSystem.current.SetSelectedGameObject(lastInventoryButton);
        }
    }

    public void Pause()
    {
        source.PlayOneShot(enter);
		MusicManager.mm.DimMusic(true);
		if (GameManager.manager.inventoryActive)
        {
            lastInventoryButton = EventSystem.current.currentSelectedGameObject;
            
            inventoryG.interactable = false;
        }
        if (GameManager.manager.inventoryMenuActive)
        {
            lastInventoryButton = EventSystem.current.currentSelectedGameObject;
			
            inventoryMenuG.interactable = false;
        }

		hidingPanel.SetActive(true);
		hidingPanel.GetComponent<CanvasGroup>().alpha = 0.4f;
        pauseMenuUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(button);
        Time.timeScale = 0f;
        
        GameIsPaused = true;
        GameManager.manager.gamePause = true;
        GameManager.manager.pauseMenuActive = true;
    }


    public void QuitGame()
	{
		Time.timeScale = 1;
		GameIsPaused = false;
		source.PlayOneShot(exit);
		pauseMenuUI.SetActive(false);
		inventory.SetActive(false);
		inventoryMenu.SetActive(false);
		signboard.SetActive(false);
		GameManager.manager.signboardActive = false;
		GameManager.manager.pauseMenuActive = false;
		GameManager.manager.inventoryMenuActive = false;
		GameManager.manager.inventoryActive = false;
		GameManager.manager.gamePause = false;
		Destroy(GameStats.stats.gameObject);
		StartCoroutine(GameManager.manager.lvlManager.FadeOffToNewScene(1f, "Menu"));
	}
}
