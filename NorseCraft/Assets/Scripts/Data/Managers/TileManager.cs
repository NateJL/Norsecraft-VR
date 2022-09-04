using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public bool showAllSettlements;
    public bool showSettlement;
    public float settlementIndexMin;
    public float settlementIndexMax;
    [Space(10)]
    public int seed = 1337;
    [Space(15)]
    public int tileSize = 500;

    public GameObject tilePrefab;
    public GameObject homeTilePrefab;
    public GameObject settlementGeneratorPrefab;

    [Space(15)]
    public GameObject[,] allTiles;
    public TileController currentPlayerTile;

    public int perlinOffsetX;
    public int perlinOffsetZ;

    public ProceduralObjectPoolManager objectPool;

    public Dictionary<Vector2, VertexData> vertexDictionary;
    public List<SettlementData> settlements;

    public GameObject settlementPrefab;
    public List<TileComponentGroup> villageComponents;

    public float loadingPercentage = 0.0f;

    [HideInInspector]
    public bool completedTileGeneration = false;
    [HideInInspector]
    public bool completedGeneration = false;

    // Use this for initialization
    public void Initialize (int worldSeed, int worldSize)
    {
        seed = worldSeed;
        Random.InitState(seed);
        perlinOffsetX = Random.Range(0, 999999);
        perlinOffsetZ = Random.Range(0, 999999);

        //objectPool.InitializePool();
        //while (!objectPool.isReady) { }

        allTiles = new GameObject[worldSize, worldSize];
        vertexDictionary = new Dictionary<Vector2, VertexData>();
        settlements = new List<SettlementData>();

        //currentPlayerTile = homeTile.GetComponent<TileController>();
        //currentPlayerTile.ActivateTile();

        IEnumerator generateTiles = GenerateTiles(worldSize);
        StartCoroutine(generateTiles);

        Debug.Log("Building Settlements...");
        StartCoroutine("BuildSettlements");
        Debug.Log("Total Vertices: " + vertexDictionary.Values.Count);
	}

    /// <summary>
    /// Generate all of the tiles or "chunks" in the world
    /// </summary>
    /// <param name="worldSize"></param>
    /// <returns></returns>
    private IEnumerator GenerateTiles(int worldSize)
    {
        for (int z = 0; z < worldSize; z++)
        {
            for (int x = 0; x < worldSize; x++)
            {
                Vector3 tilePosition = new Vector3((x * tileSize), 0, (z * tileSize));
                allTiles[x, z] = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                allTiles[x, z].name = tilePrefab.name + "(" + x + "," + z + ")";
                allTiles[x, z].transform.SetParent(transform.GetChild(0));
                allTiles[x, z].GetComponent<TileController>().Initialize(new Vector2(x, z), worldSize, perlinOffsetX, perlinOffsetZ, null);
                loadingPercentage += (1f / (worldSize * worldSize)) * 50f;
                yield return null;
            }
        }
        completedTileGeneration = true;
    }

    /// <summary>
    /// Update tiles according to the new player tile (location).
    /// </summary>
    public void UpdateTiles(TileController newPlayerTile)
    {
        if (!completedTileGeneration)
            return;

        Debug.Log("Entered: " + newPlayerTile.gameObject.name);
        currentPlayerTile = newPlayerTile;

        for(int z = 0; z < allTiles.GetLength(1); z++)
        {
            for(int x = 0; x < allTiles.GetLength(0); x++)
            {
                if(x >= currentPlayerTile.tileIndex.x - 1 && x <= currentPlayerTile.tileIndex.x + 1 &&
                   z >= currentPlayerTile.tileIndex.y - 1 && z <= currentPlayerTile.tileIndex.y + 1)
                {
                    allTiles[x, z].SetActive(true);
                }
                else
                {
                    allTiles[x, z].SetActive(false);
                }
            }
        }
    }


    /// <summary>
    /// Add a new vertex to the dictionary of all world vertices
    /// </summary>
    public void AddToVertexDictionary(VertexData newVertex)
    {
        if (!vertexDictionary.ContainsKey(newVertex.tilePos2D))
        {
            vertexDictionary.Add(newVertex.tilePos2D, newVertex);
        }
    }

    /// <summary>
    /// SettlementData parameter is checked against existing settlements for adjacent vertices. <para />
    ///     -if match is found, add vertices to existing settlement. <para />
    ///     -else add new settlement to collection.
    /// </summary>
    public void AddSettlementVertexList(SettlementData newData)
    {
        bool contains = false;
        int index = 0;
        for(int i = 0; i < settlements.Count; i++)
        {
            if(SettlementData.CheckSettlementAdjacency(settlements[i], newData))
            {
                contains = true;
                index = i;
            }

            if (contains)
                break;
        }

        if(contains)
        {
            for(int i = 0; i < newData.vertices.Count; i++)
            {
                settlements[index].vertices.Add(newData.vertices[i]);
            }
            for (int i = 0; i < newData.edgeVerts.Count; i++)
            {
                settlements[index].vertices.Add(newData.edgeVerts[i]);
            }
            for(int i = 0; i < settlements[index].streetVerts.Count; i++)
            {
                settlements[index].vertices.Add(settlements[index].streetVerts[i]);
            }
            for(int i = 0; i < settlements[index].buildingVerts.Count; i++)
            {
                settlements[index].vertices.Add(settlements[index].buildingVerts[i]);
            }
            //settlements[index].edgeVerts.Clear();
            settlements[index].streetVerts.Clear();
            settlements[index].buildingVerts.Clear();

            settlements[index].Recalculate();
        }
        else
        {
            newData.Recalculate();
            settlements.Add(newData);
        }
    }

    /// <summary>
    /// Attempts to build settlements in calculated settlement areas.
    /// </summary>
    public IEnumerator BuildSettlements()
    {
        while (!completedTileGeneration)
            yield return null;

        // Set player initial spawn to center of a settlement
        GameManager.manager.playerSpawn.transform.position = settlements[3].center + Vector3.up*5;

        //                      village type       obj type      object
        GameObject[] walls =  villageComponents[0].subgroups[0].components.ToArray();
        GameObject building = villageComponents[0].subgroups[1].components[0];
        GameObject[] ground = villageComponents[0].subgroups[2].components.ToArray();

        for (int i = 0; i < settlements.Count; i++)
        {
            GameObject village = Instantiate(settlementGeneratorPrefab);
            village.name = "Village_" + i;

            village.GetComponent<SettlementManager>().SetData(settlements[i]);
            village.GetComponent<SettlementManager>().Initialize();

            loadingPercentage += (1f / settlements.Count) * 50f;
            yield return null;
        }

        completedGeneration = true;
    }

    /// <summary>
    /// Return VertexData object associated with Vector2 parameter. <para />
    ///     -Returns null if not contained
    /// </summary>
    public VertexData GetVertexDataByKey(Vector2 reqPosition)
    {
        if (vertexDictionary.ContainsKey(reqPosition))
            return vertexDictionary[reqPosition];
        else
            return null;
    }

    private void OnDrawGizmos()
    {
        if (currentPlayerTile != null)
            Gizmos.DrawWireCube(currentPlayerTile.transform.position, currentPlayerTile.gameObject.GetComponent<BoxCollider>().size);

        if (showAllSettlements)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < settlements.Count; i++)
            {
                Gizmos.DrawSphere(settlements[i].center + Vector3.up * 30, 15f);
                Handles.Label(settlements[i].center + Vector3.up * 100, i.ToString());
            }
        }
        if (showSettlement)
        {
            for (int i = (int)settlementIndexMin; i < settlementIndexMax; i++)
            {
                Gizmos.color = Color.white;
                for (int j = 0; j < settlements[i].vertices.Count; j++)
                {
                    Gizmos.DrawSphere(settlements[i].vertices[j].worldPosition, 0.5f);
                }
                Gizmos.color = Color.blue;
                for (int j = 0; j < settlements[i].edgeVerts.Count; j++)
                {
                    Gizmos.DrawSphere(settlements[i].edgeVerts[j].worldPosition, 0.5f);
                }
                Gizmos.color = Color.red;
                for (int j = 0; j < settlements[i].buildingVerts.Count; j++)
                {
                    Gizmos.DrawSphere(settlements[i].buildingVerts[j].worldPosition, 0.5f);
                }

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(settlements[i].center + Vector3.up, 1f);
                for (int j = 0; j < settlements[i].streetVerts.Count; j++)
                {
                    Gizmos.DrawSphere(settlements[i].streetVerts[j].worldPosition, 0.5f);
                }
            }
        }
    }
}

[System.Serializable]
public class ProceduralObjectPoolManager
{
    public Transform poolParent;
    public List<ProceduralObjectPool> pools;

    public bool isReady;

    public Dictionary<string, List<GameObject>> poolDictionary;
    public Dictionary<string, GameObject> poolPrefabs;

    public void InitializePool()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();
        poolPrefabs = new Dictionary<string, GameObject>();

        foreach(ProceduralObjectPool pool in pools)
        {
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = GameObject.Instantiate(pool.prefab);
                obj.name = pool.prefab.name;
                obj.transform.SetParent(poolParent);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
            poolPrefabs.Add(pool.tag, pool.prefab);
        }
        isReady = true;
    }

    public GameObject SpawnObject(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject objectToSpawn = null;
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("TileManager: Pool with tag: " + tag + " doesn't exist");
            return objectToSpawn;
        }

        List<GameObject> objectPool = poolDictionary[tag];

        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeSelf)
            {
                objectToSpawn = obj;
                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                return objectToSpawn;
            }
        }

        if (objectToSpawn == null)
        {
            objectToSpawn = GameObject.Instantiate(poolPrefabs[tag]);
            objectToSpawn.name = poolPrefabs[tag].name;
            objectPool.Add(objectToSpawn);
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            return objectToSpawn;
        }

        return objectToSpawn;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolParent);
    }
}

[System.Serializable]
public class ProceduralObjectPool
{
    public enum ProceduralType
    {
        None = 0,
        Tile,
        Island,
        Trees,
        NUM_TYPES
    }
    public ProceduralType objectType = ProceduralType.None;

    public string tag;
    public GameObject prefab;
    public int size;

    public ProceduralObjectPool(string newTag, GameObject newPrefab, ProceduralType newType, int newSize)
    {
        tag = newTag;
        prefab = newPrefab;
        objectType = newType;
        size = newSize;
    }
}

/// <summary>
/// holds vertex data for spawnable vertices.   <para />
///     -tilePosition  (Vector3)                <para />
///     -worldPosition (Vector3)                <para />
///     -tilePos2D  (Vector2)                   <para />
///     -adjacentWorldVertices (Dictionary{VertexEdge, Vector2}) <para />
///     -isOccupied (bool)                      <para />
///     -component (GameObject)                 
/// </summary>
[System.Serializable]
public class VertexData
{
    public Vector3 tilePosition;
    public Vector3 worldPosition;
    public Vector2 tilePos2D;
    public Dictionary<VertexEdge, Vector3> adjacentWorldVertices;
    public bool isOccupied;
    public GameObject component;

    public bool isEdge;

    public enum VertexEdge
    {
        DEFAULT,
        NORTH,
        SOUTH,
        EAST,
        WEST,
        NORTHEAST,
        NORTHWEST,
        SOUTHEAST,
        SOUTHWEST
    };

    public VertexEdge edgeFace;

    public VertexData(Vector3 tilePos, Vector3 worldPos)
    {
        tilePosition = tilePos;
        worldPosition = worldPos;
        tilePos2D = new Vector2(worldPos.x, worldPos.z);
        adjacentWorldVertices = new Dictionary<VertexEdge, Vector3>();
        component = null;
        isEdge = false;
        isOccupied = false;
        edgeFace = VertexEdge.DEFAULT;
    }

    public VertexData(Vector3 tilePos, Vector3 worldPos, GameObject obj, bool edge, bool occupied, Dictionary<VertexEdge, Vector3> newAdjacentVertices)
    {
        tilePosition = tilePos;
        worldPosition = worldPos;
        tilePos2D = new Vector2(worldPos.x, worldPos.z);
        adjacentWorldVertices = newAdjacentVertices;
        component = obj;
        isEdge = edge;
        isOccupied = occupied;
        edgeFace = VertexEdge.DEFAULT;
    }

    public void SetPositions(Vector3 tilePos, Vector3 worldPos)
    {
        tilePosition = tilePos;
        worldPosition = worldPos;
        tilePos2D = new Vector2(worldPos.x, worldPos.z);
    }

    public bool IsAdjacent(Vector3 otherWorldPos)
    {
        if (adjacentWorldVertices[VertexEdge.NORTHWEST].Equals(otherWorldPos))
            return true;
        else if (adjacentWorldVertices[VertexEdge.NORTH].Equals(otherWorldPos))
            return true;
        else if (adjacentWorldVertices[VertexEdge.NORTHEAST].Equals(otherWorldPos))
            return true;
        else if (adjacentWorldVertices[VertexEdge.WEST].Equals(otherWorldPos))
            return true;
        else if (adjacentWorldVertices[VertexEdge.EAST].Equals(otherWorldPos))
            return true;
        else if (adjacentWorldVertices[VertexEdge.SOUTHWEST].Equals(otherWorldPos))
            return true;
        else if (adjacentWorldVertices[VertexEdge.SOUTH].Equals(otherWorldPos))
            return true;
        else if (adjacentWorldVertices[VertexEdge.SOUTHEAST].Equals(otherWorldPos))
            return true;
        else
            return false;
    }
}
