using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour 
{
	public void NewGameBtn(string newGameLevel)
	{
		SceneManager.LoadScene(newGameLevel);
	}

	public void ExitGameBtn()
	{
		Application.Quit();
	}
}
