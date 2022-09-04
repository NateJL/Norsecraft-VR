using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSpawner : MonoBehaviour
{
    public GameObject skeletonPrefab;
    public Transform skeletonSpawn;

    public float timePassed = 0.0f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(Time.time - timePassed > 5.0f)
            {
                timePassed = Time.time;
                Instantiate(skeletonPrefab, skeletonSpawn.position, skeletonSpawn.rotation);
            }
        }
    }
}
