using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    [HideInInspector]
    public TileManager tileManager;

    public bool showGizmos = false;

    public Vector2 tilePosition;
    public Vector2 tileIndex;
    public List<GameObject> islands;

    [Header("Generation Data")]
    [ShowOnly] public float worldSize = 0;
    public int xSize = 50;
    public int zSize = 50;
    public int xOffset = 0;
    public int zOffset = 0;
    public int quadSize = 10;
    public int islandCount = 1;
    public Vector3 centerLandmass;
    public TileMeshValues meshValues;

    [Header("Object Prefabs")]
    public GameObject basePrefab;
    public GameObject islandPrefab;
    public List<TileComponentGroup> tileComponents;
    [HideInInspector] public GameObject treeGeneratorPrefab;
    [HideInInspector] public GameObject rockGeneratorPrefab;

    [Space(15)]
    public Transform baseParent;
    public Transform treeParent;
    public Transform rockParent;
    public Transform propParent;
    public Transform fenceParent;
    public Transform structureParent;
    public Transform grassParent;

    [HideInInspector]
    public Transform islandParent;
    public GameObject baseGameObject;
    public TileData.TileType tileType;
    public TileData.TileBiome tileBiome;
    public MeshData baseMeshData;
    public Dictionary<Vector3, GameObject> spawnableVertices;
    public Dictionary<Vector3, GameObject> edgeVertices;
    //public Dictionary<Vector3, VertexData> settlementVertices;
    public List<VertexData> settlementVertices;
    public List<SettlementData> settlementData;
    private int settlementIndex = 0;

    public TileSaveData saveData;
    public List<ObjectSaveData> tileObjects;
    public bool isReady;

	// Use this for initialization
	public void Initialize(Vector2 newIndex, int worldSize, int newOffsetX, int newOffsetZ, TileSaveData newSaveData)
    {
        isReady = false;
        if (newSaveData == null)
            saveData = new TileSaveData();
        else
            saveData = newSaveData;

        xOffset = newOffsetX;
        zOffset = newOffsetZ;
        tilePosition = new Vector2(transform.position.x, transform.position.z);
        tileIndex = newIndex;
        tileBiome = TileData.TileBiome.Temperate;

        spawnableVertices = new Dictionary<Vector3, GameObject>();
        edgeVertices = new Dictionary<Vector3, GameObject>();
        settlementVertices = new List<VertexData>();
        //settlementVertices = new Dictionary<Vector3, VertexData>();
        settlementData = new List<SettlementData>();

        tileManager = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
        if (tileManager == null)
            Debug.LogError(gameObject.name + ": Failed to get TileManager.");

        islandParent = transform.GetChild(0);

        CheckBounds(worldSize);
        SpawnTileComponents();
        isReady = true;
	}

    /*
     * Spawn Components to tile such as trees and rocks
     */
    public void SpawnTileComponents()
    {
        TileComponentGroup componentGroup = null;
        if (tileBiome == TileData.TileBiome.Tropical)
            componentGroup = tileComponents[0];
        else if (tileBiome == TileData.TileBiome.Pine)
            componentGroup = tileComponents[1];

        // Spawn trees
        int treeCount = (int)Random.Range(meshValues.treeSpawns.min, meshValues.treeSpawns.max);
        SpawnObjectByGenerator(treeGeneratorPrefab, treeCount, treeParent, spawnableVertices, "Tree");

        // Spawn rocks
        int rockCount = (int)Random.Range(meshValues.rockSpawns.min, meshValues.rockSpawns.max);
        SpawnObjectByGenerator(rockGeneratorPrefab, rockCount, rockParent, spawnableVertices, "Rock");
    }

    public void SpawnObjectByGenerator(GameObject generator, int numberToSpawn, Transform objParent, Dictionary<Vector3, GameObject> spawnDict , string id)
    {
        Vector3[] vertexList = new Vector3[spawnDict.Keys.Count];
        spawnDict.Keys.CopyTo(vertexList, 0);

        if (vertexList.Length < 1)
            return;
        else if (vertexList.Length < numberToSpawn)
            numberToSpawn = vertexList.Length;

        for (int i = 0; i < numberToSpawn; i++)
        {
            int tryCount = 0;
            bool spawnedObj = false;
            while (tryCount <= 10 && !spawnedObj)
            {
                int prefabIndex = Random.Range(0, vertexList.Length);
                Vector2 worldPos2D = new Vector2(transform.position.x + baseGameObject.transform.localPosition.x + vertexList[prefabIndex].x,
                                                             transform.position.z + baseGameObject.transform.localPosition.z + vertexList[prefabIndex].z);

                if (tileManager.vertexDictionary.ContainsKey(worldPos2D) && !tileManager.vertexDictionary[worldPos2D].isOccupied)
                {
                    GameObject newObj = null;
                    Vector3 spawnPoint = vertexList[prefabIndex] + new Vector3(-(TileData.TileSize / 2), 0, -(TileData.TileSize / 2));

                    newObj = Instantiate(generator);
                    if (newObj != null)
                    {
                        newObj.name = generator.name;
                        newObj.transform.SetParent(objParent);

                        newObj.transform.localPosition = spawnPoint;
                        spawnDict[vertexList[prefabIndex]] = newObj;

                        tileManager.vertexDictionary[worldPos2D].component = newObj;
                        tileManager.vertexDictionary[worldPos2D].isOccupied = true;

                        //CalculateObjectRotation(tileManager.vertexDictionary[worldPos2D]);

                        if (id.Equals("Tree"))
                            newObj.GetComponent<TreeGenerator>().SpawnTree(tileBiome);
                        else if (id.Equals("Rock"))
                            newObj.GetComponent<RockGenerator>().SpawnRock(tileBiome);
                        spawnedObj = true;
                    }
                }
                tryCount++;
            }
        }
    }

    public void AttemptObjectSpawn(GameObject[] prefabCollection, int numberToSpawn, Transform objParent, Dictionary<Vector3, GameObject> spawnDict)
    {
        Vector3[] vertexList = new Vector3[spawnDict.Keys.Count];
        spawnDict.Keys.CopyTo(vertexList, 0);

        if (vertexList.Length < 1)
            return;
        else if (vertexList.Length < numberToSpawn)
            numberToSpawn = vertexList.Length;

        for (int i = 0; i < numberToSpawn; i++)
        {
            int tryCount = 0;
            bool spawnedObj = false;
            while (tryCount <= 10 && !spawnedObj)
            {
                int prefabIndex = Random.Range(0, vertexList.Length);
                Vector2 worldPos2D = new Vector2(transform.position.x + baseGameObject.transform.localPosition.x + vertexList[prefabIndex].x,
                                                             transform.position.z + baseGameObject.transform.localPosition.z + vertexList[prefabIndex].z);

                if (tileManager.vertexDictionary.ContainsKey(worldPos2D) && !tileManager.vertexDictionary[worldPos2D].isOccupied)
                {
                    GameObject newObj = null;
                    Vector3 spawnPoint = vertexList[prefabIndex] + new Vector3(-(TileData.TileSize / 2), 0, -(TileData.TileSize / 2));
                    int spawnPrefabIndex = 0;

                    // get random prefab from collection
                    if (prefabCollection.Length > 0)
                        spawnPrefabIndex = Random.Range(0, prefabCollection.Length);

                    // spawn object if the prefab is valid
                    if (prefabCollection[spawnPrefabIndex] != null)
                        newObj = Instantiate(prefabCollection[spawnPrefabIndex]);

                    if (newObj != null)
                    {
                        newObj.name = prefabCollection[0].name;
                        newObj.transform.SetParent(objParent);

                        Vector3 offset = spawnPoint;
                        if (newObj.GetComponent<ProceduralComponent>() != null)
                            offset += newObj.GetComponent<ProceduralComponent>().groundOffset;

                        newObj.transform.localPosition = offset;
                        //newObj.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                        spawnDict[vertexList[prefabIndex]] = newObj;

                        tileManager.vertexDictionary[worldPos2D].component = newObj;
                        tileManager.vertexDictionary[worldPos2D].isOccupied = true;

                        CalculateObjectRotation(tileManager.vertexDictionary[worldPos2D]);

                        spawnedObj = true;
                    }
                }
                tryCount++;
            }
        }
    }

    private void CalculateObjectRotation(VertexData dataV)
    {
        float childWidth = 0;
        if (dataV.component.GetComponent<ProceduralComponent>() != null)
            childWidth = dataV.component.GetComponent<ProceduralComponent>().childWidth;

        if (dataV.edgeFace == VertexData.VertexEdge.EAST)
            dataV.component.transform.localEulerAngles = new Vector3(0, 90, 0);
        else if (dataV.edgeFace == VertexData.VertexEdge.WEST)
            dataV.component.transform.localEulerAngles = new Vector3(0, -90, 0);
        else if (dataV.edgeFace == VertexData.VertexEdge.NORTH)
            dataV.component.transform.localEulerAngles = new Vector3(0, 180, 0);
        else if (dataV.edgeFace == VertexData.VertexEdge.SOUTH)
            dataV.component.transform.localEulerAngles = new Vector3(0, 0, 0);
        else if(dataV.edgeFace == VertexData.VertexEdge.NORTHEAST)
        {
            if (dataV.component.transform.childCount >= 2)
            {
                dataV.component.transform.GetChild(0).localEulerAngles = new Vector3(0, 90, 0);
                dataV.component.transform.GetChild(0).localPosition = new Vector3(0, dataV.component.transform.GetChild(0).localPosition.y, -childWidth);
                dataV.component.transform.GetChild(1).localEulerAngles = new Vector3(0, 180, 0);
            }
        }
        else if (dataV.edgeFace == VertexData.VertexEdge.NORTHWEST)
        {
            if (dataV.component.transform.childCount >= 2)
            {
                dataV.component.transform.GetChild(1).localEulerAngles = new Vector3(0, 90, 0);
                dataV.component.transform.GetChild(1).localPosition = new Vector3(0, dataV.component.transform.GetChild(1).localPosition.y, -childWidth);
            }
        }
        else if (dataV.edgeFace == VertexData.VertexEdge.SOUTHWEST)
        {
            if (dataV.component.transform.childCount >= 2)
            {
                dataV.component.transform.GetChild(1).localEulerAngles = new Vector3(0, 90, 0);
                dataV.component.transform.GetChild(1).localPosition = new Vector3(0, dataV.component.transform.GetChild(1).localPosition.y, childWidth);
            }
        }
        else if (dataV.edgeFace == VertexData.VertexEdge.SOUTHEAST)
        {
            if (dataV.component.transform.childCount >= 2)
            {
                dataV.component.transform.GetChild(0).localEulerAngles = new Vector3(0, 90, 0);
                dataV.component.transform.GetChild(0).localPosition = new Vector3(0, dataV.component.transform.GetChild(0).localPosition.y, childWidth);
            }
        }
        else
            dataV.component.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
    }

    public void SpawnIslands(TileManager newManager)
    {
        tileManager = newManager;
        islandParent = transform.GetChild(0);
        tilePosition = new Vector2(transform.position.x, transform.position.z);

        int chanceToSpawn = Random.Range(0, 100);
        if (chanceToSpawn < 50)
            return;

        int xMin = (int)(tilePosition.x - GameData.TileSize * 0.4f);
        int xMax = (int)(tilePosition.x + GameData.TileSize * 0.4f);

        int zMin = (int)(tilePosition.y - GameData.TileSize * 0.4f);
        int zMax = (int)(tilePosition.y + GameData.TileSize * 0.4f);

        for (int i = 0; i < islandCount; i++)
        {
            //Vector3 islandPosition = new Vector3(Random.Range(xMin, xMax), 0, Random.Range(zMin, zMax));
            Vector3 islandPosition = new Vector3(transform.position.x, 0, transform.position.z);
            //Vector3 islandRotation = new Vector3(0, Random.Range(0, 359), 0);
            Quaternion islandRotation = Quaternion.identity;
            GameObject newIsland = Instantiate(islandPrefab, islandPosition, islandRotation);
            newIsland.transform.SetParent(islandParent);
            newIsland.GetComponent<IslandController>().Initialize();
            islands.Add(newIsland);
        }
    }

    public void SpawnEmptyIsland()
    {
        Vector3 islandPosition = new Vector3(transform.position.x, 0, transform.position.z);
        //Vector3 islandRotation = new Vector3(0, Random.Range(0, 359), 0);
        Quaternion islandRotation = Quaternion.identity;
        GameObject newIsland = Instantiate(islandPrefab, islandPosition, islandRotation);
        newIsland.transform.SetParent(islandParent);
        newIsland.GetComponent<IslandController>().GetParents();
        islands.Add(newIsland);
    }

    public void CheckBounds(int newWorldSize)
    {
        worldSize = newWorldSize * TileData.TileSize;
        baseGameObject = Instantiate(basePrefab);
        baseGameObject.transform.SetParent(transform);
        baseGameObject.transform.localPosition = new Vector3(-(TileData.TileSize / 2), 0, -(TileData.TileSize / 2));

        // bottom bound
        if(tileIndex.y == 0)
        {
            if(tileIndex.x == 0)    // bottom left corner
            {
                tileType = TileData.TileType.BottomLeft;
            }
            else if(tileIndex.x == (newWorldSize - 1))   // bottom right corner
            {
                tileType = TileData.TileType.BottomRight;
            }
            else                    // bottom edge
            {
                tileType = TileData.TileType.BottomEdge;
            }
        }

        // top bound
        else if(tileIndex.y == (newWorldSize - 1))
        {
            if(tileIndex.x == 0)    // top left corner
            {
                tileType = TileData.TileType.TopLeft;
            }
            else if(tileIndex.x == (newWorldSize - 1))   // top right corner
            {
                tileType = TileData.TileType.TopRight;
            }
            else                    // top edge
            {
                tileType = TileData.TileType.TopEdge;
            }
        }

        // left bound
        else if(tileIndex.x == 0)
        {
            tileType = TileData.TileType.LeftEdge;
        }

        // right bound
        else if (tileIndex.x == (newWorldSize - 1))
        {
            tileType = TileData.TileType.RightEdge;
        }
        
        GenerateBaseMesh();
    }

    public void ActivateTile()
    {

    }

    public void DeactivateTile()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            tileManager.UpdateTiles(this);
        }
    }

    /*
     * Generate the base mesh for the tile
     *      - Sends found settlement data to tilemanager
     */
    public void GenerateBaseMesh()
    {
        baseMeshData.mesh = new Mesh();
        baseGameObject.GetComponent<MeshFilter>().mesh = baseMeshData.mesh;

        CreateShape();
        //CreateShapeHardEdges();
        UpdateMesh(baseGameObject, baseMeshData);
        
        List<SettlementData> tempSettlements = new List<SettlementData>();
        tempSettlements.Add(new SettlementData());
        int tempIndex = 0;

        if (settlementVertices.Count > 0)
            tempSettlements[tempIndex].AddVertex(settlementVertices[0]);

        for (int i = 1; i < settlementVertices.Count; i++)
        {
            if (tempSettlements[tempIndex].CheckVertexAdjacency(settlementVertices[i]))
                tempSettlements[tempIndex].AddVertex(settlementVertices[i]);
            else
            {
                tempSettlements.Add(new SettlementData());
                tempIndex++;

                tempSettlements[tempIndex].AddVertex(settlementVertices[i]);
            }
        }

        List<int> indices = new List<int>();
        if (tempSettlements.Count > 0)
            indices.Add(0);
        for(int i = 0; i < tempSettlements.Count-1; i++)
        {
            for(int j = i+1; j < tempSettlements.Count; j++)
            {
                if(SettlementData.CheckSettlementAdjacency(tempSettlements[i], tempSettlements[j]))
                {
                    tempSettlements[i].MergeSettlements(tempSettlements[j]);
                    if (!indices.Contains(i))
                        indices.Add(i);
                }
            }
        }

        for (int i = 0; i < indices.Count; i++)
        {
            if (tempSettlements[indices[i]].vertices.Count > 0)
                tileManager.AddSettlementVertexList(tempSettlements[indices[i]]);
        }
        //Debug.Log("Vertex Count: " + baseMeshData.mesh.vertexCount);
    }

    /// <summary>
    /// Generate the terrain for the tile, calculating height using perlin noise function.
    /// </summary>
    private void CreateShape()
    {
        baseMeshData.vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        baseMeshData.colors = new Color[baseMeshData.vertices.Length];

        Vector2 center = new Vector2(xSize / 2, zSize / 2);
        float maxDistance = Vector2.Distance(center, new Vector2(0, 0));
        float currentHeight = TileData.MinHeight;

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                SetVertex(i, x, z);
                i++;
            }
        }

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

    private void CreateShapeHardEdges()
    {
        baseMeshData.vertices = new Vector3[xSize * zSize * 6];
        baseMeshData.colors = new Color[baseMeshData.vertices.Length];

        List<Vector3> tempVerts = new List<Vector3>();

        Vector2 center = new Vector2(xSize / 2, zSize / 2);
        float maxDistance = Vector2.Distance(center, new Vector2(0, 0));
        float currentHeight = TileData.MinHeight;

        int vertexCount = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float noiseHeight = GetVertexByCoordinate(x, z);
                /*
                // bottom left
                SetVertex(vertexCount, x, z);
                vertexCount++;
                // top left
                SetVertex(vertexCount, x, z + 1);
                vertexCount++;
                // bottom right
                SetVertex(vertexCount, x + 1, z);
                vertexCount++;

                // bottom right
                SetVertex(vertexCount, x + 1, z);
                vertexCount++;
                // top left
                SetVertex(vertexCount, x, z + 1);
                vertexCount++;
                // top right
                SetVertex(vertexCount, x + 1, z + 1);
                vertexCount++;*/
            }
        }
        centerLandmass = (centerLandmass / spawnableVertices.Values.Count);
        centerLandmass.x -= centerLandmass.x % quadSize;
        centerLandmass.y -= centerLandmass.y % quadSize;
        centerLandmass.z -= centerLandmass.z % quadSize;
        //baseMeshData.spawnVertices = new Vector3[tempVerts.Count];
        //baseMeshData.spawnVertices = tempVerts.ToArray();

        baseMeshData.triangles = new int[vertexCount];
        for(int i = 0; i < vertexCount; i++)
        {
            baseMeshData.triangles[i] = i;
        }


    }

    /// <summary>
    /// Calculate vertex data given the world X coordinate and Z coordinate.
    /// </summary>
    private void SetVertex(int vertexCount, int xCoord, int zCoord)
    {
        Vector2 worldPosition = new Vector2(tilePosition.x - (TileData.TileSize / 2) + xCoord * quadSize, tilePosition.y - (TileData.TileSize / 2) + zCoord * quadSize);
        float lerp = TileData.GetPerlinValue(worldPosition, worldSize, meshValues.perlinScale, xOffset, zOffset);
        lerp -= meshValues.perlinReduction;
        if (lerp < 0.0f)
            lerp = 0.0f;
        float perlinHeight = GetPerlinHeightByVertex(xCoord, zCoord, lerp);

        // [EAST, WEST, SOUTH, NORTH, NORTHEAST, NORTHWEST, SOUTHEAST, SOUTHWEST]
        // [LEFT, RIGHT, DOWN, UP, UP-LEFT, UP-RIGHT, DOWN-LEFT, DOWN-RIGHT]
        VertexData newData = new VertexData(new Vector3(tilePosition.x, perlinHeight, tilePosition.y), new Vector3(worldPosition.x, perlinHeight, worldPosition.y));
        //float[] adjacentHeights = new float[8];

        // EAST
        worldPosition = new Vector2(tilePosition.x - (TileData.TileSize / 2) + (xCoord + 1) * quadSize,
                                    tilePosition.y - (TileData.TileSize / 2) + (zCoord) * quadSize);
        lerp = TileData.GetPerlinValue(worldPosition, worldSize, meshValues.perlinScale, xOffset, zOffset);
        lerp -= meshValues.perlinReduction;
        if (lerp < 0.0f)
            lerp = 0.0f;
        newData.adjacentWorldVertices.Add(VertexData.VertexEdge.EAST, new Vector3(worldPosition.x, GetPerlinHeightByVertex(xCoord, zCoord, lerp), worldPosition.y));

        // WEST
        worldPosition = new Vector2(tilePosition.x - (TileData.TileSize / 2) + (xCoord - 1) * quadSize,
                                    tilePosition.y - (TileData.TileSize / 2) + (zCoord) * quadSize);
        lerp = TileData.GetPerlinValue(worldPosition, worldSize, meshValues.perlinScale, xOffset, zOffset);
        lerp -= meshValues.perlinReduction;
        if (lerp < 0.0f)
            lerp = 0.0f;
        newData.adjacentWorldVertices.Add(VertexData.VertexEdge.WEST, new Vector3(worldPosition.x, GetPerlinHeightByVertex(xCoord, zCoord, lerp), worldPosition.y));

        // SOUTH
        worldPosition = new Vector2(tilePosition.x - (TileData.TileSize / 2) + (xCoord) * quadSize,
                                    tilePosition.y - (TileData.TileSize / 2) + (zCoord - 1) * quadSize);
        lerp = TileData.GetPerlinValue(worldPosition, worldSize, meshValues.perlinScale, xOffset, zOffset);
        lerp -= meshValues.perlinReduction;
        if (lerp < 0.0f)
            lerp = 0.0f;
        newData.adjacentWorldVertices.Add(VertexData.VertexEdge.SOUTH, new Vector3(worldPosition.x, GetPerlinHeightByVertex(xCoord, zCoord, lerp), worldPosition.y));

        // NORTH
        worldPosition = new Vector2(tilePosition.x - (TileData.TileSize / 2) + (xCoord) * quadSize,
                                    tilePosition.y - (TileData.TileSize / 2) + (zCoord + 1) * quadSize);
        lerp = TileData.GetPerlinValue(worldPosition, worldSize, meshValues.perlinScale, xOffset, zOffset);
        lerp -= meshValues.perlinReduction;
        if (lerp < 0.0f)
            lerp = 0.0f;
        newData.adjacentWorldVertices.Add(VertexData.VertexEdge.NORTH, new Vector3(worldPosition.x, GetPerlinHeightByVertex(xCoord, zCoord, lerp), worldPosition.y));

        // NORTHEAST
        worldPosition = new Vector2(tilePosition.x - (TileData.TileSize / 2) + (xCoord + 1) * quadSize,
                                    tilePosition.y - (TileData.TileSize / 2) + (zCoord + 1) * quadSize);
        lerp = TileData.GetPerlinValue(worldPosition, worldSize, meshValues.perlinScale, xOffset, zOffset);
        lerp -= meshValues.perlinReduction;
        if (lerp < 0.0f)
            lerp = 0.0f;
        newData.adjacentWorldVertices.Add(VertexData.VertexEdge.NORTHEAST, new Vector3(worldPosition.x, GetPerlinHeightByVertex(xCoord, zCoord, lerp), worldPosition.y));

        // NORTHWEST
        worldPosition = new Vector2(tilePosition.x - (TileData.TileSize / 2) + (xCoord - 1) * quadSize,
                                    tilePosition.y - (TileData.TileSize / 2) + (zCoord + 1) * quadSize);
        lerp = TileData.GetPerlinValue(worldPosition, worldSize, meshValues.perlinScale, xOffset, zOffset);
        lerp -= meshValues.perlinReduction;
        if (lerp < 0.0f)
            lerp = 0.0f;
        newData.adjacentWorldVertices.Add(VertexData.VertexEdge.NORTHWEST, new Vector3(worldPosition.x, GetPerlinHeightByVertex(xCoord, zCoord, lerp), worldPosition.y));

        // SOUTHEAST
        worldPosition = new Vector2(tilePosition.x - (TileData.TileSize / 2) + (xCoord + 1) * quadSize,
                                    tilePosition.y - (TileData.TileSize / 2) + (zCoord - 1) * quadSize);
        lerp = TileData.GetPerlinValue(worldPosition, worldSize, meshValues.perlinScale, xOffset, zOffset);
        lerp -= meshValues.perlinReduction;
        if (lerp < 0.0f)
            lerp = 0.0f;
        newData.adjacentWorldVertices.Add(VertexData.VertexEdge.SOUTHEAST, new Vector3(worldPosition.x, GetPerlinHeightByVertex(xCoord, zCoord, lerp), worldPosition.y));

        // SOUTHWEST
        worldPosition = new Vector2(tilePosition.x - (TileData.TileSize / 2) + (xCoord - 1) * quadSize,
                                    tilePosition.y - (TileData.TileSize / 2) + (zCoord - 1) * quadSize);
        lerp = TileData.GetPerlinValue(worldPosition, worldSize, meshValues.perlinScale, xOffset, zOffset);
        lerp -= meshValues.perlinReduction;
        if (lerp < 0.0f)
            lerp = 0.0f;
        newData.adjacentWorldVertices.Add(VertexData.VertexEdge.SOUTHWEST, new Vector3(worldPosition.x, GetPerlinHeightByVertex(xCoord, zCoord, lerp), worldPosition.y));

        baseMeshData.vertices[vertexCount] = new Vector3((xCoord * quadSize), perlinHeight, (zCoord * quadSize));
        baseMeshData.colors[vertexCount] = CalculateColor(baseMeshData.vertices[vertexCount], lerp);

        if (baseMeshData.vertices[vertexCount].y >= TileData.normalHeight && baseMeshData.vertices[vertexCount].y <= TileData.elevatedHeight)
        {
            bool isEdge = true;
            if (newData.adjacentWorldVertices[VertexData.VertexEdge.EAST].y < perlinHeight)
            {
                if (newData.adjacentWorldVertices[VertexData.VertexEdge.NORTH].y < perlinHeight)
                    newData.edgeFace = VertexData.VertexEdge.NORTHEAST;
                else if (newData.adjacentWorldVertices[VertexData.VertexEdge.SOUTH].y < perlinHeight)
                    newData.edgeFace = VertexData.VertexEdge.SOUTHEAST;
                else
                    newData.edgeFace = VertexData.VertexEdge.EAST;
            }
            else if (newData.adjacentWorldVertices[VertexData.VertexEdge.WEST].y < perlinHeight)
            {
                if (newData.adjacentWorldVertices[VertexData.VertexEdge.NORTH].y < perlinHeight)
                    newData.edgeFace = VertexData.VertexEdge.NORTHWEST;
                else if (newData.adjacentWorldVertices[VertexData.VertexEdge.SOUTH].y < perlinHeight)
                    newData.edgeFace = VertexData.VertexEdge.SOUTHWEST;
                else
                    newData.edgeFace = VertexData.VertexEdge.WEST;
            }
            else if (newData.adjacentWorldVertices[VertexData.VertexEdge.NORTH].y < perlinHeight)
                newData.edgeFace = VertexData.VertexEdge.NORTH;
            else if (newData.adjacentWorldVertices[VertexData.VertexEdge.NORTHEAST].y < perlinHeight)
                newData.edgeFace = VertexData.VertexEdge.SOUTHWEST;
            else if (newData.adjacentWorldVertices[VertexData.VertexEdge.NORTHWEST].y < perlinHeight)
                newData.edgeFace = VertexData.VertexEdge.SOUTHEAST;

            else if (newData.adjacentWorldVertices[VertexData.VertexEdge.SOUTH].y < perlinHeight)
                newData.edgeFace = VertexData.VertexEdge.SOUTH;
            else if (newData.adjacentWorldVertices[VertexData.VertexEdge.SOUTHEAST].y < perlinHeight)
                newData.edgeFace = VertexData.VertexEdge.NORTHWEST;
            else if (newData.adjacentWorldVertices[VertexData.VertexEdge.SOUTHWEST].y < perlinHeight)
                newData.edgeFace = VertexData.VertexEdge.NORTHEAST;

            else
            {
                newData.edgeFace = VertexData.VertexEdge.DEFAULT;
                isEdge = false;
            }

            //newData.tilePosition = baseMeshData.vertices[vertexCount];
            //newData.worldPosition = transform.position + baseGameObject.transform.localPosition + baseMeshData.vertices[vertexCount];
            newData.SetPositions(baseMeshData.vertices[vertexCount], transform.position + baseGameObject.transform.localPosition + baseMeshData.vertices[vertexCount]);
            newData.isOccupied = false;
            newData.isEdge = isEdge;

            // if settlement region
            if (perlinHeight == TileData.elevatedHeight)
            {
                settlementVertices.Add(newData);
            }

            // otherwise normal terrain
            else
            {
                if(isEdge)
                {
                    if (!edgeVertices.ContainsKey(baseMeshData.vertices[vertexCount]))
                        edgeVertices.Add(baseMeshData.vertices[vertexCount], null);
                }
                else
                {
                    spawnableVertices.Add(baseMeshData.vertices[vertexCount], null);
                    centerLandmass += baseMeshData.vertices[vertexCount];
                }
            }
            tileManager.AddToVertexDictionary(newData);
        }
    }

    /*
     * Return value of the vertex at a given coordinate
     */
    public float GetVertexByCoordinate(int xCoord, int zCoord)
    {
        Vector2 worldPosition = new Vector2(tilePosition.x - (TileData.TileSize / 2) + xCoord * quadSize, tilePosition.y - (TileData.TileSize / 2) + zCoord * quadSize);
        float lerp = TileData.GetPerlinValue(worldPosition, worldSize, meshValues.perlinScale, xOffset, zOffset);
        lerp -= meshValues.perlinReduction;
        if (lerp < 0.0f)
            lerp = 0.0f;
        return GetPerlinHeightByVertex(xCoord, zCoord, lerp);
    }

    /*
     * Get vertex perlin height given: 
     *          -world x coordinate
     *          -world z coordinate
     *          -lerp value
     */
    private float GetPerlinHeightByVertex(int x, int z, float lerp)
    {
        float edgeOuterScale = 5.0f;
        float edgeWallScale = 10.0f;

        float currentHeight = 0.0f;
        if (tileType == TileData.TileType.Normal)
        {
            if (lerp > meshValues.perlinLandHigherThreshold)
                currentHeight = TileData.elevatedHeight;
            else if (lerp > meshValues.perlinLandThreshold)
                currentHeight = TileData.normalHeight;
            else if (lerp > meshValues.perlinBeachThreshold)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.4f);
            else if (lerp > meshValues.perlinBeachDeeper)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.3f);
            else if (lerp > meshValues.perlinBeachDeepest)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.15f);
            else
                currentHeight = TileData.MinHeight;
        }
        // LEFT EDGE
        else if (tileType == TileData.TileType.LeftEdge)
        {
            if (x < xSize / edgeWallScale)
                currentHeight = TileData.MaxHeight;
            else if (lerp > meshValues.perlinLandHigherThreshold)
                currentHeight = TileData.elevatedHeight;
            else if (lerp > meshValues.perlinLandThreshold || x <= xSize / edgeOuterScale)
                currentHeight = TileData.normalHeight;
            else if (lerp > meshValues.perlinBeachThreshold)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.4f);
            else if (lerp > meshValues.perlinBeachDeeper)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.3f);
            else if (lerp > meshValues.perlinBeachDeepest)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.15f);
            else
                currentHeight = TileData.MinHeight;
        }
        // RIGHT EDGE
        else if (tileType == TileData.TileType.RightEdge)
        {
            if (x > xSize - xSize / edgeWallScale)
                currentHeight = TileData.MaxHeight;
            else if (lerp > meshValues.perlinLandHigherThreshold)
                currentHeight = TileData.elevatedHeight;
            else if (lerp > meshValues.perlinLandThreshold || x >= xSize - xSize / edgeOuterScale)
                currentHeight = TileData.normalHeight;
            else if (lerp > meshValues.perlinBeachThreshold)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.4f);
            else if (lerp > meshValues.perlinBeachDeeper)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.3f);
            else if (lerp > meshValues.perlinBeachDeepest)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.15f);
            else
                currentHeight = TileData.MinHeight;
        }
        // BOTTOM EDGE
        else if (tileType == TileData.TileType.BottomEdge)
        {
            if (z < zSize / edgeWallScale)
                currentHeight = TileData.MaxHeight;
            else if (lerp > meshValues.perlinLandHigherThreshold)
                currentHeight = TileData.elevatedHeight;
            else if (lerp > meshValues.perlinLandThreshold || z <= zSize / edgeOuterScale)
                currentHeight = TileData.normalHeight;
            else if (lerp > meshValues.perlinBeachThreshold)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.4f);
            else if (lerp > meshValues.perlinBeachDeeper)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.3f);
            else if (lerp > meshValues.perlinBeachDeepest)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.15f);
            else
                currentHeight = TileData.MinHeight;
        }
        // TOP EDGE
        else if (tileType == TileData.TileType.TopEdge)
        {
            if (z > zSize - zSize / edgeWallScale)
                currentHeight = TileData.MaxHeight;
            else if (lerp > meshValues.perlinLandHigherThreshold)
                currentHeight = TileData.elevatedHeight;
            else if (lerp > meshValues.perlinLandThreshold || z >= zSize - zSize / edgeOuterScale)
                currentHeight = TileData.normalHeight;
            else if (lerp > meshValues.perlinBeachThreshold)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.4f);
            else if (lerp > meshValues.perlinBeachDeeper)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.3f);
            else if (lerp > meshValues.perlinBeachDeepest)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.15f);
            else
                currentHeight = TileData.MinHeight;
        }
        // BOTTOM LEFT CORNER
        else if (tileType == TileData.TileType.BottomLeft)
        {
            if (x < xSize / edgeWallScale || z < zSize / edgeWallScale)
                currentHeight = TileData.MaxHeight;
            else if (lerp > meshValues.perlinLandHigherThreshold)
                currentHeight = TileData.elevatedHeight;
            else if (lerp > meshValues.perlinLandThreshold || x <= xSize / edgeOuterScale || z <= zSize / edgeOuterScale)
                currentHeight = TileData.normalHeight;
            else if (lerp > meshValues.perlinBeachThreshold)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.4f);
            else if (lerp > meshValues.perlinBeachDeeper)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.3f);
            else if (lerp > meshValues.perlinBeachDeepest)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.15f);
            else
                currentHeight = TileData.MinHeight;
        }
        // TOP LEFT CORNER
        else if (tileType == TileData.TileType.TopLeft)
        {
            if (x < xSize / edgeWallScale || z > zSize - zSize / edgeWallScale)
                currentHeight = TileData.MaxHeight;
            else if (lerp > meshValues.perlinLandHigherThreshold)
                currentHeight = TileData.elevatedHeight;
            else if (lerp > meshValues.perlinLandThreshold || x <= xSize / edgeOuterScale || z >= zSize / edgeOuterScale)
                currentHeight = TileData.normalHeight;
            else if (lerp > meshValues.perlinBeachThreshold)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.4f);
            else if (lerp > meshValues.perlinBeachDeeper)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.3f);
            else if (lerp > meshValues.perlinBeachDeepest)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.15f);
            else
                currentHeight = TileData.MinHeight;
        }
        // BOTTOM RIGHT CORNER
        else if (tileType == TileData.TileType.BottomRight)
        {
            if (x > xSize - xSize / edgeWallScale || z < zSize / edgeWallScale)
                currentHeight = TileData.MaxHeight;
            else if (lerp > meshValues.perlinLandHigherThreshold)
                currentHeight = TileData.elevatedHeight;
            else if (lerp > meshValues.perlinLandThreshold || x >= xSize / edgeOuterScale || z <= zSize / edgeOuterScale)
                currentHeight = TileData.normalHeight;
            else if (lerp > meshValues.perlinBeachThreshold)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.4f);
            else if (lerp > meshValues.perlinBeachDeeper)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.3f);
            else if (lerp > meshValues.perlinBeachDeepest)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.15f);
            else
                currentHeight = TileData.MinHeight;
        }
        // TOP RIGHT CORNER
        else if (tileType == TileData.TileType.TopRight)
        {
            if (x > xSize - xSize / edgeWallScale || z > zSize - zSize / edgeWallScale)
                currentHeight = TileData.MaxHeight;
            else if (lerp > meshValues.perlinLandHigherThreshold)
                currentHeight = TileData.elevatedHeight;
            else if (lerp > meshValues.perlinLandThreshold || x >= xSize / edgeOuterScale || z >= zSize / edgeOuterScale)
                currentHeight = TileData.normalHeight;
            else if (lerp > meshValues.perlinBeachThreshold)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.4f);
            else if (lerp > meshValues.perlinBeachDeeper)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.3f);
            else if (lerp > meshValues.perlinBeachDeepest)
                currentHeight = Mathf.Lerp(TileData.MinHeight, TileData.normalHeight, 0.15f);
            else
                currentHeight = TileData.MinHeight;
        }

        return currentHeight;
    }

    /*
     * Calculate the color of a given vertex based from:
     *      -Position: base color from height
     *      -Lerp: shade of color from lerp
     */
    private Color CalculateColor(Vector3 position, float lerp)
    {
        Color heightColor = meshValues.sand.lightColor;
        
        /*
        if(position.z +baseGameObject.transform.localPosition.z + transform.position.z > meshValues.snow.minHeight)
        {
            if (position.z + baseGameObject.transform.localPosition.z + transform.position.z > meshValues.snow.maxHeight)
            {
                heightColor = Color.Lerp(meshValues.snow.lightColor, meshValues.snow.darkColor, lerp);
                return heightColor;
            }
            else
            {
                heightColor = Color.Lerp(meshValues.grass.darkColor, meshValues.snow.lightColor,
                    (2500 - position.z + baseGameObject.transform.localPosition.z + transform.position.z) / (meshValues.snow.maxHeight - meshValues.snow.minHeight));
                return heightColor;
            }
        }*/

        if (position.y >= meshValues.sand.minHeight)
        {
            if (position.y >= meshValues.grass.minHeight)
            {
                if (position.y > TileData.normalHeight && position.y <= TileData.elevatedHeight)
                {
                    heightColor = meshValues.dirt.darkColor;
                }
                else if(position.y > meshValues.grass.maxHeight)
                {
                    heightColor = Color.Lerp(meshValues.stone.lightColor, meshValues.stone.darkColor, lerp);
                }
                else
                {
                    heightColor = Color.Lerp(meshValues.grass.lightColor, meshValues.grass.darkColor, lerp);
                }
            }
            else if (position.y < meshValues.grass.minHeight)
            {
                //heightColor = Color.Lerp(meshValues.sand.lightColor, meshValues.sand.darkColor, lerp);
                float tempValue = Mathf.InverseLerp(meshValues.sand.minHeight, meshValues.sand.maxHeight, position.y);
                heightColor = Color.Lerp(meshValues.sand.darkColor, meshValues.sand.lightColor, tempValue);
            }
        }
        else
        {
            heightColor = Color.Lerp(meshValues.sand.lightColor, meshValues.sand.darkColor, lerp);
        }

        return heightColor;
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

    private void OnDrawGizmos()
    {
        if (baseGameObject != null && spawnableVertices != null && showGizmos)
        {
            Gizmos.color = Color.red;
            Vector3[] tempVerts = new Vector3[spawnableVertices.Keys.Count];
            spawnableVertices.Keys.CopyTo(tempVerts, 0);
            for(int i = 0; i < tempVerts.Length; i++)
            {
                Gizmos.DrawSphere(transform.position + baseGameObject.transform.localPosition + tempVerts[i], 0.5f);
            }
            tempVerts = new Vector3[edgeVertices.Keys.Count];
            edgeVertices.Keys.CopyTo(tempVerts, 0);
            Gizmos.color = Color.blue;
            for(int i = 0; i < tempVerts.Length; i++)
            {
                Gizmos.DrawSphere(transform.position + baseGameObject.transform.localPosition + tempVerts[i], 0.5f);
            }
        }
    }
}

[System.Serializable]
public class TileMeshValues
{
    public float perlinScale = 20.0f;
    public float perlinLandThreshold = 0.4f;
    public float perlinLandHigherThreshold = 0.5f;
    public float perlinBeachThreshold = 0.3f;
    public float perlinBeachDeeper = 0.25f;
    public float perlinBeachDeepest = 0.15f;
    public float perlinReduction = 0.1f;
    public TileMeshScaleValues scaleValues;

    public TileMeshColorValues sand;
    public TileMeshColorValues grass;
    public TileMeshColorValues stone;
    public TileMeshColorValues snow;
    public TileMeshColorValues dirt;

    public SpawnData treeSpawns;
    public SpawnData rockSpawns;
    public SpawnData grassSpawns;
}

[System.Serializable]
public class TileMeshColorValues
{
    public Color lightColor;
    public Color darkColor;
    public float minHeight;
    public float maxHeight;
}

[System.Serializable]
public class TileMeshScaleValues
{
    public float mountainHeightScale = 10.0f;
    public float groundHeightScale = 2.0f;
    public float oceanFloorHeightScale = 3.0f;
}

public static class TileData
{
    public static float TileSize = 500;
    public static float VertWidth = 50;
    public static float QuadSize = 10;

    public static float MinHeight = -5;
    public static float MaxHeight = 15;
    public static float normalHeight = 1;
    public static float elevatedHeight = 3;
    public static float heightRange = 10.0f;

    public enum TileType
    {
        Normal,
        BottomLeft,
        BottomEdge,
        BottomRight,
        LeftEdge,
        RightEdge,
        TopLeft,
        TopEdge,
        TopRight
    }

    public enum TileBiome
    {
        Temperate,
        Tropical,
        Pine,
        Snow
    }

    public static float GetPerlinValue(Vector2 tilePosition, float worldSize, float scale, float offsetX, float offsetZ)
    {
        float xCoord = tilePosition.x / worldSize * scale + offsetX;
        float zCoord = tilePosition.y / worldSize * scale + offsetZ;
        return Mathf.PerlinNoise(xCoord, zCoord);
    }
}

[System.Serializable]
public class TileComponentGroup
{
    public string groupName;
    public List<TileComponentSubGroup> subgroups;

    public TileComponentGroup()
    {
        groupName = "New Group";
        subgroups = new List<TileComponentSubGroup>();
    }
}

[System.Serializable]
public class TileComponentSubGroup
{
    public string subgroupName;

    public enum TileComponentBiome
    {
        Grass = 0,
        Snow,
        NUM_OF_TILE_BIOMES
    }
    public TileComponentBiome groupType = TileComponentBiome.Grass;

    public List<GameObject> components;

    public TileComponentSubGroup()
    {
        subgroupName = "New Subgroup";
        groupType = TileComponentBiome.Grass;
        components = new List<GameObject>();
    }
}
