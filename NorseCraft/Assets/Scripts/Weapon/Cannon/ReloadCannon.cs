using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadCannon : MonoBehaviour
{
    private MeshRenderer reloadMesh;
    private SphereCollider reloadCollider;

    private FireCannon cannon;

	// Use this for initialization
	void Start ()
    {
        reloadMesh = GetComponent<MeshRenderer>();
        reloadCollider = GetComponent<SphereCollider>();
        cannon = transform.parent.GetComponent<FireCannon>();
        if (cannon == null)
            Debug.LogError("Failed to get FireCannon parent.");
	}

    public void FiredShot()
    {
        reloadMesh.enabled = true;
        reloadCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemController item = other.gameObject.GetComponent<ItemController>();
        if (item == null && (other.gameObject.transform.parent != null))
            item = other.transform.parent.gameObject.GetComponent<ItemController>();

        if (item != null)
        {
            if (item.data.name == "Cannonball" && !cannon.IsLoaded())
            {
                CannonPoleGrabber grabber = item.gameObject.transform.parent.GetComponent<CannonPoleGrabber>();
                if(grabber != null)
                {
                    grabber.RemoveCannonball();
                }

                Destroy(item.gameObject);
                reloadMesh.enabled = false;
                reloadCollider.enabled = false;
                cannon.LoadCannon();
            }
        }
    }
}
