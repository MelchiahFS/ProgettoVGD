using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LootGenerator : MonoBehaviour {

    private static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
    public float minMeeleDmg, maxMeeleDmg, minRangedDmg, maxRangedDmg, minRng, maxRng, minFR, maxFR, minSP, maxSP; //minimo e massimo danno (per meele e ranged), range, fire rate e shot speed
    private Array enumValues;
	public Item startingMeeleSO, startingRangedSO;
    public GameObject sceneItemPrefab;
    private SceneItem si;
    private ItemSpriteSelector select;
    private SpriteRenderer render;
    private ItemStats actualItem;
    public Room actualRoom;
    private PlayerHealth ph;
    public GameObject pricePrefab, altarPrefab, writingsPrefab;
    private bool keyGen = false;

    private int roomsWithEnemies = 0;

    void Start()
    {
        roomsWithEnemies = GameManager.manager.lvlManager.roomNumber - 3; //tolgo la stanza iniziale, lo shop e la stanza del boss

		//imposto i valori dei range per le armi che verranno generate
		if (GameStats.stats.levelNumber == 1)
		{
			minMeeleDmg = 15;
			maxMeeleDmg = 30;
			minRangedDmg = 5;
			maxRangedDmg = 15;
			minRng = 5;
			maxRng = 7;
			minFR = 0.8f;
			maxFR = 1.5f;
			minSP = 5;
			maxSP = 10;

		}
		else if (GameStats.stats.levelNumber == 2)
		{
			minMeeleDmg = 20;
			maxMeeleDmg = 35;
			minRangedDmg = 10;
			maxRangedDmg = 20;
			minRng = 6;
			maxRng = 9;
			minFR = 0.6f;
			maxFR = 1.3f;
			minSP = 10;
			maxSP = 18;
		}
		else if (GameStats.stats.levelNumber == 3)
		{
			minMeeleDmg = 25;
			maxMeeleDmg = 40;
			minRangedDmg = 15;
			maxRangedDmg = 25;
			minRng = 7;
			maxRng = 10;
			minFR = 0.4f;
			maxFR = 1.1f;
			minSP = 15;
			maxSP = 26;
		}
		else if (GameStats.stats.levelNumber == 4)
		{
			minMeeleDmg = 30;
			maxMeeleDmg = 45;
			minRangedDmg = 18;
			maxRangedDmg = 30;
			minRng = 8;
			maxRng = 12;
			minFR = 0.2f;
			maxFR = 0.9f;
			minSP = 20;
			maxSP = 40;
		}
		else
		{
			minMeeleDmg = 35;
			maxMeeleDmg = 50;
			minRangedDmg = 22;
			maxRangedDmg = 35;
			minRng = 10;
			maxRng = 14;
			minFR = 0.1f;
			maxFR = 0.6f;
			minSP = 30;
			maxSP = 60;
		}
		
		ph = GetComponentInParent<PlayerHealth>();

        select = ItemSpriteSelector.iss;
        
		if (GameStats.stats.levelNumber == 1)
		{
			GameStats.stats.consumables = new List<ItemStats>();

			//converto gli scriptableObject in oggetti utilizzabili nel gioco
			InitializeConsumables();

			//assegno le armi di base al player
			SetStartingWeapons();
		}
		else
		{
			GetComponent<Weapon>().EquipWeapon(GameStats.stats.itemList[GameStats.stats.index]);
		}
		
		actualRoom = GameManager.manager.ActualRoom;
	}

    void Update()
    {
        //se la stanza è lo shop
        if (actualRoom.shopRoom && !actualRoom.hasGenReward)
        {
            //e se il player è effettivamente dentro la stanza allora genero gli oggetti da acquistare
            if (transform.position.x > actualRoom.gridPos.x && transform.position.x < (actualRoom.gridPos.x + GameManager.manager.lvlManager.roomSizeX)
                && transform.position.y > actualRoom.gridPos.y && transform.position.y < (actualRoom.gridPos.y + GameManager.manager.lvlManager.roomSizeY))
            {
                int i = 0;
				//per ogni posizione relativa agli oggetti spawno armi in una riga e consumables nelle altre
                foreach (Vector2 pos in actualRoom.freePositions)
                {
                    if (i < 3)
                        InstantiateWeapon(pos);
                    else
                        InstantiateConsumable(pos);
                    i++;
                }
                i = 0;
				//impedisco che vengano generati altri oggetti
                actualRoom.hasGenReward = true;
            }
            
        }

        //se la stanza non ha nemici e la ricompensa non è ancora stata generata
        else if (!actualRoom.startRoom && !actualRoom.hasGenReward && actualRoom.enemyNumber == 0 && actualRoom.enemyWaves == 0)
        {
            //e se il player è effettivamente dentro la stanza
            if (transform.position.x > actualRoom.gridPos.x && transform.position.x < (actualRoom.gridPos.x + GameManager.manager.lvlManager.roomSizeX)
                && transform.position.y > actualRoom.gridPos.y && transform.position.y < (actualRoom.gridPos.y + GameManager.manager.lvlManager.roomSizeY))
            {
				//se la chiave ancora non è stata generata
                if (!keyGen)
                {
					//imposto una probabilità di spawn per la chiave che aumenta con le stanze ripulite dai nemici
                    float keyProb = (float) 1 / roomsWithEnemies * 100;
                    int randomValue = rnd.Next(100);
					//se il valore casuale rientra nel range precedentemente calcolato spawno la chiave
                    if (randomValue <= (int)keyProb)
                    {
                        InstantiateKey(actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)]);
                        keyGen = true;
                    }
					//altrimenti decremento il counter delle stanze con nemici
                    else
                    {
                        roomsWithEnemies--;
                    }
                }
				//-------------------------------------------------------------------------------


				if (actualRoom.bossRoom)
				{
					//istanzio l'altare
					GameObject altar = Instantiate(altarPrefab, new Vector2(actualRoom.gridPos.x + GameManager.manager.lvlManager.roomSizeX / 2, actualRoom.gridPos.y + GameManager.manager.lvlManager.roomSizeY / 2), Quaternion.identity) as GameObject;

					render = altar.GetComponent<SpriteRenderer>();
					
					StartCoroutine(GameManager.manager.lvlManager.FadeIn(render, 0.3f));

					actualRoom.toSort.Add(altar);

					//se non è l'ultimo livello sull'altare apparirà un'arma
					if (GameStats.stats.levelNumber < 5)
					{
						Vector2 pos = actualRoom.freePositions[0];
						pos = new Vector2(pos.x, pos.y + 0.4f);
						InstantiateWeapon(pos);
					}
					//altrimenti apparirà l'oggetto che farà finire il gioco
					else
					{
						Vector2 pos = actualRoom.freePositions[0];
						pos = new Vector2(pos.x, pos.y + 0.6f);
						GameObject writings = Instantiate(writingsPrefab, pos, Quaternion.identity) as GameObject;
						Color c = Color.red;
						c.a = 0;
						writings.GetComponent<SpriteRenderer>().color = c;
						MusicManager.mm.Victory();
						StartCoroutine(GameManager.manager.lvlManager.FadeIn(writings.GetComponent<SpriteRenderer>(), 0.3f));
					}
				}
				else
				{
					//ho il 50% di ottenere o no una ricompensa
					if (rnd.Next(100) >= 50)
					{
						//se sono nel 50% di probabilità di spawn della ricompensa, imposto una probabilità per decidere che ricompensa spawnare
						if (rnd.Next(100) < 20)
							InstantiateWeapon(actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)]);
						else
						{
							if (rnd.Next(100) < 40)
								InstantiateConsumable(actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)]);
							else
								InstantiateMoney(actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)]);
						}
					}
				}
				
				//impedisco che vengano generate altre ricompense per la stanza attuale
				actualRoom.hasGenReward = true;
			}   
        }
    }

	//spawna un consumable
    public void InstantiateConsumable(Vector3 pos)
    {
        GameObject cons = Instantiate(sceneItemPrefab, pos, Quaternion.identity) as GameObject;
        si = cons.GetComponent<SceneItem>();
        si.Info = GameStats.stats.consumables[rnd.Next(GameStats.stats.consumables.Count)];
        render = cons.GetComponent<SpriteRenderer>();
        render.sprite = si.Info.sprite;
        if (actualRoom.shopRoom)
        {
            GameObject price = Instantiate(pricePrefab, cons.transform) as GameObject;
            si.Info.toBuy = true;
            si.Info.price = 30;
            price.GetComponentInChildren<Text>().text = si.Info.price.ToString();
            price.transform.SetParent(cons.transform, false);
        }

        //imposto i consumable come trasparenti, per poi fare un effetto di fade-in quando verranno attivati 
        Color color = render.color;
        color.a = 0;
        render.color = color;

        StartCoroutine(GameManager.manager.lvlManager.FadeIn(render, 0.3f));
    }

	//spawna un'arma
    public void InstantiateWeapon(Vector3 pos)
    {
        GameObject weapon = Instantiate(sceneItemPrefab, pos, Quaternion.identity) as GameObject;
        si = weapon.GetComponent<SceneItem>();
        render = weapon.GetComponent<SpriteRenderer>();
        SetWeaponStats(si);
        if (actualRoom.shopRoom)
        {
            GameObject price = Instantiate(pricePrefab, weapon.transform) as GameObject;
            si.Info.toBuy = true;
            si.Info.price = 100;
            price.GetComponentInChildren<Text>().text = si.Info.price.ToString();
            price.transform.SetParent(weapon.transform, false);
        }
		else if (actualRoom.bossRoom)
		{
			render.sortingLayerName = "HUD";
		}

        //imposto i consumable come trasparenti, per poi fare un effetto di fade-in quando verranno attivati 
        Color color = render.color;
        color.a = 0;
        render.color = color;

        StartCoroutine(GameManager.manager.lvlManager.FadeIn(render, 0.3f));
    }

	//setta le statistiche dell'arma da spawnare
    private void SetWeaponStats(SceneItem i)
    {
        i.Info = new ItemStats();

        i.Info.itemType = ItemStats.ItemType.weapon;
        Array enumValues = Enum.GetValues(typeof(ItemStats.WeaponType));
        i.Info.weaponType = (ItemStats.WeaponType) enumValues.GetValue(rnd.Next(enumValues.Length));

        if (i.Info.weaponType == ItemStats.WeaponType.meele)
        {
            i.Info.sprite = select.meeleWeapons[rnd.Next(select.meeleWeapons.Count)];
            i.GetComponent<SpriteRenderer>().sprite = i.Info.sprite;
            i.GetComponent<SpriteRenderer>().sortingLayerName = "Items";

            i.Info.itemName = "Sword";
			i.Info.damage = (float) Math.Truncate(UnityEngine.Random.Range(minMeeleDmg, maxMeeleDmg) * 10) / 10;
            i.Info.description = "Damage: " + i.Info.damage;
        }
        else
        {
            i.Info.sprite = select.rangedWeapons[rnd.Next(select.rangedWeapons.Count)];
            i.GetComponent<SpriteRenderer>().sprite = i.Info.sprite;
            i.GetComponent<SpriteRenderer>().sortingLayerName = "Items";

            i.Info.itemName = "Magic Staff";

            enumValues = Enum.GetValues(typeof(ItemStats.FireType));
            i.Info.fireType = (ItemStats.FireType)enumValues.GetValue(rnd.Next(enumValues.Length));

            enumValues = Enum.GetValues(typeof(ItemStats.BulletType));
            i.Info.bulletType = (ItemStats.BulletType)enumValues.GetValue(rnd.Next(enumValues.Length));

			i.Info.damage = (float)Math.Truncate(UnityEngine.Random.Range(minRangedDmg, maxRangedDmg) * 10) / 10;
			i.Info.range = (float)Math.Truncate(UnityEngine.Random.Range(minRng, maxRng) * 10) / 10;
			i.Info.fireRate = (float)Math.Truncate(UnityEngine.Random.Range(minFR, maxFR) * 10) / 10;
			i.Info.shotSpeed = (float)Math.Truncate(UnityEngine.Random.Range(minSP, maxSP) * 10) / 10;

			if (i.Info.fireType == ItemStats.FireType.single)
                i.Info.description += "Fire Type: Single Shot";
            else if (i.Info.fireType == ItemStats.FireType.multiple)
                i.Info.description += "Fire Type: Triple Shot";
            else if (i.Info.fireType == ItemStats.FireType.splitShot)
                i.Info.description += "Fire Type: Split Shot";
            else if (i.Info.fireType == ItemStats.FireType.bidirectional)
                i.Info.description += "Fire Type: Bidirectional Shot";

            if (i.Info.bulletType == ItemStats.BulletType.normal)
                i.Info.description += "\r\nBullet Type: Standard";
            else if (i.Info.bulletType == ItemStats.BulletType.poisonous)
                i.Info.description += "\r\nBullet Type: Poisonous";
            else if (i.Info.bulletType == ItemStats.BulletType.burning)
                i.Info.description += "\r\nBullet Type: Burning";
            else if (i.Info.bulletType == ItemStats.BulletType.slowing)
                i.Info.description += "\r\nBullet Type: Slowing";

            i.Info.description +=  "\r\nDamage: " + i.Info.damage + "\r\nRange: " + i.Info.range + "\r\nFire Rate: " + i.Info.fireRate + "\r\nShot Speed: " + i.Info.shotSpeed ;

        }
        
    }

	private void InstantiateMoney(Vector3 pos)
	{
		GameObject money = Instantiate(sceneItemPrefab, pos, Quaternion.identity) as GameObject;
		si = money.GetComponent<SceneItem>();
		render = money.GetComponent<SpriteRenderer>();
		si.Info = new ItemStats();
		si.Info.itemType = ItemStats.ItemType.money;
		si.GetComponent<SpriteRenderer>().sortingLayerName = "Items";
		si.Info.itemName = "Money";

		if (rnd.Next(100) < 75)
		{
			si.Info.moneyAmount = 5;
			si.Info.sprite = select.money[rnd.Next(0,3)];
		}
		else if (rnd.Next(100) < 75)
		{
			si.Info.moneyAmount = 10;
			si.Info.sprite = select.money[rnd.Next(3,6)];
		}
		else
		{
			si.Info.moneyAmount = 20;
			si.Info.sprite = select.money[rnd.Next(6,9)];
		}
		si.GetComponent<SpriteRenderer>().sprite = si.Info.sprite;

		Color color = render.color;
		color.a = 0;
		render.color = color;

		StartCoroutine(GameManager.manager.lvlManager.FadeIn(render, 0.3f));
	}

    private void InstantiateKey(Vector3 pos)
    {
        GameObject key = Instantiate(sceneItemPrefab, pos, Quaternion.identity) as GameObject;
        si = key.GetComponent<SceneItem>();
        render = key.GetComponent<SpriteRenderer>();
        si.Info = new ItemStats();
        si.Info.itemType = ItemStats.ItemType.key;
        si.Info.sprite = select.keys[rnd.Next(select.keys.Count)];
        si.GetComponent<SpriteRenderer>().sprite = si.Info.sprite;
        si.GetComponent<SpriteRenderer>().sortingLayerName = "Items";
        si.Info.itemName = "Boss Key";
        //imposto i consumable come trasparenti, per poi fare un effetto di fade-in quando verranno attivati 
        Color color = render.color;
        color.a = 0;
        render.color = color;

        StartCoroutine(GameManager.manager.lvlManager.FadeIn(render, 0.3f));
    }

    //------------------------------
    //Effetti relativi ai consumables

    public void ApplyEffect(ItemStats.ConsumableType consumable)
    {
        Room actualRoom = GameManager.manager.ActualRoom;

        switch (consumable)
        {
            case ItemStats.ConsumableType.healthUp25:
                ph.HealthUp(25);
                break;

            case ItemStats.ConsumableType.healthUp50:
                ph.HealthUp(50);
                break;

            case ItemStats.ConsumableType.slowDownAll:
                foreach (GameObject en in actualRoom.enemies)
                {
                    EnemyHealth eh = en.GetComponent<EnemyHealth>();
                    if (eh.faster)
                        eh.SetNormalSpeed();
                    else
                        //chiamo startCoroutine in enemyHealth per poterlo fermare da lì, in modo da non ottenere il bug di Unity "Coroutine continue failure"
                        eh.slowCO = eh.StartCoroutine(eh.SlowDown());
                }
                break;

            case ItemStats.ConsumableType.speedUpAll:
                foreach (GameObject en in actualRoom.enemies)
                {
                    EnemyHealth eh = en.GetComponent<EnemyHealth>();
                    if (eh.slowed)
                        eh.SetNormalSpeed();
                    else
                        eh.fastCO = eh.StartCoroutine(eh.SpeedUp());
                }
                break;

            case ItemStats.ConsumableType.poisonAll:
                foreach (GameObject en in actualRoom.enemies)
                {
                    EnemyHealth eh = en.GetComponent<EnemyHealth>();
                    eh.poisonCO = eh.StartCoroutine(eh.Poisoned());
                }
                break;

            case ItemStats.ConsumableType.poisonSelf:
                ph.poisonCO = ph. StartCoroutine(ph.Poisoned());
                break;

            case ItemStats.ConsumableType.damageAll:
                foreach (GameObject en in actualRoom.enemies)
                {
                    en.GetComponent<EnemyHealth>().TakeDamage(20);
                }
                break;

            case ItemStats.ConsumableType.damageSelf:
                ph.ConsumableDamage(20);
                break;

			case ItemStats.ConsumableType.flipMovement:
                ph.flipMovCO = ph.StartCoroutine(ph.FlipMovement());
                break;

            case ItemStats.ConsumableType.invincible:
                ph.invCO = ph.StartCoroutine(ph.Invincible());
                break;

            case ItemStats.ConsumableType.getDoubleDamage:
                //se il player è invincibile il consumable non ha effetto
                if (!ph.invincible)
                    ph.gddCO = ph.StartCoroutine(ph.GetDoubleDamage());
                break;

            case ItemStats.ConsumableType.slowDownSelf:
                if (ph.faster)
                    ph.SetNormalSpeed();
                else
                    ph.slowDownCO = ph.StartCoroutine(ph.SlowDown());
                break;

            case ItemStats.ConsumableType.speedUpSelf:
                if (ph.slower)
                    ph.SetNormalSpeed();
                else
                    ph.speedUpCO = ph.StartCoroutine(ph.SpeedUp());
                break;

            case ItemStats.ConsumableType.doubleDamage:
                //non posso duplicare più volte il danno
                if (!ph.dd)
                {
                    if (ph.hd)
                        ph.SetNormalDamage();
                    else
                        ph.ddCO = ph.StartCoroutine(ph.DoubleDamage());
                }
                break;

            case ItemStats.ConsumableType.halfDamage:
                if (!ph.hd)
                {
                    if (ph.dd)
                        ph.SetNormalDamage();
                    else
                        ph.hdCO = ph.StartCoroutine(ph.HalfDamage());
                }
                break;

            case ItemStats.ConsumableType.flipAttack:
                ph.flipAttCO = ph.StartCoroutine(ph.FlipAttack());
                break;
        }
    }

	private void InitializeConsumables()
	{
		foreach (Item c in GameStats.stats.consumablesSO)
		{
			ItemStats item = new ItemStats();

			if (c.itemName == "Little Health Potion" || c.itemName == "Big Health Potion")
			{
				item.itemName = c.itemName;
				item.sprite = c.icon;
			}
			else
			{
				if (rnd.Next(2) == 0)
				{
					item.itemName = "Book of " + c.itemName;
					item.sprite = select.books[rnd.Next(select.books.Count)];
				}
				else
				{
					item.itemName = "Scroll of " + c.itemName;
					item.sprite = select.scrolls[rnd.Next(select.scrolls.Count)];
				}
			}
			item.description = c.itemDescription;
			item.itemType = c.type;
			item.consumableType = c.consumableType;
			GameStats.stats.consumables.Add(item);
		}
	}

	private void SetStartingWeapons()
	{
		//imposto l'arma meele iniziale
		ItemStats startingMeele = new ItemStats();
		startingMeele.itemType = startingMeeleSO.type;
		startingMeele.weaponType = startingMeeleSO.weaponType;
		startingMeele.itemName = startingMeeleSO.itemName;
		
		startingMeele.damage = (float)Math.Truncate(((minMeeleDmg + maxMeeleDmg) / 2) * 10) / 10;
		startingMeele.description = "Damage: " + startingMeele.damage;

		startingMeele.sprite = startingMeeleSO.icon;

		//imposto l'arma ranged iniziale
		ItemStats startingRanged = new ItemStats();
		startingRanged.itemType = startingRangedSO.type;
		startingRanged.weaponType = startingRangedSO.weaponType;
		startingRanged.itemName = startingRangedSO.itemName;

		startingRanged.damage = (float)Math.Truncate(((minRangedDmg + maxRangedDmg) / 2) * 10) / 10;
		startingRanged.range = (float)Math.Truncate(((minRng + maxRng) / 2) * 10) / 10;
		startingRanged.fireRate = (float)Math.Truncate(((minFR + maxFR) / 2) * 10) / 10;
		startingRanged.shotSpeed = (float)Math.Truncate(((minSP + maxSP) / 2) * 10) / 10;

		startingRanged.description = "Fire Type: Single Shot \r\nBullet Type: Standard \r\nDamage: " + startingRanged.damage + "\r\nRange: " + startingRanged.range
			+ "\r\nFire Rate: " + startingRanged.fireRate + "\r\nShot Speed: " + startingRanged.shotSpeed;
		startingRanged.sprite = startingRangedSO.icon;

		Inventory.instance.AddSlot(startingMeele);
		Inventory.instance.AddSlot(startingRanged);

		//equipaggio l'arma nel primo slot
		GameStats.stats.index = 0;
		GetComponent<Weapon>().EquipWeapon(GameStats.stats.itemList[GameStats.stats.index]);
		GameStats.stats.equippedSlot = GameStats.stats.index;
	}
}
