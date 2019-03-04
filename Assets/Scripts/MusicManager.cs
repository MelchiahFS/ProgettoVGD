using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager mm;
	public AudioClip[] levels = new AudioClip[5]; //lista delle tracce musicali corrispondenti ai vari livelli
	public AudioClip gameOver, victory; //tracce audio di gameOver e vittoria
	public AudioSource musicController;
	private float volume;

	void Awake()
	{
		if (mm == null)
		{
			mm = this;
		}
		else
		{
			if (mm != this)
			{
				Destroy(this);
			}
		}
		musicController = GetComponent<AudioSource>();
	}

	//seleziono la traccia corrispondente al livello
    void Start()
    {
		musicController.clip = levels[GameStats.stats.levelNumber - 1];
		musicController.Play();
    }

	//seleziono la traccia di gameOver
	public void GameOver()
	{
		musicController.clip = gameOver;
		musicController.Play();
	}

	//seleziono la traccia per la vittoria
	public void Victory()
	{
		musicController.clip = victory;
		musicController.Play();
	}

	//dimezza il volume (usato dal menu di pausa)
	public void DimMusic(bool toDim)
	{
		if (toDim)
		{
			volume = musicController.volume;
			musicController.volume = volume / 2;
		}
		else
		{
			musicController.volume = volume;
		}
	}
}
