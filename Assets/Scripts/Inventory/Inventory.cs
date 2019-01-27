using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour {

    public static Inventory instance;
    public LootGenerator loot;
    public Weapon weapon;

    //public ItemStats[] itemList = new ItemStats[20];
    public InventorySlot[] inventorySlots = new InventorySlot[20];

    private ItemStats emptySlot;
    
    public int index;
    private int count;
    public Text itemName;  //Nome
    public Text use; //Tasto Usa - Equipaggia
    public Text isWeaponEquipped; //L'arma è equipaggiata?
    public Text description; //Descrizione
    private int equippedSlot;
    private bool equipped;
    private bool menuShown;

    public GameObject inventoryMenu; //menu interno dell'inventario
    public GameObject inventoryUI; //inventario
    public GameObject button; //primo oggetto dell'inventario (in alto a sinistra)
    public GameObject buttonMenu; //primo tasto del menu dell'inventario
    private GameObject lastButton; //ultimo tasto premuto
    private GameObject lastInventoryButton; //ultimo oggetto selezionato dell'inventario

    public AudioClip pickKey, pickItem, buyItem, useItem, dropItem, enter, exit, move, select, empty;
    private AudioSource source;

    public static bool inventoryActive = false;



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

    void Start()
    {
        menuShown = false;
        isWeaponEquipped.text = "";
        emptySlot = new ItemStats();
        emptySlot.itemType = ItemStats.ItemType.emptyslot;
        //InitializeItemList();
        UpdateSlotUI();
        if(IsInventoryEmpty())
            ResetAllSlots();
        Debug.Log("inventory inizializza armi");
        loot = GameObject.Find("EquippedWeapon").GetComponent<LootGenerator>();
        weapon = GameObject.Find("EquippedWeapon").GetComponent<Weapon>();
        source = GetComponent<AudioSource>();
        count = 0;
    }

    private bool IsInventoryEmpty()
    {
		for (int i = 0; i < GameStats.stats.itemList.Length; i++)
		{
			if (GameStats.stats.itemList[i].itemType != ItemStats.ItemType.emptyslot)
				return false;
        }
        return true;
    }

    void Update()
    {
        //prevengo che sia possibile aprire l'inventario se il menu di pausa è attivo
        if (!GameManager.manager.pauseMenuActive)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (inventoryActive)
                {
                    ResumeI();
                    source.PlayOneShot(exit);
                }
                else
                {
                    PauseI();
                }
            }

            //se il menu dell'inventario è attivo
            if (menuShown)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                {
                    HideMenu();
                    source.PlayOneShot(exit);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
                {
                    source.PlayOneShot(move);
                }
            }
            else if (inventoryActive)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
                    Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    source.PlayOneShot(move);
                }
            }
        }


        if (inventoryActive /*|| menuShown*/)     //DA FARE: controllare che resti selezionato l'oggetto dell'inventario giusto entrando nel menu dell'inventario
        {

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
        inventoryActive = false;
        menuShown = false;
        GameManager.manager.gamePause = false;
        GameManager.manager.inventoryActive = false;
        inventoryMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void PauseI()
    {
        inventoryUI.SetActive(true);
        Time.timeScale = 0f;
        inventoryActive = true;
        GameManager.manager.gamePause = true;
        GameManager.manager.inventoryActive = true;
        EventSystem.current.SetSelectedGameObject(button);
        source.PlayOneShot(enter);
    }

    public void Use()
    {
		if (GameStats.stats.itemList[index].itemType == ItemStats.ItemType.consumable)
		{
			loot.ApplyEffect(GameStats.stats.itemList[index].consumableType);
			
			if (GameStats.stats.itemList[index].currentStack == 1)
			{
				JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), GameStats.stats.itemList[index]);
			}
            else
            {
				GameStats.stats.itemList[index].currentStack--;
			}
            source.PlayOneShot(useItem);
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
		weapon.EquipWeapon(GameStats.stats.itemList[index]);
		GameStats.stats.equippedSlot = index;
	}

    private void Add(ItemStats item)
    {
		for (int i = 0; i < GameStats.stats.itemList.Length; i++)
		{
			if (GameStats.stats.itemList[i].itemName == item.itemName && GameStats.stats.itemList[i].itemType != ItemStats.ItemType.weapon) //se ci sono già oggetti identici
			{
				if (GameStats.stats.itemList[i].currentStack < GameStats.stats.itemList[i].maxStack) //controllo che lo slot non sia pieno
				{
					GameStats.stats.itemList[i].currentStack++;
					UpdateSlotUI();
					return;
				}
			}
			else if (GameStats.stats.itemList[i].itemType == ItemStats.ItemType.emptyslot) //se invece lo slot è vuoto aggiungo qui l'oggetto
			{
				JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(item), GameStats.stats.itemList[i]);
				count++;
				return;
			}
		}
	}

    private void ResetAllSlots()
    {
		for (int i = 0; i < GameStats.stats.itemList.Length; i++)
		{
			JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), GameStats.stats.itemList[i]);
		}
	}

    public void DropItem()
    {
		if (GameStats.stats.itemList[index].currentStack == 1)
		{
			JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), GameStats.stats.itemList[index]);
			count--;
		}
		else
		{
			GameStats.stats.itemList[index].currentStack--;
		}
		source.PlayOneShot(dropItem);
        UpdateSlotUI();
        HideMenu();
    }

    public void ShowMenu(int i)
    {
		if (GameStats.stats.itemList[i].itemType != ItemStats.ItemType.emptyslot)
		{
            lastInventoryButton = EventSystem.current.currentSelectedGameObject;
            menuShown = true;
            GameManager.manager.inventoryMenuActive = true;
            inventoryMenu.SetActive(true);
			
			if (GameStats.stats.itemList[i].itemType == ItemStats.ItemType.consumable)
			{
                use.text = "Use";
            }
            else
            {
                use.text = "Equip";
            }
			
			if (i == GameStats.stats.equippedSlot)
				isWeaponEquipped.text = "Equipped";
            else
                isWeaponEquipped.text = "";

            EventSystem.current.SetSelectedGameObject(buttonMenu);
			description.text = GameStats.stats.itemList[i].description;
			itemName.text = GameStats.stats.itemList[i].itemName;
			index = i;
            source.PlayOneShot(enter);
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            source.PlayOneShot(empty);
        }
    }

    public void HideMenu()
    {
        menuShown = false;
        GameManager.manager.inventoryMenuActive = false;
        inventoryMenu.SetActive(false);
        //EventSystem.current.SetSelectedGameObject(button);
        EventSystem.current.SetSelectedGameObject(lastInventoryButton);
    }

    public void PlayExitSound()
    {
        source.PlayOneShot(exit);
    }

    public void UpdateSlotUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
			inventorySlots[i].UpdateSlot(GameStats.stats.itemList[i]);
		}
    }

    //public void AddSlot(ItemStats info)
    //{
    //    if (count < 20)
    //    {
    //        bool hasAdded = Add(info);
    //        if (hasAdded)
    //        {
    //            //playerStats.Inizializate(info);
    //            UpdateSlotUI();
    //            source.PlayOneShot(pickItem);
    //        }
    //    }
    //    else
    //    {
    //        source.PlayOneShot(empty);
    //            Debug.Log("inventario pieno");
    //    }
    //}

    public bool AddSlot(ItemStats info)
    {
        if (count < 20)
        {
            Add(info);

            //playerStats.Inizializate(info);
            UpdateSlotUI();

            return true;
        }
        else
        {
            //source.PlayOneShot(empty);
            //Debug.Log("inventario pieno");
            return false;
        }
    }

    //private void InitializeItemList()
    //{
    //    for (int i = 0; i < GameManager.manager.itemList.Length; i++)
    //    {
    //        GameManager.manager.itemList[i] = new ItemStats();
    //        GameManager.manager.itemList[i].itemType = ItemStats.ItemType.emptyslot;
    //    }
    //}

}

