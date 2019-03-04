using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public GameObject icon; //immagine dello slot attuale
    public Text stack; //numero oggetti dello stesso tipo


    public void UpdateSlot(ItemStats i)
    {
		//se l'oggetto nello slot è diverso dallo slot vuoto
        if (i.itemType != ItemStats.ItemType.emptyslot)
        {
			//imposto l'immagine e il numero 
            icon.GetComponent<Image>().sprite = i.sprite;
            if (i.currentStack > 1)
            {
                stack.text = "x" + i.currentStack.ToString();
            }
            else
            {
                stack.text = "";
            }
            icon.SetActive(true);

        }
		//se lo slot è vuoto disattivo l'immagine
        else
        {
            icon.SetActive(false);
        }
    }

}
