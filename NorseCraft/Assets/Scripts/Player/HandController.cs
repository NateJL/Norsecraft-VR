using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Material validGrabMaterial;
    public Material notValidGrabMaterial;
    public Material defaultMaterial;

    private Renderer grabColliderRenderer;
    private PlayerManager manager;

    [Header("Hand Data")]
    [ShowOnly] public bool isGrabbing;
    [ShowOnly] public Vector3 currentVelocity;
    public GameObject grabbedItem;
    public GameObject closestItem;

    [Space(10)]
    public float velocityDamping = 1f;
    private Vector3 lastPosition;

    // Use this for initialization
    void Start ()
    {
        isGrabbing = false;
        grabColliderRenderer = GetComponent<MeshRenderer>();
        grabColliderRenderer.material = defaultMaterial;
	}
    
    public void setParent(PlayerManager newParent)
    {
        manager = newParent;
    }

    /*
     * Function to be called when the player presses the grab trigger, returning the current closest item
     */
    public GameObject grabItem()
    {
        lastPosition = transform.position;
        if (closestItem.GetComponent<ItemController>() != null)                 // if object is Item
        {
            if (closestItem.GetComponent<ItemController>().isGrabbable && !closestItem.GetComponent<ItemController>().isGrabbed)
            {
                grabbedItem = closestItem;
                grabbedItem.GetComponent<ItemController>().Grab(transform);
            }
        }
        else if(closestItem.GetComponent<SteeringController>() != null)         // if object is steering wheel
        {
            grabbedItem = closestItem;
            grabbedItem.GetComponent<SteeringController>().SendCurrentHandPosition(gameObject);
        }
        else if(closestItem.GetComponent<LadderController>() != null)           // if object is ladder
        {
            grabbedItem = closestItem;
            grabbedItem.GetComponent<LadderController>().GrabBegin(manager.gameObject.transform.InverseTransformPoint(transform.position));
        }
        else if(closestItem.GetComponent<RudderController>() != null)           // if object is a rudder
        {
            grabbedItem = closestItem;
        }
        else if(closestItem.GetComponent<SailGrabber>() != null)
        {
            grabbedItem = closestItem;
        }
        else if(closestItem.GetComponent<ContainerLidController>() != null)
        {
            grabbedItem = closestItem;
        }

        return grabbedItem;
    }

    public void ReleaseGrab()
    {
        if (grabbedItem != null)
        {
            if (grabbedItem.GetComponent<ItemController>() != null)
            {
                grabbedItem.GetComponent<ItemController>().Release();
                grabbedItem.transform.parent = null;
                grabbedItem.GetComponent<Rigidbody>().AddForce(currentVelocity * velocityDamping, ForceMode.Impulse);
            }
            else if(grabbedItem.GetComponent<SteeringController>() != null)
            {
                grabbedItem.GetComponent<SteeringController>().RemoveCurrentHandPosition(gameObject);
            }
            else if (grabbedItem.GetComponent<LadderController>() != null)
            {
                grabbedItem.GetComponent<LadderController>().GrabEnd(transform);
            }
            else if (grabbedItem.GetComponent<SailGrabber>() != null)
            {
                grabbedItem.GetComponent<SailGrabber>().ReleaseRope();
            }
            grabbedItem = null;
        }
        isGrabbing = false;
    }

    /*
     * Update grab data for:
     *      -ItemControllers (throwing)
     *      -SteeringControllers (steering ship)
     *      -LadderControllers (climbing)
     *      -RudderControllers (steering longship)
     *      -SailGrabbers (raise/lower sails)
     *      -ContainerLidControllers (open lid)
     */
    public void UpdateGrab()
    {
        currentVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
        if(grabbedItem.GetComponent<ItemController>() != null)
        {
            grabbedItem.GetComponent<ItemController>().UpdateItem();
        }
        else if (grabbedItem.GetComponent<SteeringController>() != null)
        {
            grabbedItem.GetComponent<SteeringController>().SendCurrentHandPosition(gameObject);
        }
        else if (grabbedItem.GetComponent<LadderController>() != null)
        {
            grabbedItem.GetComponent<LadderController>().HoldingLadder(manager.gameObject.transform.InverseTransformPoint(transform.position));
        }
        else if(grabbedItem.GetComponent<RudderController>() != null)
        {
            grabbedItem.GetComponent<RudderController>().MoveWithHand(gameObject.transform);
        }
        else if (grabbedItem.GetComponent<SailGrabber>() != null)
        {
            if(grabbedItem.GetComponent<SailGrabber>().PullSailRope(transform))
            {
                ReleaseGrab();
            }
        }
        else if (grabbedItem.GetComponent<ContainerLidController>() != null)
        {
            grabbedItem.GetComponent<ContainerLidController>().UpdateLidRotation(gameObject.transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ItemController>() != null ||
            other.gameObject.GetComponent<SteeringController>() != null ||
            other.gameObject.GetComponent<LadderController>() != null ||
            other.gameObject.GetComponent<RudderController>() != null ||
            other.gameObject.GetComponent<SailGrabber>() != null ||
            other.gameObject.GetComponent<ContainerLidController>() != null)
        {
            grabColliderRenderer.material = validGrabMaterial;
            closestItem = other.gameObject;
        }
        else
        {
            grabColliderRenderer.material = notValidGrabMaterial;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        grabColliderRenderer.material = defaultMaterial;
        if (closestItem != null)
        {
            if (closestItem.GetInstanceID() == other.gameObject.GetInstanceID())
            {
                closestItem = null;
            }
        }
    }
}
