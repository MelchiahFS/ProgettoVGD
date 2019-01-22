using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class SaveManager{

    public static void SavePlayerStats(PlayerStats playerStats)
    {
        BinaryFormatter bf = new BinaryFormatter();

        string filePath = Application.persistentDataPath + "/player_stats.txt";

        FileStream fs = new FileStream(filePath, FileMode.Create);

        SavedData saveData = new SavedData(playerStats);

        bf.Serialize(fs, saveData);
        fs.Close();
    }

    public static SavedData LoadPlayerStats()
    {
        string filePath = Application.persistentDataPath + "/player_stats.txt";
        if(File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(filePath, FileMode.Open);

            SavedData savedData = bf.Deserialize(fs) as SavedData;
            fs.Close();

            return savedData;
        }
        else
        {
            return null;
        }
    }
}
