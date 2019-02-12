using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Signboard : MonoBehaviour {

	private bool boardActive = false;
	public GameObject signboard, inputHint, hidingPanel;
	public Text signboardText;

	public AudioClip open, close;
	private AudioSource source;

	private string[] levelBoards = { "you are in level 1", "this is the end" };

	void Start()
	{
		source = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (!GameManager.manager.inventoryActive && !GameManager.manager.pauseMenuActive && !GameManager.manager.ending)
		{
			if (GameManager.manager.signboardContact)
			{
				if (Input.GetKeyDown(KeyCode.E))
				{
					if (!boardActive)
					{
						source.PlayOneShot(open);
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
						source.PlayOneShot(close);
						boardActive = false;
						signboard.SetActive(false);
						inputHint.SetActive(false);
						GameManager.manager.signboardActive = false;
						GameManager.manager.gamePause = false;
						
						if (GameStats.stats.levelNumber == 5 && GameManager.manager.ActualRoom.bossRoom)
						{
							if (!GameManager.manager.ending)
							{
								Destroy(GameStats.stats.gameObject);
								StartCoroutine(GameManager.manager.lvlManager.FadeOffToNewScene(1f, "Menu"));
							}
								
						}

						Time.timeScale = 1;
					}
				}
			}
		}
	}

}
