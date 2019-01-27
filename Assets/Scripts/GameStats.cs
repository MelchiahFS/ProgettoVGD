using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
	public static GameStats stats = null;

	public int levelNumber;
	public int playerMoney;
	public float maxHealth;
	public float playerHealth;

	public ItemStats[] itemList = new ItemStats[20];
	public int equippedSlot = 21;


	void Awake()
	{
		if (stats == null)
		{
			DontDestroyOnLoad(gameObject);
			stats = this;
		}
		else if (stats != this)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

}
