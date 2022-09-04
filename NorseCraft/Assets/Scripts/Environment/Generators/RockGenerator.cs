using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGenerator : MonoBehaviour
{
    public TileData.TileBiome biome = TileData.TileBiome.Temperate;
    [Space(10)]
    public GameObject currentRock;
    [ShowOnly] public int rockIndex = 0;

    [Tooltip("Percent change out of 100 to spawn a large rock instead of small")]
    public int largeChance = 80;

    [Space(15)]
    public List<RockCollection> rockCollection;

    /// <summary>
    /// Generate a random rock associated with the assigned biome. <para />
    ///     Biomes: (Temperate, Tropical, Pine, Snow)
    /// </summary>
    public void SpawnRock(TileData.TileBiome newBiome)
    {
        biome = newBiome;
        List<GameObject> rocks = new List<GameObject>();
        int prefabIndex = 0;

        DestroyRock();

        bool spawnlarge = false;
        int chanceToSpawnLarge = Random.Range(0, 101);
        if (chanceToSpawnLarge > largeChance)
            spawnlarge = true;

        switch (biome)
        {
            case TileData.TileBiome.Temperate:
                if (rockCollection.Count > 0)
                {
                    if (spawnlarge)
                        rocks = rockCollection[0].largeRocks;
                    else
                        rocks = rockCollection[0].smallRocks;
                    prefabIndex = Random.Range(0, rocks.Count);
                }
                break;

            case TileData.TileBiome.Tropical:
                if (rockCollection.Count > 1)
                {
                    if (spawnlarge)
                        rocks = rockCollection[1].largeRocks;
                    else
                        rocks = rockCollection[1].smallRocks;
                    prefabIndex = Random.Range(0, rocks.Count);
                }
                break;

            case TileData.TileBiome.Pine:
                if (rockCollection.Count > 2)
                {
                    if (spawnlarge)
                        rocks = rockCollection[2].largeRocks;
                    else
                        rocks = rockCollection[2].smallRocks;
                    prefabIndex = Random.Range(0, rocks.Count);
                }
                break;

            case TileData.TileBiome.Snow:
                if (rockCollection.Count > 3)
                {
                    if (spawnlarge)
                        rocks = rockCollection[3].largeRocks;
                    else
                        rocks = rockCollection[3].smallRocks;
                    prefabIndex = Random.Range(0, rocks.Count);
                }
                break;

            default:
                rocks = null;
                break;
        }

        if (rocks == null)
            return;

        if (rocks[prefabIndex] != null)
        {
            GameObject newRock = Instantiate(rocks[prefabIndex], transform.position, transform.rotation);
            newRock.name = rocks[prefabIndex].name + " (" + biome.ToString() + ")";
            newRock.transform.SetParent(transform);
            newRock.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            newRock.transform.localPosition = new Vector3(Random.Range(-2.5f, 2.5f), 0, Random.Range(-2.5f, 2.5f));
            currentRock = newRock;
            rockIndex = prefabIndex;

            if(newRock.GetComponent<ProceduralComponent>() != null)
            {
                newRock.transform.position += newRock.GetComponent<ProceduralComponent>().groundOffset;
            }
        }
    }

    /// <summary>
    /// Destroy the current selected tree
    /// </summary>
    public void DestroyRock()
    {
        if (currentRock != null)
        {
            GameData.SafeDestroy(currentRock);
        }
    }

}

[System.Serializable]
public class RockCollection
{
    public string type;
    public List<GameObject> smallRocks;
    public List<GameObject> largeRocks;

    public RockCollection()
    {
        type = "new rock type";
        smallRocks = new List<GameObject>();
        largeRocks = new List<GameObject>();
    }
}
