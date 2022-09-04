using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject islandBase;
    public List<ProceduralComponentList> islandDocks;
    public List<ProceduralComponentList> islandFences;
    public List<ProceduralComponentList> islandRocks;
    public List<ProceduralComponentList> islandTrees;
    public List<ProceduralComponentList> islandGrass;
    public List<ProceduralComponentList> islandStructures;
    public static string[] islandComponentList = { "Trees", "Rocks", "Grass", "Docks", "Fences", "Structures" };
    [Space(15)]
    public Transform baseParent;
    public Transform treeParent;
    public Transform rockParent;
    public Transform propParent;
    public Transform fenceParent;
    public Transform structureParent;
    public Transform grassParent;
    [Space(15)]
    public IslandData data;

    //private PoolManager objectPool;
    //private TileManager tileManager;

    // Use this for initialization
    public void Initialize ()
    {
        //objectPool = GameManager.manager.poolManager;
        //tileManager = GameManager.manager.tileManager;
        data.islandPosition = transform.position;
        GetParents();

        data.islandType = (IslandData.IslandType)Random.Range(0, (int)IslandData.IslandType.NUM_ISLAND_TYPES - 1);
        data.islandBiome = (IslandData.IslandBiome)Random.Range(0, (int)IslandData.IslandBiome.NUM_ISLAND_BIOMES - 1);

        SpawnIslandBase();
        SpawnIslandDock();
        SpawnIslandFences();
        SpawnIslandTrees();
        SpawnIslandRocks();
        SpawnGrass();
        SpawnStructures();
	}

    public void GetParents()
    {
        data.placedObjectDictionary = new Dictionary<Vector2, GameObject>();

        // Get or Create base parent
        if (transform.childCount > 0)
            baseParent = transform.GetChild(0);
        else
        {
            GameObject newParent = new GameObject();
            newParent.name = "BaseParent";
            newParent.transform.SetParent(transform);
            newParent.transform.localPosition = Vector3.zero;
            newParent.transform.localEulerAngles = Vector3.zero;
            baseParent = newParent.transform;
        }

        // Get or Create tree parent
        if (transform.childCount > 1)
            treeParent = transform.GetChild(1);
        else
        {
            GameObject newParent = new GameObject();
            newParent.name = "TreeParent";
            newParent.transform.SetParent(transform);
            newParent.transform.localPosition = Vector3.zero;
            newParent.transform.localEulerAngles = Vector3.zero;
            treeParent = newParent.transform;
        }

        // Get or Create rock parent
        if (transform.childCount > 2)
            rockParent = transform.GetChild(2);
        else
        {
            GameObject newParent = new GameObject();
            newParent.name = "RockParent";
            newParent.transform.SetParent(transform);
            newParent.transform.localPosition = Vector3.zero;
            newParent.transform.localEulerAngles = Vector3.zero;
            rockParent = newParent.transform;
        }

        // Get or Create prop parent
        if (transform.childCount > 3)
            propParent = transform.GetChild(3);
        else
        {
            GameObject newParent = new GameObject();
            newParent.name = "PropParent";
            newParent.transform.SetParent(transform);
            newParent.transform.localPosition = Vector3.zero;
            newParent.transform.localEulerAngles = Vector3.zero;
            propParent = newParent.transform;
        }

        // Get or Create fence parent
        if (transform.childCount > 4)
            fenceParent = transform.GetChild(4);
        else
        {
            GameObject newParent = new GameObject();
            newParent.name = "FenceParent";
            newParent.transform.SetParent(transform);
            newParent.transform.localPosition = Vector3.zero;
            newParent.transform.localEulerAngles = Vector3.zero;
            fenceParent = newParent.transform;
        }

        // Get or Create grass parent
        if (transform.childCount > 5)
            grassParent = transform.GetChild(5);
        else
        {
            GameObject newParent = new GameObject();
            newParent.name = "GrassParent";
            newParent.transform.SetParent(transform);
            newParent.transform.localPosition = Vector3.zero;
            newParent.transform.localEulerAngles = Vector3.zero;
            grassParent = newParent.transform;
        }

        // Get or Create structure parent
        if (transform.childCount > 6)
            structureParent = transform.GetChild(6);
        else
        {
            GameObject newParent = new GameObject();
            newParent.name = "StructureParent";
            newParent.transform.SetParent(transform);
            newParent.transform.localPosition = Vector3.zero;
            newParent.transform.localEulerAngles = Vector3.zero;
            structureParent = newParent.transform;
        }
    }

    /*
     * Spawn and generate the base mesh for the island
     */
    public void SpawnIslandBase()
    {
        if (islandBase == null)
            return;

        transform.localPosition = new Vector3(-100, 0, -100);
        GameObject newBase = Instantiate(islandBase, transform.position, transform.rotation);
        newBase.name = islandBase.name;
        newBase.transform.SetParent(baseParent);
        data.GenerateBaseMesh(newBase, data.islandHeight);
    }

    /*
     * Spawn the dock for the island based off of the island type
     */
    public void SpawnIslandDock()
    {
        Vector3 dockPosition = new Vector3(data.centerPoint.x, 0, 0);

        GameObject newDock = Instantiate(islandDocks[(int)data.islandType].components[(int)data.islandBiome]);
        newDock.name = islandDocks[(int)data.islandType].components[(int)data.islandBiome].name;
        newDock.transform.SetParent(propParent);
        newDock.transform.localPosition = dockPosition;
        newDock.transform.localEulerAngles = Vector3.zero;
    }

    /*
     * Spawn border fences for the island
     */
    public void SpawnIslandFences()
    {
        if (data.islandType == IslandData.IslandType.Wild)
            return;

        float z = 12.5f;
        GameObject newFencePrefab = islandFences[(int)data.islandType].components[0];
        while (z < (data.centerPoint.y - data.villageRadius))      // Spawn path to village
        {
            Vector3 fencePosition = new Vector3(data.centerPoint.x + data.baseData.tileSize, data.islandHeight, z);
            GameObject newFence = Instantiate(newFencePrefab);
            newFence.name = islandFences[(int)data.islandType].components[(int)data.islandBiome].name;
            newFence.transform.SetParent(fenceParent);
            newFence.transform.localPosition = fencePosition;
            newFence.transform.localEulerAngles = new Vector3(0, 90, 0);

            fencePosition = new Vector3(data.centerPoint.x - data.baseData.tileSize, data.islandHeight, z);
            newFence = Instantiate(newFencePrefab);
            newFence.name = islandFences[(int)data.islandType].components[(int)data.islandBiome].name;
            newFence.transform.SetParent(fenceParent);
            newFence.transform.localPosition = fencePosition;
            newFence.transform.localEulerAngles = new Vector3(0, 90, 0);
            z += data.baseData.tileSize;
        }
        newFencePrefab = islandFences[(int)data.islandType].components[1];
        while (z < (data.centerPoint.y + data.villageRadius))      // Spawn east/west village border
        {
            Vector3 fencePosition = new Vector3(data.centerPoint.x + data.villageRadius, data.islandHeight, z);
            GameObject newFence = Instantiate(newFencePrefab);
            newFence.name = islandFences[(int)data.islandType].components[(int)data.islandBiome].name;
            newFence.transform.SetParent(fenceParent);
            newFence.transform.localPosition = fencePosition;
            newFence.transform.localEulerAngles = new Vector3(0, 90, 0);

            fencePosition = new Vector3(data.centerPoint.x - data.villageRadius, data.islandHeight, z);
            newFence = Instantiate(newFencePrefab);
            newFence.name = islandFences[(int)data.islandType].components[(int)data.islandBiome].name;
            newFence.transform.SetParent(fenceParent);
            newFence.transform.localPosition = fencePosition;
            newFence.transform.localEulerAngles = new Vector3(0, 90, 0);
            z += data.baseData.tileSize;
        }
        z = data.centerPoint.y - data.villageRadius;
        float x = ((float)data.baseData.tileSize * 1.5f);
        while ((data.centerPoint.x + x) < (data.centerPoint.x + data.villageRadius))      // Spawn north/south village border
        {
            Vector3 fencePosition = new Vector3(data.centerPoint.x + x, data.islandHeight, z);
            GameObject newFence = Instantiate(newFencePrefab);
            newFence.name = islandFences[(int)data.islandType].components[(int)data.islandBiome].name;
            newFence.transform.SetParent(fenceParent);
            newFence.transform.localPosition = fencePosition;
            newFence.transform.localEulerAngles = new Vector3(0, 0, 0);

            fencePosition = new Vector3(data.centerPoint.x - x, data.islandHeight, z);
            newFence = Instantiate(newFencePrefab);
            newFence.name = islandFences[(int)data.islandType].components[(int)data.islandBiome].name;
            newFence.transform.SetParent(fenceParent);
            newFence.transform.localPosition = fencePosition;
            newFence.transform.localEulerAngles = new Vector3(0, 0, 0);

            fencePosition = new Vector3(data.centerPoint.x + x, data.islandHeight, z + data.villageRadius * 2);
            newFence = Instantiate(newFencePrefab);
            newFence.name = islandFences[(int)data.islandType].components[(int)data.islandBiome].name;
            newFence.transform.SetParent(fenceParent);
            newFence.transform.localPosition = fencePosition;
            newFence.transform.localEulerAngles = new Vector3(0, 0, 0);

            fencePosition = new Vector3(data.centerPoint.x - x, data.islandHeight, z + data.villageRadius * 2);
            newFence = Instantiate(newFencePrefab);
            newFence.name = islandFences[(int)data.islandType].components[(int)data.islandBiome].name;
            newFence.transform.SetParent(fenceParent);
            newFence.transform.localPosition = fencePosition;
            newFence.transform.localEulerAngles = new Vector3(0, 0, 0);

            x += data.baseData.tileSize;
        }
    }

    /*
     * Attempt to spawn trees on the island
     */
    public void SpawnIslandTrees()
    {
        int treeCount = Random.Range((int)data.treesMinMax.min, (int)data.treesMinMax.max);
        for (int i = 0; i < treeCount; i++)
        {
            GameObject newTreePrefab = islandTrees[(int)data.islandType].components[(int)data.islandBiome];
            bool foundPoint = false;
            int attemptCounter = 0;
            while (!foundPoint)
            {
                if (attemptCounter > 10)
                    break;
                bool isValid = true;
                float x = Random.Range(data.minPoint.x, data.maxPoint.x);
                float z = Random.Range(data.minPoint.y, data.maxPoint.y);
                z = z - (z % data.baseData.tileSize);
                x = x - (x % data.baseData.tileSize);

                if (data.islandType != IslandData.IslandType.Wild)
                {
                    // check if spawn is inside village
                    if (x >= (data.centerPoint.x - data.villageRadius) && x <= (data.centerPoint.x + data.villageRadius) &&
                        z >= (data.centerPoint.y - data.villageRadius) && z <= (data.centerPoint.y + data.villageRadius))
                    {
                        isValid = false;
                    }

                    // check if spawn in inside path from dock to village
                    if (z <= (data.centerPoint.y - data.villageRadius) &&
                       x >= (data.centerPoint.x - data.baseData.tileSize) && x <= (data.centerPoint.x + data.baseData.tileSize))
                    {
                        isValid = false;
                    }
                }

                Vector2 spawnPoint = new Vector2(x, z);
                if (!data.placedObjectDictionary.ContainsKey(spawnPoint) && isValid)
                {
                    foundPoint = true;
                    GameObject newTree = Instantiate(newTreePrefab);
                    newTree.name = newTreePrefab.name;
                    newTree.transform.SetParent(treeParent);
                    newTree.transform.localPosition = new Vector3(spawnPoint.x, data.islandHeight, spawnPoint.y);
                    newTree.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                    data.placedObjectDictionary.Add(spawnPoint, newTree);
                }
                else
                    foundPoint = false;
                attemptCounter++;
            }
        }
    }

    /*
     * Attempt to spawn a rock for the island
     */
    public void SpawnIslandRocks()
    {
        int rockCount = Random.Range((int)data.rocksMinMax.min, (int)data.rocksMinMax.max);
        for (int i = 0; i < rockCount; i++)
        {
            int rockIndex = Random.Range(0, islandRocks[(int)data.islandType].multiComponents[(int)data.islandBiome].components.Count);
            GameObject newRockPrefab = islandRocks[(int)data.islandType].multiComponents[(int)data.islandBiome].components[rockIndex];
            bool foundPoint = false;
            int attemptCounter = 0;
            while (!foundPoint)
            {
                if (attemptCounter > 10)
                    break;

                bool isValid = true;
                float x = Random.Range(data.minPoint.x, data.maxPoint.x);
                float z = Random.Range(data.minPoint.y, data.maxPoint.y);
                z = z - (z % data.baseData.tileSize);
                x = x - (x % data.baseData.tileSize);

                if (data.islandType != IslandData.IslandType.Wild)
                {
                    // check if spawn is inside village
                    if (x >= (data.centerPoint.x - data.villageRadius) && x <= (data.centerPoint.x + data.villageRadius) &&
                        z >= (data.centerPoint.y - data.villageRadius) && z <= (data.centerPoint.y + data.villageRadius))
                    {
                        isValid = false;
                    }

                    // check if spawn in inside path from dock to village
                    if (z <= (data.centerPoint.y - data.villageRadius) &&
                       x >= (data.centerPoint.x - data.baseData.tileSize) && x <= (data.centerPoint.x + data.baseData.tileSize))
                    {
                        isValid = false;
                    }
                }

                Vector2 spawnPoint = new Vector2(x, z);
                if (!data.placedObjectDictionary.ContainsKey(spawnPoint) && isValid)
                {
                    foundPoint = true;
                    if (newRockPrefab != null)
                    {
                        GameObject newRock = Instantiate(newRockPrefab);
                        newRock.name = newRockPrefab.name;
                        newRock.transform.SetParent(rockParent);
                        newRock.transform.localPosition = new Vector3(spawnPoint.x, data.islandHeight, spawnPoint.y) + newRock.GetComponent<ProceduralComponent>().groundOffset;
                        newRock.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                        data.placedObjectDictionary.Add(spawnPoint, newRock);
                    }
                }
                else
                    foundPoint = false;
                attemptCounter++;
            }
        }
    }

    public void SpawnGrass()
    {
        int grassCount = Random.Range((int)data.grassMinMax.min, (int)data.grassMinMax.max);
        for (int i = 0; i < grassCount; i++)
        {
            int grassIndex = Random.Range(0, islandGrass[(int)data.islandType].multiComponents[(int)data.islandBiome].components.Count);
            GameObject newGrassPrefab = islandGrass[(int)data.islandType].multiComponents[(int)data.islandBiome].components[grassIndex];
            bool foundPoint = false;
            int attemptCounter = 0;
            while (!foundPoint)
            {
                if (attemptCounter > 25)
                    break;

                bool isValid = true;
                float x = Random.Range(data.minPoint.x, data.maxPoint.x);
                float z = Random.Range(data.minPoint.y, data.maxPoint.y);
                z = z - (z % data.baseData.tileSize);
                x = x - (x % data.baseData.tileSize);

                if (data.islandType != IslandData.IslandType.Wild)
                {
                    // check if spawn is inside village
                    if (x >= (data.centerPoint.x - data.villageRadius) && x <= (data.centerPoint.x + data.villageRadius) &&
                        z >= (data.centerPoint.y - data.villageRadius) && z <= (data.centerPoint.y + data.villageRadius))
                    {
                        isValid = false;
                    }

                    // check if spawn in inside path from dock to village
                    if (z <= (data.centerPoint.y - data.villageRadius) &&
                       x >= (data.centerPoint.x - data.baseData.tileSize) && x <= (data.centerPoint.x + data.baseData.tileSize))
                    {
                        isValid = false;
                    }
                }

                Vector2 spawnPoint = new Vector2(x, z);
                if (data.placedObjectDictionary.ContainsKey(spawnPoint))
                {
                    if(data.placedObjectDictionary[spawnPoint].GetComponent<ProceduralComponent>() != null &&
                       data.placedObjectDictionary[spawnPoint].GetComponent<ProceduralComponent>().componentType.Equals(ProceduralComponent.ComponentType.Grass))
                    {
                        isValid = false;
                    }
                    else
                        newGrassPrefab = islandGrass[(int)data.islandType].multiComponents[(int)data.islandBiome].components[0];
                }
                if (newGrassPrefab != null && isValid)
                {
                    foundPoint = true;
                    GameObject newGrass = Instantiate(newGrassPrefab);
                    newGrass.name = newGrassPrefab.name;
                    newGrass.transform.SetParent(grassParent);
                    newGrass.transform.localPosition = new Vector3(spawnPoint.x, data.islandHeight, spawnPoint.y) + newGrass.GetComponent<ProceduralComponent>().groundOffset;
                    newGrass.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                }
                else
                    break;
                attemptCounter++;
            }
        }
    }

    public void SpawnStructures()
    {
        int structureCount = Random.Range((int)data.structuresMinMax.min, (int)data.structuresMinMax.max);
        for (int i = 0; i < structureCount; i++)
        {
            int structureIndex = 0;
            GameObject newStructurePrefab = islandStructures[(int)data.islandType].multiComponents[(int)data.islandBiome].components[structureIndex];
            bool foundPoint = false;
            int attemptCounter = 0;
            while (!foundPoint)
            {
                if (attemptCounter > 10)
                    break;

                bool isValid = true;
                float x = 0;
                float z = 0;
                Vector3 structureRotation = Vector3.zero;

                int placementIndex = Random.Range(0, 2);
                if(placementIndex == 0) // west side
                {
                    x = data.centerPoint.x - data.baseData.tileSize * 1.5f;
                    z = Random.Range(data.centerPoint.y - data.villageRadius + (data.baseData.tileSize * 0.5f), data.centerPoint.y + data.villageRadius - (data.baseData.tileSize * 0.5f));
                    if (z < data.centerPoint.y && z > data.centerPoint.y - data.baseData.tileSize)
                        z -= data.baseData.tileSize;
                    else if (z > data.centerPoint.y && z < data.centerPoint.y + data.baseData.tileSize)
                        z += data.baseData.tileSize;
                    structureRotation = new Vector3(0, 90, 0);
                }
                else if(placementIndex == 1) // east side
                {
                    x = data.centerPoint.x + data.baseData.tileSize * 1.5f;
                    z = Random.Range(data.centerPoint.y - data.villageRadius + (data.baseData.tileSize * 0.5f), data.centerPoint.y + data.villageRadius - (data.baseData.tileSize * 0.5f));
                    if (z < data.centerPoint.y && z > data.centerPoint.y - data.baseData.tileSize)
                        z -= data.baseData.tileSize;
                    else if (z > data.centerPoint.y && z < data.centerPoint.y + data.baseData.tileSize)
                        z += data.baseData.tileSize;
                    structureRotation = new Vector3(0, 270, 0);
                }
                else if (placementIndex == 2)
                {

                }
                else if (placementIndex == 3)
                {

                }
                else if (placementIndex == 4)
                {

                }
                //float x = Random.Range(data.centerPoint.x - data.villageRadius + (data.baseData.tileSize * 0.5f), data.centerPoint.x + data.villageRadius - (data.baseData.tileSize * 0.5f));
                z = z - (z % (data.baseData.tileSize*0.5f));
                x = x - (x % (data.baseData.tileSize * 0.5f));

                if (z % data.baseData.tileSize == 0)
                    z += (data.baseData.tileSize * 0.5f);
                if (x % data.baseData.tileSize == 0)
                    x += (data.baseData.tileSize * 0.5f);

                if(x > data.centerPoint.x - data.baseData.tileSize && x < data.centerPoint.x + data.baseData.tileSize)
                {
                    if (x < data.centerPoint.x)
                        x -= data.baseData.tileSize;
                    else
                        x += data.baseData.tileSize;
                }

                if (data.islandType == IslandData.IslandType.Wild)
                {
                    isValid = false;
                }

                Vector2 spawnPoint = new Vector2(x, z);
                if (!data.placedObjectDictionary.ContainsKey(spawnPoint) && isValid)
                {
                    foundPoint = true;
                    if (newStructurePrefab != null)
                    {
                        GameObject newStructure = Instantiate(newStructurePrefab);
                        newStructure.name = newStructurePrefab.name;
                        newStructure.transform.SetParent(structureParent);
                        newStructure.transform.localPosition = new Vector3(spawnPoint.x, data.islandHeight, spawnPoint.y);
                        newStructure.transform.localEulerAngles = structureRotation;
                        data.placedObjectDictionary.Add(spawnPoint, newStructure);
                    }
                }
                else
                    foundPoint = false;
                attemptCounter++;
            }
        }
    }

    /*
     * Reset the island, destroying all children
     */
    public void ResetIsland()
    {
        for(int i = transform.childCount-1; i >= 0; i--)
        {
            for (int j = transform.GetChild(i).childCount-1; j >= 0; j--)
            {
                if (transform.GetChild(i).childCount == 0)
                    return;
                PrefabPlacer.SafeDestroy(transform.GetChild(i).GetChild(j).gameObject);
            }
        }
        data.placedObjectDictionary.Clear();
    }

    
    private void OnDrawGizmos()
    {
        if(data.baseData.islandBase != null)
        {
            Gizmos.color = Color.red;
            for(int i = 0; i < data.baseData.baseMeshData.vertices.Length; i++)
            {
                Gizmos.DrawSphere(transform.position + data.baseData.baseMeshData.vertices[i], 0.3f);
            }
        }
    }

    /*
    private void OnValidate()
    {
        GetParents();

        if(islandDocks.Count < (int)IslandData.IslandType.NUM_ISLAND_TYPES)
        {
            int diff = ((int)IslandData.IslandType.NUM_ISLAND_TYPES) - islandDocks.Count;
            for (int i = 0; i < diff; i++)
                islandDocks.Add(new ProceduralComponentList());
        }
        if (islandFences.Count < (int)IslandData.IslandType.NUM_ISLAND_TYPES)
        {
            int diff = ((int)IslandData.IslandType.NUM_ISLAND_TYPES) - islandFences.Count;
            for (int i = 0; i < diff; i++)
                islandFences.Add(new ProceduralComponentList());
        }
        if (islandTrees.Count < (int)IslandData.IslandType.NUM_ISLAND_TYPES)
        {
            int diff = ((int)IslandData.IslandType.NUM_ISLAND_TYPES) - islandTrees.Count;
            for (int i = 0; i < diff; i++)
                islandTrees.Add(new ProceduralComponentList());
        }
        if (islandRocks.Count < (int)IslandData.IslandType.NUM_ISLAND_TYPES)
        {
            int diff = ((int)IslandData.IslandType.NUM_ISLAND_TYPES) - islandRocks.Count;
            for (int i = 0; i < diff; i++)
                islandRocks.Add(new ProceduralComponentList());
        }
        if (islandGrass.Count < (int)IslandData.IslandType.NUM_ISLAND_TYPES)
        {
            int diff = ((int)IslandData.IslandType.NUM_ISLAND_TYPES) - islandGrass.Count;
            for (int i = 0; i < diff; i++)
                islandGrass.Add(new ProceduralComponentList());
        }
        if (islandStructures.Count < (int)IslandData.IslandType.NUM_ISLAND_TYPES)
        {
            int diff = ((int)IslandData.IslandType.NUM_ISLAND_TYPES) - islandStructures.Count;
            for (int i = 0; i < diff; i++)
                islandStructures.Add(new ProceduralComponentList());
        }

        for (int i = 0; i < islandDocks.Count; i++)
        {
            islandDocks[i].componentType = ((IslandData.IslandType)i).ToString();
            if(islandDocks[i].components.Count < (int)IslandData.IslandBiome.NUM_ISLAND_BIOMES)
            {
                int diff = ((int)IslandData.IslandBiome.NUM_ISLAND_BIOMES) - islandDocks[i].components.Count;
                for (int j = 0; j < diff; j++)
                    islandDocks[i].components.Add(null);
            }
        }
        for (int i = 0; i < islandFences.Count; i++)
        {
            islandFences[i].componentType = ((IslandData.IslandType)i).ToString();
            if (islandFences[i].components.Count < (int)IslandData.IslandBiome.NUM_ISLAND_BIOMES)
            {
                int diff = ((int)IslandData.IslandBiome.NUM_ISLAND_BIOMES) - islandFences[i].components.Count;
                for (int j = 0; j < diff; j++)
                    islandFences[i].components.Add(null);
            }
        }
        for (int i = 0; i < islandTrees.Count; i++)
        {
            islandTrees[i].componentType = ((IslandData.IslandType)i).ToString();
            if (islandTrees[i].components.Count < (int)IslandData.IslandBiome.NUM_ISLAND_BIOMES)
            {
                int diff = ((int)IslandData.IslandBiome.NUM_ISLAND_BIOMES) - islandTrees[i].components.Count;
                for (int j = 0; j < diff; j++)
                    islandTrees[i].components.Add(null);
            }
        }

        // check rock prefabs
        for (int i = 0; i < islandRocks.Count; i++)
        {
            islandRocks[i].componentType = ((IslandData.IslandType)i).ToString();
            if (islandRocks[i].multiComponents.Count < (int)IslandData.IslandBiome.NUM_ISLAND_BIOMES)
            {
                int diff = ((int)IslandData.IslandBiome.NUM_ISLAND_BIOMES) - islandRocks[i].multiComponents.Count;
                for (int j = 0; j < diff; j++)
                {
                    islandRocks[i].multiComponents.Add(new ProceduralComponentData());
                    islandRocks[i].multiComponents[islandRocks[i].multiComponents.Count - 1].components.Add(null);
                }
            }
            for (int j = 0; j < islandRocks[i].multiComponents.Count; j++)
            {
                islandRocks[i].multiComponents[j].componentType = ((IslandData.IslandBiome)j).ToString();
            }
        }

        // Check grass prefabs
        for (int i = 0; i < islandGrass.Count; i++)
        {
            islandGrass[i].componentType = ((IslandData.IslandType)i).ToString();
            if (islandGrass[i].multiComponents.Count < (int)IslandData.IslandBiome.NUM_ISLAND_BIOMES)
            {
                int diff = ((int)IslandData.IslandBiome.NUM_ISLAND_BIOMES) - islandGrass[i].multiComponents.Count;
                for (int j = 0; j < diff; j++)
                {
                    islandGrass[i].multiComponents.Add(new ProceduralComponentData());
                    islandGrass[i].multiComponents[islandGrass[i].multiComponents.Count - 1].components.Add(null);
                }
            }
            for (int j = 0; j < islandGrass[i].multiComponents.Count; j++)
            {
                islandGrass[i].multiComponents[j].componentType = ((IslandData.IslandBiome)j).ToString();
            }
        }

        // check structure prefabs
        for (int i = 0; i < islandStructures.Count; i++)
        {
            islandStructures[i].componentType = ((IslandData.IslandType)i).ToString();
            if (islandStructures[i].multiComponents.Count < (int)IslandData.IslandBiome.NUM_ISLAND_BIOMES)
            {
                int diff = ((int)IslandData.IslandBiome.NUM_ISLAND_BIOMES) - islandStructures[i].multiComponents.Count;
                for (int j = 0; j < diff; j++)
                {
                    islandStructures[i].multiComponents.Add(new ProceduralComponentData());
                    islandStructures[i].multiComponents[islandStructures[i].multiComponents.Count - 1].components.Add(null);
                }
            }
            for (int j = 0; j < islandStructures[i].multiComponents.Count; j++)
            {
                islandStructures[i].multiComponents[j].componentType = ((IslandData.IslandBiome)j).ToString();
            }
        }
    }
    */
}

[System.Serializable]
public class IslandData
{
    public enum IslandType
    {
        Wild = 0,
        Pirate,
        Viking,
        NUM_ISLAND_TYPES
    }
    public enum IslandBiome
    {
        Sand = 0,
        Grass,
        Snow,
        NUM_ISLAND_BIOMES
    }

    public string islandName;
    public int islandID;
    [Space(10)]
    public IslandType islandType = IslandType.Wild;
    public IslandBiome islandBiome = IslandBiome.Sand;
    [Space(10)]
    public SpawnData rocksMinMax;
    public SpawnData treesMinMax;
    public SpawnData grassMinMax;
    public SpawnData structuresMinMax;
    [Space(10)]
    public float islandHeight = 1;
    [ShowOnly] public Vector2 centerPoint;
    [ShowOnly] public Vector2 minPoint;
    [ShowOnly] public Vector2 maxPoint;
    [ShowOnly] public float villageRadius = 0;


    [ShowOnly]public Vector3 islandPosition;

    public Dictionary<Vector2, GameObject> placedObjectDictionary;

    public IslandBaseData baseData;

    public void GenerateBaseMesh(GameObject newBase, float islandHeight)
    {
        //baseData.xSize = 32;
        //baseData.zSize = 32;

        centerPoint = new Vector2((baseData.xSize / 2) * baseData.tileSize,
                                       (baseData.zSize / 2) * baseData.tileSize);

        maxPoint = new Vector2((baseData.xSize * baseData.tileSize) - (baseData.tileSize * baseData.tileSize),
                                    (baseData.zSize * baseData.tileSize) - (baseData.tileSize * baseData.tileSize));

        minPoint = new Vector2((baseData.tileSize * baseData.tileSize),
                                    (baseData.tileSize * baseData.tileSize));

        villageRadius = baseData.tileSize * Random.Range(5, 10);

        baseData.GenerateBaseMesh(newBase, islandHeight, this);
    }
}

[System.Serializable]
public class IslandBaseData
{
    public GameObject islandBase;
    public int tileSize = 5;
    public int xSize = 1;
    public int zSize = 1;

    public MeshData baseMeshData;

    public void GenerateBaseMesh(GameObject newBase, float islandHeight, IslandData parentData)
    {
        islandBase = newBase;
        baseMeshData.mesh = new Mesh();
        islandBase.GetComponent<MeshFilter>().mesh = baseMeshData.mesh;

        //CreateShape(islandHeight, parentData);
        CreateShapeHardEdges(islandBase, parentData);
        //CreateShapePerlinNoise(islandHeight, parentData);
        UpdateMesh(islandBase, baseMeshData);
        Debug.Log("Vertex Count: " + baseMeshData.vertices.Length);
    }

    private void CreateShape(float islandHeight, IslandData parentData)
    {
        baseMeshData.vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        baseMeshData.colors = new Color[baseMeshData.vertices.Length];

        List<Vector3> tempVerts = new List<Vector3>();

        Vector2 center = new Vector2(xSize / 2, zSize / 2);
        float maxDistance = Vector2.Distance(center, new Vector2(0,0));
        float posX = Random.Range(0, 1000);
        float posZ = Random.Range(0, 1000);
        for(int i = 0, z = 0; z <= zSize; z++)
        {
            for(int x = 0; x <= xSize; x++)
            {
                float distanceToCenter = Vector2.Distance(center, new Vector2(x, z));

                if (x == 0 || x == xSize || z == 0 || z == zSize)
                {
                    baseMeshData.vertices[i] = new Vector3((x*tileSize), -3, (z*tileSize));
                }
                else if(distanceToCenter >= (xSize / 2))
                {
                    baseMeshData.vertices[i] = new Vector3((x * tileSize), -1.5f, (z * tileSize));
                }
                else
                {
                    //float y = Mathf.PerlinNoise((posX + x) * 0.3f, (posZ + z) * 0.3f);
                    //y *= 2f;
                    baseMeshData.vertices[i] = new Vector3((x * tileSize), islandHeight, (z * tileSize));
                    tempVerts.Add(baseMeshData.vertices[i]);
                }
                SetVertexColor(i, baseMeshData.vertices[i], parentData);
                
                i++;
            }
        }
        //baseMeshData.spawnVertices = new Vector3[tempVerts.Count];
        baseMeshData.spawnVertices = tempVerts.ToArray();

        baseMeshData.triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for(int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                if ((x < xSize / 2 && z < zSize / 2) || (x >= xSize / 2 && z >= zSize / 2))
                {
                    baseMeshData.triangles[tris + 0] = vert + 0;            // bottom left
                    baseMeshData.triangles[tris + 1] = vert + xSize + 1;    // top left
                    baseMeshData.triangles[tris + 2] = vert + 1;            // bottom right

                    baseMeshData.triangles[tris + 3] = vert + 1;            // bottom right
                    baseMeshData.triangles[tris + 4] = vert + xSize + 1;    // top left
                    baseMeshData.triangles[tris + 5] = vert + xSize + 2;    // top right

                }
                else
                {
                    baseMeshData.triangles[tris + 0] = vert + 0;            // bottom left
                    baseMeshData.triangles[tris + 1] = vert + xSize + 1;    // top left
                    baseMeshData.triangles[tris + 2] = vert + xSize + 2;    // top right

                    baseMeshData.triangles[tris + 3] = vert + 1;            // bottom right
                    baseMeshData.triangles[tris + 4] = vert + 0;            // bottom left
                    baseMeshData.triangles[tris + 5] = vert + xSize + 2;    // top right
                }
                vert++;
                tris += 6;
            }
            vert++;
        }

        
    }

    private void CreateShapeHardEdges(GameObject islandBase, IslandData parentData)
    {
        baseMeshData.vertices = new Vector3[xSize * zSize * 6];
        baseMeshData.colors = new Color[baseMeshData.vertices.Length];

        Vector2 center = new Vector2(xSize / 2, zSize / 2);
        float maxDistance = Vector2.Distance(center, new Vector2(0, 0));
        float posX = Random.Range(0, 1000);
        float posZ = Random.Range(0, 1000);

        int vertexCount = 0;
        for(int z = 0; z < zSize; z++)
        {
            for(int x = 0; x < xSize; x++)
            {
                Vector2 worldPosition = new Vector2(islandBase.transform.position.x + x * tileSize, islandBase.transform.position.z + z * tileSize);
                //float height = TileData.GetPerlinValue(worldPosition, tileSize * xSize * zSize, 20f);
                float height = 1;

                if (x == 0)
                {
                    baseMeshData.vertices[vertexCount] = new Vector3(x * tileSize, -2, z * tileSize);                // bottom left
                    SetVertexColor(vertexCount, baseMeshData.vertices[vertexCount], parentData);
                    vertexCount++;

                    baseMeshData.vertices[vertexCount] = new Vector3(x * tileSize, -2, z * tileSize + tileSize);     // top left
                    SetVertexColor(vertexCount, baseMeshData.vertices[vertexCount], parentData);
                    vertexCount++;
                }
                else
                {
                    worldPosition = new Vector2(islandBase.transform.position.x + x * tileSize, islandBase.transform.position.z + z * tileSize);
                    //height = TileData.GetPerlinValue(worldPosition, tileSize * xSize * zSize, 20f);
                    height *= 5f;
                    baseMeshData.vertices[vertexCount] = new Vector3(x * tileSize, height, z * tileSize);                // bottom left
                    SetVertexColor(vertexCount, baseMeshData.vertices[vertexCount], parentData);
                    vertexCount++;

                    worldPosition = new Vector2(islandBase.transform.position.x + x * tileSize, islandBase.transform.position.z + z * tileSize + tileSize);
                    //height = TileData.GetPerlinValue(worldPosition, tileSize * xSize * zSize, 20f);
                    height *= 5f;
                    baseMeshData.vertices[vertexCount] = new Vector3(x * tileSize, height, z * tileSize + tileSize);     // top left
                    SetVertexColor(vertexCount, baseMeshData.vertices[vertexCount], parentData);
                    vertexCount++;
                }

                worldPosition = new Vector2(islandBase.transform.position.x + x * tileSize + tileSize, islandBase.transform.position.z + z * tileSize);
                //height = TileData.GetPerlinValue(worldPosition, tileSize * xSize * zSize, 20f);
                height *= 5f;
                baseMeshData.vertices[vertexCount] = new Vector3(x * tileSize + tileSize, height, z * tileSize);     // bottom right
                SetVertexColor(vertexCount, baseMeshData.vertices[vertexCount], parentData);
                vertexCount++;




                worldPosition = new Vector2(islandBase.transform.position.x + x * tileSize + tileSize, islandBase.transform.position.z + z * tileSize);
                //height = TileData.GetPerlinValue(worldPosition, tileSize * xSize * zSize, 20f);
                height *= 5f;
                baseMeshData.vertices[vertexCount] = new Vector3(x * tileSize + tileSize, height, z * tileSize);             // bottom right
                SetVertexColor(vertexCount, baseMeshData.vertices[vertexCount], parentData);
                vertexCount++;

                if (x == 0)
                {
                    baseMeshData.vertices[vertexCount] = new Vector3(x * tileSize, -2, z * tileSize + tileSize);             // top left
                    SetVertexColor(vertexCount, baseMeshData.vertices[vertexCount], parentData);
                    vertexCount++;
                }
                else
                {
                    worldPosition = new Vector2(islandBase.transform.position.x + x * tileSize, islandBase.transform.position.z + z * tileSize + tileSize);
                    //height = TileData.GetPerlinValue(worldPosition, tileSize * xSize * zSize, 20f);
                    height *= 5f;
                    baseMeshData.vertices[vertexCount] = new Vector3(x * tileSize, height, z * tileSize + tileSize);             // top left
                    SetVertexColor(vertexCount, baseMeshData.vertices[vertexCount], parentData);
                    vertexCount++;
                }

                worldPosition = new Vector2(islandBase.transform.position.x + x * tileSize + tileSize, islandBase.transform.position.z + z * tileSize + tileSize);
                //height = TileData.GetPerlinValue(worldPosition, tileSize * xSize * zSize, 20f);
                height *= 5f;
                baseMeshData.vertices[vertexCount] = new Vector3(x * tileSize + tileSize, height, z * tileSize + tileSize);  // top right
                SetVertexColor(vertexCount, baseMeshData.vertices[vertexCount], parentData);
                vertexCount++;


            }
        }

        baseMeshData.triangles = new int[vertexCount];

        for(int i = 0; i < vertexCount; i++)
        {
            baseMeshData.triangles[i] = i;
        }


    }

    private void CreateShapePerlinNoise(float islandHeight, IslandData parentData)
    {
        baseMeshData.vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        baseMeshData.colors = new Color[baseMeshData.vertices.Length];

        List<Vector3> tempVerts = new List<Vector3>();

        Vector2 center = new Vector2(xSize / 2, zSize / 2);
        float maxDistance = Vector2.Distance(center, new Vector2(0, 0));
        float posX = Random.Range(0, 1000);
        float posZ = Random.Range(0, 1000);
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float xCoord = x/xSize;
                float zCoord = z/zSize;
                float y = Mathf.PerlinNoise(xCoord, zCoord);
                baseMeshData.vertices[i] = new Vector3((x * tileSize), y, (z * tileSize));

                SetVertexColor(i, baseMeshData.vertices[i], parentData);

                i++;
            }
        }
        //baseMeshData.spawnVertices = new Vector3[tempVerts.Count];
        baseMeshData.spawnVertices = tempVerts.ToArray();

        baseMeshData.triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                if ((x < xSize / 2 && z < zSize / 2) || (x >= xSize / 2 && z >= zSize / 2))
                {
                    baseMeshData.triangles[tris + 0] = vert + 0;            // bottom left
                    baseMeshData.triangles[tris + 1] = vert + xSize + 1;    // top left
                    baseMeshData.triangles[tris + 2] = vert + 1;            // bottom right

                    baseMeshData.triangles[tris + 3] = vert + 1;            // bottom right
                    baseMeshData.triangles[tris + 4] = vert + xSize + 1;    // top left
                    baseMeshData.triangles[tris + 5] = vert + xSize + 2;    // top right

                }
                else
                {
                    baseMeshData.triangles[tris + 0] = vert + 0;            // bottom left
                    baseMeshData.triangles[tris + 1] = vert + xSize + 1;    // top left
                    baseMeshData.triangles[tris + 2] = vert + xSize + 2;    // top right

                    baseMeshData.triangles[tris + 3] = vert + 1;            // bottom right
                    baseMeshData.triangles[tris + 4] = vert + 0;            // bottom left
                    baseMeshData.triangles[tris + 5] = vert + xSize + 2;    // top right
                }
                vert++;
                tris += 6;
            }
            vert++;
        }


    }

    private void UpdateMesh(GameObject obj, MeshData meshData)
    {
        meshData.mesh.Clear();

        meshData.mesh.vertices = meshData.vertices;
        meshData.mesh.triangles = meshData.triangles;
        meshData.mesh.colors = meshData.colors;

        meshData.mesh.RecalculateNormals();
        obj.GetComponent<MeshCollider>().sharedMesh = meshData.mesh;
    }

    public void SetVertexColor(int index, Vector3 position, IslandData parentData)
    {
        if (position.y < 0)
            baseMeshData.colors[index] = baseMeshData.sandColor;
        else
        {
            if (position.x >= (parentData.centerPoint.x - parentData.villageRadius) &&
                position.x <= (parentData.centerPoint.x + parentData.villageRadius) &&
                position.z >= (parentData.centerPoint.y - parentData.villageRadius) &&
                position.z <= (parentData.centerPoint.y + parentData.villageRadius) &&
                parentData.islandType != IslandData.IslandType.Wild)
            {
                baseMeshData.colors[index] = baseMeshData.sandColor;
            }
            else if (position.x == parentData.centerPoint.x)
            {
                if (position.z < (parentData.centerPoint.y - parentData.villageRadius) &&
                    parentData.islandType != IslandData.IslandType.Wild)
                {
                    baseMeshData.colors[index] = baseMeshData.sandColor;
                }
                else
                {
                    float xCoord = parentData.islandPosition.x + (position.x / (float)xSize);
                    float zCoord = parentData.islandPosition.z + (position.z / (float)zSize);
                    float lerp = Mathf.PerlinNoise(xCoord, zCoord);
                    baseMeshData.colors[index] = Color.Lerp(baseMeshData.grassLightColor, baseMeshData.grassDarkColor, lerp);
                }
            }
            else
            {
                float xCoord = parentData.islandPosition.x + (position.x / (float)xSize);
                float zCoord = parentData.islandPosition.z + (position.z / (float)zSize);
                float lerp = Mathf.PerlinNoise(xCoord, zCoord);
                baseMeshData.colors[index] = Color.Lerp(baseMeshData.grassLightColor, baseMeshData.grassDarkColor, lerp);
            }
        }
    }
}

[System.Serializable]
public class SpawnData
{
    public float min;
    public float max;
}

[System.Serializable]
public class ProceduralComponentList
{
    [HideInInspector]
    public string componentType;
    public List<GameObject> components;
    public List<ProceduralComponentData> multiComponents;

    public ProceduralComponentList()
    {
        componentType = "proceduralComponentList";
        components = new List<GameObject>();
        multiComponents = new List<ProceduralComponentData>();
    }
}

[System.Serializable]
public class ProceduralComponentData
{
    [HideInInspector]
    public string componentType;
    public List<GameObject> components;

    public ProceduralComponentData()
    {
        componentType = "None";
        components = new List<GameObject>();
    }
}

public class MeshTriangle
{
    public Vector3[] vertices;

    public MeshTriangle()
    {
        vertices = new Vector3[3];
    }
}

