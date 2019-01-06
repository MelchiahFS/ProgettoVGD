using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour 
{

    public GameObject button;

    public void Start()
    {
        EventSystem.current.SetSelectedGameObject(button);
    }

    void Update()
    {
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
		SceneManager.LoadScene(newGameLevel);
		return;
	}

	public void ExitGameBtn()
	{
		Application.Quit();
	}
}
