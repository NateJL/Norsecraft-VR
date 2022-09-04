using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongShipSailController : MonoBehaviour
{

    private ShipManager shipManager;

	// Use this for initialization
	void Start ()
    {
        shipManager = transform.parent.gameObject.GetComponent<ShipManager>();
	}

    private void ToggleSails()
    {
        if (shipManager.shipComponentManager.sailing)
            shipManager.shipComponentManager.RaiseSails();
        else
            shipManager.shipComponentManager.LowerSails();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<HandController>() != null)
        {
            if (other.gameObject.GetComponent<HandController>().isGrabbing)
            {
                ToggleSails();
            }
        }
    }
}
