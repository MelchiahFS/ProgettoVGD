using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour {

    public static Inventory instance;
    public LootGenerator loot;
    public Weapon weapon;

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
    private int index;
    private int count;
    private bool isEquipped;
    public Text text1;
    public Text text2;
    public Text text3;
    private int equippedSlot;
    private bool equipped;
    private bool menuShown;

    public GameObject inventoryUI;
    public GameObject button;
    public GameObject buttonMenu;
    private GameObject lastButton;

    public static bool GameIsPaused = false, inventoryActive = false;


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
        //DontDestroyOnLoad(this);
    }

    private void Start()
    {
        menuShown = false;
        text2.text = "";
        equippedSlot = 21;
        emptySlot = new ItemStats();
        emptySlot.itemType = ItemStats.ItemType.emptyslot;
        InitializeItemList();
        UpdateSlotUI();
        ResetAllSlots();
        loot = GameObject.Find("EquippedWeapon").GetComponent<LootGenerator>();
        weapon = GameObject.Find("EquippedWeapon").GetComponent<Weapon>();
        count = 0;
        isEquipped = false;
    }

    void Update()
    {
        if (!GameManager.manager.pauseMenuActive)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (GameIsPaused)
                    ResumeI();
                else
                    PauseI();
            }
            else if (inventoryActive && Input.GetKeyDown(KeyCode.RightArrow))
            {
                HideMenu();
            }
        }
        

        if (GameIsPaused || inventoryActive)
        {
            if (menuShown)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                    HideMenu();
            }

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastButton);
            }
            else
            {
                lastButton = EventSystem.current.currentSelectedGameObject;
            }
        }
    }

    public void ResumeI()
    {
        inventoryUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        GameManager.manager.gamePause = false;
        GameManager.manager.inventoryActive = false;
        options.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void PauseI()
    {
        inventoryUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        GameManager.manager.gamePause = true;
        GameManager.manager.inventoryActive = true;
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void Use()
    {
        if (itemList[index].itemType == ItemStats.ItemType.consumable)
        {
            loot.ApplyEffect(itemList[index].consumableType);
            if (itemList[index].currentStack == 1)
            {
                JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), itemList[index]);
            }
            else
            {
                itemList[index].currentStack--;
            }
        }
        else
        {
            Equip();
        }
        UpdateSlotUI();
        HideMenu();
        ResumeI();
    }

    public void Equip()
    {
        weapon.EquipWeapon(itemList[index]);
        equippedSlot = index;
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
                    count++;
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
        if (itemList[index].currentStack == 1)
        {
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), itemList[index]);
            count--;
        }
        else
        {
            itemList[index].currentStack--;
        }
        UpdateSlotUI();
        HideMenu();
    }

    public void ShowMenu(int i)
    {
        menuShown = true;
        if (itemList[i].itemType != ItemStats.ItemType.emptyslot)
        {
            inventoryActive = true;
            options.SetActive(true);
            if (itemList[i].itemType == ItemStats.ItemType.consumable)
            {
                text1.text = "Use";
                text3.text = "";
            }
            else
            {
                text1.text = "Equip";
                text3.text = "dmg:" + itemList[i].damage + "range:" + itemList[i].range;
            }

            if (i == equippedSlot)
                text2.text = "Equipped";
            else
                text2.text = "";

            EventSystem.current.SetSelectedGameObject(buttonMenu);
            text.text = itemList[i].itemName;
            index = i;
        }
        else
            return;
    }

    public void HideMenu()
    {
        menuShown = false;
        inventoryActive = false;
        options.SetActive(false);
        EventSystem.current.SetSelectedGameObject(button);
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
        if (count < 20)
        {
            bool hasAdded = Add(info);
            if (hasAdded)
            {
                UpdateSlotUI();
            }
        }
        else
        {
                Debug.Log("inventario pieno");
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
