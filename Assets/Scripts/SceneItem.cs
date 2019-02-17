using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]

public class SceneItem : MonoBehaviour
{

    public Sprite sprite;
    private ItemStats info;
    private bool contact = false;
	private GameObject player;
	private Text money;

    private AudioSource source;

    void Start()
    {
        source = GameObject.Find("InventoryHandler").GetComponent<AudioSource>();
		player = GameObject.Find("Player");
		foreach (Text t in player.GetComponentsInChildren<Text>())
		{
			if (t.gameObject.name == "Money")
				money = t;
		}
    }

    void Update()
    {
        if (contact)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (info.toBuy)
                {					
					if (GameStats.stats.playerMoney >= info.price)
					{
                        ItemStats item = new ItemStats();
                        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(info), item);
                        if (Inventory.instance.AddSlot(item))
                        {
							GameStats.stats.playerMoney -= info.price;
							
							money.text = "$" + GameStats.stats.playerMoney.ToString();

							source.PlayOneShot(Inventory.instance.buyItem);
                            Destroy(gameObject);
                        }
                        else
                        {
                            source.PlayOneShot(Inventory.instance.empty);
                        }

                    }
                    else
                    {
                        source.PlayOneShot(Inventory.instance.empty);
                    }
                }
                else
                {

					if (info.itemType == ItemStats.ItemType.key)
					{
						GameObject container = player.transform.Find("HealthBar").gameObject;
						GameObject key = container.transform.Find("Key").gameObject;
						key.GetComponent<Image>().sprite = info.sprite;
						key.GetComponent<Image>().enabled = true;

						source.PlayOneShot(Inventory.instance.pickKey);

						player.GetComponent<RoomChange>().hasKey = true;
						Destroy(gameObject);

					}
					else
					{
						if (info.itemType == ItemStats.ItemType.money)
						{
							GameStats.stats.playerMoney += info.moneyAmount;

							money.text = "$" + GameStats.stats.playerMoney.ToString();

							source.PlayOneShot(Inventory.instance.buyItem);
							Destroy(gameObject);
						}
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

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            contact = true;
            
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            contact = false;

        }
    }

    public ItemStats Info
    {
        get { return info; }
        set { info = value; }
    }

}
