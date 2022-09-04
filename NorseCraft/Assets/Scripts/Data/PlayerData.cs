using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string name;

    public int health;
    public int experience;
    public int level;

    public int hunger;
    public int thirst;

    public int gold;

    public ShipData playerShipData;
    public InventoryData playerInventoryData;

    public QuestSaveData[] questData;

    public int[] reputations;
    public float[] skills;

    public PlayerData(string newName, int newHealth, int newExperience, int newLevel, int newGold, int newHunger, int newThirst, ShipData shipData, InventoryData inventory)
    {
        name = newName;
        health = newHealth;
        level = newLevel;
        experience = newExperience;
        gold = newGold;
        hunger = newHunger;
        thirst = newThirst;

        playerShipData = shipData;
        playerInventoryData = inventory;
        reputations = new int[GameData.factionCount];
        skills = new float[GameData.skillCount];
    }
}

[System.Serializable]
public class InventoryData
{
    public string leftHipItem;
    public string rightHipItem;
    public string frontHipItem;
    public string backItem;

    public InventoryData()
    {
        leftHipItem = "null";
        rightHipItem = "null";
        frontHipItem = "null";
        backItem = "null";
    }

    public InventoryData(string left, string middle, string right, string back)
    {
        leftHipItem = left;
        rightHipItem = right;
        frontHipItem = middle;
        backItem = back;
    }
}
