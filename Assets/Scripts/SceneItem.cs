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

    private AudioSource source;

    void Start()
    {
        source = GameObject.Find("InventoryHandler").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (contact)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (info.toBuy)
                {
                    PlayerHealth ph = GameObject.Find("Player").GetComponent<PlayerHealth>();
                    
                    //if (ph.playerMoney >= info.price)
                    if (GameManager.manager.playerMoney >= info.price)
                    {
                        ItemStats item = new ItemStats();
                        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(info), item);
                        if (Inventory.instance.AddSlot(item))
                        {
                            GameManager.manager.playerMoney -= info.price;
                            //ph.playerMoney -= info.price;
                            ph.gameObject.GetComponentInChildren<Text>().text = GameManager.manager.playerMoney.ToString();
                            //ph.gameObject.GetComponentInChildren<Text>().text = ph.playerMoney.ToString();
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
                    if (info.itemType != ItemStats.ItemType.key)
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
                    else
                    {
                        GameObject player = GameObject.Find("Player");
                        GameObject container = player.transform.Find("HealthBar").gameObject;
                        GameObject key = container.transform.Find("Key").gameObject;
                        key.GetComponent<Image>().sprite = info.sprite;
                        key.GetComponent<Image>().enabled = true;

                        source.PlayOneShot(Inventory.instance.pickKey);

                        player.GetComponent<RoomChange>().hasKey = true;
                        Destroy(gameObject);
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
