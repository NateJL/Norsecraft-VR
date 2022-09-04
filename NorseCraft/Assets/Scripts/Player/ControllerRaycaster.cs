using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ControllerRaycaster : MonoBehaviour
{
    // parent player manager
    private PlayerManager playerManager;

    // attached LineRenderer
    private LineRenderer lineRenderer;

    [Header("Laser Settings")]
    public float laserWidth = 0.02f;
    public float laserMaxLength = 2f;

    [Tooltip("Whether script is attached to left or right hand controller")]
    public bool left_hand = false;

    public Material raycastGreen;
    public Material raycastRed;

    private GameObject lastHit;
    private bool hasSelected;

    // Use this for initialization
    void Start ()
    {
        hasSelected = false;
        playerManager = transform.parent.parent.parent.GetComponent<PlayerManager>();
        lineRenderer = GetComponent<LineRenderer>();
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        lineRenderer.SetPositions(initLaserPositions);
        lineRenderer.startWidth = lineRenderer.endWidth = laserWidth;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (left_hand && (playerManager.leftHandItem == null))
        {
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.5f)
            {
                ShootLaserFromTargetPosition(transform.position, transform.rotation * Vector3.forward, laserMaxLength);
                lineRenderer.enabled = true;
                if (OVRInput.Get(OVRInput.Button.One))
                {
                    if (!hasSelected)
                    {
                        hasSelected = true;
                        if (lastHit != null)
                        {
                            lastHit.GetComponent<HitReceiver>().OnRaycastSelect();
                        }
                    }
                }
                else
                {
                    hasSelected = false;
                }
            }
            else
            {
                lineRenderer.enabled = false;
                if(lastHit != null)
                {
                    lastHit.GetComponent<HitReceiver>().OnRaycastEnd();
                    lastHit = null;
                }
            }
        }
        else if (!left_hand && (playerManager.rightHandItem == null))
        {
            if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f)
            {
                ShootLaserFromTargetPosition(transform.position, transform.rotation * Vector3.forward, laserMaxLength);
                lineRenderer.enabled = true;
                if (OVRInput.Get(OVRInput.Button.One))
                {
                    if (!hasSelected)
                    {
                        hasSelected = true;
                        if (lastHit != null)
                        {
                            lastHit.GetComponent<HitReceiver>().OnRaycastSelect();
                        }
                    }
                }
                else
                {
                    hasSelected = false;
                }
            }
            else
            {
                lineRenderer.enabled = false;
                if (lastHit != null)
                {
                    lastHit.GetComponent<HitReceiver>().OnRaycastEnd();
                    lastHit = null;
                }
            }
        }
    }

    /*
     * Function to shoot a raycast from the hand forward
     */
    void ShootLaserFromTargetPosition(Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        if (Physics.Raycast(ray, out raycastHit, length))
        {
            lineRenderer.material = raycastGreen;
            HitReceiver other = raycastHit.transform.gameObject.GetComponent<HitReceiver>();
            if(other != null)
            {
                if (lastHit != null)
                {
                    if (raycastHit.transform.gameObject.GetInstanceID() != lastHit.GetInstanceID())
                    {
                        lastHit.GetComponent<HitReceiver>().OnRaycastEnd();
                    }
                }
                lastHit = raycastHit.transform.gameObject;
                lastHit.GetComponent<HitReceiver>().OnRaycastHit();
            }
            else
            {
                if(raycastHit.transform.gameObject.CompareTag("InventorySlot"))
                {
                    endPosition = raycastHit.point;
                }
            }

        }
        else
        {
            lineRenderer.material = raycastRed;
            if (lastHit != null)
            {
                lastHit.GetComponent<HitReceiver>().OnRaycastEnd();
                lastHit = null;
            }
        }

        lineRenderer.SetPosition(0, targetPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    private void OnDestroy()
    {
        Destroy(gameObject.GetComponent<LineRenderer>().material);
    }
}
