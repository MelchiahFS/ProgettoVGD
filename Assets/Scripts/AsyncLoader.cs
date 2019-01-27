using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AsyncLoader : MonoBehaviour {

    public Text loadingText;
    private bool loadScene = false;
    
    void Update()
    {
        
        loadingText.text = "Loading Level " + GameStats.stats.levelNumber;

        //fa lampeggiare il messaggio di caricamento modificando continuamente il canale alfa tra 0 e 1
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));

        if (!loadScene)
            StartCoroutine(LoadNewScene());

    }


    // Coroutine che carica la nuova scena in modo asincrono
    IEnumerator LoadNewScene()
    {
        loadScene = true;
        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        yield return new WaitForSeconds(3);

        

        //Restituisce un'operazione asincrona che contiene informazioni sul progresso del caricamento
        AsyncOperation async = SceneManager.LoadSceneAsync("MainScene", LoadSceneMode.Single);

        // finché la nuova scena non è stata caricata non faccio nulla
        while (!async.isDone)
        {
            yield return null;
        }

    }
}
