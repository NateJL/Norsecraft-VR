using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;

[CustomEditor(typeof(CraftingAreaController))]
public class CraftingEditor : Editor
{
    public List<bool> showCraftable;
    public int numberOfCraftables;

    public int removeResourceIndex = 0;
    public int removeCraftableIndex = 0;

    int toolbarIndex = 0;
    string[] toolbarStrings = { "Default", "Custom" , "Craftables" };

    private void OnEnable()
    {
        showCraftable = new List<bool>();
        numberOfCraftables = ((CraftingAreaController)target).craftables.Count;
        for (int i = 0; i < numberOfCraftables; i++)
        {
            showCraftable.Add(false);
        }
    }

    public override void OnInspectorGUI()
    {
        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);

        CraftingAreaController controller = (CraftingAreaController)target;

        if (toolbarIndex == 0)
        {
            base.OnInspectorGUI();
        }
        else if(toolbarIndex == 1)
        {
            controller.craftingCanvas = (GameObject)EditorGUILayout.ObjectField("Canvas", controller.craftingCanvas, typeof(GameObject), true);
            controller.smelterController = (GameObject)EditorGUILayout.ObjectField("Smelter", controller.smelterController, typeof(GameObject), true);
            controller.craftableSpawn = (Transform)EditorGUILayout.ObjectField("Item Spawn", controller.craftableSpawn, typeof(Transform), true);
            controller.itemDisplaySpawn = (Transform)EditorGUILayout.ObjectField("Item Display Spawn", controller.itemDisplaySpawn, typeof(Transform), true);

            controller.playerResourcesText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Player Resources Text", controller.playerResourcesText, typeof(TextMeshProUGUI), true);
            controller.craftableItemName = (TextMeshProUGUI)EditorGUILayout.ObjectField("Item Name Text", controller.craftableItemName, typeof(TextMeshProUGUI), true);
            controller.craftableItemResources = (TextMeshProUGUI)EditorGUILayout.ObjectField("Item Resources Text", controller.craftableItemResources, typeof(TextMeshProUGUI), true);

            controller.completionBar = (GameObject)EditorGUILayout.ObjectField("Completion Bar", controller.completionBar, typeof(GameObject), true);
            controller.completionText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Completion Text", controller.completionText, typeof(TextMeshProUGUI), true);

            controller.craftableButtonsParent = (GameObject)EditorGUILayout.ObjectField("Button Parent", controller.craftableButtonsParent, typeof(GameObject), true);
            controller.craftableButtonPrefab = (GameObject)EditorGUILayout.ObjectField("Button Prefab", controller.craftableButtonPrefab, typeof(GameObject), true);
            
        }
        else if(toolbarIndex == 2)
        {
            EditorGUILayout.LabelField("Craftables", EditorStyles.boldLabel);

            numberOfCraftables = controller.craftables.Count;
            if (numberOfCraftables > showCraftable.Count)
            {
                int difference = numberOfCraftables - showCraftable.Count;
                for (int i = 0; i < difference; i++)
                {
                    showCraftable.Add(false);
                }
            }

            DrawCraftables(controller);
        }

    }

    private void DrawActiveCraftable(CraftingAreaController controller)
    {
        EditorGUILayout.LabelField("Active Craftable", EditorStyles.boldLabel);
        EditorGUI.indentLevel += 2;
        if (controller.activeCraftable.itemPrefab != null)
        {
            controller.activeCraftable.itemPrefab = (GameObject)EditorGUILayout.ObjectField(controller.activeCraftable.itemPrefab, typeof(GameObject), true);
            if (controller.activeCraftable.itemPrefab.GetComponent<ItemController>() != null)
            {
                EditorGUILayout.LabelField("Item Name: " + controller.activeCraftable.itemPrefab.GetComponent<ItemController>().data.name);
                EditorGUILayout.LabelField("Item Value: " + controller.activeCraftable.itemPrefab.GetComponent<ItemController>().data.value.ToString() + " gold");
            }
            else
            {
                EditorGUILayout.LabelField("Item Name: " + controller.activeCraftable.itemPrefab.name);
                EditorGUILayout.LabelField("Item Value: " + "?? gold");
            }
            EditorGUILayout.Toggle("isReady", controller.activeCraftable.isReady);
            EditorGUILayout.LabelField("completed", ((int)(controller.activeCraftable.completion * 100f)) + " %");
            EditorGUILayout.LabelField("Total Resources", controller.activeCraftable.resourceCount + " %");

            for (int j = 0; j < controller.activeCraftable.resourcesRequired.Count; j++)
            {
                EditorGUILayout.LabelField(controller.activeCraftable.resourcesRequired[j].resourcePrefab.GetComponent<ItemController>().data.name +
                                            ": " + controller.activeCraftable.resourcesRequired[j].resourcesPlaced +
                                            "/" + controller.activeCraftable.resourcesRequired[j].resourceCount);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Item Name: ");
            EditorGUILayout.LabelField("Item Value: ");
            EditorGUILayout.Toggle("isReady", false);
            EditorGUILayout.LabelField("completed", "0 %");
        }
        EditorGUI.indentLevel -= 2;
    }

    /*
     * Draw craftables
     */
    private void DrawCraftables(CraftingAreaController controller)
    {
        for (int k = 0; k < (int)CraftableItem.CraftableType.NUM_CRAFTABLE_TYPES; k++)
        {
            EditorGUILayout.LabelField(((CraftableItem.CraftableType)k).ToString(), EditorStyles.boldLabel);
            for (int i = 0; i < controller.craftables.Count; i++)
            {
                if (controller.craftables[i].craftableType == ((CraftableItem.CraftableType)k))
                {
                    if (controller.craftables[i].itemPrefab != null)
                        showCraftable[i] = EditorGUILayout.Foldout(showCraftable[i], "(" + i + ")" + controller.craftables[i].itemPrefab.name, true, EditorStyles.foldout);
                    else
                        showCraftable[i] = EditorGUILayout.Foldout(showCraftable[i], "(" + i + ")new craftable", true, EditorStyles.foldout);

                    if (showCraftable[i])
                    {
                        EditorGUI.indentLevel += 2;
                        controller.craftables[i].itemPrefab = (GameObject)EditorGUILayout.ObjectField(controller.craftables[i].itemPrefab, typeof(GameObject), true);
                        controller.craftables[i].craftableType = (CraftableItem.CraftableType)EditorGUILayout.EnumPopup("Type: ", controller.craftables[i].craftableType);

                        if (controller.craftables[i].itemPrefab != null && controller.craftables[i].itemPrefab.GetComponent<ItemController>() != null)
                        {
                            EditorGUILayout.LabelField("Item Name: " + controller.craftables[i].itemPrefab.GetComponent<ItemController>().data.name);
                            EditorGUILayout.LabelField("Item Value: " + controller.craftables[i].itemPrefab.GetComponent<ItemController>().data.value.ToString() + " gold");
                        }
                        EditorGUILayout.LabelField("Resources Required", EditorStyles.boldLabel);
                        EditorGUI.indentLevel += 2;
                        for (int j = 0; j < controller.craftables[i].resourcesRequired.Count; j++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            controller.craftables[i].resourcesRequired[j].resourceCount = EditorGUILayout.IntField(controller.craftables[i].resourcesRequired[j].resourceCount, GUILayout.Width(100));
                            EditorGUI.indentLevel -= 2;
                            controller.craftables[i].resourcesRequired[j].resourcePrefab = (GameObject)EditorGUILayout.ObjectField(controller.craftables[i].resourcesRequired[j].resourcePrefab, typeof(GameObject), true);
                            EditorGUI.indentLevel += 2;
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUI.indentLevel -= 2;

                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("New Resource"))
                        {
                            controller.craftables[i].resourcesRequired.Add(new CraftingResource());
                        }
                        if (GUILayout.Button("Remove Resource At Index:"))
                        {
                            controller.craftables[i].resourcesRequired.RemoveAt(removeResourceIndex);
                        }
                        removeResourceIndex = EditorGUILayout.IntField(removeResourceIndex, GUILayout.Width(60));
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel -= 2;
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                    }
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Add Craftable"))
        {
            controller.craftables.Add(new CraftableItem());
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove Craftable At Index:"))
        {
            controller.craftables.RemoveAt(removeCraftableIndex);
        }
        removeCraftableIndex = EditorGUILayout.IntField(removeCraftableIndex, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

}
