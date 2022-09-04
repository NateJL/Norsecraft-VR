using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VendorController : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject mainCanvas;
    public RectTransform background;
    public TextMeshProUGUI vendorInventoryCount;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;

    [Header("Windows")]
    public GameObject mainWindow;
    public GameObject BuyStoreWindow;
    public GameObject infoWindow;

    [Header("Weapon Spawn")]
    public Transform itemSpawn;
    public Transform itemDisplaySpawn;

    [Header("Inventory")]
    public GameObject[] items;
    private int numberOfItems;
    private int currentItemIndex;
    private GameObject currentItem;

    private bool isAnimation;
    private float animTime = 2.0f;
    private float currentHeight;

    private float maxHeight = 180.0f;
    private float minHeight = 20.0f;
    private Vector2 defaultSize = new Vector2(120, 180);

    private enum Window
    {
        None,
        Main,
        BuyStore,
        Info
    }

    private Window currentWindow;

    private PoolManager objectPool;

    // Use this for initialization
    void Start()
    {
        objectPool = GameManager.manager.poolManager;
        numberOfItems = items.Length;
        currentItemIndex = 0;

        vendorInventoryCount.SetText((currentItemIndex + 1) + "/" + numberOfItems);

        defaultSize = background.sizeDelta;
        mainCanvas.SetActive(false);
        currentWindow = Window.Main;
        StartAnimation();
    }

    public void UpdateBuyCanvas(int inputOption)
    {
        if (currentItem != null)
        {
            objectPool.ReturnObject(currentItem);
            currentItem = null;
        }
        currentItemIndex += inputOption;
        if (currentItemIndex < 0)
            currentItemIndex = items.Length - 1;
        else if (currentItemIndex >= items.Length)
            currentItemIndex = 0;

        /*var item = (GameObject)Instantiate(
                items[currentItemIndex],
                itemDisplaySpawn.position,
                itemDisplaySpawn.rotation);*/
        var item = objectPool.SpawnWithParent(
                items[currentItemIndex].name,
                itemDisplaySpawn.position,
                itemDisplaySpawn.rotation,
                itemDisplaySpawn);

        currentItem = item;
        if (item.GetComponent<ItemController>() != null)
        {
            item.transform.localPosition = item.GetComponent<ItemController>().displayOffset;
            item.transform.localRotation = Quaternion.Euler(item.GetComponent<ItemController>().displayRotation);
            item.transform.localScale = item.GetComponent<ItemController>().displayScale;

            item.GetComponent<Rigidbody>().useGravity = false;
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<ItemController>().isGrabbable = false;

            itemNameText.SetText(item.GetComponent<ItemController>().data.name);
            itemPriceText.SetText(item.GetComponent<ItemController>().data.value + " gold");
        }
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
        BuyStoreWindow.SetActive(false);
        infoWindow.SetActive(false);
    }

    private void ActivateWindow(Window window)
    {
        switch (window)
        {
            case Window.Main:
                mainWindow.SetActive(true);
                break;

            case Window.BuyStore:
                BuyStoreWindow.SetActive(true);
                UpdateBuyCanvas(0);
                break;

            case Window.Info:
                infoWindow.SetActive(true);
                break;

            default:
                mainWindow.SetActive(true);
                break;
        }
    }

    public void BuyItemButtonAction()
    {
        objectPool.SpawnFromPool(items[currentItemIndex].name, itemSpawn.position, itemSpawn.rotation);
    }

    public void OpenBuyWindowButtonAction()
    {
        ClearWindows();
        currentWindow = Window.BuyStore;
        StartAnimation();
        StartCoroutine("OpenQuestCanvas");
    }

    public void OpenSellWindowButtonAction()
    {
        ClearWindows();
        currentWindow = Window.Info;
        StartAnimation();
        StartCoroutine("OpenQuestCanvas");
    }

    public void BackButtonAction()
    {
        switch (currentWindow)
        {
            case Window.Main:

                break;

            case Window.BuyStore:
                objectPool.ReturnObject(currentItem);
                currentItem = null;
                ClearWindows();
                currentWindow = Window.Main;
                StartAnimation();
                break;

            case Window.Info:
                ClearWindows();
                currentWindow = Window.Main;
                StartAnimation();
                break;

            default:
                break;
        }
        StartCoroutine("OpenQuestCanvas");
    }

    /*
     * Coroutine function to animate the opening of the canvas
     */
    IEnumerator OpenQuestCanvas()
    {
        StartAnimation();
        while (isAnimation)
        {
            float change = (maxHeight - minHeight) * Time.deltaTime;
            background.sizeDelta = new Vector2(background.sizeDelta.x, background.sizeDelta.y + change);

            if (background.sizeDelta.y >= maxHeight)
            {
                ActivateWindow(currentWindow);
                isAnimation = false;
                background.sizeDelta = defaultSize;
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mainCanvas.SetActive(true);
            StartCoroutine("OpenQuestCanvas");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (currentItem != null)
                objectPool.ReturnObject(currentItem);
            mainCanvas.SetActive(false);
            currentItemIndex = 0;
            currentWindow = Window.Main;
            StartAnimation();
        }
    }
}
