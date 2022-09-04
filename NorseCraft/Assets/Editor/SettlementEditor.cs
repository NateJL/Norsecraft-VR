using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SettlementManager))]
public class SettlementEditor : Editor
{
    int toolbarIndex = 0;
    string[] toolbarStrings = { "Custom", "Default", "Vertices", "Prefabs" };

    int xSize = 10;
    int zSize = 10;

    List<bool> showVerts;

    Material mat;

    private void OnEnable()
    {
        var shader = Shader.Find("Hidden/Internal-Colored");
        mat = new Material(shader);
        showVerts = new List<bool>();
    }

    public override void OnInspectorGUI()
    {
        SettlementManager controller = (SettlementManager)target;

        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        if (toolbarIndex == 0)
            DrawValues(controller);
        else if (toolbarIndex == 1)
            base.OnInspectorGUI();
        else if (toolbarIndex == 2)
            DrawVertexDictionary(controller);
        else if (toolbarIndex == 3)
            DrawPrefabs(controller);
    }

    private void DrawValues(SettlementManager controller)
    {
        EditorGUILayout.LabelField("Vert Dictionary: " + controller.data.vertDict.Keys.Count);
        int vertCount = controller.data.vertices.Count + controller.data.edgeVerts.Count + controller.data.streetVerts.Count + controller.data.buildingVerts.Count;
        EditorGUILayout.LabelField("Total Vertices: " + vertCount);
        EditorGUILayout.LabelField("Size: " + controller.data.size.ToString());

        EditorGUILayout.BeginHorizontal();
        controller.showVerts = EditorGUILayout.Toggle(controller.showVerts);
        EditorGUILayout.LabelField("Vertices: " + controller.data.vertices.Count);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        controller.showEdges = EditorGUILayout.Toggle(controller.showEdges);
        EditorGUILayout.LabelField("Edges: " + controller.data.edgeVerts.Count);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        controller.showStreets = EditorGUILayout.Toggle(controller.showStreets);
        EditorGUILayout.LabelField("Streets: " + controller.data.streetVerts.Count);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        controller.showBuildings = EditorGUILayout.Toggle(controller.showBuildings);
        EditorGUILayout.LabelField("Buildings: " + controller.data.buildingVerts.Count);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        controller.showWireframe = EditorGUILayout.Toggle("Show Wireframe", controller.showWireframe);
        controller.showIndex = EditorGUILayout.Toggle("Show Vertex Indices", controller.showIndex);
        controller.showVertDictionary = EditorGUILayout.Toggle("Show Vertex Dict.", controller.showVertDictionary);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        xSize = EditorGUILayout.IntField("X Size", xSize);
        zSize = EditorGUILayout.IntField("Z Size", zSize);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Data"))
        {
            GenerateData(controller);
        }

        if (GUILayout.Button("Randomize"))
        {
            for (int i = controller.transform.childCount - 1; i >= 0; i--)
            {
                GameData.SafeDestroy(controller.transform.GetChild(i).gameObject);
            }

            GenerateData(controller);

            controller.Initialize();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset"))
        {
            for(int i = controller.transform.childCount-1; i >= 0; i--)
            {
                GameData.SafeDestroy(controller.transform.GetChild(i).gameObject);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
    }

    private void DrawGraph(SettlementManager controller)
    {
        Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            mat.SetPass(0);

            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            foreach (VertexData vdata in controller.data.vertDict.Values)
            {
                DrawVertex(controller, vdata);
            }
            GL.End();
            GL.PopMatrix();
            GUI.EndClip();
        }
    }

    private void DrawVertex(SettlementManager controller, VertexData vdata)
    {
        int vertexSize = 3;
        int lowerBound = 250;
        int centerX = 200;

        Vector3 topLeft = new Vector3(vdata.tilePos2D.x - controller.data.center.x - vertexSize + centerX, lowerBound - vdata.tilePos2D.y + controller.data.center.z + vertexSize, 0);
        Vector3 topRight = new Vector3(vdata.tilePos2D.x - controller.data.center.x + vertexSize + centerX, lowerBound - vdata.tilePos2D.y + controller.data.center.z + vertexSize, 0);
        Vector3 bottomLeft = new Vector3(vdata.tilePos2D.x - controller.data.center.x - vertexSize + centerX, lowerBound - vdata.tilePos2D.y + controller.data.center.z - vertexSize, 0);
        Vector3 bottomRight = new Vector3(vdata.tilePos2D.x - controller.data.center.x + vertexSize + centerX, lowerBound - vdata.tilePos2D.y + controller.data.center.z - vertexSize, 0);

        GL.Vertex(topLeft);
        GL.Vertex(topRight);
        GL.Vertex(bottomRight);
        GL.Vertex(bottomLeft);
    }

    private void DrawPrefabs(SettlementManager controller)
    {
        EditorGUILayout.LabelField("Generic Prefabs");
        controller.buildingGenerator = (GameObject)EditorGUILayout.ObjectField("Building Generator", controller.buildingGenerator, typeof(GameObject), true);
        controller.grassPrefab = (GameObject)EditorGUILayout.ObjectField("Grass Tile", controller.grassPrefab, typeof(GameObject), true);
        controller.cobblestonePrefab = (GameObject)EditorGUILayout.ObjectField("Path", controller.cobblestonePrefab, typeof(GameObject), true);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Wall Prefabs");
        for (int i = 0; i < controller.walls.prefabs.Count; i++)
        {
            controller.walls.prefabs[i] = (GameObject)EditorGUILayout.ObjectField(controller.walls.prefabs[i], typeof(GameObject), true);
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add wall Prefab"))
        {
            controller.walls.prefabs.Add(null);
        }
        if (GUILayout.Button("Remove Last"))
        {
            controller.walls.prefabs.RemoveAt(controller.walls.prefabs.Count - 1);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Custom Prefabs");
        for (int i = 0; i < controller.customPrefabs.Count; i++)
        {
            controller.customPrefabs[i].prefab = (GameObject)EditorGUILayout.ObjectField(controller.customPrefabs[i].prefabName, controller.customPrefabs[i].prefab, typeof(GameObject), true);
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Custom Prefab"))
        {
            controller.customPrefabs.Add(new SettlementPrefab());
        }
        if (GUILayout.Button("Remove Last"))
        {
            controller.customPrefabs.RemoveAt(controller.customPrefabs.Count - 1);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawVertexDictionary(SettlementManager controller)
    {
        if(showVerts.Count < controller.data.vertDict.Keys.Count)
        {
            int diff = controller.data.vertDict.Keys.Count - showVerts.Count;
            for (int i = 0; i < diff; i++)
                showVerts.Add(false);
        }

        Vector2[] keys = new Vector2[controller.data.vertDict.Keys.Count];
        VertexData[] verts = new VertexData[controller.data.vertDict.Values.Count];
        controller.data.vertDict.Keys.CopyTo(keys, 0);
        controller.data.vertDict.Values.CopyTo(verts, 0);

        EditorGUILayout.Space();
        controller.showVertIndex =(int) EditorGUILayout.Slider(controller.showVertIndex, 0, controller.data.vertDict.Keys.Count-1);
        EditorGUILayout.Space();

        for (int i = 0; i < keys.Length; i++)
        {
            showVerts[i] = EditorGUILayout.Foldout(showVerts[i], keys[i].ToString(), EditorStyles.foldout);
            if(showVerts[i])
            {
                EditorGUILayout.Vector3Field("World Pos.", verts[i].worldPosition);
                EditorGUILayout.Vector3Field("Tile Pos.", verts[i].tilePosition);
            }
        }
    }

    private void GenerateData(SettlementManager controller)
    {
        controller.data = new SettlementData();

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                VertexData newVertex = new VertexData(new Vector3(x * TileData.QuadSize, 0, z * TileData.QuadSize), 
                                                      new Vector3(x * TileData.QuadSize, 0, z * TileData.QuadSize) + controller.transform.position);

                if (x == 0 || x == xSize - 1 || z == 0 || z == zSize - 1)
                    newVertex.isEdge = true;

                if (x == 0 && z == 0)
                    newVertex.edgeFace = VertexData.VertexEdge.SOUTHWEST;
                else if(x == xSize - 1 && z == 0)
                    newVertex.edgeFace = VertexData.VertexEdge.SOUTHEAST;
                else if (x == 0 && z == zSize - 1)
                    newVertex.edgeFace = VertexData.VertexEdge.NORTHWEST;
                else if (x == xSize - 1 && z == zSize - 1)
                    newVertex.edgeFace = VertexData.VertexEdge.NORTHEAST;
                else if (x == 0)
                    newVertex.edgeFace = VertexData.VertexEdge.WEST;
                else if (x == xSize - 1)
                    newVertex.edgeFace = VertexData.VertexEdge.EAST;
                else if (z == 0)
                    newVertex.edgeFace = VertexData.VertexEdge.SOUTH;
                else if (z == zSize - 1)
                    newVertex.edgeFace = VertexData.VertexEdge.NORTH;

                controller.data.vertices.Add(newVertex);
            }
        }

        controller.data.Recalculate();
    }
}
