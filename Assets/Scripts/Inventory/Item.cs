using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName ="Inventory/Item")]
public class Item : ScriptableObject
{

    public string itemName;
    public string itemDescription;
    public Sprite icon;
    public int currentStack = 1;
    public int maxStack = 1;

    public ItemStats.ItemType type = ItemStats.ItemType.basic ;
    public ItemStats.ConsumableType consumableType;

}
