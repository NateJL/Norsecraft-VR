using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Header("Waypoint Data")]
    public Vector3 waypointPosition;
    public Vector3 waypointRotation;
    [Space(10)]
    public GameObject[] nextWaypoints;

    [Space(10)]
    public float distanceThreshold = 1.0f;

    // Use this for initialization
    void Start()
    {
        waypointPosition = transform.position;
        waypointRotation = transform.rotation.eulerAngles;
    }

    /*
     * Returns a random next waypoint in the stored array of possible waypoints
     */
    public GameObject GetRandomWaypoint()
    {
        int index = Random.Range(0, nextWaypoints.Length - 1);
        return nextWaypoints[index];
    }

    /*
     * Returns true if the given position is within the distance threshold of the waypoint
     */
    public bool ReachedWaypoint(Vector3 characterPos)
    {
        Vector3 currentPos = new Vector3(transform.position.x, characterPos.y, transform.position.z);
        float distance = Vector3.Distance(currentPos, characterPos);
        if (distance <= distanceThreshold)
            return true;
        else
            return false;
    }


    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + Vector3.up, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);

        if (nextWaypoints.Length >= 1)
        {
            for (int i = 0; i < nextWaypoints.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine((transform.position), (nextWaypoints[i].transform.position + Vector3.up));
            }
        }
    }

}
