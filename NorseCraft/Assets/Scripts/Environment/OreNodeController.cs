using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreNodeController : MonoBehaviour
{
    public GameObject orePrefab;
    [Tooltip("Where a mined piece of ore spawns, first child transform of node")]
    private Transform oreSpawn;
    [Tooltip("Parent of current ores, third child transform of node")]
    private Transform oreParent;
    [Tooltip("Parent of ore spawn point collection, second child transform of node")]
    private Transform[] oreSpawns;

    public List<GameObject> currentOres;

    private PoolManager objectPool;

	// Use this for initialization
	void Start ()
    {
        objectPool = GameManager.manager.poolManager;
        if (objectPool == null)
            Debug.LogError(gameObject.name + ": Failed to get object pool");

        currentOres = new List<GameObject>();

        oreSpawn = transform.GetChild(0);
        oreParent = transform.GetChild(2);
        oreSpawns = new Transform[transform.GetChild(1).childCount];
        for(int i = 0; i < oreSpawns.Length; i++)
        {
            oreSpawns[i] = transform.GetChild(1).GetChild(i);
        }
        SpawnOres();
	}
	
    /*
     * Get ore objects from object pool and place them in proper positions
     */
	public void SpawnOres()
    {
        for(int i = 0; i < oreSpawns.Length; i++)
        {
            GameObject newOre = objectPool.SpawnFromPool(orePrefab.name, oreParent.position, oreParent.rotation);
            newOre.transform.SetParent(oreParent);
            newOre.transform.SetPositionAndRotation(oreSpawns[i].position, oreSpawns[i].rotation);
            newOre.transform.localScale = new Vector3(2, 2, 2);
            newOre.GetComponent<Rigidbody>().isKinematic = true;
            newOre.GetComponent<Rigidbody>().useGravity = false;
            newOre.GetComponent<ItemController>().isGrabbable = false;
            currentOres.Add(newOre);
        }
    }

    /*
     * Return ore objects to the object pool
     */
    public void ReturnOres()
    {
        for(int i = 0; i < currentOres.Count; i++)
        {
            objectPool.ReturnObject(currentOres[i]);
        }
        currentOres.Clear();
    }

    /*
     * Mine a single ore from the node
     */
    public void MineOre()
    {
        if(currentOres.Count > 0)
        {
            GameObject minedOre = currentOres[0];
            currentOres.RemoveAt(0);
            minedOre.transform.SetPositionAndRotation(oreSpawn.position, oreSpawn.rotation);
            minedOre.transform.SetParent(null);
            minedOre.GetComponent<Rigidbody>().isKinematic = false;
            minedOre.GetComponent<Rigidbody>().useGravity = true;
            minedOre.GetComponent<ItemController>().isGrabbable = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Blade"))
        {
            if (other.transform.parent != null && other.transform.parent.GetComponent<ItemController>() != null)
            {
                if (other.transform.parent.GetComponent<ItemController>().data.name.Equals("Pickaxe"))
                {
                    MineOre();
                    other.transform.parent.GetComponent<ItemController>().UseItem();
                }
            }
        }

    }
}
