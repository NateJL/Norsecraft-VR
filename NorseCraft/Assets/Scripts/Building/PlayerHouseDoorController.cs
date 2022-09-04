using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHouseDoorController : MonoBehaviour
{
    public Transform playerDestination;

	// Use this for initialization
	void Start ()
    {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.transform.SetPositionAndRotation(playerDestination.position, playerDestination.rotation);
        }
    }
}
