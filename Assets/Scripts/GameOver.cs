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
		//se il player è morto e la schermata di gameOver non è ancora aperta la attivo
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
			Canvas.ForceUpdateCanvases();
		}

		//se la schermata di gameOver è attiva tengo il focus sul tasto attivo
		if (isGameOver)
		{
			if (EventSystem.current.currentSelectedGameObject == null)
			{
				EventSystem.current.SetSelectedGameObject(lastButton);
				Canvas.ForceUpdateCanvases();
			}
			else
			{
				lastButton = EventSystem.current.currentSelectedGameObject;
			}
		}

		if (isGameOver)
		{
			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
			{
				source.PlayOneShot(move);
			}
		}
	}

	//inizia una nuova partita
	public void NewGame()
	{
		gameOverScreen.SetActive(false);
		source.PlayOneShot(select);
		Destroy(GameStats.stats.gameObject);
		StartCoroutine(GameManager.manager.lvlManager.FadeOffToNewScene(1f, "LoadingScreen"));
	}

	//ritorna al menu principale
	public void BackToMainMenu()
	{
		gameOverScreen.SetActive(false);
		source.PlayOneShot(select);
		Destroy(GameStats.stats.gameObject);
		StartCoroutine(GameManager.manager.lvlManager.FadeOffToNewScene(1f, "Menu"));
	}

	
}
