using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuManager : MonoBehaviour 
{

    public GameObject button;
	public GameObject commands, mainMenu, inputHint, hidingPanel;
    public AudioSource sound;
    public AudioClip move, select, music;
    public float time = 0.5f;
	private bool menu = true, comm = false, fadeIn = true, fadeOff = false;
	private bool glowInEnd = false, glowOffEnd = false;
	public TMP_Text title;
	private Material titleMaterial;

	void Awake()
	{
		titleMaterial = title.fontSharedMaterial;
		Time.timeScale = 1;
	}
		
    void Start()
    {
		StartCoroutine(StartMenu(1.2f));
		StartCoroutine(GlowingInTitle(0.5f));
	}

    void Update()
    {
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
		if (!fadeIn && !fadeOff)
		{
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
				if (Input.GetKeyDown(KeyCode.E))
				{
					DeactivateCommandWindow();
					sound.PlayOneShot(select, 0.5f);
				}
			}
		}
		
	}


    public void NewGameBtn()
	{
        StartCoroutine(FadeOffMusic(1f));
		StartCoroutine(EnterGame(1f));
	}

	public void ExitGameBtn()
	{
        Application.Quit();
	}


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
