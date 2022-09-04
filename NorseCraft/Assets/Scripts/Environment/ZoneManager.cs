using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public ZoneData data;
    [Space(20)]
    public GameObject detailObjects;
    public GameObject closeObjects;
    public GameObject farObjects;
    
    public ZoneTreeData treeData;

    private PoolManager objectPool;

    // Use this for initialization
    void Start ()
    {
        objectPool = GameManager.manager.poolManager;
        data.zonePosition = transform.position;
        LoadDistantObjects();

        treeData.InitializeTrees();
    }

    /*
     * Disable details and enable distant objects
     */
    private void LoadDistantObjects()
    {
        if (detailObjects != null)
            detailObjects.SetActive(false);
        if (closeObjects != null)
            closeObjects.SetActive(false);
        if (farObjects != null)
            farObjects.SetActive(true);
    }

    /*
     * Disable distant objects and enable details
     */
    private void LoadDetailObjects()
    {
        if (farObjects != null)
            farObjects.SetActive(false);
        if (detailObjects != null)
            detailObjects.SetActive(true);
        if (closeObjects != null)
            closeObjects.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LoadDetailObjects();
            treeData.SpawnTrees(objectPool);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LoadDistantObjects();
            treeData.DespawnTrees(objectPool);
        }
    }

}

[System.Serializable]
public class ZoneData
{
    public string zoneName;

    [ShowOnly]public Vector3 zonePosition;
}

[System.Serializable]
public class ZoneTreeData
{
    public GameObject treeParent;
    public GameObject treePrefab;
    [Space(10)]
    public Transform[] zoneTreeTransforms;
    public TreeData[] trees;

    public void InitializeTrees()
    {
        zoneTreeTransforms = new Transform[treeParent.transform.childCount];
        trees = new TreeData[zoneTreeTransforms.Length];
        for (int i = 0; i < zoneTreeTransforms.Length; i++)
        {
            zoneTreeTransforms[i] = treeParent.transform.GetChild(i);
            trees[i] = new TreeData(null, 0.0f, true);
        }
    }

    /*
     * Spawn all standing trees
     */
    public void SpawnTrees(PoolManager objectPool)
    {
        for (int i = 0; i < zoneTreeTransforms.Length; i++)
        {
            if (trees[i].isStanding)
            {
                trees[i].tree = objectPool.SpawnFromPool(treePrefab.name, zoneTreeTransforms[i].position, zoneTreeTransforms[i].rotation);
            }
        }
    }

    /*
     * Despawn all trees
     */
    public void DespawnTrees(PoolManager objectPool)
    {
        for (int i = 0; i < zoneTreeTransforms.Length; i++)
        {
            if (trees[i].tree != null)
            {
                objectPool.ReturnObject(trees[i].tree);
                trees[i].tree = null;
            }
        }
    }
}

[System.Serializable]
public class TreeData
{
    public GameObject tree;
    public float timeChopped;
    public bool isStanding;

    public TreeData()
    {
        tree = null;
        timeChopped = 0.0f;
        isStanding = false;
    }

    public TreeData(GameObject newTree, float timeAtChop, bool alive)
    {
        tree = newTree;
        timeChopped = timeAtChop;
        isStanding = alive;
    }
}
