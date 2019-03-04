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
		//se non è in atto un cambio di scena
		if (!GameManager.manager.ending)
		{
			//se il player non sta morendo posso aprire e chiudere il menu di pausa
			if (!GameManager.manager.isDying)
			{
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					if (GameIsPaused)
						Resume();
					else
						Pause();
				}

				//se il menu di pausa è attivo imposto il focus sul primo tasto
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

	//Chiude il menu di pausa
	public void Resume()
	{
		source.PlayOneShot(exit);
		MusicManager.mm.DimMusic(false); //ripristino il volume iniziale
		
		//disabilito il menu di pausa
		pauseMenuUI.SetActive(false);
		hidingPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(null);
        
		//segnalo la chiusura del menu di pausa
        GameIsPaused = false;
        GameManager.manager.pauseMenuActive = false;

		//se in precedenza non c'era l'inventario o il cartello aperto rimuovo lo stato di pausa
        if (!GameManager.manager.inventoryActive && !GameManager.manager.signboardActive)
        {
            Time.timeScale = 1f;
            GameManager.manager.gamePause = false;
        }
		//altrimenti tengo la pausa e ripristino il focus sull'ultimo tasto selezionato nell'inventario
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

	//Apre il menu di pausa
    public void Pause()
    {
        source.PlayOneShot(enter);
		//abbasso il volume
		MusicManager.mm.DimMusic(true);
		//se era attivo l'inventario salvo l'ultimo tasto selezionato
		if (GameManager.manager.inventoryActive)
        {
            lastInventoryButton = EventSystem.current.currentSelectedGameObject;
            
            inventoryG.interactable = false;
        }
		//faccio lo stesso se era attivo il menu interno dell'inventario
        if (GameManager.manager.inventoryMenuActive)
        {
            lastInventoryButton = EventSystem.current.currentSelectedGameObject;
			
            inventoryMenuG.interactable = false;
        }

		//scurisco il retro scena
		hidingPanel.SetActive(true);
		hidingPanel.GetComponent<CanvasGroup>().alpha = 0.4f;

		//attivo la schermata di pausa
        pauseMenuUI.SetActive(true);
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(button);

		//metto in pausa il gioco
        Time.timeScale = 0f;
        
        GameIsPaused = true;
        GameManager.manager.gamePause = true;
        GameManager.manager.pauseMenuActive = true;
    }

	//Termina la partita e riporta al menu principale
    public void QuitGame()
	{
		Time.timeScale = 1;
		GameIsPaused = false;
		source.PlayOneShot(exit);

		//disabilito tutte le schermate e segnalo la loro chiusura
		pauseMenuUI.SetActive(false);
		inventory.SetActive(false);
		inventoryMenu.SetActive(false);
		signboard.SetActive(false);
		GameManager.manager.signboardActive = false;
		GameManager.manager.pauseMenuActive = false;
		GameManager.manager.inventoryMenuActive = false;
		GameManager.manager.inventoryActive = false;
		GameManager.manager.gamePause = false;

		//distruggo le informazioni riguardanti la partita attuale
		Destroy(GameStats.stats.gameObject);

		//carico il menu principale
		StartCoroutine(GameManager.manager.lvlManager.FadeOffToNewScene(1f, "Menu"));
	}
}
