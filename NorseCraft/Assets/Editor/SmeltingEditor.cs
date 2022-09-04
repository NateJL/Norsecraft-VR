using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(SmelterController))]
public class SmeltingEditor : Editor
{
    public List<bool> showSmelts;
    public int numberOfSmeltables;

    public int removeSmeltableIndex = 0;

    public bool showDefault = false;

    private void OnEnable()
    {
        showSmelts = new List<bool>();
        numberOfSmeltables = ((SmelterController)target).smelterTable.Count;
        for (int i = 0; i < numberOfSmeltables; i++)
        {
            showSmelts.Add(false);
        }
    }

    public override void OnInspectorGUI()
    {
        showDefault = EditorGUILayout.Toggle("Default Inspector", showDefault);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (showDefault)
        {
            base.OnInspectorGUI();
        }
        else
        {
            SmelterController controller = (SmelterController)target;

            controller.temperatureText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Temp. Text", controller.temperatureText, typeof(TextMeshProUGUI), true);
            controller.barSpawnTransform = (Transform)EditorGUILayout.ObjectField("Bar Spawn", controller.barSpawnTransform, typeof(Transform), true);
            controller.oreSpawnTransform = (Transform)EditorGUILayout.ObjectField("Ore Spawn", controller.oreSpawnTransform, typeof(Transform), true);
            controller.minColor = EditorGUILayout.ColorField("Min Color", controller.minColor);
            controller.maxColor = EditorGUILayout.ColorField("Max Color", controller.maxColor);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            controller.minTemperature = EditorGUILayout.FloatField("Min Temp.", controller.minTemperature);
            controller.maxTemperature = EditorGUILayout.FloatField("Max Temp.", controller.maxTemperature);
            controller.temperatureGrowth = EditorGUILayout.FloatField("Temp. Growth Scale", controller.temperatureGrowth);
            controller.FuelTempGrowth = EditorGUILayout.FloatField("Fuel +Temp.", controller.FuelTempGrowth);

            EditorGUILayout.LabelField("current Temp: " + ((int)controller.currentTemperature).ToString());
            EditorGUILayout.LabelField("target Temp: " + ((int)controller.targetTemperature).ToString());
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Smeltable Table", EditorStyles.boldLabel);

            numberOfSmeltables = controller.smelterTable.Count;
            if (numberOfSmeltables > showSmelts.Count)
            {
                int difference = numberOfSmeltables - showSmelts.Count;
                for (int i = 0; i < difference; i++)
                {
                    showSmelts.Add(false);
                }
            }

            DrawSmeltables(controller);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Smeltable"))
            {
                controller.smelterTable.Add(new SmeltableLookupTable());
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove Smeltable At Index:"))
            {
                controller.smelterTable.RemoveAt(removeSmeltableIndex);
            }
            removeSmeltableIndex = EditorGUILayout.IntField(removeSmeltableIndex, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        }

    }

    public void DrawSmeltables(SmelterController controller)
    {
        for(int i = 0; i < controller.smelterTable.Count; i++)
        {
            if (controller.smelterTable[i].orePrefab != null)
                controller.smelterTable[i].oreTag = controller.smelterTable[i].orePrefab.name;
            if (controller.smelterTable[i].barPrefab != null)
                controller.smelterTable[i].barTag = controller.smelterTable[i].barPrefab.name;

            if (controller.smelterTable[i].barPrefab != null)
                showSmelts[i] = EditorGUILayout.Foldout(showSmelts[i], "(" + i + ")" + controller.smelterTable[i].barPrefab.name, true, EditorStyles.foldout);
            else if (controller.smelterTable[i].orePrefab != null)
                showSmelts[i] = EditorGUILayout.Foldout(showSmelts[i], "(" + i + ")" + controller.smelterTable[i].orePrefab.name, true, EditorStyles.foldout);
            else
                showSmelts[i] = EditorGUILayout.Foldout(showSmelts[i], "(" + i + ")smeltable", true, EditorStyles.foldout);

            if(showSmelts[i])
            {
                EditorGUI.indentLevel += 2;
                controller.smelterTable[i].meltingPoint = EditorGUILayout.IntField("Melting Point", controller.smelterTable[i].meltingPoint);
                controller.smelterTable[i].orePrefab = (GameObject)EditorGUILayout.ObjectField(controller.smelterTable[i].oreTag, controller.smelterTable[i].orePrefab, typeof(GameObject), true);

                controller.smelterTable[i].barPrefab = (GameObject)EditorGUILayout.ObjectField(controller.smelterTable[i].barTag, controller.smelterTable[i].barPrefab, typeof(GameObject), true);
                EditorGUI.indentLevel -= 2;
            }
        }
    }
}
