using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailGrabber : MonoBehaviour
{
    private ShipManager shipManager;
    private GameObject attachedSails;

    [Space(10)]
    [Tooltip("The end of the rope attached to the sails, child(0) of transform")]
    private Vector3 endOfRope;
    public int ropeVertices = 2;
    public float ropeThickness = 0.1f;
    private Vector3[] linePositions;

    [Space(10)]
    private LineRenderer lineRenderer;
    public Material ropeMaterial;

    private Vector3 initPosition;
    private Quaternion initRotation;
    private bool raise = false;
    [ShowOnly] public float distance = 0.0f;


    // Use this for initialization
    void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = ropeVertices;
        linePositions = new Vector3[ropeVertices];
        for (int i = 0; i < linePositions.Length; i++)
            linePositions[i] = Vector3.zero;
        lineRenderer.SetPositions(linePositions);
        lineRenderer.startWidth = lineRenderer.endWidth = ropeThickness;
        lineRenderer.material = ropeMaterial;
        lineRenderer.enabled = false;

        endOfRope = transform.GetChild(0).position;
        initPosition = transform.position;
        initRotation = transform.rotation;

        shipManager = transform.parent.parent.gameObject.GetComponent<ShipManager>();

        EnableRopeGrabber();
	}
	
	public void EnableRopeGrabber()
    {
        lineRenderer.enabled = true;
        UpdateRope();
    }

    public void DisableeRopeGrabber()
    {
        lineRenderer.enabled = false;
    }

    public void UpdateRope()
    {
        Vector3 pos1 = endOfRope;
        Vector3 pos2 = new Vector3((endOfRope.x + transform.position.x) * 0.5f,
                                    transform.position.y,
                                    (endOfRope.z + transform.position.z) * 0.5f);
        Vector3 pos3 = transform.position;
        float lerp = (1f / (float)ropeVertices);

        lineRenderer.SetPosition(0, endOfRope);
        for (int i = 1; i < ropeVertices - 1; i++)
        {
            Vector3 linePos = Vector3.Lerp(Vector3.Lerp(pos1, pos2, lerp), Vector3.Lerp(pos2, pos3, lerp), lerp);
            lineRenderer.SetPosition(i, linePos);
            lerp += (1f / (float)ropeVertices);
        }
        lineRenderer.SetPosition(ropeVertices - 1, transform.position);
    }

    public bool PullSailRope(Transform hand)
    {
        bool isTriggered = false;
        transform.SetPositionAndRotation(hand.position, hand.rotation);
        distance = Vector3.Distance(initPosition, transform.position);
        if (distance > 1.0f)
        {
            raise = shipManager.ManagerSailsAction(raise);
            isTriggered = true;
        }
        UpdateRope();
        return isTriggered;
    }

    public void ReleaseRope()
    {
        transform.SetPositionAndRotation(initPosition, initRotation);
        distance = 0.0f;
        UpdateRope();
    }
}
