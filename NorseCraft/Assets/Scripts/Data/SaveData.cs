using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public PlayerData playerData;
    public QuestSaveData[] questData;



    public SaveData(PlayerData player, QuestSaveData[] quests)
    {
        playerData = player;
        questData = quests;
    }
	
}

[System.Serializable]
public class TileSaveData
{
    public int x, z;

    public ObjectSaveData[] objects;
}

[System.Serializable]
public class ObjectSaveData
{
    public string objectName;

    public float x, y, z;
}
