using UnityEngine;

[System.Serializable]
public class SavedData {

    public string playerhealth;
    public ItemStats[] itemListSave = new ItemStats[20];

    public SavedData(PlayerStats playerStats)
    {
        playerhealth = playerStats.playerhealth;
    }

}
