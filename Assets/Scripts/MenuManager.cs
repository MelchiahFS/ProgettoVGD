using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour 
{

    public GameObject button;
	public GameObject commands, mainMenu, inputHint;
    public AudioSource sound;
    public AudioClip move, select, music;
    public float time = 0.5f;
	private bool menu = true, comm = false;
	int i = 0;

    public void Start()
    {
        EventSystem.current.SetSelectedGameObject(button);
        sound = GetComponent<AudioSource>();
        sound.clip = music;
        sound.Play();
    }

    void Update()
    {
		//if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
		//{
		//    sound.PlayOneShot(move, 0.5f);
		//}
		//else if (Input.GetKeyDown(KeyCode.Return))
		//{
		//    sound.PlayOneShot(select, 0.5f);
		//}

		//if (EventSystem.current.currentSelectedGameObject == null)
		//{
		//    EventSystem.current.SetSelectedGameObject(button);
		//}
		//else
		//{
		//    button = EventSystem.current.currentSelectedGameObject;
		//}
		if (menu)
		{
			if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
			{
				sound.PlayOneShot(move, 0.5f);
			}
			else if (Input.GetKeyDown(KeyCode.Return))
			{
				sound.PlayOneShot(select, 0.5f);
			}

			if (EventSystem.current.currentSelectedGameObject == null)
			{
				EventSystem.current.SetSelectedGameObject(button);
			}
			else
			{
				button = EventSystem.current.currentSelectedGameObject;
			}
		}
		else if (comm)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				DeactivateCommandWindow();
				sound.PlayOneShot(select, 0.5f);
			}
		}


	}


    public void NewGameBtn(string newGameLevel)
	{
        StartCoroutine(NewGame(newGameLevel));
	}

	public void ExitGameBtn()
	{
        Application.Quit();
	}


    public IEnumerator NewGame(string newGameLevel)
    {
        float rate = 1 / 1.5f;
        while (sound.volume > 0)
        {
            sound.volume -= rate * Time.deltaTime;
            yield return 0;
        }
		//SceneManager.LoadScene(newGameLevel);
		SceneManager.LoadScene("LoadingScreen");
		yield break;
    }

	public void ActivateCommandWindow()
	{
		button = EventSystem.current.currentSelectedGameObject;
		commands.SetActive(true);
		inputHint.SetActive(true);
		comm = true;
		mainMenu.SetActive(false);
		menu = false;
		sound.PlayOneShot(select, 0.5f);
	}

	public void DeactivateCommandWindow()
	{
		commands.SetActive(false);
		inputHint.SetActive(false);
		comm = false;
		mainMenu.SetActive(true);
		menu = true;
		sound.PlayOneShot(select, 0.5f);
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(button);
	}
}
