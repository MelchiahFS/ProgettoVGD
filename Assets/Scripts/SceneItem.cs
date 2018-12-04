using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                //Debug.Log(info.itemName);
                //Debug.Log(info.description);
                ItemStats item = new ItemStats();
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(info), item);
                Inventory.instance.AddSlot(item);
                Destroy(gameObject);
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
