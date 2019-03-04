using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour {

    public static Inventory instance;

    public LootGenerator loot;
    public Weapon weapon;

    public InventorySlot[] inventorySlots = new InventorySlot[20];

    private ItemStats emptySlot;
    
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

	public GameObject dropButton;

    public AudioClip pickKey, pickItem, buyItem, useItem, dropItem, enter, exit, move, select, empty;
    private AudioSource source;

    public bool inventoryActive = false;

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
		
		source = GetComponent<AudioSource>();
	}

	void Start()
    {
		menuShown = false;
        isWeaponEquipped.text = "";

		//creo un oggetto di tipo slot vuoto
        emptySlot = new ItemStats();
        emptySlot.itemType = ItemStats.ItemType.emptyslot;

		//se l'inventario è vuoto imposto tutti gli slot con degli oggetti "slot vuoto"
		if (IsInventoryEmpty())
		{
			ResetAllSlots();
		}
		//aggiorno la visualizzazione dell'inventario
		UpdateSlotUI();
        
		
	}


    void Update()
    {
		//impedisco qualsiasi operazione sull'inventario se il player è morto o se la scena attuale sta cambiando
		if (!GameManager.manager.isDying && !GameManager.manager.ending)
		{
			//impedisco di aprire l'inventario se il menu di pausa è attivo o se il player sta leggendo un cartello
			if (!GameManager.manager.pauseMenuActive && !GameManager.manager.signboardActive)
			{
				//se l'inventario è già aperto lo chiudo, altrimenti lo apro
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
					//posso chiuderlo muovendo verso destra
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
				//altrimenti se è attiva solo la schermata principale dell'inventario
				else if (inventoryActive)
				{
					if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
						Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
					{
						source.PlayOneShot(move);
					}
				}
			}

			//reimposto la selezione dei tasti per evitare la perdita di focus
			if (inventoryActive)
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
	}
	
	//Controlla se l'inventario è vuoto
	private bool IsInventoryEmpty()
	{
		for (int i = 0; i < GameStats.stats.itemList.Length; i++)
		{
			if (GameStats.stats.itemList[i].itemType != ItemStats.ItemType.emptyslot)
				return false;
		}
		return true;
	}

	//chiude l'inventario
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

	//apre l'inventario
    public void PauseI()
    {
        inventoryUI.SetActive(true);
        Time.timeScale = 0f;
        inventoryActive = true;
        GameManager.manager.gamePause = true;
        GameManager.manager.inventoryActive = true;
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(button);
        source.PlayOneShot(enter);
    }

	//utilizza un oggetto o equipaggia un'arma
    public void Use()
    {
		//se l'oggetto selezionato è un consumable
		if (GameStats.stats.itemList[GameStats.stats.index].itemType == ItemStats.ItemType.consumable)
		{

			//setto il consumable come già utilizzato, in modo da mostrarne le info nel menu
			ItemStats s = GameStats.stats.consumables.Find(x => x.itemName.Equals(GameStats.stats.itemList[GameStats.stats.index].itemName));
			s.used = true;

			//applica gli effetti
			loot = GameManager.manager.playerReference.GetComponentInChildren<LootGenerator>();
			loot.ApplyEffect(GameStats.stats.itemList[GameStats.stats.index].consumableType);
			
			//se c'è solo un oggetto nello stack lo rimuovo dall'inventario
			if (GameStats.stats.itemList[GameStats.stats.index].currentStack == 1)
			{
				JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), GameStats.stats.itemList[GameStats.stats.index]);
			}
			//altrimenti decremento lo stack
            else
            {
				GameStats.stats.itemList[GameStats.stats.index].currentStack--;
			}
            source.PlayOneShot(useItem);
        }
		//se invece è un'arma la equipaggio
        else
        {
            Equip();
        }
        UpdateSlotUI();
        HideMenu();
        ResumeI();
    }

	//equipaggia un'arma
    public void Equip()
    {
		weapon = GameManager.manager.playerReference.GetComponentInChildren<Weapon>();
		weapon.EquipWeapon(GameStats.stats.itemList[GameStats.stats.index]);
		GameStats.stats.equippedSlot = GameStats.stats.index;
	}

	//se c'è posto aggiunge uno slot all'inventario, in caso contrario restituisce false
	public bool AddSlot(ItemStats info)
	{
		if (GameStats.stats.count < 20)
		{
			Add(info);
			UpdateSlotUI();

			return true;
		}
		else
		{
			return false;
		}
	}

	//aggiunge un oggetto all'inventario
	private void Add(ItemStats item)
    {
		for (int i = 0; i < GameStats.stats.itemList.Length; i++)
		{
			//se ci sono già oggetti identici
			if (GameStats.stats.itemList[i].itemName == item.itemName && GameStats.stats.itemList[i].itemType != ItemStats.ItemType.weapon) 
			{
				//controllo che lo slot non sia pieno
				if (GameStats.stats.itemList[i].currentStack < GameStats.stats.itemList[i].maxStack) 
				{
					GameStats.stats.itemList[i].currentStack++;
					UpdateSlotUI();
					return;
				}
			}
			//se invece lo slot è vuoto aggiungo qui l'oggetto
			else if (GameStats.stats.itemList[i].itemType == ItemStats.ItemType.emptyslot)
			{
				JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(item), GameStats.stats.itemList[i]);
				GameStats.stats.count++;
				return;
			}
		}
	}

	//mostra il menu delle opzioni disponibili per l'oggetto selezionato
    public void ShowMenu(int i)
    {
		//se nello slot selezionato c'è un oggetto
		if (GameStats.stats.itemList[i].itemType != ItemStats.ItemType.emptyslot)
		{
            lastInventoryButton = EventSystem.current.currentSelectedGameObject;
            menuShown = true;
            GameManager.manager.inventoryMenuActive = true;
            inventoryMenu.SetActive(true);
			
			//imposto il testo adatto al tipo di oggetto
			if (GameStats.stats.itemList[i].itemType == ItemStats.ItemType.consumable)
			{
                use.text = "Use";
            }
            else
            {
                use.text = "Equip";
            }
			
			//se l'oggetto attuale è l'arma equipaggiata impedisco di dropparla (in modo da obbligare il player ad avere sempre almeno un'arma in mano)
			if (i == GameStats.stats.equippedSlot)
			{
				isWeaponEquipped.text = "Equipped";
				dropButton.SetActive(false);
			}
			//altrimenti può droppare l'oggetto
            else
			{
				isWeaponEquipped.text = "";
				dropButton.SetActive(true);
			}


			//solo se il consumable è già stato utilizzato mostro le informazioni
			if (GameStats.stats.itemList[i].itemType == ItemStats.ItemType.consumable)
			{
				//controllo se il tipo di consumable è già stato precedentemente utilizzato
				if (GameStats.stats.consumables.Find(x => x.itemName.Equals(GameStats.stats.itemList[i].itemName)).used)
				{
					itemName.text = GameStats.stats.itemList[i].itemName;
					description.text = GameStats.stats.itemList[i].description;
				}
				//se non è mai stato utilizzato non stampo le effettive informazioni
				else
				{
					itemName.text = "???";
					description.text = "Unknown effects";
				}
			}
			//se non è un consumable stampo le informazioni dell'arma
			else
			{
				itemName.text = GameStats.stats.itemList[i].itemName;
				description.text = GameStats.stats.itemList[i].description;
			}

			EventSystem.current.SetSelectedGameObject(buttonMenu);
			GameStats.stats.index = i;
            source.PlayOneShot(enter);

			//obbligo i canvas ad aggiornarsi per permettere ai pannelli di ridimensionarsi a seconda della quantità di testo ed elementi contenuti
            Canvas.ForceUpdateCanvases();
        }
		//se l'oggetto è di tipo slot vuoto non faccio niente
        else
        {
            source.PlayOneShot(empty);
        }
    }

	//nasconde il menu delle opzioni dell'inventario
	public void HideMenu()
	{
		menuShown = false;
		GameManager.manager.inventoryMenuActive = false;
		inventoryMenu.SetActive(false);
		EventSystem.current.SetSelectedGameObject(lastInventoryButton);
	}

	//lanciato dal tasto Back nel menu dell'inventario
	public void PlayExitSound()
	{
		source.PlayOneShot(exit);
	}

	//aggiorna la visualizzazione di tutti gli slot dell'inventario
	public void UpdateSlotUI()
	{
		for (int i = 0; i < inventorySlots.Length; i++)
		{
			inventorySlots[i].UpdateSlot(GameStats.stats.itemList[i]);
		}
	}

	//"riempie" l'inventario di oggetti di tipo slot vuoto
	private void ResetAllSlots()
	{
		for (int i = 0; i < GameStats.stats.itemList.Length; i++)
		{
			JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), GameStats.stats.itemList[i]);
		}
	}

	//elimina un oggetto dall'inventario
	public void DropItem()
	{
		//se c'è solo un oggetto nello stack lo elimino
		if (GameStats.stats.itemList[GameStats.stats.index].currentStack == 1)
		{
			JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(emptySlot), GameStats.stats.itemList[GameStats.stats.index]);
			GameStats.stats.count--;
		}
		//altrimenti decremento il valore dello stack
		else
		{
			GameStats.stats.itemList[GameStats.stats.index].currentStack--;
		}
		source.PlayOneShot(dropItem);
		UpdateSlotUI();
		HideMenu();
	}
}

