using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreeGenerator))]
public class TreeGeneratorEditor : Editor
{
    int toolbarIndex = 0;
    string[] toolbarStrings = { "Default", "Custom" };

    public override void OnInspectorGUI()
    {
        TreeGenerator controller = (TreeGenerator)target;

        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        switch(toolbarIndex)
        {
            case 0:
                DrawBasicInspector(controller);
                break;

            case 1:
                DrawCustomInspector();
                break;
        }
    }

    private void DrawBasicInspector(TreeGenerator controller)
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        if(GUILayout.Button("Spawn Tree"))
        {
            controller.SpawnTree(controller.biome);
        }
        if(GUILayout.Button("Destroy Current Tree"))
        {
            controller.DestroyTree();
        }
    }

    private void DrawCustomInspector()
    {

    }
}
