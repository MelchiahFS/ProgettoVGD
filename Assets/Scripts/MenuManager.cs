using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour 
{

    public GameObject button;
    public AudioSource sound;
    public AudioClip move, select, music;
    public float time = 0.5f;

    public void Start()
    {
        EventSystem.current.SetSelectedGameObject(button);
        sound = GetComponent<AudioSource>();
        sound.clip = music;
        sound.Play();
    }

    void Update()
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
        SceneManager.LoadScene(newGameLevel);
        yield break;
    }
}
