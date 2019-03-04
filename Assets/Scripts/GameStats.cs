using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
	public static GameStats stats = null;

	public int levelNumber; //numero del livello attuale
	public int playerMoney; //soldi guadagnati dal player
	public int playerPoints; //punti delle uccisioni
	public float maxHealth; //vita massima del player
	public float playerHealth; //vita attuale del player

	public int count; //numero degli slot occupati nell'inventario
	public int index; //indice dell'oggetto attualmente selezionato nell'inventario
	
	//imposto la sprite dei proiettili per ogni tipo di nemico per tutta la partita
	public Dictionary<string, Sprite> enemyBulletSprites = new Dictionary<string, Sprite>();

	public List<Item> consumablesSO; //scriptable objects da convertire in consumables della classe ItemStats
	public List<ItemStats> consumables; //agisce come "matrice" dei consumables
	public List<ItemStats> hpUp;
	public List<GameObject> enemyList; //lista di tutti i nemici che verranno istanziati in tutti i livelli

	public ItemStats[] itemList = new ItemStats[20];
	public int equippedSlot;


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
