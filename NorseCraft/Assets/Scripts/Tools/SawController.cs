using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawController : MonoBehaviour
{
    public GameObject woodPlankPrefab;

    private ItemController parentController;

    private PoolManager objectPool;

    // Use this for initialization
    void Start()
    {
        objectPool = GameManager.manager.poolManager;
        parentController = transform.parent.GetComponent<ItemController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ItemController>() != null)
        {
            if (other.gameObject.GetComponent<ItemController>().data.name.Equals("Wood Log"))
            {
                if (parentController.isActive)
                {
                    Quaternion plankRot = other.transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0));
                    GameObject newPlank = objectPool.SpawnFromPool(woodPlankPrefab.name, other.transform.position, plankRot);
                    newPlank.transform.localScale = newPlank.GetComponent<ItemController>().baseScale;
                    objectPool.ReturnObject(other.gameObject);
                    parentController.UseItem();
                }
            }
        }
    }
}
