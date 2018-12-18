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

    public ItemStats Info
    {
        get { return info; }
        set { info = value; }
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
                    if (ph.playerMoney >= info.price)
                    {
                        ph.playerMoney -= info.price;
                        ph.gameObject.GetComponentInChildren<Text>().text = ph.playerMoney.ToString();
                        ItemStats item = new ItemStats();
                        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(info), item);
                        Inventory.instance.AddSlot(item);
                        Destroy(gameObject);
                    }
                }
                else
                {
                    ItemStats item = new ItemStats();
                    JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(info), item);
                    Inventory.instance.AddSlot(item);
                    Destroy(gameObject);
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

}
