using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour 
{
	public GameObject gameManager;
		
		
	void Awake ()
	{
        //Controlla se esiste già un GameManager
        if (GameManager.manager == null)
        {
            Instantiate(gameManager);
        }
	}
}