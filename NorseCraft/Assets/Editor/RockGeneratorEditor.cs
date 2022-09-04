using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RockGenerator))]
public class RockGeneratorEditor : Editor
{
    int toolbarIndex = 0;
    string[] toolbarStrings = { "Default", "Custom" };

    public override void OnInspectorGUI()
    {
        RockGenerator controller = (RockGenerator)target;

        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        switch (toolbarIndex)
        {
            case 0:
                DrawBasicInspector(controller);
                break;

            case 1:
                DrawCustomInspector();
                break;
        }
    }

    private void DrawBasicInspector(RockGenerator controller)
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        if (GUILayout.Button("Spawn Rock"))
        {
            controller.SpawnRock(controller.biome);
            Debug.Log("Spawning Rock: " + controller.currentRock.name);
        }
        if (GUILayout.Button("Destroy Current Rock"))
        {
            controller.DestroyRock();
        }
    }

    private void DrawCustomInspector()
    {

    }
}

