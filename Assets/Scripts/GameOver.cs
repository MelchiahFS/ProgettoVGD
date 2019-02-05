using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOver : MonoBehaviour
{

	public GameObject gameOverScreen, hidingPanel;
	public GameObject lastButton;
	public bool isGameOver = false;
	public AudioSource source;
	public AudioClip move, select;

	void Start()
	{
		source = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (GameManager.manager.dead && !isGameOver)
		{
			gameOverScreen.SetActive(true);
			hidingPanel.SetActive(true);
			hidingPanel.GetComponent<CanvasGroup>().alpha = 0.4f;
			isGameOver = true;
			Time.timeScale = 0;
			GameManager.manager.gamePause = true;
			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(lastButton);
		}

		if (EventSystem.current.currentSelectedGameObject == null)
		{
			EventSystem.current.SetSelectedGameObject(lastButton);
		}
		else
		{
			lastButton = EventSystem.current.currentSelectedGameObject;
		}

		if (isGameOver)
		{
			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
			{
				source.PlayOneShot(move);
			}
		}
	}

	public void NewGame()
	{
		source.PlayOneShot(select);
		Destroy(GameStats.stats.gameObject);
		StartCoroutine(GameManager.manager.lvlManager.FadeOffToNewScene(1f, "LoadingScreen"));
	}

	public void BackToMainMenu()
	{
		source.PlayOneShot(select);
		Destroy(GameStats.stats.gameObject);
		StartCoroutine(GameManager.manager.lvlManager.FadeOffToNewScene(1f, "Menu"));
	}

	
}
