using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingGenerator))]
public class BuildingGeneratorEditor : Editor
{
    public List<ShowPrefabData> showPrefabData = null;

    int toolbarIndex = 0;
    string[] toolbarStrings = { "Values", "Prefabs" };

    private void OnEnable()
    {
        showPrefabData = new List<ShowPrefabData>();
    }

    public override void OnInspectorGUI()
    {
        BuildingGenerator controller = (BuildingGenerator)target;
        

        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        if (toolbarIndex == 0)
        {
            controller.buildingType = (BuildingGenerator.BuildingType)EditorGUILayout.EnumPopup("Building Type", controller.buildingType);
            controller.numberOfFloors = EditorGUILayout.IntField("Number of Floors", controller.numberOfFloors);
            EditorGUILayout.Space();

            controller.foundationParent =(Transform) EditorGUILayout.ObjectField("Foundation Parent", controller.foundationParent, typeof(Transform), true);
            controller.secondFloorParent = (Transform)EditorGUILayout.ObjectField("2nd Floor Parent", controller.secondFloorParent, typeof(Transform), true);
            controller.thirdFloorParent = (Transform)EditorGUILayout.ObjectField("3rd Floor Parent", controller.thirdFloorParent, typeof(Transform), true);
            controller.fourthFloorParent = (Transform)EditorGUILayout.ObjectField("4th Floor Parent", controller.fourthFloorParent, typeof(Transform), true);
            EditorGUILayout.Space();
            controller.currentFoundation = (GameObject)EditorGUILayout.ObjectField("Foundation Object", controller.currentFoundation, typeof(GameObject), true);
            controller.currentSecondFloor = (GameObject)EditorGUILayout.ObjectField("2nd Floor Object", controller.currentSecondFloor, typeof(GameObject), true);
            controller.currentThirdFloor = (GameObject)EditorGUILayout.ObjectField("3rd Floor Object", controller.currentThirdFloor, typeof(GameObject), true);
            controller.currentFourthFloor = (GameObject)EditorGUILayout.ObjectField("4th Floor Object", controller.currentFourthFloor, typeof(GameObject), true);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate Building"))
            {
                controller.GenerateBuilding();
            }
        }
        else if (toolbarIndex == 1)
        {
            EditorGUILayout.LabelField("Component Prefabs");
            if (GUILayout.Button("Refresh"))
            {
                showPrefabData = new List<ShowPrefabData>();
                for (int i = 0; i < controller.prefabData.groups.Count; i++)
                {
                    ShowPrefabData newData = new ShowPrefabData();
                    for (int j = 0; j < controller.prefabData.groups[i].subgroups.Count; j++)
                    {
                        newData.showSubgroups.Add(false);
                    }
                    showPrefabData.Add(newData);
                }
            }
            EditorGUILayout.Space();

            if (controller.prefabData.groups.Count > showPrefabData.Count)
            {
                int diff = controller.prefabData.groups.Count - showPrefabData.Count;
                for (int i = 0; i < diff; i++)
                    showPrefabData.Add(new ShowPrefabData());
            }

            for (int i = 0; i < controller.prefabData.groups.Count; i++)
            {
                showPrefabData[i].showRoot = EditorGUILayout.Foldout(showPrefabData[i].showRoot, controller.prefabData.groups[i].groupName);
                if (showPrefabData[i].showRoot)
                {
                    controller.prefabData.groups[i].groupName = EditorGUILayout.TextField("Group", controller.prefabData.groups[i].groupName);

                    if (controller.prefabData.groups[i].subgroups.Count > showPrefabData[i].showSubgroups.Count)
                    {
                        int diff = controller.prefabData.groups[i].subgroups.Count - showPrefabData[i].showSubgroups.Count;
                        for (int j = 0; j < diff; j++)
                            showPrefabData[i].showSubgroups.Add(false);
                    }

                    EditorGUI.indentLevel += 2;
                    for (int j = 0; j < controller.prefabData.groups[i].subgroups.Count; j++)
                    {
                        showPrefabData[i].showSubgroups[j] = EditorGUILayout.Foldout(showPrefabData[i].showSubgroups[j], controller.prefabData.groups[i].subgroups[j].subgroupName);
                        if (showPrefabData[i].showSubgroups[j])
                        {
                            controller.prefabData.groups[i].subgroups[j].subgroupName = EditorGUILayout.TextField("Subgroup", controller.prefabData.groups[i].subgroups[j].subgroupName);
                            EditorGUI.indentLevel += 2;
                            for (int k = 0; k < controller.prefabData.groups[i].subgroups[j].prefabs.Count; k++)
                            {
                                controller.prefabData.groups[i].subgroups[j].prefabs[k] = (GameObject)EditorGUILayout.ObjectField(controller.prefabData.groups[i].subgroups[j].prefabs[k], typeof(GameObject), true);
                            }
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Add Prefab"))
                            {
                                controller.prefabData.groups[i].subgroups[j].prefabs.Add(null);
                            }
                            if (GUILayout.Button("Remove Prefab"))
                            {
                                controller.prefabData.groups[i].subgroups[j].prefabs.RemoveAt(controller.prefabData.groups[i].subgroups[j].prefabs.Count - 1);
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
                        controller.prefabData.groups[i].subgroups.Add(new BuildingPrefabSubgroup());
                    }
                    if (GUILayout.Button("Remove Subgroup"))
                    {
                        controller.prefabData.groups[i].subgroups.RemoveAt(controller.prefabData.groups[i].subgroups.Count - 1);
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
                controller.prefabData.groups.Add(new BuildingPrefabGroup());
            }
            if (GUILayout.Button("Remove Group"))
            {
                controller.prefabData.groups.RemoveAt(controller.prefabData.groups.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

}
