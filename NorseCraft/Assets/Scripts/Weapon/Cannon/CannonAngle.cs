using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonAngle : MonoBehaviour {

    public GameObject cannonBarrel;

    [Header("Grabbed Data")]
    public bool leftHandGrab;
    public bool rightHandGrab;

    private Vector3 rotateUp;
    private Vector3 rotateDown;
    private Vector3 rotateLeft;
    private Vector3 rotateRight;
    private float minAngle = -12.0f;

	// Use this for initialization
	void Start ()
    {
        cannonBarrel = transform.parent.gameObject;
        rotateUp = new Vector3(0.3f, 0, 0);
        rotateDown = new Vector3(-0.3f, 0, 0);
        rotateLeft = new Vector3(0, -0.2f, 0);
        rotateRight = new Vector3(0, 0.2f, 0);

        leftHandGrab = false;
        rightHandGrab = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name == "LCube" || other.gameObject.name == "RCube")
        {
            GameObject hand = other.gameObject;

            if (hand.name == "LCube")
            {
                Debug.Log(hand.transform.InverseTransformPoint(transform.parent.parent.position).x);
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) > 0.05f)
                {
                    if (hand.transform.position.y > (transform.position.y + 0.1f))
                    {
                        cannonBarrel.transform.Rotate(rotateUp);
                    }
                    else if (hand.transform.position.y < (transform.position.y -0.1f))
                    {
                        cannonBarrel.transform.Rotate(rotateDown);
                    }

                    //if(hand.transform.InverseTransformPoint(transform.parent.position).x > 2)
                    //{
                    //    transform.parent.parent.Rotate(rotateRight);
                    //}
                    //else if(hand.transform.InverseTransformPoint(transform.parent.position).x < -2)
                    //{
                    //    transform.parent.parent.transform.Rotate(rotateLeft);
                    //}
                }
            }

            else if(hand.name == "RCube")
            {
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.05f)
                {
                    if (hand.transform.position.y > (transform.position.y + 0.1f))
                    {
                        cannonBarrel.transform.Rotate(rotateUp);
                    }
                    else if (hand.transform.position.y < (transform.position.y - 0.1f))
                    {
                        cannonBarrel.transform.Rotate(rotateDown);
                    }
                }
            }
        }
    }
}
