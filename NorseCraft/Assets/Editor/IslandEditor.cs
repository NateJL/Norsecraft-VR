using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IslandController))]
public class IslandEditor : Editor
{

    private void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        IslandController controller = (IslandController)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn Base"))
        {
            controller.SpawnIslandBase();
        }
        if(GUILayout.Button("Spawn Dock"))
        {
            controller.SpawnIslandDock();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn Fences"))
        {
            controller.SpawnIslandFences();
        }
        if (GUILayout.Button("Spawn Rocks"))
        {
            controller.SpawnIslandRocks();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn Trees"))
        {
            controller.SpawnIslandTrees();
        }
        if (GUILayout.Button("Spawn Grass"))
        {
            controller.SpawnGrass();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Spawn Structures"))
        {
            controller.SpawnStructures();
        }
        if (GUILayout.Button("placeholder"))
        {
            
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Randomize"))
        {
            controller.ResetIsland();
            controller.SpawnIslandBase();
            controller.SpawnIslandDock();
            controller.SpawnIslandFences();
            controller.SpawnIslandTrees();
            controller.SpawnIslandRocks();
            controller.SpawnGrass();
            controller.SpawnStructures();
        }
        if(GUILayout.Button("Clear"))
        {
            controller.ResetIsland();
        }
        EditorGUILayout.EndHorizontal();

    }
}
