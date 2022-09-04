using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public TileData.TileBiome biome = TileData.TileBiome.Temperate;
    [Space(10)]
    public GameObject currentTree;
    [ShowOnly] public int treeIndex = 0;
    [Space(15)]
    public List<TreeCollection> treeCollection;

    /// <summary>
    /// Generate a random tree associated with the assigned biome. <para />
    ///     Biomes: (Temperate, Tropical, Pine, Snow)
    /// </summary>
    public void SpawnTree(TileData.TileBiome newBiome)
    {
        biome = newBiome;
        List<GameObject> trees = new List<GameObject>();
        int prefabIndex = 0;

        DestroyTree();

        switch(biome)
        {
            case TileData.TileBiome.Temperate:
                if (treeCollection.Count > 0)
                {
                    trees = treeCollection[0].trees;
                    prefabIndex = Random.Range(0, trees.Count);
                }
                break;

            case TileData.TileBiome.Tropical:
                if (treeCollection.Count > 1)
                {
                    trees = treeCollection[1].trees;
                    prefabIndex = Random.Range(0, trees.Count);
                }
                break;

            case TileData.TileBiome.Pine:
                if (treeCollection.Count > 2)
                {
                    trees = treeCollection[2].trees;
                    prefabIndex = Random.Range(0, trees.Count);
                }
                break;

            case TileData.TileBiome.Snow:
                if (treeCollection.Count > 3)
                {
                    trees = treeCollection[3].trees;
                    prefabIndex = Random.Range(0, trees.Count);
                }
                break;

            default:
                trees = null;
                break;
        }

        if (trees == null)
            return;

        if(trees[prefabIndex] != null)
        {
            GameObject newTree = Instantiate(trees[prefabIndex], transform.position, transform.rotation);
            newTree.name = trees[prefabIndex].name + " (" + biome.ToString() + ")";
            newTree.transform.SetParent(transform);
            newTree.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            currentTree = newTree;
            treeIndex = prefabIndex;
        }
    }

    /// <summary>
    /// Destroy the current selected tree
    /// </summary>
    public void DestroyTree()
    {
        if(currentTree != null)
        {
            GameData.SafeDestroy(currentTree);
        }
    }
	
}

[System.Serializable]
public class TreeCollection
{
    public string type;
    public List<GameObject> trees;

    public TreeCollection()
    {
        type = "new tree type";
        trees = new List<GameObject>();
    }
}
