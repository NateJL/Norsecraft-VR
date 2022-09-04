using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRaycaster : MonoBehaviour
{
    [Header("Raycast Transforms")]
    public GameObject raycastFront;
    public GameObject raycastFrontLeft;
    public GameObject raycastFrontRight;
    public GameObject raycastMidLeft;
    public GameObject raycastMidRight;
    public GameObject raycastBackLeft;
    public GameObject raycastBackRight;

    [Header("Raycast Data")]
    public RaycastData frontCenterData;
    public RaycastData frontLeftData;
    public RaycastData frontRightData;
    public RaycastData midLeftData;
    public RaycastData midRightData;
    public RaycastData backLeftData;
    public RaycastData backRightData;
    [Space(10)]
    public float distanceThreshold = 0.5f;

    [Header("Laser Settings")]
    public float laserWidth = 0.02f;
    public float laserMaxLength = 2f;
    public Material raycastGreen;
    public Material raycastRed;

    // Use this for initialization
    void Start ()
    {
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        raycastFront.GetComponent<LineRenderer>().SetPositions(initLaserPositions);
        raycastFront.GetComponent<LineRenderer>().startWidth = raycastFront.GetComponent<LineRenderer>().endWidth = laserWidth;

        raycastFrontLeft.GetComponent<LineRenderer>().SetPositions(initLaserPositions);
        raycastFrontLeft.GetComponent<LineRenderer>().startWidth = raycastFront.GetComponent<LineRenderer>().endWidth = laserWidth;

        raycastMidLeft.GetComponent<LineRenderer>().SetPositions(initLaserPositions);
        raycastMidLeft.GetComponent<LineRenderer>().startWidth = raycastFront.GetComponent<LineRenderer>().endWidth = laserWidth;

        raycastBackLeft.GetComponent<LineRenderer>().SetPositions(initLaserPositions);
        raycastBackLeft.GetComponent<LineRenderer>().startWidth = raycastFront.GetComponent<LineRenderer>().endWidth = laserWidth;

        raycastFrontRight.GetComponent<LineRenderer>().SetPositions(initLaserPositions);
        raycastFrontRight.GetComponent<LineRenderer>().startWidth = raycastFront.GetComponent<LineRenderer>().endWidth = laserWidth;

        raycastMidRight.GetComponent<LineRenderer>().SetPositions(initLaserPositions);
        raycastMidRight.GetComponent<LineRenderer>().startWidth = raycastFront.GetComponent<LineRenderer>().endWidth = laserWidth;

        raycastBackRight.GetComponent<LineRenderer>().SetPositions(initLaserPositions);
        raycastBackRight.GetComponent<LineRenderer>().startWidth = raycastFront.GetComponent<LineRenderer>().endWidth = laserWidth;

        frontCenterData = new RaycastData(laserMaxLength, Vector3.zero, null);
        frontLeftData = new RaycastData(laserMaxLength, Vector3.zero, null);
        frontRightData = new RaycastData(laserMaxLength, Vector3.zero, null);
        midLeftData = new RaycastData(laserMaxLength, Vector3.zero, null);
        midRightData = new RaycastData(laserMaxLength, Vector3.zero, null);
        backLeftData = new RaycastData(laserMaxLength, Vector3.zero, null);
        backRightData = new RaycastData(laserMaxLength, Vector3.zero, null);
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        ShootLaserFromTargetPosition(raycastFront, raycastFront.transform.position, raycastFront.transform.forward, laserMaxLength);

        ShootLaserFromTargetPosition(raycastFrontLeft, raycastFrontLeft.transform.position, raycastFrontLeft.transform.forward, laserMaxLength);
        ShootLaserFromTargetPosition(raycastFrontRight, raycastFrontRight.transform.position, raycastFrontRight.transform.forward, laserMaxLength);

        ShootLaserFromTargetPosition(raycastMidLeft, raycastMidLeft.transform.position, raycastMidLeft.transform.forward, laserMaxLength);
        ShootLaserFromTargetPosition(raycastMidRight, raycastMidRight.transform.position, raycastMidRight.transform.forward, laserMaxLength);

        ShootLaserFromTargetPosition(raycastBackLeft, raycastBackLeft.transform.position, raycastBackLeft.transform.forward, laserMaxLength);
        ShootLaserFromTargetPosition(raycastBackRight, raycastBackRight.transform.position, raycastBackRight.transform.forward, laserMaxLength);
    }

    /*
     * Function to shoot a raycast from the hand forward
     */
    void ShootLaserFromTargetPosition(GameObject raycaster, Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        RaycastHit raycastHit;
        Vector3 endPosition = targetPosition + (length * direction);

        if (Physics.Raycast(ray, out raycastHit, length))
        {
            raycaster.GetComponent<LineRenderer>().material = raycastGreen;
            GameObject other = raycastHit.transform.gameObject;
            endPosition = raycastHit.point;
            if (!other.CompareTag("Island") && !other.CompareTag("Tile"))
            {
                if (raycaster == raycastFront)
                {
                    frontCenterData.distance = Vector3.Distance(targetPosition, raycastHit.point);
                    frontCenterData.normal = raycastHit.normal;
                    frontCenterData.objectHit = other;
                }

                else if (raycaster == raycastFrontLeft)
                {
                    frontLeftData.distance = Vector3.Distance(targetPosition, raycastHit.point);
                    frontLeftData.normal = raycastHit.normal;
                    frontLeftData.objectHit = other;
                }

                else if (raycaster == raycastFrontRight)
                {
                    frontRightData.distance = Vector3.Distance(targetPosition, raycastHit.point);
                    frontRightData.normal = raycastHit.normal;
                    frontRightData.objectHit = other;
                }
                else if(raycaster == raycastMidLeft)
                {
                    midLeftData.distance = Vector3.Distance(targetPosition, raycastHit.point);
                    midLeftData.normal = raycastHit.normal;
                    midLeftData.objectHit = other;
                }

                else if(raycaster == raycastMidRight)
                {
                    midRightData.distance = Vector3.Distance(targetPosition, raycastHit.point);
                    midRightData.normal = raycastHit.normal;
                    midRightData.objectHit = other;
                }
                else if (raycaster == raycastBackLeft)
                {
                    backLeftData.distance = Vector3.Distance(targetPosition, raycastHit.point);
                    backLeftData.normal = raycastHit.normal;
                    backLeftData.objectHit = other;
                }
                else if (raycaster == raycastBackRight)
                {
                    backRightData.distance = Vector3.Distance(targetPosition, raycastHit.point);
                    backRightData.normal = raycastHit.normal;
                    backRightData.objectHit = other;
                }
            }
        }
        else
        {
            raycaster.GetComponent<LineRenderer>().material = raycastRed;

            if (raycaster == raycastFront)
            {
                frontCenterData = new RaycastData(length);
            }

            else if (raycaster == raycastFrontLeft)
            {
                frontLeftData = new RaycastData(length);
            }

            else if (raycaster == raycastFrontRight)
            {
                frontRightData = new RaycastData(length);
            }
            else if (raycaster == raycastMidLeft)
            {
                midLeftData = new RaycastData(length);
            }
            else if (raycaster == raycastMidRight)
            {
                midRightData = new RaycastData(length);
            }
            else if (raycaster == raycastBackLeft)
            {
                backLeftData = new RaycastData(length);
            }
            else if (raycaster == raycastBackRight)
            {
                backRightData = new RaycastData(length);
            }
        }

        raycaster.GetComponent<LineRenderer>().SetPosition(0, targetPosition);
        raycaster.GetComponent<LineRenderer>().SetPosition(1, endPosition);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(raycastFront.transform.position, raycastFront.transform.forward * laserMaxLength);

        Gizmos.DrawRay(raycastFrontLeft.transform.position, raycastFrontLeft.transform.forward * laserMaxLength);
        Gizmos.DrawRay(raycastFrontRight.transform.position, raycastFrontRight.transform.forward * laserMaxLength);

        Gizmos.DrawRay(raycastMidLeft.transform.position, raycastMidLeft.transform.forward * laserMaxLength);
        Gizmos.DrawRay(raycastMidRight.transform.position, raycastMidRight.transform.forward * laserMaxLength);

        Gizmos.DrawRay(raycastBackLeft.transform.position, raycastBackLeft.transform.forward * laserMaxLength);
        Gizmos.DrawRay(raycastBackRight.transform.position, raycastBackRight.transform.forward * laserMaxLength);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(raycastFront.transform.position, 0.1f);
        Gizmos.DrawSphere(raycastFrontLeft.transform.position, 0.1f);
        Gizmos.DrawSphere(raycastFrontRight.transform.position, 0.1f);
        Gizmos.DrawSphere(raycastMidLeft.transform.position, 0.1f);
        Gizmos.DrawSphere(raycastMidRight.transform.position, 0.1f);
        Gizmos.DrawSphere(raycastBackLeft.transform.position, 0.1f);
        Gizmos.DrawSphere(raycastBackRight.transform.position, 0.1f);
    }
}

[System.Serializable]
public class RaycastData
{
    public float distance;

    public Vector3 normal;

    public GameObject objectHit;

    public RaycastData(float newDistance, Vector3 newNormal, GameObject newHit)
    {
        distance = newDistance;
        normal = newNormal;
        objectHit = newHit;
    }

    public RaycastData(float newDistance)
    {
        distance = newDistance;
        normal = Vector3.zero;
        objectHit = null;
    }
}
