using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fence : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject mainCanvas;
    public RectTransform background;
    public GameObject sellWindow;
    public TextMeshProUGUI itemListText;
    public TextMeshProUGUI valueText;

    private List<GameObject> currentItems;

    private int totalSellValue;

    private bool isAnimation;
    private float animTime = 2.0f;
    private float currentHeight;

    private float maxHeight = 180.0f;
    private float minHeight = 20.0f;
    private Vector2 defaultSize = new Vector2(120, 180);

    // Use this for initialization
    void Start ()
    {
        currentItems = new List<GameObject>();
        totalSellValue = 0;

        itemListText.SetText("no items");
        valueText.SetText(totalSellValue + " gold");
        mainCanvas.SetActive(false);
        StartAnimation();
	}

    private void StartAnimation()
    {
        sellWindow.SetActive(false);
        isAnimation = true;
        background.sizeDelta = new Vector2(background.sizeDelta.x, minHeight);
    }

    public void SellButtonAction()
    {
        GameObject[] itemsArray = currentItems.ToArray();
        for(int i = 0; i < itemsArray.Length; i++)
        {
            GameManager.manager.playerData.gold += itemsArray[i].GetComponent<ItemController>().data.value;
            currentItems.Remove(itemsArray[i]);
            Destroy(itemsArray[i]);
        }
        UpdateItems();
    }

    private void UpdateItems()
    {
        string currentItemsText = "";
        totalSellValue = 0;
        int itemCount = 0;
        foreach(GameObject item in currentItems)
        {
            ItemController controller = item.GetComponent<ItemController>();
            currentItemsText += (controller.data.name + " (" + controller.data.value + " gold)\n");
            totalSellValue += controller.data.value;
            itemCount++;
        }

        if (itemCount == 0)
            itemListText.SetText("no items");
        else
            itemListText.SetText(currentItemsText);

        valueText.SetText(totalSellValue + " gold");
    }

    public void AddItemToTable(GameObject newItem)
    {
        foreach(GameObject item in currentItems)
        {
            if (newItem.GetInstanceID() == item.GetInstanceID())
                return;
        }
        currentItems.Add(newItem);
        UpdateItems();
    }

    public void RemoveItemFromTable(GameObject removedItem)
    {
        GameObject foundItem = null;
        foreach (GameObject item in currentItems)
        {
            if (removedItem.GetInstanceID() == item.GetInstanceID())
            {
                foundItem = item;
            }
        }

        if (foundItem != null)
            currentItems.Remove(foundItem);
        UpdateItems();
    }




    private void OnTriggerStay(Collider other)
    {
        if (mainCanvas.activeSelf)
        {
            if (isAnimation)
            {
                float change = (maxHeight - minHeight) * Time.deltaTime;
                background.sizeDelta = new Vector2(background.sizeDelta.x, background.sizeDelta.y + change);

                if (background.sizeDelta.y >= maxHeight)
                {
                    sellWindow.SetActive(true);
                    isAnimation = false;
                    background.sizeDelta = defaultSize;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mainCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mainCanvas.SetActive(false);
            StartAnimation();
        }
    }
}
