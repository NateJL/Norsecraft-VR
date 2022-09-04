using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : MonoBehaviour
{
    private ItemController parentController;

    // Use this for initialization
    void Start ()
    {
        parentController = transform.parent.GetComponent<ItemController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<TreeController>() != null)
        {
            if(parentController.isActive)
            {
                other.gameObject.GetComponent<TreeController>().ChopTree();
                parentController.UseItem();
            }
        }
    }
}
