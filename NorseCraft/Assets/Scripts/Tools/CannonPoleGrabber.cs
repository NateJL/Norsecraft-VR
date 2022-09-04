using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonPoleGrabber : MonoBehaviour
{
    public GameObject currentCannonball;

    private MeshRenderer cannoballColliderMesh;

	// Use this for initialization
	void Start ()
    {
        cannoballColliderMesh = GetComponent<MeshRenderer>();
	}

    public void RemoveCannonball()
    {
        currentCannonball = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemController item = other.gameObject.GetComponent<ItemController>();
        if(item == null && (other.gameObject.transform.parent != null))
            item = other.transform.parent.gameObject.GetComponent<ItemController>();

        if(currentCannonball == null && item != null)
        {
            if(item.data.name == "Cannonball")
            {
                currentCannonball = item.gameObject;
                //other.gameObject.GetComponent<MeshCollider>().enabled = false;
                currentCannonball.GetComponent<Rigidbody>().useGravity = false;
                currentCannonball.GetComponent<Rigidbody>().isKinematic = true;
                currentCannonball.transform.parent = gameObject.transform;
                currentCannonball.transform.SetPositionAndRotation(transform.position, transform.rotation);
                cannoballColliderMesh.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentCannonball != null)
        {
            if (other.gameObject.GetInstanceID() == currentCannonball.GetInstanceID())
            {
                currentCannonball = null;
                cannoballColliderMesh.enabled = true;
            }
        }
    }
}
