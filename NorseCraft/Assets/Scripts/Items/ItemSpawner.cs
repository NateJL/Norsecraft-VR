using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn point")]
    public Transform itemSpawn;
    public Transform itemDisplaySpawn;

    [Header("Canvas Data")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemValueText;
    public TextMeshProUGUI itemListCountText;

    public GameObject[] items;

    private int currentItemIndex;
    private GameObject currentItem;

	// Use this for initialization
	void Start ()
    {
        SpawnDisplayItem();
	}
	
	public void LeftButtonPress()
    {
        currentItemIndex--;
        if (currentItemIndex < 0)
            currentItemIndex = items.Length - 1;
        SpawnDisplayItem();
    }

    public void RightButtonPress()
    {
        currentItemIndex++;
        if (currentItemIndex >= items.Length)
            currentItemIndex = 0;
        SpawnDisplayItem();
    }

    public void SpawnDisplayItem()
    {
        
    }

    public void SpawnItemButton()
    {
        
    }
}
