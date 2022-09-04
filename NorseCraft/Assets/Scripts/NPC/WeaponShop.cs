using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponShop : MonoBehaviour
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
    public GameObject SellStoreWindow;

    [Header("Weapon Spawn")]
    public Transform weaponSpawn;
    public Transform weaponDisplaySpawn;

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
        SellStore
    }

    private Window currentWindow;

    // Use this for initialization
    void Start ()
    {
        numberOfItems = items.Length;
        currentItemIndex = 0;

        vendorInventoryCount.SetText((currentItemIndex + 1) + "/" + numberOfItems);

        mainCanvas.SetActive(false);
        currentWindow = Window.Main;
        StartAnimation();
    }

    public void UpdateBuyCanvas(int inputOption)
    {
        if (currentItem != null)
            Destroy(currentItem);
        currentItemIndex += inputOption;
        if (currentItemIndex < 0)
            currentItemIndex = items.Length - 1;
        else if (currentItemIndex >= items.Length)
            currentItemIndex = 0;

        var item = (GameObject)Instantiate(
                items[currentItemIndex],
                weaponDisplaySpawn.position,
                weaponDisplaySpawn.rotation);

        currentItem = item;
        if (item.GetComponent<ItemController>() != null)
        {
            item.transform.parent = gameObject.transform;
            //item.transform.Rotate(item.GetComponent<ItemController>().itemRotation);

            item.GetComponent<Rigidbody>().useGravity = false;
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<OVRGrabbable>().enabled = false;

            itemNameText.SetText(item.GetComponent<ItemController>().data.name);
            itemPriceText.SetText(item.GetComponent<ItemController>().data.value + " gold");
        }
        vendorInventoryCount.SetText((currentItemIndex+1) + "/" + numberOfItems);
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
        SellStoreWindow.SetActive(false);
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

            case Window.SellStore:
                SellStoreWindow.SetActive(true);
                break;

            default:
                mainWindow.SetActive(true);
                break;
        }
    }

    public void BuyWeaponButton()
    {
        Instantiate(items[currentItemIndex], weaponSpawn.position, weaponSpawn.rotation);
    }

    public void OpenBuyWindowButtonAction()
    {
        ClearWindows();
        currentWindow = Window.BuyStore;
        StartAnimation();
    }

    public void OpenSellWindowButtonAction()
    {
        ClearWindows();
        currentWindow = Window.SellStore;
        StartAnimation();
    }

    public void BackButtonAction()
    {
        switch (currentWindow)
        {
            case Window.BuyStore:
                ClearWindows();
                currentWindow = Window.Main;
                StartAnimation();
                break;

            case Window.SellStore:
                ClearWindows();
                currentWindow = Window.Main;
                StartAnimation();
                break;

            default:
                break;
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
                    ActivateWindow(currentWindow);
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
            if(currentItem != null)
                Destroy(currentItem);
            mainCanvas.SetActive(false);
            currentItemIndex = 0;
            currentWindow = Window.Main;
            StartAnimation();
        }
    }
}
