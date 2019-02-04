using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Signboard : MonoBehaviour {

	private bool boardActive = false;
	public GameObject signboard, inputHint;
	public Text signboardText;

	private string[] levelBoards = { "you are in level 1", "this is the end" };

	void Update()
	{
		if (!GameManager.manager.inventoryActive && !GameManager.manager.pauseMenuActive)
		{
			if (GameManager.manager.signboardContact)
			{
				if (Input.GetKeyDown(KeyCode.E))
				{
					if (!boardActive)
					{
						boardActive = true;
						signboard.SetActive(true);
						inputHint.SetActive(true);
						GameManager.manager.signboardActive = true;
						GameManager.manager.gamePause = true;
						Time.timeScale = 0;

						//DA FARE: CREARE TESTO PER OGNI LIVELLO E PER FINE GIOCO ---------------------------------------------
						if (GameManager.manager.ActualRoom.startRoom)
						{
							//signboardText.text = levelBoards[GameStats.stats.levelNumber - 1];
							signboardText.text = levelBoards[0];
						}
						else if (GameManager.manager.ActualRoom.bossRoom)
						{
							signboardText.text = levelBoards[1];
						}
						Canvas.ForceUpdateCanvases();
					}
					else
					{
						boardActive = false;
						signboard.SetActive(false);
						inputHint.SetActive(false);
						GameManager.manager.signboardActive = false;
						GameManager.manager.gamePause = false;
						Time.timeScale = 1;

						//SI PUO' FARE UNA COROUTINE PER UN FADE-OFF DELLA SCENA ALLA CUI FINE VIENE CARICATO IL MENU PRINCIPALE
						if (GameStats.stats.levelNumber == 5 && GameManager.manager.ActualRoom.bossRoom)
						{
							Destroy(GameStats.stats.gameObject);
							SceneManager.LoadScene("Menu", LoadSceneMode.Single);
						}
					}
				}
			}
		}
	}
}
