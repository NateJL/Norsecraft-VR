using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlacerEditor : EditorWindow
{

    public bool[] showPrefabs;
    PrefabPlacer.PlaceMode newMode = PrefabPlacer.PlaceMode.Tiles;

    Texture2D prefabPreview = null;

    [MenuItem("Tools/Object Placement Tool")]
    static void Init()
    {
        PlacerEditor window = (PlacerEditor)EditorWindow.GetWindow(typeof(PlacerEditor), false, "Object Placeer");
        window.Show();
    }

    private void OnEnable()
    {
        GameObject placer = GameObject.FindGameObjectWithTag("EditorOnly");
        PrefabPlacer controller = null;
        if (placer != null)
        {
            controller = placer.GetComponent<PrefabPlacer>();
            controller.GetNewPrefabs(newMode);
        }
    }

    public void OnGUI()
    {
        //GUI.Box(new Rect((Screen.width) / 2 - (Screen.width) / 8, (Screen.height) / 2 - (Screen.height) / 8, (Screen.width) / 4, (Screen.height) / 4), "Object Placement Tool");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Object Placement Tool", EditorStyles.boldLabel);
        PrefabPlacer controller = GameObject.FindGameObjectWithTag("EditorOnly").GetComponent<PrefabPlacer>();

        newMode  = (PrefabPlacer.PlaceMode)EditorGUILayout.EnumPopup("Mode", newMode);
        if(newMode != controller.placeMode)
        {
            controller.GetNewPrefabs(newMode);
            if(controller.currentPrefabArray != null && controller.currentPrefabArray.Length > 0)
                showPrefabs = new bool[controller.currentPrefabArray.Length];
        }

        
        if(Selection.activeGameObject != null)
        {
            controller.currentGameObject = Selection.activeGameObject;
            if (controller.currentGameObject.transform.parent != null)
                controller.parent = controller.currentGameObject.transform.parent.gameObject;
            controller.position = controller.currentGameObject.transform.localPosition;
            controller.rotation = controller.currentGameObject.transform.localEulerAngles;
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawCurrentGameObject(controller);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.LabelField("Spawn Coordinates", EditorStyles.boldLabel);
        controller.position = EditorGUILayout.Vector3Field("Current Position", controller.position);
        controller.incrementPos = EditorGUILayout.Vector3Field("Spawn Offset", controller.incrementPos);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        controller.rotation = EditorGUILayout.Vector3Field("Current Rotation", controller.rotation);
        controller.incrementRot = EditorGUILayout.Vector3Field("Spawn Rotation", controller.incrementRot);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        DrawSpawnPrefabButtons(controller);
    }

    /*
     * Draw data about current selected gameobject
     */
    private void DrawCurrentGameObject(PrefabPlacer controller)
    {
        EditorGUILayout.LabelField("Selected GameObject", EditorStyles.boldLabel);
        EditorGUILayout.ObjectField("GameObject", controller.currentGameObject, typeof(GameObject), true);
        if (controller.currentGameObject != null)
        {
            controller.currentGameObject.transform.localPosition = EditorGUILayout.Vector3Field("Position", controller.currentGameObject.transform.localPosition);
            controller.currentGameObject.transform.localEulerAngles = EditorGUILayout.Vector3Field("Rotation", controller.currentGameObject.transform.localEulerAngles);
            EditorGUILayout.Space();
            if (controller.currentGameObject.transform.parent != null)
            {
                EditorGUILayout.ObjectField("Parent", controller.currentGameObject.transform.parent.gameObject, typeof(GameObject), true);
                EditorGUILayout.LabelField("Parent children: " + controller.currentGameObject.transform.parent.childCount);
            }
            else
            {
                EditorGUILayout.ObjectField("Parent", null, typeof(GameObject), true);
                EditorGUILayout.LabelField("Parent children: 0");
            }
        }
        else
        {
            EditorGUILayout.Vector3Field("Position", Vector3.zero);
            EditorGUILayout.Vector3Field("Rotation", Vector3.zero);
            EditorGUILayout.ObjectField("Parent", null, typeof(GameObject), true);
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+X"))
        {
            if (controller.currentGameObject != null)
                controller.currentGameObject.transform.localPosition += new Vector3(1, 0, 0);
        }
        if (GUILayout.Button("+Y"))
        {
            if (controller.currentGameObject != null)
                controller.currentGameObject.transform.localPosition += new Vector3(0, 1, 0);
        }
        if (GUILayout.Button("+Z"))
        {
            if (controller.currentGameObject != null)
                controller.currentGameObject.transform.localPosition += new Vector3(0, 0, 1);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("-X"))
        {
            if (controller.currentGameObject != null)
                controller.currentGameObject.transform.localPosition += new Vector3(-1, 0, 0);
        }
        if (GUILayout.Button("-Y"))
        {
            if (controller.currentGameObject != null)
                controller.currentGameObject.transform.localPosition += new Vector3(0, -1, 0);
        }
        if (GUILayout.Button("-Z"))
        {
            if (controller.currentGameObject != null)
                controller.currentGameObject.transform.localPosition += new Vector3(0, 0, -1);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("Rotate GameObject 90*"))
        {
            controller.rotateCurrentGameObject();
        }
        if (GUILayout.Button("Delete Selected GameObject"))
        {
            if (Selection.activeGameObject != null)
                PrefabPlacer.SafeDestroy(Selection.activeGameObject);
        }
    }

    /*
     * Draw buttons for selected object array
     */
    private void DrawSpawnPrefabButtons(PrefabPlacer controller)
    {
        prefabPreview = null;
        controller.parent =(GameObject) EditorGUILayout.ObjectField("Parent", controller.parent.gameObject, typeof(GameObject), true);
        EditorGUILayout.LabelField("Parent children: " + controller.parent.transform.childCount);
        if (controller.currentPrefabArray.Length > 0)
        {
            for (int i = 0; i < controller.currentPrefabArray.Length; i++)
            {
                showPrefabs[i] = EditorGUILayout.Foldout(showPrefabs[i], controller.currentPrefabArray[i].tag, true, EditorStyles.foldout);
                if (showPrefabs[i])
                {
                    prefabPreview =(Texture2D) AssetPreview.GetAssetPreview(controller.currentPrefabArray[i].prefab);
                    if(prefabPreview != null)
                        GUILayout.Label(prefabPreview);

                    if (GUILayout.Button("Spawn: " + controller.currentPrefabArray[i].tag))
                    {
                        Selection.activeGameObject = controller.SpawnPrefab(i);
                    }
                }
            }
        }
    }

    private void Update()
    {
        Repaint();
    }
}
