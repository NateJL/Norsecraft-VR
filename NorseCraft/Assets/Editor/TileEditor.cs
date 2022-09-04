using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileController))]
public class TileEditor : Editor
{
    int toolbarIndex = 0;
    string[] toolbarStrings = { "Default", "MeshData", "Generators" , "Components" };

    public bool showVertices;

    public TileData.TileType tileType = TileData.TileType.Normal;

    public Vector2 tileLocation = new Vector2();
    public int worldSize = 8;

    public List<ShowPrefabData> showPrefabData = null;


    private void OnEnable()
    {
        showPrefabData = new List<ShowPrefabData>();
    }

    public override void OnInspectorGUI()
    {
        TileController controller = (TileController)target;

        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (toolbarIndex == 0)
            base.OnInspectorGUI();

        else if (toolbarIndex == 1)
            DrawMeshDataWindow(controller);

        else if (toolbarIndex == 2)
            DrawGeneratorsWindow(controller);

        else if (toolbarIndex == 3)
            DrawComponentsWindow(controller);

    }

    private void DrawMeshDataWindow(TileController controller)
    {
        EditorGUILayout.Space();
        showVertices = EditorGUILayout.Toggle("Show Vertices", showVertices);
        worldSize = EditorGUILayout.IntField("WorldSize", worldSize);
        tileType = (TileData.TileType)EditorGUILayout.EnumPopup(tileType);
        tileLocation = EditorGUILayout.Vector2Field("Position", tileLocation);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Base Mesh"))
        {
            controller.Initialize(new Vector2(0, 2), worldSize, 0, 0, null);
        }
        if (GUILayout.Button("Destroy Base Mesh"))
        {
            PrefabPlacer.SafeDestroy(controller.baseGameObject);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Island"))
        {
            controller.SpawnEmptyIsland();
        }
        if (GUILayout.Button("Destroy Islands"))
        {
            for (int i = controller.islandParent.childCount - 1; i >= 0; i--)
                PrefabPlacer.SafeDestroy(controller.islandParent.GetChild(i).gameObject);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGeneratorsWindow(TileController controller)
    {
        EditorGUILayout.LabelField("Component Generators");
        EditorGUILayout.Space();

        controller.treeGeneratorPrefab = (GameObject)EditorGUILayout.ObjectField("Tree Generator", controller.treeGeneratorPrefab, typeof(GameObject), true);
        controller.rockGeneratorPrefab = (GameObject)EditorGUILayout.ObjectField("Rock Generator", controller.rockGeneratorPrefab, typeof(GameObject), true);
    }

    private void DrawComponentsWindow(TileController controller)
    {
        EditorGUILayout.LabelField("Component Prefabs");
        if (GUILayout.Button("Refresh"))
        {
            showPrefabData = new List<ShowPrefabData>();
            for (int i = 0; i < controller.tileComponents.Count; i++)
            {
                ShowPrefabData newData = new ShowPrefabData();
                for (int j = 0; j < controller.tileComponents[i].subgroups.Count; j++)
                {
                    newData.showSubgroups.Add(false);
                }
                showPrefabData.Add(newData);
            }
        }
        EditorGUILayout.Space();

        if (controller.tileComponents.Count > showPrefabData.Count)
        {
            int diff = controller.tileComponents.Count - showPrefabData.Count;
            for (int i = 0; i < diff; i++)
                showPrefabData.Add(new ShowPrefabData());
        }

        for (int i = 0; i < controller.tileComponents.Count; i++)
        {
            showPrefabData[i].showRoot = EditorGUILayout.Foldout(showPrefabData[i].showRoot, controller.tileComponents[i].groupName);
            if (showPrefabData[i].showRoot)
            {
                controller.tileComponents[i].groupName = EditorGUILayout.TextField("Group", controller.tileComponents[i].groupName);

                if (controller.tileComponents[i].subgroups.Count > showPrefabData[i].showSubgroups.Count)
                {
                    int diff = controller.tileComponents[i].subgroups.Count - showPrefabData[i].showSubgroups.Count;
                    for (int j = 0; j < diff; j++)
                        showPrefabData[i].showSubgroups.Add(false);
                }

                EditorGUI.indentLevel += 2;
                for (int j = 0; j < controller.tileComponents[i].subgroups.Count; j++)
                {
                    showPrefabData[i].showSubgroups[j] = EditorGUILayout.Foldout(showPrefabData[i].showSubgroups[j], controller.tileComponents[i].subgroups[j].subgroupName);
                    if (showPrefabData[i].showSubgroups[j])
                    {
                        controller.tileComponents[i].subgroups[j].subgroupName = EditorGUILayout.TextField("Subgroup", controller.tileComponents[i].subgroups[j].subgroupName);
                        EditorGUI.indentLevel += 2;
                        for (int k = 0; k < controller.tileComponents[i].subgroups[j].components.Count; k++)
                        {
                            controller.tileComponents[i].subgroups[j].components[k] = (GameObject)EditorGUILayout.ObjectField(controller.tileComponents[i].subgroups[j].components[k], typeof(GameObject), true);
                        }
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Prefab"))
                        {
                            controller.tileComponents[i].subgroups[j].components.Add(null);
                        }
                        if (GUILayout.Button("Remove Prefab"))
                        {
                            controller.tileComponents[i].subgroups[j].components.RemoveAt(controller.tileComponents[i].subgroups[j].components.Count - 1);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel -= 2;
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Subgroup"))
                {
                    controller.tileComponents[i].subgroups.Add(new TileComponentSubGroup());
                }
                if (GUILayout.Button("Remove Subgroup"))
                {
                    controller.tileComponents[i].subgroups.RemoveAt(controller.tileComponents[i].subgroups.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel -= 2;
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Group"))
        {
            controller.tileComponents.Add(new TileComponentGroup());
        }
        if (GUILayout.Button("Remove Group"))
        {
            controller.tileComponents.RemoveAt(controller.tileComponents.Count - 1);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnSceneGUI()
    {
        TileController controller = (TileController)target;

        if (showVertices)
            DrawTileVertices(controller, GizmoType.Active);
    }

    static void DrawTileVertices(TileController scr, GizmoType gizmoType)
    {
        if (scr.baseGameObject != null)
        {
            Mesh mesh = scr.baseGameObject.GetComponent<MeshFilter>().sharedMesh;
            Handles.color = Color.red;
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                //Gizmos.DrawSphere(scr.transform.position + mesh.vertices[i], 0.3f);
                Handles.SphereHandleCap(i, scr.transform.position + mesh.vertices[i], Quaternion.identity, 0.3f, EventType.ScrollWheel);
            }
        }
    }
}

public class ShowPrefabData
{
    public bool showRoot;
    public List<bool> showSubgroups;

    public ShowPrefabData()
    {
        showRoot = false;
        showSubgroups = new List<bool>();
    }
}
