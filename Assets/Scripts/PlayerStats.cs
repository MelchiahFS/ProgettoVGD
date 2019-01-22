using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public string playerhealth;
    public ItemStats[] itemListSave = new ItemStats[20];

    //public void Inizializate(ItemStats item)
    //{
    //    for (int i = 0; i < itemListSave.Length; i++)
    //    {
    //        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(item), itemListSave[i]);
    //            Debug.Log(itemListSave[i].itemName);
    //    }
    //}

    public void SaveStats()
    {
        SaveManager.SavePlayerStats(this);
    }

	public void LoadStats()
    {
        SavedData savedData = SaveManager.LoadPlayerStats();

        playerhealth = savedData.playerhealth;
        for (int i = 0; i < itemListSave.Length; i++)
        {
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(savedData.itemListSave[i]), itemListSave[i]);
        }
    }
}
