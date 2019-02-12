using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager mm;
	public AudioClip[] levels = new AudioClip[5];
	public AudioClip gameOver, victory;
	public AudioSource musicController;

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

    void Start()
    {
		musicController.clip = levels[GameStats.stats.levelNumber - 1];
		musicController.Play();
    }

	public void GameOver()
	{
		musicController.clip = gameOver;
		musicController.Play();
	}

	public void Victory()
	{
		musicController.clip = victory;
		musicController.Play();
	}

	public void DimMusic(bool toDim)
	{
		if (toDim)
			musicController.volume = 0.5f;
		else
			musicController.volume = 1;
	}
}
