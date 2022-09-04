using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftingAreaController : MonoBehaviour
{
    [Header("Object References")]
    public GameObject craftingCanvas;
    public GameObject smelterController;
    public Transform craftableSpawn;
    public Transform itemDisplaySpawn;

    [Space(20)]
    public TextMeshProUGUI playerResourcesText;
    public TextMeshProUGUI craftableItemName;
    public TextMeshProUGUI craftableItemResources;
    public GameObject completionBar;
    public TextMeshProUGUI completionText;
    public GameObject craftableButtonsParent;
    public GameObject craftableButtonPrefab;
    public GameObject categoryButtonsParent;
    public GameObject categoryButtonPrefab;
    public TextMeshProUGUI categoryLabel;

    [Space(20)]
    [Header("Current Craftable")]
    public CraftableItem activeCraftable;
    public CraftableItem.CraftableType activeCategory;

    [Space(20)]
    [Header("Craftables")]
    public List<CraftableItem> craftables;

    [HideInInspector] public GameObject selectedCraftableGameObject;
    [HideInInspector] public int selectedIndex;
    [HideInInspector] public List<GameObject> playerResources;
    [HideInInspector] public List<ElementUI> craftableButtons;
    [HideInInspector] public List<ElementUI> categoryButtons;

    private PoolManager objectPool;

	// Use this for initialization
	void Start ()
    {
        activeCategory = CraftableItem.CraftableType.Miscellaneous;
        objectPool = GameManager.manager.poolManager;
        selectedCraftableGameObject = null;
        playerResources = new List<GameObject>();
        craftableButtons = new List<ElementUI>();
        categoryButtons = new List<ElementUI>();
        craftableSpawn = transform.GetChild(1);

        SelectCategory((int)activeCategory);
        //UpdateCraftableList();
        //selectedIndex = 0;
        //SelectCraftable(selectedIndex);
        //UpdateCanvasText();
        DeactivateObjects();
    }

    public void SelectCategory(int index)
    {
        activeCategory = (CraftableItem.CraftableType)index;
        categoryLabel.SetText(activeCategory.ToString());
        for(int i = 0; i < craftables.Count; i++)
        {
            if(craftables[i].craftableType == activeCategory)
            {
                selectedIndex = i;
                break;
            }
        }
        UpdateCraftableList();
        SelectCraftable(selectedIndex);
        UpdateCanvasText();
    }

    /*
     * Select a current active craftable by index
     */
    public void SelectCraftable(int index)
    {
        if(selectedCraftableGameObject != null)
        {
            objectPool.ReturnObject(selectedCraftableGameObject);
            selectedCraftableGameObject = null;
        }

        for(int i = 0; i < craftableButtons.Count; i++)
        {
            craftableButtons[i].element.GetComponent<CraftingButtonController>().Select(false);
        }
        craftableButtons[index].element.GetComponent<CraftingButtonController>().Select(true);

        GameObject item = objectPool.SpawnWithParent(
                            craftables[index].itemPrefab.name,
                            itemDisplaySpawn.position,
                            itemDisplaySpawn.rotation,
                            itemDisplaySpawn);

        selectedCraftableGameObject = item;
        selectedIndex = index;
        activeCraftable = (CraftableItem)craftables[index].Clone();

        activeCraftable.isReady = true;
        for (int i = 0; i < activeCraftable.resourcesRequired.Count; i++)
        {
            for (int j = 0; j < playerResources.Count; j++)
            {
                if (playerResources[j].GetComponent<ItemController>() != null &&
                   activeCraftable.resourcesRequired[i].resourcePrefab.GetComponent<ItemController>() != null)
                {
                    if (playerResources[j].GetComponent<ItemController>().data.name.Equals(activeCraftable.resourcesRequired[i].resourcePrefab.GetComponent<ItemController>().data.name))
                    {
                        activeCraftable.resourcesRequired[i].resourcesPlaced += 1;
                    }
                    if (activeCraftable.resourcesRequired[i].resourcesPlaced < activeCraftable.resourcesRequired[i].resourceCount)
                    {
                        activeCraftable.isReady = false;
                    }
                }
            }
        }

        if (selectedCraftableGameObject.GetComponent<ItemController>() != null)
        {
            selectedCraftableGameObject.transform.localPosition = selectedCraftableGameObject.GetComponent<ItemController>().displayOffset;
            selectedCraftableGameObject.transform.localRotation = Quaternion.Euler(selectedCraftableGameObject.GetComponent<ItemController>().displayRotation);
            selectedCraftableGameObject.transform.localScale = selectedCraftableGameObject.GetComponent<ItemController>().displayScale;

            selectedCraftableGameObject.GetComponent<Rigidbody>().useGravity = false;
            selectedCraftableGameObject.GetComponent<Rigidbody>().isKinematic = true;
            selectedCraftableGameObject.GetComponent<ItemController>().isGrabbable = false;
        }

        UpdateCanvasText();
    }

    /*
     * Update the craftables button list
     */
    private void UpdateCraftableList()
    {
        // Clear craftable buttons
        for(int i = 0; i < craftableButtons.Count; i++)
        {
            craftableButtons[i].element.GetComponent<CraftingButtonController>().ClearButtonData();
            objectPool.ReturnObjectUI(craftableButtons[i]);
        }
        craftableButtons.Clear();

        // Clear category buttons
        for(int i = 0; i < categoryButtons.Count; i++)
        {
            categoryButtons[i].element.GetComponent<CraftingButtonController>().ClearButtonData();
            objectPool.ReturnObjectUI(categoryButtons[i]);
        }
        categoryButtons.Clear();

        // Loop through and spawn craftable category buttons
        float categoryButtonOffset = 0.0f;
        for(int i = 0; i < (int)CraftableItem.CraftableType.NUM_CRAFTABLE_TYPES; i++)
        {
            ElementUI newButton = objectPool.SpawnObjectUI(categoryButtonPrefab.name, transform.position, transform.rotation, categoryButtonsParent.transform);

            CraftingButtonController buttonController = newButton.element.GetComponent<CraftingButtonController>();
            if (buttonController == null)
                Debug.LogError("Failed to get button controller");

            buttonController.SetButtonData(this, ((CraftableItem.CraftableType)i).ToString()[0].ToString(), i);

            buttonController.Select(false);

            Vector3 offset = new Vector3(0, categoryButtonOffset, 0);
            newButton.element.transform.localPosition = offset;
            newButton.element.transform.localEulerAngles = new Vector3(0, 180, 0);
            newButton.element.transform.localScale = new Vector3(1, 1, 1);
            categoryButtonOffset -= 22.0f;

            categoryButtons.Add(newButton);
        }

        // Loop through and spawn craftable buttons
        for (int i = 0; i < craftables.Count; i++)
        {
            ElementUI newButton = objectPool.SpawnObjectUI(craftableButtonPrefab.name, transform.position, transform.rotation, craftableButtonsParent.transform);

            CraftingButtonController buttonController = newButton.element.GetComponent<CraftingButtonController>();
            if (buttonController == null)
                Debug.LogError("Failed to get button controller");

            if (craftables[i].itemPrefab.GetComponent<ItemController>() != null)
                buttonController.SetButtonData(this, craftables[i].itemPrefab.GetComponent<ItemController>().data.name, i);
            else
                buttonController.SetButtonData(this, craftables[i].itemPrefab.name, i);

            buttonController.Select(false);

            Vector3 offset = new Vector3(0, 0, 0);
            newButton.element.transform.localPosition = offset;
            newButton.element.transform.localEulerAngles = new Vector3(0, 180, 0);
            newButton.element.transform.localScale = new Vector3(1, 1, 1);
            //craftableButtonOffset -= 15.0f;
            newButton.element.SetActive(false);
            craftableButtons.Add(newButton);
        }

        float craftableButtonOffset = 0.0f;
        for(int i = 0; i < craftables.Count; i++)
        {
            if (craftables[i].craftableType == activeCategory)
            {
                craftableButtons[i].element.SetActive(true);
                Vector3 offset = new Vector3(0, craftableButtonOffset, 0);
                craftableButtons[i].element.transform.localPosition = offset;
                craftableButtonOffset -= 15.0f;
            }
        }
    }

    public void AttemptToCraft(GameObject hammer)
    {
        // if all the required materials are on the crafting table
        if(activeCraftable.isReady)
        {
            // loop through the required materials
            for (int i = 0; i < activeCraftable.resourcesRequired.Count; i++)
            {
                // loop through the materials placed on the crafting table
                for(int j = 0; j < playerResources.Count; j++)
                {
                    // if there is a match, then remove the material and increment the craftable status
                    if(playerResources[j].GetComponent<ItemController>().data.name.Equals(activeCraftable.resourcesRequired[i].resourcePrefab.GetComponent<ItemController>().data.name))
                    {
                        activeCraftable.completion += (1f / (float)activeCraftable.resourceCount);

                        objectPool.ReturnObject(playerResources[j]);
                        playerResources.Remove(playerResources[j]);
                        activeCraftable.resourcesRequired[i].resourceCount -= 1;
                        activeCraftable.resourcesRequired[i].resourcesPlaced -= 1;

                        hammer.GetComponent<ItemController>().UseItem();

                        // if the craftable is complete, spawn the item and reset the active craftable
                        if (activeCraftable.completion >= 1.0f)
                        {
                            Vector3 spawnPosition = craftableSpawn.position + craftables[selectedIndex].itemPrefab.GetComponent<ItemController>().spawnOffset;
                            Quaternion spawnRotation = craftableSpawn.rotation * Quaternion.Euler(craftables[selectedIndex].itemPrefab.GetComponent<ItemController>().spawnRotation);
                            objectPool.SpawnFromPool(craftables[selectedIndex].itemPrefab.name, spawnPosition, spawnRotation);

                            activeCraftable = (CraftableItem)craftables[selectedIndex].Clone();

                            SelectCraftable(selectedIndex);
                        }

                        UpdateCanvasText();
                        return;
                    }
                }
            }
        }

        UpdateCanvasText();
    }

    /*
     * Update data text on the canvas
     */
    public void UpdateCanvasText()
    {
        activeCraftable.CheckReady();

        if (activeCraftable.isReady)
            craftableButtons[selectedIndex].element.GetComponent<CraftingButtonController>().SelectedIsReady();
        else
            craftableButtons[selectedIndex].element.GetComponent<CraftingButtonController>().Select(true);

        // Update item name text
        craftableItemName.SetText(selectedCraftableGameObject.GetComponent<ItemController>().data.name);

        // Update item required resources text
        string newRequiredResources = "";
        for (int i = 0; i < activeCraftable.resourcesRequired.Count; i++)
        {
            if (activeCraftable.resourcesRequired[i].resourcePrefab.GetComponent<ItemController>() != null)
                newRequiredResources += (activeCraftable.resourcesRequired[i].resourcePrefab.GetComponent<ItemController>().data.name +
                                 "  (" + activeCraftable.resourcesRequired[i].resourcesPlaced + "/" + activeCraftable.resourcesRequired[i].resourceCount + ")\n");
            else
                newRequiredResources += (activeCraftable.resourcesRequired[i].resourcePrefab.name + "  x" + activeCraftable.resourcesRequired[i].resourceCount + "\n");
        }
        craftableItemResources.SetText(newRequiredResources);

        float completionBarWidth = 100f * activeCraftable.completion;
        completionBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, completionBarWidth);
        completionText.SetText((int)(activeCraftable.completion*100f) + "%");

        // Update player resources text
        string currentItemsText = "";
        int itemCount = 0;
        foreach (GameObject resource in playerResources)
        {
            ItemController controller = resource.GetComponent<ItemController>();
            currentItemsText += (controller.data.name + "\n");
            itemCount++;
        }

        if (itemCount == 0)
            playerResourcesText.SetText("no items");
        else
            playerResourcesText.SetText(currentItemsText);
    }

    /*
     * Add a resource from the table to the list of player resources
     */
    public void AddItemToTable(GameObject newItem)
    {
        foreach (GameObject resource in playerResources)
        {
            if (newItem.GetInstanceID() == resource.GetInstanceID())
                return;
        }
        playerResources.Add(newItem);

        for(int i = 0; i < activeCraftable.resourcesRequired.Count; i++)
        {
            if(newItem.GetComponent<ItemController>() != null &&
               activeCraftable.resourcesRequired[i].resourcePrefab.GetComponent<ItemController>() != null)
            {
                if(newItem.GetComponent<ItemController>().data.name.Equals(activeCraftable.resourcesRequired[i].resourcePrefab.GetComponent<ItemController>().data.name))
                {
                    activeCraftable.resourcesRequired[i].resourcesPlaced += 1;

                    break;
                }
            }
        }

        UpdateCanvasText();
    }

    /*
     * Remove resource from list of player resources
     */
    public void RemoveItemFromTable(GameObject removedItem)
    {
        GameObject foundItem = null;
        foreach (GameObject resource in playerResources)
        {
            if (removedItem.GetInstanceID() == resource.GetInstanceID())
            {
                foundItem = resource;
            }
        }

        if (foundItem != null)
        {
            for (int i = 0; i < activeCraftable.resourcesRequired.Count; i++)
            {
                if (foundItem.GetComponent<ItemController>() != null &&
                   activeCraftable.resourcesRequired[i].resourcePrefab.GetComponent<ItemController>() != null)
                {
                    if (foundItem.GetComponent<ItemController>().data.name.Equals(activeCraftable.resourcesRequired[i].resourcePrefab.GetComponent<ItemController>().data.name))
                    {
                        activeCraftable.resourcesRequired[i].resourcesPlaced -= 1;
                        break;
                    }
                }
            }
            playerResources.Remove(foundItem);
        }
        UpdateCanvasText();
    }

    /*
     * Deactivate crafting objects
     */
    private void DeactivateObjects()
    {
        if (selectedCraftableGameObject != null)
        {
            objectPool.ReturnObject(selectedCraftableGameObject);
            selectedCraftableGameObject = null;
        }
        craftingCanvas.SetActive(false);
        //smelterController.SetActive(false);
    }

    /*
     * Activate crafting objects
     */
    private void ActivateObjects()
    {
        SelectCraftable(selectedIndex);
        craftingCanvas.SetActive(true);
        //smelterController.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            ActivateObjects();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DeactivateObjects();
        }
    }

}

[System.Serializable]
public class CraftableItem : ICloneable
{
    public bool isReady;
    public float completion;
    public int resourceCount;
    public GameObject itemPrefab;
    public List<CraftingResource> resourcesRequired;

    public enum CraftableType
    {
        Miscellaneous = 0,
        Containers,
        Tools,
        Weapons,
        NUM_CRAFTABLE_TYPES
    }
    public CraftableType craftableType = CraftableType.Miscellaneous;

    /*
     * Default Constructor
     */
    public CraftableItem()
    {
        isReady = false;
        completion = 0.0f;
        resourceCount = 0;
        itemPrefab = null;
        resourcesRequired = new List<CraftingResource>();
    }

    public CraftableItem(GameObject newPrefab, List<CraftingResource> newResources)
    {
        isReady = false;
        itemPrefab = newPrefab;
        resourcesRequired = new List<CraftingResource>();
        completion = 0.0f;
        resourceCount = 0;
        for(int i = 0; i < newResources.Count; i++)
        {
            resourcesRequired.Add((CraftingResource)newResources[i].Clone());
            resourceCount += resourcesRequired[i].resourceCount;
        }
    }

    public void CheckReady()
    {
        isReady = false;
        for (int i = 0; i < resourcesRequired.Count; i++)
        {
            if (resourcesRequired[i].resourcesPlaced < resourcesRequired[i].resourceCount)
            {
                isReady = false;
                return;
            }
            else
                isReady = true;
        }
    }

    public object Clone()
    {
        CraftableItem clone = new CraftableItem(this.itemPrefab, this.resourcesRequired);
        return clone;
    }
}

[System.Serializable]
public class CraftingResource : ICloneable
{
    public GameObject resourcePrefab;
    public int resourceCount;
    public int resourcesPlaced;

    public CraftingResource()
    {
        resourcePrefab = null;
        resourceCount = 0;
        resourcesPlaced = 0;
    }

    public CraftingResource(GameObject prefab, int count, int placed)
    {
        resourcePrefab = prefab;
        resourceCount = count;
        resourcesPlaced = placed;
    }

    public object Clone()
    {
        CraftingResource clone = new CraftingResource(this.resourcePrefab, this.resourceCount, this.resourcesPlaced);
        return clone;
    }

    public bool IsReady()
    {
        if (resourcesPlaced >= resourceCount)
            return true;
        else
            return false;
    }
}
