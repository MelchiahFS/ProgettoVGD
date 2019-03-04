using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]

public class SceneItem : MonoBehaviour
{
    public Sprite sprite; //sprite dell'oggetto
    private ItemStats info; //informazioni che definiscono l'oggetto
    private bool contact = false; //indica se il player è nel range dell'oggetto per poterlo prendere
	private GameObject player;
	private Text money;

    private AudioSource source;

    void Start()
    {
        source = GameObject.Find("InventoryHandler").GetComponent<AudioSource>();
		player = GameManager.manager.playerReference;

		//recupero il riferimento al testo raffigurante i soldi del player
		foreach (Text t in player.GetComponentsInChildren<Text>())
		{
			if (t.gameObject.name == "Money")
				money = t;
		}
    }

    void Update()
    {
		//se il player è nel range d'azione dell'oggetto
        if (contact)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
				//se è un oggetto da acquistare
                if (info.toBuy)
                {					
					//se ha abbastanza soldi
					if (GameStats.stats.playerMoney >= info.price)
					{
						//salvo una copia delle info dell'oggetto
                        ItemStats item = new ItemStats();
                        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(info), item);

						//se c'è posto nell'inventario lo aggiungo e aggiorno la quantità di soldi del player
                        if (Inventory.instance.AddSlot(item))
                        {
							GameStats.stats.playerMoney -= info.price;
							
							money.text = "$ " + GameStats.stats.playerMoney.ToString();

							source.PlayOneShot(Inventory.instance.buyItem);

							//infine distruggo l'oggetto di scena
                            Destroy(gameObject);
                        }
						//se non c'è posto non faccio nulla
                        else
                        {
                            source.PlayOneShot(Inventory.instance.empty);
                        }

                    }
					//se non ha abbastanza soldi non faccio nulla
                    else
                    {
                        source.PlayOneShot(Inventory.instance.empty);
                    }
                }
				//se non è un oggetto in vendita
                else
                {
					//se l'oggetto è la chiave della boss room
					if (info.itemType == ItemStats.ItemType.key)
					{
						//mostro la chiave in basso a sinistra nella schermata di gioco
						GameObject container = player.transform.Find("HealthBar").gameObject;
						GameObject key = container.transform.Find("Key").gameObject;
						key.GetComponent<Image>().sprite = info.sprite;
						key.GetComponent<Image>().enabled = true;

						source.PlayOneShot(Inventory.instance.pickKey);

						//segnalo il possesso della chiave
						player.GetComponent<RoomChange>().hasKey = true;
						Destroy(gameObject);

					}
					else
					{
						//se invece sono soldi incremento la quantità di soldi del player
						if (info.itemType == ItemStats.ItemType.money)
						{
							GameStats.stats.playerMoney += info.moneyAmount;

							money.text = "$ " + GameStats.stats.playerMoney.ToString();

							source.PlayOneShot(Inventory.instance.buyItem);
							Destroy(gameObject);
						}
						//altrimenti, se c'è posto nell'inventario aggiungo l'oggetto
						else
						{
							ItemStats item = new ItemStats();
							JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(info), item);
							if (Inventory.instance.AddSlot(item))
							{
								source.PlayOneShot(Inventory.instance.pickItem);
								Destroy(gameObject);
							}
							else
							{
								source.PlayOneShot(Inventory.instance.empty);
							}
						}
						
					}
				}

            }
        }
        
    }

	//Segnala che il player può prendere l'oggetto
    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            contact = true;
            
        }
    }

	//Segnala che il player è fuori dal range dell'oggetto
    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            contact = false;

        }
    }

	//Salva e restituisce le informazioni dell'oggetto
	public ItemStats Info
	{
		get { return info; }
		set { info = value; }
	}

}
