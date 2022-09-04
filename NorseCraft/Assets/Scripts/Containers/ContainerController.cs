using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : MonoBehaviour
{
    [HideInInspector]
    public ItemController itemController;

    public Transform itemsParent;
    public List<GameObject> items;

    private void Start()
    {
        itemController = GetComponent<ItemController>();
        itemsParent = transform.GetChild(0);
        items = new List<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<ItemController>() != null)
        {
            items.Add(other.gameObject);
            other.gameObject.transform.SetParent(itemsParent);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(items.Contains(other.gameObject))
        {
            if (other.gameObject.transform.parent != null && other.gameObject.transform.parent.Equals(itemsParent))
                other.gameObject.transform.SetParent(null);

            items.Remove(other.gameObject);
        }
    }
}

[System.Serializable]
public class ContainerData
{
    public string name;
    public int numberOfItems;
    public string[] itemNames;
    
    public ContainerData()
    {
        name = "No Name";
        numberOfItems = 0;
        itemNames = new string[numberOfItems];
    }

    public ContainerData(string containerName, int containerNumberOfItems, string[] containerItemNames)
    {
        name = containerName;
        numberOfItems = containerNumberOfItems;
        itemNames = containerItemNames;
    }
}
