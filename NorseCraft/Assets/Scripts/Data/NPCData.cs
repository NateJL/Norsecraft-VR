using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData
{
    public GameData.Faction faction;

    [Tooltip("Will be assigned automatically by the Character")]
    public int factionID;

    public int experienceReward;

    public int reputationLoss;

    public GameObject[] loot;

    public enum ActionType
    {
        None,
        Stand,
        Path
    };

    public ActionType actionType;

    public GameObject currentDestination;

}
