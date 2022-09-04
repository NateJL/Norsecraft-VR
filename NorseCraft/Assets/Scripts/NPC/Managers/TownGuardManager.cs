using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownGuardManager : MonoBehaviour
{
    [Header("Guard Prefabs")]
    public GameObject standingGuardPrefab;
    public GameObject pathingGuardPrefab;
    [Space(10)]
    public GameObject guardParent;
    public GameObject spawnParent;
    private GameObject[] spawnPoints;

    public int pathingGuardCount = 1;
    [Space(20)]
    public List<GameObject> spawnedGuards;

    private PoolManager objectPool;

    //private GameObject[] spawnWaypoints;


    // Use this for initialization
    void Start ()
    {
        objectPool = GameManager.manager.poolManager;
        int standingSpawnCount = spawnParent.transform.childCount;
        spawnPoints = new GameObject[standingSpawnCount];
        spawnedGuards = new List<GameObject>();
        for(int i = 0; i < standingSpawnCount; i++)
        {
            spawnPoints[i] = spawnParent.transform.GetChild(i).gameObject;
        }

        /*
        int pathingSpawnCount = transform.GetChild(1).childCount;
        spawnWaypoints = new GameObject[pathingSpawnCount];
        for (int i = 0; i < pathingSpawnCount; i++)
        {
            spawnWaypoints[i] = transform.GetChild(1).GetChild(i).gameObject;
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            for(int i = 0; i < spawnPoints.Length; i++)
            {
                //GameObject newGuard = Instantiate(standingGuardPrefab, spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                GameObject newGuard = objectPool.SpawnFromPool(standingGuardPrefab.name, spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                newGuard.transform.parent = guardParent.transform;
                spawnedGuards.Add(newGuard);
            }
            /*
            GameObject pathingGuard = Instantiate(pathingGuardPrefab, spawnWaypoints[0].transform.position, spawnWaypoints[0].transform.rotation);
            pathingGuard.GetComponent<TownGuardController>().npcData.currentDestination = spawnWaypoints[0].GetComponent<Waypoint>().GetRandomWaypoint();
            pathingGuard.transform.parent = guardParent.transform;
            spawnedGuards[spawnPoints.Length] = pathingGuard;
            */
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach(GameObject guard in spawnedGuards)
            {
                objectPool.ReturnObject(guard);
            }
            spawnedGuards.Clear();
        }
    }

}
