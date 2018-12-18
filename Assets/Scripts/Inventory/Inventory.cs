using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public Text text;
    public GameObject options;
    public int index;

    public GameObject inventoryUI;
    public GameObject button;

    public static bool GameIsPaused = false;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (GameIsPaused)
                ResumeI();
            else
                PauseI();
        }

    }

    public void ResumeI()
    {
        inventoryUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        GameManager.manager.gamePause = false;
    }

    public void PauseI()
    {
        inventoryUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        GameManager.manager.gamePause = true;
        EventSystem.current.SetSelectedGameObject(button);
    }

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


    private void ResetAllSlots()
    {
        for (int i = 0; i < itemList.Length; i++)
        {
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), itemList[i]);
        }
    }

    public void DropItem()
    {
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), itemList[index]);
        UpdateSlotUI();
        HideMenu();
    }

    public void ShowMenu(int i)
    {
        options.SetActive(true);
        text.text = itemList[i].itemName;
        index = i;
    }

    public void HideMenu()
    {
        options.SetActive(false);
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
