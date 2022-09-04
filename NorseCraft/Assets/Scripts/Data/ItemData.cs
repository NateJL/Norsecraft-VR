using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string name;

    public int value;

    public ItemData(string itemName, int itemValue)
    {
        name = itemName;
        value = itemValue;
    }
	
}
