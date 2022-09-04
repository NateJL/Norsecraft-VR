using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceTableController : MonoBehaviour
{
    private Fence fenceParent;

	// Use this for initialization
	void Start ()
    {
        fenceParent = transform.parent.GetComponent<Fence>();
        if (fenceParent == null)
            Debug.LogError("Fence table failed to get parent");
	}

    private void OnTriggerEnter(Collider other)
    {
        GameObject realItem = other.gameObject;
        ItemController item = other.gameObject.GetComponent<ItemController>();
        if ((item == null) && (other.transform.parent != null))
        {
            item = other.transform.parent.GetComponent<ItemController>();
            realItem = other.transform.parent.gameObject;
        }
        if(item != null)
        {
            fenceParent.AddItemToTable(realItem);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject realItem = other.gameObject;
        ItemController item = other.gameObject.GetComponent<ItemController>();
        if ((item == null) && (other.transform.parent != null))
        {
            item = other.transform.parent.GetComponent<ItemController>();
            realItem = other.transform.parent.gameObject;
        }
        if (item != null)
        {
            fenceParent.RemoveItemFromTable(realItem);
        }
    }
}
