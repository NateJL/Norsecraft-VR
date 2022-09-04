using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobberController : MonoBehaviour
{
    private Rigidbody bobber_rb;

    private FishingPoleController poleParent;

    public Vector3 highestPoint;

	// Use this for initialization
	void Start ()
    {
        bobber_rb = GetComponent<Rigidbody>();
        highestPoint = transform.position;
	}

    private void Update()
    {
        poleParent.UpdateCast();

        if (transform.position.y > highestPoint.y)
            highestPoint = transform.position;
    }

    public void SetPoleParent(FishingPoleController pole)
    {
        poleParent = pole;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    public void ResetBobber()
    {
        bobber_rb.velocity = Vector3.zero;
        bobber_rb.useGravity = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ocean"))
        {
            bobber_rb.velocity = Vector3.zero;
            bobber_rb.useGravity = false;
            poleParent.WaitForFish();
        }
    }
}
