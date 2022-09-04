using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SettlementManager : MonoBehaviour
{
    public bool showWireframe = false;
    public bool showIndex = false;
    public int showVertIndex = 0;
    [Space(10)]
    public bool showVerts = false;
    public bool showEdges = false;
    public bool showStreets = false;
    public bool showBuildings = false;
    public bool showVertDictionary = false;

    public SettlementData data;

    public GameObject buildingGenerator;
    public GameObject grassPrefab;
    public GameObject cobblestonePrefab;
    public PrefabGroup walls;
    public List<SettlementPrefab> customPrefabs;

    /// <summary>
    /// Initialize the settlement <para />
    ///     -generate streets
    ///     -generate buildings
    /// </summary>
    public void Initialize()
    {
        data.GenerateStreetVertices();

        // spawn walls
        GenerateWalls();

        // spawn buildings
        GenerateInterior();
    }

    /// <summary>
    /// Generate boundary walls around the outer edges of the settlement.
    /// </summary>
    public void GenerateWalls()
    {
        for (int i = 0; i < data.edgeVerts.Count; i++)
        {
            bool isEntrance = false;
            GameObject wallObj_01 = null;
            GameObject wallObj_02 = null;
            GameObject wallObj_03 = null;
            if (data.edgeVerts[i].edgeFace == VertexData.VertexEdge.NORTHEAST || data.edgeVerts[i].edgeFace == VertexData.VertexEdge.NORTHWEST ||
                data.edgeVerts[i].edgeFace == VertexData.VertexEdge.SOUTHEAST || data.edgeVerts[i].edgeFace == VertexData.VertexEdge.SOUTHWEST)
            {
                wallObj_01 = Instantiate(walls.prefabs[2]);
                wallObj_02 = Instantiate(walls.prefabs[3]);
                wallObj_03 = Instantiate(walls.prefabs[3]);
            }
            else if (data.edgeVerts[i].worldPosition.x == data.center.x ||
                     data.edgeVerts[i].worldPosition.z == data.center.z)
            {
                wallObj_01 = Instantiate(walls.prefabs[2]);
                wallObj_01.name = "Tower01";
                wallObj_02 = Instantiate(walls.prefabs[2]);
                wallObj_02.name = "Tower02";
                isEntrance = true;
            }
            else
                wallObj_01 = Instantiate(walls.prefabs[0]);

            if(wallObj_01 != null)
                wallObj_01.transform.SetParent(transform);
            if (wallObj_02 != null)
                wallObj_02.transform.SetParent(transform);
            if (wallObj_03 != null)
                wallObj_03.transform.SetParent(transform);

            if (wallObj_01 != null)
            {
                wallObj_01.transform.position = data.edgeVerts[i].worldPosition;
                if (data.edgeVerts[i].edgeFace == VertexData.VertexEdge.EAST || data.edgeVerts[i].edgeFace == VertexData.VertexEdge.WEST)
                {
                    if (isEntrance)
                    {
                        wallObj_01.transform.position = data.edgeVerts[i].worldPosition + new Vector3(0, 0, TileData.QuadSize / 2f);
                        wallObj_02.transform.position = data.edgeVerts[i].worldPosition - new Vector3(0, 0, TileData.QuadSize / 2f);
                    }
                    else
                        wallObj_01.transform.eulerAngles = new Vector3(0, 90, 0);
                }
                else if (data.edgeVerts[i].edgeFace == VertexData.VertexEdge.NORTH || data.edgeVerts[i].edgeFace == VertexData.VertexEdge.SOUTH)
                {
                    if (isEntrance)
                    {
                        wallObj_01.transform.position = data.edgeVerts[i].worldPosition + new Vector3(TileData.QuadSize / 2f, 0, 0);
                        wallObj_02.transform.position = data.edgeVerts[i].worldPosition - new Vector3(TileData.QuadSize / 2f, 0, 0);
                    }
                }
                else if (data.edgeVerts[i].edgeFace == VertexData.VertexEdge.NORTHEAST || data.edgeVerts[i].edgeFace == VertexData.VertexEdge.NORTHWEST ||
                         data.edgeVerts[i].edgeFace == VertexData.VertexEdge.SOUTHEAST || data.edgeVerts[i].edgeFace == VertexData.VertexEdge.SOUTHWEST)
                {
                    if (data.edgeVerts[i].edgeFace == VertexData.VertexEdge.NORTHEAST)
                    {
                        wallObj_02.transform.position = data.edgeVerts[i].worldPosition + new Vector3(-2.5f, 0, 0);
                        wallObj_03.transform.position = data.edgeVerts[i].worldPosition + new Vector3(0, 0, -2.5f);
                        wallObj_03.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    else if (data.edgeVerts[i].edgeFace == VertexData.VertexEdge.SOUTHEAST)
                    {
                        wallObj_02.transform.position = data.edgeVerts[i].worldPosition + new Vector3(-2.5f, 0, 0);
                        wallObj_03.transform.position = data.edgeVerts[i].worldPosition + new Vector3(0, 0, 2.5f);
                        wallObj_03.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    else if (data.edgeVerts[i].edgeFace == VertexData.VertexEdge.SOUTHWEST)
                    {
                        wallObj_02.transform.position = data.edgeVerts[i].worldPosition + new Vector3(2.5f, 0, 0);
                        wallObj_03.transform.position = data.edgeVerts[i].worldPosition + new Vector3(0, 0, 2.5f);
                        wallObj_03.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    else if (data.edgeVerts[i].edgeFace == VertexData.VertexEdge.NORTHWEST)
                    {
                        wallObj_02.transform.position = data.edgeVerts[i].worldPosition + new Vector3(2.5f, 0, 0);
                        wallObj_03.transform.position = data.edgeVerts[i].worldPosition + new Vector3(0, 0, -2.5f);
                        wallObj_03.transform.eulerAngles = new Vector3(0, 90, 0);
                    }

                }

                data.edgeVerts[i].isOccupied = true;
                data.edgeVerts[i].component = wallObj_01;
            }
        }
    }

    /// <summary>
    /// Generate objects within the interior of the settlement. <para />
    ///     -Buildings <para />
    ///     -Grass <para />
    ///     -Pathways
    /// </summary>
    public void GenerateInterior()
    {
        // spawn buildings 
        
        int numberOfBuildings = data.vertDict.Keys.Count / 10;
        if (data.buildingVerts.Count > 0)
        {
            for (int j = 0; j < numberOfBuildings; j++)
            {
                int tryCount = 0;
                bool foundVertex = false;
                while (!foundVertex && tryCount < 10)
                {
                    int vertexIndex = Random.Range(0, data.buildingVerts.Count);
                    if (!data.buildingVerts[vertexIndex].isOccupied)
                    {
                        foundVertex = true;
                        int floorCount = Random.Range(0, 5);
                        GameObject newBuilding = Instantiate(buildingGenerator);
                        newBuilding.GetComponent<BuildingGenerator>().GenerateBuilding();
                        newBuilding.transform.position = data.buildingVerts[vertexIndex].worldPosition;
                        newBuilding.transform.SetParent(transform);

                        data.buildingVerts[vertexIndex].isOccupied = true;
                        data.buildingVerts[vertexIndex].component = newBuilding;
                    }
                    tryCount++;
                }
            }
        }

        // spawn grass
        for (int i = 0; i < data.vertices.Count; i++)
        {
            GameObject newGrass = Instantiate(grassPrefab);
            newGrass.transform.position = data.vertices[i].worldPosition + new Vector3(TileData.QuadSize / 2, 0.1f, TileData.QuadSize / 2);
            newGrass.transform.SetParent(transform);
        }
        for (int i = 0; i < data.buildingVerts.Count; i++)
        {
            if (!data.buildingVerts[i].isOccupied)
            {
                GameObject newGrass = Instantiate(grassPrefab);
                newGrass.transform.position = data.buildingVerts[i].worldPosition + new Vector3(TileData.QuadSize / 2, 0.1f, TileData.QuadSize / 2);
                newGrass.transform.SetParent(transform);
            }
        }

        // spawn pathway
        for (int i = 0; i < data.streetVerts.Count; i++)
        {
            GameObject newGround = Instantiate(cobblestonePrefab);
            newGround.transform.position = data.streetVerts[i].worldPosition;
            newGround.transform.SetParent(transform);

            data.streetVerts[i].isOccupied = true;
            data.streetVerts[i].component = newGround;

            if (data.streetVerts[i].worldPosition.z == data.center.z)
                newGround.transform.localEulerAngles = new Vector3(0, 90, 0);
        }
    }

    /// <summary>
    /// Set the SettlementData associated with this settlement.
    /// </summary>
    public void SetData(SettlementData newData)
    {
        data = newData;
        transform.position = newData.center;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 wireframeSize = new Vector3(10, 5, 10);

        if(showVerts)
        {
            Gizmos.color = Color.white;
            for (int i = 0; i < data.vertices.Count; i++)
            {
                Gizmos.DrawSphere(data.vertices[i].worldPosition, 0.5f);
                if (showWireframe)
                    Gizmos.DrawWireCube(data.vertices[i].worldPosition + Vector3.up*5, wireframeSize);
                if(showIndex)
                    Handles.Label(data.vertices[i].worldPosition + Vector3.up * 10, i.ToString());
            }
        }
        if (showEdges)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < data.edgeVerts.Count; i++)
            {
                Gizmos.DrawSphere(data.edgeVerts[i].worldPosition, 0.5f);
                if (showWireframe)
                    Gizmos.DrawWireCube(data.edgeVerts[i].worldPosition + Vector3.up * 5, wireframeSize);
                if (showIndex)
                    Handles.Label(data.edgeVerts[i].worldPosition + Vector3.up * 10, i.ToString());
            }
        }
        if (showStreets)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < data.streetVerts.Count; i++)
            {
                Gizmos.DrawSphere(data.streetVerts[i].worldPosition, 0.5f);
                if (showWireframe)
                    Gizmos.DrawWireCube(data.streetVerts[i].worldPosition + Vector3.up * 5, wireframeSize);
                if (showIndex)
                    Handles.Label(data.streetVerts[i].worldPosition + Vector3.up * 10, i.ToString());
            }
        }
        if (showBuildings)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < data.buildingVerts.Count; i++)
            {
                Gizmos.DrawSphere(data.buildingVerts[i].worldPosition, 0.5f);
                if (showWireframe)
                    Gizmos.DrawWireCube(data.buildingVerts[i].worldPosition + Vector3.up * 5, wireframeSize);
                if (showIndex)
                    Handles.Label(data.buildingVerts[i].worldPosition + Vector3.up * 10, i.ToString());
            }
        }
        if(showVertDictionary)
        {
            Gizmos.color = Color.green;
            int count = 0;
            foreach(VertexData vdata in data.vertDict.Values)
            {
                if (count == showVertIndex)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(vdata.worldPosition + Vector3.up * 3, 1);
                    Gizmos.color = Color.blue;
                    Vector2 tempVert = new Vector2(vdata.adjacentWorldVertices[VertexData.VertexEdge.NORTH].x, vdata.adjacentWorldVertices[VertexData.VertexEdge.NORTH].z);
                    if (data.vertDict.ContainsKey(tempVert))
                        Gizmos.DrawSphere(data.vertDict[tempVert].worldPosition + Vector3.up * 2, 1);
                    tempVert = new Vector2(vdata.adjacentWorldVertices[VertexData.VertexEdge.SOUTH].x, vdata.adjacentWorldVertices[VertexData.VertexEdge.SOUTH].z);
                    if (data.vertDict.ContainsKey(tempVert))
                        Gizmos.DrawSphere(data.vertDict[tempVert].worldPosition + Vector3.up * 2, 1);
                    tempVert = new Vector2(vdata.adjacentWorldVertices[VertexData.VertexEdge.EAST].x, vdata.adjacentWorldVertices[VertexData.VertexEdge.EAST].z);
                    if (data.vertDict.ContainsKey(tempVert))
                        Gizmos.DrawSphere(data.vertDict[tempVert].worldPosition + Vector3.up * 2, 1);
                    tempVert = new Vector2(vdata.adjacentWorldVertices[VertexData.VertexEdge.WEST].x, vdata.adjacentWorldVertices[VertexData.VertexEdge.WEST].z);
                    if (data.vertDict.ContainsKey(tempVert))
                        Gizmos.DrawSphere(data.vertDict[tempVert].worldPosition + Vector3.up * 2, 1);
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.DrawSphere(vdata.worldPosition + Vector3.up * 1, 0.5f);
                }

                if (showWireframe)
                    Gizmos.DrawWireCube(vdata.worldPosition + Vector3.up * 5, wireframeSize);
                if (showIndex)
                    Handles.Label(vdata.worldPosition + Vector3.up * 10, count.ToString());
                count++;
            }
        }
    }
}

[System.Serializable]
public class SettlementData
{
    public Vector3 center;

    public List<VertexData> vertices;
    public List<VertexData> edgeVerts;
    public List<VertexData> streetVerts;
    public List<VertexData> buildingVerts;
    public Dictionary<Vector2, VertexData> vertDict;

    public List<GameObject> props;

    public enum SettlementSize
    {
        Small,
        Medium,
        Large
    }
    public SettlementSize size = SettlementSize.Small;

    public SettlementData()
    {
        vertices = new List<VertexData>();
        edgeVerts = new List<VertexData>();
        streetVerts = new List<VertexData>();
        buildingVerts = new List<VertexData>();
        vertDict = new Dictionary<Vector2, VertexData>();
        props = new List<GameObject>();
        center = Vector3.zero;
    }

    public SettlementData(VertexData[] newVerts)
    {
        vertices = new List<VertexData>(newVerts);
        edgeVerts = new List<VertexData>();
        streetVerts = new List<VertexData>();
        buildingVerts = new List<VertexData>();
        vertDict = new Dictionary<Vector2, VertexData>();
        props = new List<GameObject>();
        center = Vector3.zero;
        Recalculate();
    }

    /// <summary>
    /// Recalculate values of the settlement: <para />
    ///     -Set generation values based on vertex count (size) <para />
    ///     -Calculate center of settlement. <para />
    ///     -Check for edge vertices.
    /// </summary>
    public void Recalculate()
    {
        CleanVertexLists();

        Vector3 tempVert = Vector3.zero;
        for (int i = 0; i < vertices.Count; i++)
            tempVert += vertices[i].worldPosition;
        center = tempVert / vertices.Count;
        center.x = center.x - (center.x % TileData.QuadSize);
        center.z = center.z - (center.z % TileData.QuadSize);

        if (vertDict.Keys.Count < 100)
            size = SettlementSize.Small;
        else if (vertDict.Keys.Count < 250)
            size = SettlementSize.Medium;
        else
            size = SettlementSize.Large;

        // Loop through all vertices to calculate street and building vertices
        for (int i = vertices.Count - 1; i >= 0; i--)
        {
            if (vertices[i].isEdge)
            {
                if (!edgeVerts.Contains(vertices[i]))
                    edgeVerts.Add(vertices[i]);
                vertices.Remove(vertices[i]);
            }
        }
    }

    public void GenerateStreetVertices()
    {
        // Calculate main road through center of settlement
        for (int i = vertices.Count - 1; i >= 0; i--)
        {
            if (!vertices[i].isEdge && 
                size != SettlementSize.Small && 
                (vertices[i].worldPosition.x == center.x || vertices[i].worldPosition.z == center.z))
            {
                streetVerts.Add(vertices[i]);
                vertices.Remove(vertices[i]);
            }
        }

        if(size != SettlementSize.Small)
        {
            int numberOfStreets = vertDict.Keys.Count / 50;

            // generate street vertices
            for (int i = 0; i < numberOfStreets; i++)
            {
                int randomDirection = Random.Range(0, 2);
                int vertexIndex = Random.Range(0, streetVerts.Count);
                VertexData currentVert = streetVerts[vertexIndex];

                VertexData.VertexEdge direction;
                if(currentVert.worldPosition.x == center.x)
                {
                    if (randomDirection == 0)
                        direction = VertexData.VertexEdge.WEST;
                    else
                        direction = VertexData.VertexEdge.EAST;
                }
                else
                {
                    if (randomDirection == 0)
                        direction = VertexData.VertexEdge.NORTH;
                    else
                        direction = VertexData.VertexEdge.SOUTH;
                }

                bool validSpawn = false;
                bool finished = false;
                int tryCount = 0;
                while (!finished)
                {
                    Vector2 tempVert = new Vector2();

                    tempVert = new Vector2(currentVert.adjacentWorldVertices[direction].x, currentVert.adjacentWorldVertices[direction].z);

                    if (vertDict.ContainsKey(tempVert))
                    {
                        if (vertices.Contains(vertDict[tempVert]))
                        {
                            currentVert = vertDict[tempVert];
                            vertices.Remove(currentVert);
                            streetVerts.Add(currentVert);
                            validSpawn = true;
                        }
                        else
                        {
                            if (validSpawn)
                                finished = true;
                        }
                    }
                    else
                        finished = true;

                    if (!validSpawn)
                        tryCount++;

                    if (tryCount > 10)
                        finished = true;
                }
            }

            // generate building vertices
            for(int i = 0; i < streetVerts.Count; i++)
            {
                Vector2 northVertex = new Vector2(streetVerts[i].adjacentWorldVertices[VertexData.VertexEdge.NORTH].x, streetVerts[i].adjacentWorldVertices[VertexData.VertexEdge.NORTH].z);
                Vector2 southVertex = new Vector2(streetVerts[i].adjacentWorldVertices[VertexData.VertexEdge.SOUTH].x, streetVerts[i].adjacentWorldVertices[VertexData.VertexEdge.SOUTH].z);
                Vector2 eastVertex = new Vector2(streetVerts[i].adjacentWorldVertices[VertexData.VertexEdge.EAST].x, streetVerts[i].adjacentWorldVertices[VertexData.VertexEdge.EAST].z);
                Vector2 westVertex = new Vector2(streetVerts[i].adjacentWorldVertices[VertexData.VertexEdge.WEST].x, streetVerts[i].adjacentWorldVertices[VertexData.VertexEdge.WEST].z);

                if(vertDict.ContainsKey(northVertex))
                {
                    if(vertices.Contains(vertDict[northVertex]))
                    {
                        vertices.Remove(vertDict[northVertex]);
                        buildingVerts.Add(vertDict[northVertex]);
                    }
                }
                if (vertDict.ContainsKey(southVertex))
                {
                    if (vertices.Contains(vertDict[southVertex]))
                    {
                        vertices.Remove(vertDict[southVertex]);
                        buildingVerts.Add(vertDict[southVertex]);
                    }
                }
                if (vertDict.ContainsKey(eastVertex))
                {
                    if (vertices.Contains(vertDict[eastVertex]))
                    {
                        vertices.Remove(vertDict[eastVertex]);
                        buildingVerts.Add(vertDict[eastVertex]);
                    }
                }
                if (vertDict.ContainsKey(westVertex))
                {
                    if (vertices.Contains(vertDict[westVertex]))
                    {
                        vertices.Remove(vertDict[westVertex]);
                        buildingVerts.Add(vertDict[westVertex]);
                    }
                }
            }
        }
    }

    public void CleanVertexLists()
    {
        for (int i = vertices.Count - 1; i > 0; i--)
        {
            for(int j = i - 1; j >= 0; j--)
            {
                if (vertices[i].tilePos2D.Equals(vertices[j].tilePos2D))
                {
                    vertices.RemoveAt(i);
                    break;
                }
            }
        }

        for (int i = edgeVerts.Count - 1; i > 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                if (edgeVerts[i].tilePos2D.Equals(edgeVerts[j].tilePos2D))
                {
                    edgeVerts.RemoveAt(i);
                    break;
                }
            }
        }

        for (int i = streetVerts.Count - 1; i > 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                if (streetVerts[i].tilePos2D.Equals(streetVerts[j].tilePos2D))
                {
                    streetVerts.RemoveAt(i);
                    break;
                }
            }
        }

        for (int i = buildingVerts.Count - 1; i > 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                if (buildingVerts[i].tilePos2D.Equals(buildingVerts[j].tilePos2D))
                {
                    buildingVerts.RemoveAt(i);
                    break;
                }
            }
        }

        vertDict.Clear();
        for(int i = 0; i < vertices.Count; i++)
        {
            vertDict.Add(vertices[i].tilePos2D, vertices[i]);
        }
        for(int i = 0; i < edgeVerts.Count; i++)
        {
            if (!vertDict.ContainsKey(edgeVerts[i].tilePos2D))
                vertDict.Add(edgeVerts[i].tilePos2D, edgeVerts[i]);
        }
        for (int i = 0; i < streetVerts.Count; i++)
        {
            if (!vertDict.ContainsKey(streetVerts[i].tilePos2D))
                vertDict.Add(streetVerts[i].tilePos2D, streetVerts[i]);
        }
        for (int i = 0; i < buildingVerts.Count; i++)
        {
            if (!vertDict.ContainsKey(buildingVerts[i].tilePos2D))
                vertDict.Add(buildingVerts[i].tilePos2D, buildingVerts[i]);
        }
    }

    /// <summary>
    /// Add a new vertex to the settlements vertex collection
    /// </summary>
    public void AddVertex(VertexData newData)
    {
        vertices.Add(newData);
        vertDict.Add(newData.tilePos2D, newData);
    }

    public bool CheckVertexAssociation(VertexData newVertex)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            if (newVertex.adjacentWorldVertices[VertexData.VertexEdge.EAST] == vertices[i].worldPosition ||
               newVertex.adjacentWorldVertices[VertexData.VertexEdge.WEST] == vertices[i].worldPosition ||
               newVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTH] == vertices[i].worldPosition ||
               newVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTHEAST] == vertices[i].worldPosition ||
               newVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTHWEST] == vertices[i].worldPosition ||
               newVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTH] == vertices[i].worldPosition ||
               newVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTHEAST] == vertices[i].worldPosition ||
               newVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTHWEST] == vertices[i].worldPosition)
            {
                return true;
            }
        }
        for (int i = 0; i < edgeVerts.Count; i++)
        {
            if (newVertex.adjacentWorldVertices[VertexData.VertexEdge.EAST] == edgeVerts[i].worldPosition ||
                newVertex.adjacentWorldVertices[VertexData.VertexEdge.WEST] == edgeVerts[i].worldPosition ||
                newVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTH] == edgeVerts[i].worldPosition ||
                newVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTHEAST] == edgeVerts[i].worldPosition ||
                newVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTHWEST] == edgeVerts[i].worldPosition ||
                newVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTH] == edgeVerts[i].worldPosition ||
                newVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTHEAST] == edgeVerts[i].worldPosition ||
                newVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTHWEST] == edgeVerts[i].worldPosition)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the given vertex is adjacent to any vertices in the settlement.
    /// </summary>
    public bool CheckVertexAdjacency(VertexData otherVertex)
    {
        Vector2 pos2D = new Vector2(otherVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTHWEST].x, otherVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTHWEST].z);
        if (vertDict.ContainsKey(pos2D))
            return true;
        pos2D = new Vector2(otherVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTH].x, otherVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTH].z);
        if (vertDict.ContainsKey(pos2D))
            return true;
        pos2D = new Vector2(otherVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTHEAST].x, otherVertex.adjacentWorldVertices[VertexData.VertexEdge.NORTHEAST].z);
        if (vertDict.ContainsKey(pos2D))
            return true;
        pos2D = new Vector2(otherVertex.adjacentWorldVertices[VertexData.VertexEdge.WEST].x, otherVertex.adjacentWorldVertices[VertexData.VertexEdge.WEST].z);
        if (vertDict.ContainsKey(pos2D))
            return true;
        pos2D = new Vector2(otherVertex.adjacentWorldVertices[VertexData.VertexEdge.EAST].x, otherVertex.adjacentWorldVertices[VertexData.VertexEdge.EAST].z);
        if (vertDict.ContainsKey(pos2D))
            return true;
        pos2D = new Vector2(otherVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTHWEST].x, otherVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTHWEST].z);
        if (vertDict.ContainsKey(pos2D))
            return true;
        pos2D = new Vector2(otherVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTH].x, otherVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTH].z);
        if (vertDict.ContainsKey(pos2D))
            return true;
        pos2D = new Vector2(otherVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTHEAST].x, otherVertex.adjacentWorldVertices[VertexData.VertexEdge.SOUTHEAST].z);
        if (vertDict.ContainsKey(pos2D))
            return true;

        return false;
    }

    /// <summary>
    /// Add all vertex data from given settlement to this settlement
    /// </summary>
    public void MergeSettlements(SettlementData newData)
    {
        for(int i = 0; i < newData.vertices.Count; i++)
            vertices.Add(newData.vertices[i]);

        for (int i = 0; i < newData.edgeVerts.Count; i++)
            edgeVerts.Add(newData.edgeVerts[i]);

        for (int i = 0; i < newData.streetVerts.Count; i++)
            vertices.Add(newData.streetVerts[i]);

        for (int i = 0; i < newData.buildingVerts.Count; i++)
            vertices.Add(newData.buildingVerts[i]);

        foreach (VertexData vData in newData.vertDict.Values)
            vertDict.Add(vData.tilePos2D, vData);
    }

    /// <summary>
    /// Compare 2 settlements vertices, return true if adjacent (connected).
    /// </summary>
    public static bool CheckSettlementAdjacency(SettlementData s1, SettlementData s2)
    {
        bool isAdjacent = false;

        foreach(Vector2 tilePos in s1.vertDict.Keys)
        {
            if(s2.CheckVertexAdjacency(s1.vertDict[tilePos]))
            {
                isAdjacent = true;
                break;
            }
        }

        return isAdjacent;
    }
}

[System.Serializable]
public class SettlementPrefab
{
    public string prefabName;
    public GameObject prefab;

    public SettlementPrefab()
    {
        prefabName = "New Prefab";
        prefab = null;
    }
}

[System.Serializable]
public class PrefabGroup
{
    public string groupName;
    public List<GameObject> prefabs;

    public PrefabGroup()
    {
        groupName = "New Group";
        prefabs = new List<GameObject>();
        prefabs.Add(null);
    }
}
