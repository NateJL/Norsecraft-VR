using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMountController : MonoBehaviour
{
    private Transform playerSpawn;

	// Use this for initialization
	void Start ()
    {
        playerSpawn = transform.GetChild(0);
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<LongShipController>() != null)
        {
            other.gameObject.GetComponent<LongShipController>().Dock(transform);
            /*
            other.gameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            foreach(GameObject child in other.gameObject.GetComponent<LongShipController>().childCollection)
            {
                if (child.GetComponent<PlayerManager>() != null)
                    child.transform.SetPositionAndRotation(playerSpawn.position, playerSpawn.rotation);
            }
            */
        }
    }
}
