using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    public GameObject inventorySlots;

    private Transform[] slotTransforms;

    public GameObject[] slotItems;

    private int inventorySlotCount;

    private void Awake()
    {
        inventorySlotCount = inventorySlots.transform.childCount;
        slotTransforms = new Transform[inventorySlotCount];
        slotItems = new GameObject[inventorySlotCount];
        for (int i = 0; i < inventorySlotCount; i++)
        {
            slotTransforms[i] = inventorySlots.transform.GetChild(i).gameObject.transform;
        }
    }

    // Use this for initialization
    void Start ()
    {

    }

    public void AddItem(GameObject item)
    {
        for(int i = 0; i < inventorySlotCount; i++)
        {
            if(slotItems[i] == null)
            {
                slotItems[i] = item;
                slotItems[i].transform.SetPositionAndRotation(slotTransforms[i].position, slotTransforms[i].rotation);
                slotItems[i].GetComponent<Rigidbody>().useGravity = false;
                slotItems[i].GetComponent<Rigidbody>().isKinematic = true;
                slotItems[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                slotItems[i].transform.parent = gameObject.transform;
                break;
            }
        }
        CheckDuplicates();
    }

    public void RemoveItem(GameObject item)
    {
        for(int i = 0; i < inventorySlotCount; i++)
        {
            if(slotItems[i] != null)
            {
                if (item.GetInstanceID() == slotItems[i].GetInstanceID())
                {
                    slotItems[i].GetComponent<Rigidbody>().useGravity = true;
                    slotItems[i].GetComponent<Rigidbody>().isKinematic = false;
                    slotItems[i].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    slotItems[i] = null;
                    break;
                }
            }
        }
        //CheckDuplicates();
    }

    public void CheckDuplicates()
    {
        for(int i = 0; i < inventorySlotCount-1; i++)
        {
            for(int j = i+1; j < inventorySlotCount; j++)
            {
                if(slotItems[i] != null && slotItems[j] != null)
                {
                    if (slotItems[i].GetInstanceID() == slotItems[j].GetInstanceID())
                        slotItems[j] = null;
                }
            }
            if(slotItems[i] != null)
                slotItems[i].transform.SetPositionAndRotation(slotTransforms[i].position, slotTransforms[i].rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemController itemController = other.gameObject.GetComponent<ItemController>();
        if (itemController != null)
        {
            AddItem(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ItemController itemController = other.gameObject.GetComponent<ItemController>();
        if (itemController != null)
        {
            RemoveItem(other.gameObject);
        }
    }
}
