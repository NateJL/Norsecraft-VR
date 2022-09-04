using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : Editor
{
    bool running = false;

    int toolbarIndex = 0;
    string[] toolbarStrings = { "Default", "Custom", "Components"};

    public bool showAllSettlements;
    public bool showSettlement;
    public int settlementIndex;

    bool firstPass = true;

    public List<ShowPrefabData> showPrefabData = null;

    private void OnEnable()
    {
        showPrefabData = new List<ShowPrefabData>();
    }

    public override void OnInspectorGUI()
    {
        TileManager controller = (TileManager)target;

        if (GameManager.manager != null)
        {
            running = true;
            if(firstPass)
            {
                firstPass = false;
                showSettlement = true;
                controller.settlementIndexMin = 2;
                controller.settlementIndexMax = 2;
            }
        }
        else
            running = false;

        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);
        if (!running)
            EditorGUILayout.LabelField("Not Running");
        else
            EditorGUILayout.LabelField("Running");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (toolbarIndex == 0)
            base.OnInspectorGUI();

        else if (toolbarIndex == 1)
        {
            controller.showAllSettlements = EditorGUILayout.Toggle("Show Settlement Centers", controller.showAllSettlements);
            controller.showSettlement = EditorGUILayout.Toggle("Show Settlement", controller.showSettlement);
            EditorGUILayout.LabelField((int)controller.settlementIndexMin + " - " + (int)controller.settlementIndexMax);
            EditorGUILayout.MinMaxSlider(ref controller.settlementIndexMin, ref controller.settlementIndexMax, 0, controller.settlements.Count);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Number of Settlements: " + controller.settlements.Count);
        }
        else if(toolbarIndex == 2)
        {
            EditorGUILayout.LabelField("Component Prefabs");
            if (GUILayout.Button("Refresh"))
            {
                showPrefabData = new List<ShowPrefabData>();
                for (int i = 0; i < controller.villageComponents.Count; i++)
                {
                    ShowPrefabData newData = new ShowPrefabData();
                    for (int j = 0; j < controller.villageComponents[i].subgroups.Count; j++)
                    {
                        newData.showSubgroups.Add(false);
                    }
                    showPrefabData.Add(newData);
                }
            }
            EditorGUILayout.Space();

            if (controller.villageComponents.Count > showPrefabData.Count)
            {
                int diff = controller.villageComponents.Count - showPrefabData.Count;
                for (int i = 0; i < diff; i++)
                    showPrefabData.Add(new ShowPrefabData());
            }

            for (int i = 0; i < controller.villageComponents.Count; i++)
            {
                showPrefabData[i].showRoot = EditorGUILayout.Foldout(showPrefabData[i].showRoot, controller.villageComponents[i].groupName);
                if (showPrefabData[i].showRoot)
                {
                    controller.villageComponents[i].groupName = EditorGUILayout.TextField("Group", controller.villageComponents[i].groupName);

                    if (controller.villageComponents[i].subgroups.Count > showPrefabData[i].showSubgroups.Count)
                    {
                        int diff = controller.villageComponents[i].subgroups.Count - showPrefabData[i].showSubgroups.Count;
                        for (int j = 0; j < diff; j++)
                            showPrefabData[i].showSubgroups.Add(false);
                    }

                    EditorGUI.indentLevel += 2;
                    for (int j = 0; j < controller.villageComponents[i].subgroups.Count; j++)
                    {
                        showPrefabData[i].showSubgroups[j] = EditorGUILayout.Foldout(showPrefabData[i].showSubgroups[j], controller.villageComponents[i].subgroups[j].subgroupName);
                        if (showPrefabData[i].showSubgroups[j])
                        {
                            controller.villageComponents[i].subgroups[j].subgroupName = EditorGUILayout.TextField("Subgroup", controller.villageComponents[i].subgroups[j].subgroupName);
                            EditorGUI.indentLevel += 2;
                            for (int k = 0; k < controller.villageComponents[i].subgroups[j].components.Count; k++)
                            {
                                controller.villageComponents[i].subgroups[j].components[k] = (GameObject)EditorGUILayout.ObjectField(controller.villageComponents[i].subgroups[j].components[k], typeof(GameObject), true);
                            }
                            EditorGUILayout.Space();
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Add Prefab"))
                            {
                                controller.villageComponents[i].subgroups[j].components.Add(null);
                            }
                            if (GUILayout.Button("Remove Prefab"))
                            {
                                controller.villageComponents[i].subgroups[j].components.RemoveAt(controller.villageComponents[i].subgroups[j].components.Count - 1);
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
                        controller.villageComponents[i].subgroups.Add(new TileComponentSubGroup());
                    }
                    if (GUILayout.Button("Remove Subgroup"))
                    {
                        controller.villageComponents[i].subgroups.RemoveAt(controller.villageComponents[i].subgroups.Count - 1);
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
                controller.villageComponents.Add(new TileComponentGroup());
            }
            if (GUILayout.Button("Remove Group"))
            {
                controller.villageComponents.RemoveAt(controller.villageComponents.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}


