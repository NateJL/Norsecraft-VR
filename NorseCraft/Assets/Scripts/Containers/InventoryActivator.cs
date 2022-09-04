using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryActivator : MonoBehaviour
{
    public GameObject Player;

    public GameObject grabbableChest;

    public GameObject interactiveChest;

    public AttachItem chestMount;

    public bool isMounted;

	// Use this for initialization
	void Start ()
    {
        isMounted = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Vector3.Distance(Player.transform.position, gameObject.transform.position) > 5.0f)
        {
            chestMount.MountItem(gameObject);
        }

		if(!isMounted)
        {
            if(!gameObject.GetComponent<OVRGrabbable>().isGrabbed)
            {
                grabbableChest.SetActive(false);
                interactiveChest.SetActive(true);
                gameObject.GetComponent<OVRGrabbable>().enabled = false;
            }
        }
        else
        {
            grabbableChest.SetActive(true);
            interactiveChest.SetActive(false);
            gameObject.GetComponent<OVRGrabbable>().enabled = true;
        }
	}
}
