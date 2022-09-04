using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringController : MonoBehaviour
{
    public float turnAngle;
    public float prevTurnAngle;
    public float initialTurnAngle;

    private float maxTurnAngle = 90.0f;
    private float minTurnAngle = -90.0f;

    private Vector3 initialLocationPosition;
    private Vector3 initialLocalRotation;

    public float rotSpeed = 360f;

    public GameObject currentHand;

    public bool isGrabbed = false;

	// Use this for initialization
	void Start ()
    {
        turnAngle = 0.0f;
        prevTurnAngle = 0.0f;
        initialTurnAngle = 0.0f;
        initialLocationPosition = transform.localPosition;
        initialLocalRotation = transform.localEulerAngles;
    }

    /*
     * Send hand data to wheel
     */
    public void SendCurrentHandPosition(GameObject handRequest)
    {
        if (currentHand == null)
        {
            currentHand = handRequest;
            isGrabbed = true;
        }
        else if (handRequest != currentHand)
            return;

        Vector3 wheelPos = new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, 0);
        Vector3 handPos = transform.parent.parent.InverseTransformPoint(currentHand.transform.position);
        handPos = new Vector3(handPos.x, handPos.y, 0);

        turnAngle = Vector3.SignedAngle(wheelPos, handPos, Vector3.back) * 20.0f;

        if (turnAngle > maxTurnAngle)
            turnAngle = maxTurnAngle;
        else if (turnAngle < minTurnAngle)
            turnAngle = minTurnAngle;
        //Debug.Log("Angle: " + (turnAngle));

        transform.localEulerAngles = new Vector3(0, 0, turnAngle);
    }

    /*
     * Set current hand to null
     */
    public void RemoveCurrentHandPosition(GameObject handRequest)
    {
        if (currentHand == handRequest)
        {
            currentHand = null;
            isGrabbed = false;
        }
    }



    /*
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name == "LCube" && currentHand == null)
        {
            currentHand = other.gameObject;
            if (currentHand.GetComponent<HandController>().isGrabbing)
            {
                Vector3 wheelPos = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
                Vector3 handPos = transform.parent.InverseTransformPoint(other.transform.position);
                handPos = new Vector3(handPos.x, handPos.y, 0);

                initialTurnAngle = turnAngle;
                prevTurnAngle = initialTurnAngle;
                Debug.Log("Steering with Left");
            }
            else
                currentHand = null;
        }
        else if(other.gameObject.name == "RCube" && currentHand == null)
        {
            currentHand = other.gameObject;
            if (currentHand.GetComponent<HandController>().isGrabbing)
            {
                Vector3 wheelPos = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
                Vector3 handPos = transform.parent.InverseTransformPoint(other.transform.position);
                handPos = new Vector3(handPos.x, handPos.y, 0);

                initialTurnAngle = turnAngle;
                prevTurnAngle = initialTurnAngle;
                Debug.Log("Steering with Right");
            }
            else
                currentHand = null;
        }

        if(currentHand == other.gameObject && currentHand.GetComponent<HandController>().isGrabbing)
        {
            Vector3 wheelPos = new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, 0);
            Vector3 handPos = transform.parent.parent.InverseTransformPoint(other.transform.position);
            handPos = new Vector3(handPos.x, handPos.y, 0);

            turnAngle = Vector3.SignedAngle(wheelPos, handPos, Vector3.back) * 20.0f;
            //turnAngle = initialTurnAngle + (initialTurnAngle - turnAngle);
            //prevTurnAngle = turnAngle;

            if (turnAngle > maxTurnAngle)
                turnAngle = maxTurnAngle;
            else if (turnAngle < minTurnAngle)
                turnAngle = minTurnAngle;
            //Debug.Log("Angle: " + (turnAngle));

            transform.localEulerAngles = new Vector3(0, 0, turnAngle);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "LCube" && currentHand == other.gameObject)
        {
            currentHand = null;
            Debug.Log("Left hand left");
        }
        else if (other.gameObject.name == "RCube" && currentHand == other.gameObject)
        {
            currentHand = null;
            Debug.Log("Right hand left");
        }
    }*/
}
