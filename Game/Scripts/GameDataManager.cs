using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameDataManager : MonoBehaviourSingleton<GameDataManager>
{
    string dataPath;
    string dataName;

    private void Awake()
    {
        dataName = "/Save.bin";
#if (UNITY_EDITOR)
        dataPath = Application.dataPath + dataName;
#else
        dataPath = Application.persistentDataPath + dataName;
#endif
    }

    public void SaveData(bool[,] data)
    {
        GameData gameData = new GameData(data);
        BinarySerialize(gameData);
    }
    public GameData LoadData()
    {
        return BinaryDeserialize();
    }
    void BinarySerialize(GameData gameData)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(dataPath, FileMode.Create);
        binaryFormatter.Serialize(fileStream, gameData);
        fileStream.Close();
    }
    GameData BinaryDeserialize()
    {
        if (File.Exists(dataPath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(dataPath, FileMode.Open);
            GameData gamedata = (GameData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return gamedata;
        }
        else
            return null;
    }
}

[System.Serializable]
public class GameData
{
    public bool[,] bools;
    public GameData(bool[,] bools)
    {
        this.bools = bools;
    }
}