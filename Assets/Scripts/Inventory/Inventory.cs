using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public static Inventory instance;
    public Item[] itemList = new Item[20];
    public InventorySlot[] inventorySlots = new InventorySlot[20];

    private bool Add(Item item)
    {
        for(int i = 0; i < itemList.Length; i++)
        {
            if(itemList[i] == null)
            {
                itemList[i] = item;
                return true;
            }
        }
        return false;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            if(instance != this)
            {
                Destroy(this);
            }
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].UpdateSlot();
        }
    }

    public void AddSlot(Item item)
    {
        bool hasAdded = Add(item);
        if (hasAdded)
        {
            UpdateSlotUI();
        }
    }
}
