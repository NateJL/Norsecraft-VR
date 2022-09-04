using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AdmiraltyQuartermasterController : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject mainCanvas;
    public RectTransform background;
    public TextMeshProUGUI vendorInventoryCount;

    [Header("Windows")]
    public GameObject mainWindow;

    [Header("Weapon Spawn")]
    public Transform weaponSpawn;

    [Header("Inventory")]
    public GameObject inventoryParent;
    public GameObject[] items;
    private int numberOfItems;
    private int currentItemIndex;

    private bool isAnimation;
    private float animTime = 2.0f;
    private float currentHeight;

    private float maxHeight = 180.0f;
    private float minHeight = 20.0f;
    private Vector2 defaultSize = new Vector2(120, 180);

    // Use this for initialization
    void Start ()
    {
        numberOfItems = inventoryParent.transform.childCount;
        currentItemIndex = 0;
        items = new GameObject[numberOfItems];
        for (int i = 0; i < numberOfItems; ++i)
        {
            items[i] = inventoryParent.transform.GetChild(i).gameObject;
            items[i].SetActive(false);
        }
        if(items.Length > 0)
            items[currentItemIndex].SetActive(true);
        vendorInventoryCount.SetText((currentItemIndex + 1) + "/" + numberOfItems);

        mainCanvas.SetActive(false);
        StartAnimation();
    }

    public void UpdateBuyCanvas(int inputOption)
    {
        items[currentItemIndex].SetActive(false);
        currentItemIndex += inputOption;
        if (currentItemIndex < 0)
            currentItemIndex = items.Length - 1;
        else if (currentItemIndex >= items.Length)
            currentItemIndex = 0;
        items[currentItemIndex].SetActive(true);
        vendorInventoryCount.SetText((currentItemIndex + 1) + "/" + numberOfItems);
    }

    private void StartAnimation()
    {
        ClearWindows();
        isAnimation = true;
        background.sizeDelta = new Vector2(background.sizeDelta.x, minHeight);
    }

    public void ClearWindows()
    {
        mainWindow.SetActive(false);
    }

    public void LeftButtonPress()
    {
        UpdateBuyCanvas(-1);
    }

    public void RightButtonPress()
    {
        UpdateBuyCanvas(1);
    }

    public void BuyItemAction()
    {
        if(GameManager.manager.playerData.gold >= items[currentItemIndex].GetComponent<ShopItemController>().itemPrefab.GetComponent<ItemController>().data.value)
        {
            Instantiate(items[currentItemIndex].GetComponent<ShopItemController>().itemPrefab, weaponSpawn.position, weaponSpawn.rotation);
            GameManager.manager.playerData.gold -= items[currentItemIndex].GetComponent<ShopItemController>().itemPrefab.GetComponent<ItemController>().data.value;
            GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Bought: " + items[currentItemIndex].GetComponent<ShopItemController>().itemPrefab.GetComponent<ItemController>().data.name +
                                                                                            "\nFor: " + items[currentItemIndex].GetComponent<ShopItemController>().itemPrefab.GetComponent<ItemController>().data.value + " Gold");
        }
        else
        {
            GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.red, "Costs: " + items[currentItemIndex].GetComponent<ShopItemController>().itemPrefab.GetComponent<ItemController>().data.value + " Gold");
        }
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
                    mainWindow.SetActive(true);
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
