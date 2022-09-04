using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    private Vector3 initialGrabPosition;

    private GameObject platform;

	// Use this for initialization
	void Start ()
    {
        platform = transform.GetChild(0).gameObject;
        initialGrabPosition = Vector3.zero;
	}
	
	public void GrabBegin(Vector3 initialGrab)
    {
        initialGrabPosition = initialGrab;
    }

    public void HoldingLadder(Vector3 currentHandPosition)
    {
        Vector3 newPosition = currentHandPosition - initialGrabPosition;
        initialGrabPosition = currentHandPosition;
        platform.transform.Translate(new Vector3(0, -newPosition.y, 0));
    }

    public void GrabEnd(Transform endPos)
    {

    }
}
