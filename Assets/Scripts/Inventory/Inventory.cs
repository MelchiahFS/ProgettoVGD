﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    public static Inventory instance;

    //public Item[] itemList = new Item[20];
    //public InventorySlot[] inventorySlots = new InventorySlot[20];

    //public Item emptySlot;

    //private bool Add(Item item)
    //{
    //    for (int i = 0; i < itemList.Length; i++)
    //    {
    //        if(itemList[i].itemName == item.itemName)
    //        {
    //            if(itemList[i].currentStack < itemList[i].maxStack)
    //            {
    //                itemList[i].currentStack++;
    //                UpdateSlotUI();
    //                return true;
    //            }
    //        }
    //    }
    //        for (int i = 0; i < itemList.Length; i++)
    //    {
    //        if(itemList[i].type == ItemStats.ItemType.emptyslot)
    //        {
    //            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(item), itemList[i]);
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    //private void Awake()
    //{
    //    if(instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        if(instance != this)
    //        {
    //            Destroy(this);
    //        }
    //    }
    //    DontDestroyOnLoad(this);
    //}

    //private void Start()
    //{
    //    UpdateSlotUI();
    //    ResetAllSlots();
    //}

    //private void ResetAllSlots()
    //{
    //    for(int i = 0; i < itemList.Length; i++)
    //    {
    //        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), itemList[i]);
    //    }
    //}

    //public void UpdateSlotUI()
    //{
    //    for(int i = 0; i < inventorySlots.Length; i++)
    //    {
    //        inventorySlots[i].UpdateSlot();
    //    }
    //}

    //public void AddSlot(Item item)
    //{
    //    bool hasAdded = Add(item);
    //    if (hasAdded)
    //    {
    //        UpdateSlotUI();
    //    }
    //}

    public ItemStats[] itemList = new ItemStats[20];
    public InventorySlot[] inventorySlots = new InventorySlot[20];

    public GameObject go;
    private ItemStats emptySlot;

    private bool Add(ItemStats item)
    {
        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i].itemName == item.itemName)
            {
                if (itemList[i].currentStack < itemList[i].maxStack)
                {
                    itemList[i].currentStack++;
                    UpdateSlotUI();
                    return true;
                }
            }
        }
        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i].itemType == ItemStats.ItemType.emptyslot)
            {
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(item), itemList[i]);
                return true;
            }
        }
        return false;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(this);
            }
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        emptySlot = new ItemStats();
        emptySlot.itemType = ItemStats.ItemType.emptyslot;
        InitializeItemList();
        UpdateSlotUI();
        ResetAllSlots();
    }

    private void ResetAllSlots()
    {
        for (int i = 0; i < itemList.Length; i++)
        {
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), itemList[i]);
        }
    }

    public void UpdateSlotUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].UpdateSlot(itemList[i]);
        }
    }

    public void AddSlot(ItemStats info)
    {
        bool hasAdded = Add(info);
        if (hasAdded)
        {
            UpdateSlotUI();
        }
    }

    private void InitializeItemList()
    {
        for (int i = 0; i < itemList.Length; i++)
        {
            itemList[i] = new ItemStats();
            itemList[i].itemType = ItemStats.ItemType.emptyslot;
        }
    }

}
