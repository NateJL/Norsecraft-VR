using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipData
{
    [ShowOnly] public int id;

    [ShowOnly] public string Size;
    [ShowOnly] public int durability;

    [ShowOnly] public bool isFloating;

    [ShowOnly] public string HullType;
    [ShowOnly] public string WheelType;
    [ShowOnly] public string SailType;
    [ShowOnly] public string FlagType;

    public ShipData()
    {
        id = -1;
        Size = "null";
        durability = 0;
        isFloating = false;
    }

    public ShipData(int boatID, string boatSize, int boatDurability, bool boatIsFloating)
    {
        id = boatID;
        Size = boatSize;
        durability = boatDurability;
        isFloating = boatIsFloating;
    }

    public void SetShipComponents(string newHull, string newWheel, string newSail, string newFlag)
    {
        HullType = newHull;
        WheelType = newWheel;
        SailType = newSail;
        FlagType = newFlag;
    }
}
