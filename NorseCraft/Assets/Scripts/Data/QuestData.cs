using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public string questName;

    public GameData.Faction faction; 

    [TextArea]
    public string questDescription;
    [TextArea]
    public string questResponse;

    public bool isRepeatable;

    public enum QuestType
    {
        None,
        Pay,
        Talk,
        Fetch,
        Kill,
        Buy,
        Escort
    };

    public QuestType questType;

    public QuestRequirements requirements;

    public QuestReward reward;

    public QuestSaveData saveData;

    public QuestData()
    {
        questName = "quest name";
        faction = GameData.Faction.None;
        questDescription = "quest description";
        questResponse = "quest response";
        isRepeatable = false;
        questType = QuestType.None;
        requirements = new QuestRequirements();
        reward = new QuestReward();
        saveData = new QuestSaveData();
    }
}

[System.Serializable]
public class QuestSaveData
{
    public int questID;
    public bool isAccepted;
    public bool isCompleted;

    public QuestSaveData()
    {
        questID = -1;
        isAccepted = false;
        isCompleted = false;
    }
}


[System.Serializable]
public class QuestRequirements
{
    public List<int> prerequisiteQuests;
    public int goldGiven;
    public int goldRequired;
    public List<GameObject> itemsRequired;
    public List<int> numberOfItemsRequired;

    public QuestRequirements()
    {
        prerequisiteQuests = new List<int>();
        goldGiven = 0;
        goldRequired = 0;
        itemsRequired = new List<GameObject>();
        numberOfItemsRequired = new List<int>();
    }
}


[System.Serializable]
public class QuestReward
{
    public int gold;
    public int experience;

    public int factionID;
    public int reputation;

    public QuestReward()
    {
        gold = 0;
        experience = 0;
        factionID = 0;
        reputation = 0;
    }
}
