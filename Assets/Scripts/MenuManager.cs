using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuManager : MonoBehaviour 
{
    public GameObject button; //primo tasto su cui effettuare il focus
	public GameObject commands, mainMenu, inputHint, hidingPanel;
    public AudioSource sound;
    public AudioClip move, select, music;
	private bool menu = true, comm = false; //indica se menu o schermata dei comandi sono attive
	private bool fadeIn = true, fadeOff = false; //indica se la scena sta iniziando o finendo
	private bool glowInEnd = false, glowOffEnd = false; //indica se il titolo ha finito di lampeggiare
	public TMP_Text title;
	private Material titleMaterial;

	void Awake()
	{
		titleMaterial = title.fontSharedMaterial; //imposto il material per il titolo
		Time.timeScale = 1;
	}
		
    void Start()
    {
		StartCoroutine(StartMenu(1.2f)); //effetto di fadeIn e focus sul primo tasto
		StartCoroutine(GlowingInTitle(0.5f)); //fa lampeggiare il titolo
	}

    void Update()
    {
		//permette al titolo di lampeggiare continuamente
		if (glowInEnd)
		{
			glowInEnd = false;
			StartCoroutine(GlowingOffTitle(0.5f));
		}
		else if (glowOffEnd)
		{
			glowOffEnd = false;
			StartCoroutine(GlowingInTitle(0.5f));
		}
		//se la scena si è già aperta e non si sta chiudendo
		if (!fadeIn && !fadeOff)
		{
			//se il menu è attivo
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
			//altrimenti se la schermata dei comandi è attiva
			else if (comm)
			{
				//se premo E la chiudo
				if (Input.GetKeyDown(KeyCode.E))
				{
					DeactivateCommandWindow();
				}
			}
		}
		
	}

	//Inizia una nuova partita creando un fadeOff sia musicale che visivo
    public void NewGameBtn()
	{
		sound.PlayOneShot(select, 0.5f);
		StartCoroutine(FadeOffMusic(1f));
		StartCoroutine(EnterGame(1f));
	}

	//Chiude il gioco
	public void ExitGameBtn()
	{
        Application.Quit();
	}

	//Crea un fadeOff musicale
    public IEnumerator FadeOffMusic(float fadeTime)
    {
        float rate = 1 / fadeTime;
        while (sound.volume > 0)
        {
            sound.volume -= rate * Time.deltaTime;
            yield return 0;
        }
		yield break;
    }

	//Apre la schermata dei comandi
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

	//Chiude la schermata dei comandi
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

	//Crea un effetto di fadeIn per il menu e lo rende interagibile
	private IEnumerator StartMenu(float fadeTime)
	{
		CanvasGroup cg = hidingPanel.GetComponent<CanvasGroup>();
		cg.alpha = 1;
		float alpha = 1;
		float rate = 1 / fadeTime;
		while (alpha > 0)
		{
			alpha -= Time.deltaTime * rate;
			cg.alpha = alpha;
			yield return null;
		}
		hidingPanel.SetActive(false);
		mainMenu.GetComponent<CanvasGroup>().interactable = true;
		fadeIn = false;
		EventSystem.current.SetSelectedGameObject(button);
		sound = GetComponent<AudioSource>();
		sound.clip = music;
		sound.Play();
		yield break;
	}

	//Crea un fadeOff visivo e carica la schermata di caricamento della partita
	private IEnumerator EnterGame(float fadeTime)
	{
		fadeOff = true;
		mainMenu.GetComponent<CanvasGroup>().interactable = false;
		hidingPanel.SetActive(true);
		CanvasGroup cg = hidingPanel.GetComponent<CanvasGroup>();
		cg.alpha = 0;
		float alpha = 0;
		float rate = 1 / fadeTime;
		while (alpha < 1)
		{
			alpha += Time.deltaTime * rate;
			cg.alpha = alpha;
			yield return null;
		}
		SceneManager.LoadScene("LoadingScreen");
		yield break;
	}

	//Aumenta la luminosità del titolo da 0 a 1
	private IEnumerator GlowingInTitle(float fadeTime)
	{
		float glow = 1;
		float rate = 1 / fadeTime;
		while (glow > 0)
		{
			glow -= Time.deltaTime * rate;
			titleMaterial.SetFloat("_GlowPower", glow);
			yield return null;
		}
		glowInEnd = true;
	}

	//Abbassa la luminosità del titolo da 1 a 0
	private IEnumerator GlowingOffTitle(float fadeTime)
	{
		float glow = 0;
		float rate = 1 / fadeTime;
		while (glow < 1)
		{
			glow += Time.deltaTime * rate;
			titleMaterial.SetFloat("_GlowPower", glow);
			yield return null;
		}
		glowOffEnd = true;
	}
}
