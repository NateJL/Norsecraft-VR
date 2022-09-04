using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourcesController : MonoBehaviour
{
    public CraftingAreaController parentController;

	// Use this for initialization
	void Start ()
    {
        parentController = transform.parent.GetComponent<CraftingAreaController>();

        if (parentController == null)
            Debug.LogError(gameObject.name + ": Failed to get parent CraftingAreaController.");
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<ItemController>() != null)
        {
            if(other.gameObject.GetComponent<HammerController>() != null)
            {
                parentController.AttemptToCraft(other.gameObject);
            }
            else
                parentController.AddItemToTable(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<ItemController>() != null)
        {
            if (other.gameObject.GetComponent<HammerController>() != null)
            {

            }
            else
                parentController.RemoveItemFromTable(other.gameObject);
        }
    }
}
