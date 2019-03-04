using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AsyncLoader : MonoBehaviour {

    public Text loadingText;
    private bool loadScene = false;
	private AsyncOperation async;

	void Start()
	{
		Time.timeScale = 1;
		async = null;
		loadScene = false;
		loadingText.text = "Loading Level " + GameStats.stats.levelNumber;
	}
    
    void Update()
    {
        
        //fa lampeggiare il messaggio di caricamento modificando continuamente il canale alfa tra 0 e 1
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
        if (!loadScene)
            StartCoroutine(LoadNewScene());

    }


    // Coroutine che carica la nuova scena in modo asincrono
    IEnumerator LoadNewScene()
	{
		loadScene = true;
		
		//aspetta tre secondi per mostrare la scritta di caricamento
		yield return new WaitForSeconds(3);
		
        //Restituisce un'operazione asincrona che contiene informazioni sul progresso del caricamento
        async = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);

        // finché la nuova scena non è stata caricata non faccio nulla
        while (!async.isDone)
        {
            yield return null;
        }

		yield break;
    }
}
