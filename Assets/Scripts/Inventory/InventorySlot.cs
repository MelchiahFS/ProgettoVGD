﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public GameObject icon;
    public Text stack;

    public void UpdateSlot(ItemStats i)
    {
        
        if (i.itemType != ItemStats.ItemType.emptyslot)
        {
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
        else
        {
            icon.SetActive(false);
        }
    }

}
