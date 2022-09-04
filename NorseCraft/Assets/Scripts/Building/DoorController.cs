using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject interiorParent;
    public Transform playerDestination;

    [Tooltip("check box for door to activate interior parent on trigger")]
    public bool isEntrance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(isEntrance)
            {
                if(interiorParent != null)
                {
                    interiorParent.SetActive(true);
                }
            }

            if(playerDestination != null)
                other.transform.SetPositionAndRotation(playerDestination.position, playerDestination.rotation);

            if(!isEntrance)
            {
                if (interiorParent != null)
                {
                    interiorParent.SetActive(false);
                }
            }
        }
    }
}
