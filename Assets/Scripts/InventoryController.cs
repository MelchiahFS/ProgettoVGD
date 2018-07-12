using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {

    public static bool GameIsPaused = false;
    private int inventorySpace = 0;
    public GameObject Inventory;
    public GameObject slot1, slot2, slot3, slot4, slot5, slot6;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
        if ((inventorySpace < 6)&&(!GameIsPaused))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                switch (inventorySpace)
                {
                    case 0:
                        slot1.SetActive(true);
                        inventorySpace++;
                        break;
                    case 1:
                        slot2.SetActive(true);
                        inventorySpace++;
                        break;
                    case 2:
                        slot3.SetActive(true);
                        inventorySpace++;
                        break;
                    case 3:
                        slot4.SetActive(true);
                        inventorySpace++;
                        break;
                    case 4:
                        slot5.SetActive(true);
                        inventorySpace++;
                        break;
                    case 5:
                        slot6.SetActive(true);
                        inventorySpace++;
                        break;
                }
            }
        }
    }

    public void Resume()
    {
        Inventory.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        Inventory.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void EliminateSlot(GameObject slot)
    {
        slot.SetActive(false);
        inventorySpace--;
        return;
    }
}
