using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipWright : MonoBehaviour
{
    [Header("Player Ship")]
    public GameObject currentShip;

    [Header("Canvas References")]
    public GameObject mainCanvas;
    public RectTransform background;
    public TextMeshProUGUI playerShipDataText;
    public TextMeshProUGUI playerShipHullText;
    public TextMeshProUGUI playerShipWheelText;
    public TextMeshProUGUI playerShipSailsText;
    public TextMeshProUGUI playerShipFlagText;
    [Space(10)]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemValueText;
    public TextMeshProUGUI itemListCountText;

    [Header("Windows")]
    public GameObject mainWindow;
    public GameObject storeWindow;

    [Header("Store data")]
    public Transform shipDockTransform;
    public Transform itemDisplayTransform;
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
        Store
    }

    private Window currentWindow;

    private PoolManager objectPool;

    // Use this for initialization
    void Start ()
    {
        objectPool = GameManager.manager.poolManager;
        numberOfItems = items.Length;
        currentItemIndex = 0;

        itemListCountText.SetText((currentItemIndex + 1) + "/" + numberOfItems);

        defaultSize = background.sizeDelta;
        mainCanvas.SetActive(false);
        currentWindow = Window.Main;
        StartAnimation();
    }

    /*
     * Function to update the canvas owned by the shipwright character
     */
    public void UpdateCanvas(int inputOption)
    {
        currentShip = GameManager.manager.playerShip;
        ShipData data = new ShipData();
        ShipComponentManager shipComponents = new ShipComponentManager();
        string[] componentNames = new string[4];
        if(currentShip != null)
        {
            data = GameManager.manager.playerData.playerShipData;
            shipComponents = currentShip.GetComponent<ShipManager>().shipComponentManager;
            if (shipComponents.shipHull != null)
                componentNames[0] = shipComponents.shipHull.GetComponent<ShipComponent>().data.name;
            if (shipComponents.shipWheel != null)
                componentNames[1] = shipComponents.shipWheel.GetComponent<ShipComponent>().data.name;
            if (shipComponents.shipSail != null)
                componentNames[2] = shipComponents.shipSail.GetComponent<ShipComponent>().data.name;
            if (shipComponents.shipFlag != null)
                componentNames[3] = shipComponents.shipFlag.GetComponent<ShipComponent>().data.name;
        }



        playerShipDataText.SetText("Size: " + data.Size + 
                                   "\nDurability: " + data.durability +
                                   "\nID: " + data.id +
                                   "\nafloat: " + data.isFloating);

        playerShipHullText.SetText("Hull: " + componentNames[0]);
        playerShipWheelText.SetText("Steering: " + componentNames[1]);
        playerShipSailsText.SetText("Sails: " + componentNames[2]);
        playerShipFlagText.SetText("Flag: " + componentNames[3]);

        // Update Store Canvas
        if(storeWindow.activeSelf)
        {
            if(currentItem != null)
            {
                objectPool.ReturnObject(currentItem);
                currentItem = null;
            }
            currentItemIndex += inputOption;
            if (currentItemIndex < 0)
                currentItemIndex = items.Length - 1;
            else if (currentItemIndex >= items.Length)
                currentItemIndex = 0;

            GameObject item = objectPool.SpawnWithParent(
                items[currentItemIndex].name,
                itemDisplayTransform.position,
                itemDisplayTransform.rotation,
                itemDisplayTransform);

            currentItem = item;

            if(item.GetComponent<ShipComponent>() != null)
            {
                item.transform.localPosition = item.GetComponent<ShipComponent>().displayPositionOffset;
                item.transform.localRotation = Quaternion.Euler(item.GetComponent<ShipComponent>().displayRotationOffset);
                item.transform.localScale = item.GetComponent<ShipComponent>().displayScale;

                itemNameText.SetText(item.GetComponent<ShipComponent>().data.name);
                itemValueText.SetText(item.GetComponent<ShipComponent>().data.value + " gold");
            }

            itemListCountText.SetText((currentItemIndex + 1) + "/" + numberOfItems);
        }
    }

    /*
     * Function to reset all animation variables to start values
     */
    private void StartAnimation()
    {
        ClearWindows();
        isAnimation = true;
        background.sizeDelta = new Vector2(background.sizeDelta.x, minHeight);
    }

    /*
     * Function to clear all windows (active = false)
     */
    public void ClearWindows()
    {
        mainWindow.SetActive(false);
        storeWindow.SetActive(false);
    }

    /*
     * Function to activate a window according to current window mode
     */
    private void ActivateWindow(Window window)
    {
        switch(window)
        {
            case Window.Main:
                mainWindow.SetActive(true);
                break;

            case Window.Store:
                currentItemIndex = 0;
                storeWindow.SetActive(true);
                UpdateCanvas(0);
                break;

            default:
                mainWindow.SetActive(true);
                break;
        }
    }

    /*
    * Function called on store button action press to activate store window 
    */
    public void StoreButtonAction()
    {
        ClearWindows();
        currentWindow = Window.Store;
        StartAnimation();
        StartCoroutine("OpenQuestCanvas");
    }

    /*
     * Function called on back button action press
     */
    public void BackButtonAction()
    {
        switch(currentWindow)
        {
            case Window.Store:
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
     * Function to buy the current selected item and add to ship
     */
    public void BuyShipComponentAction()
    {
        if(true)
        {
            ShipComponentManager shipComponents = currentShip.GetComponent<ShipManager>().shipComponentManager;
            switch (currentItem.GetComponent<ShipComponent>().data.componentType)
            {
                case ShipComponentData.ShipComponentType.Hull:
                    if(shipComponents.shipHull != null)
                    {

                    }
                    break;

                case ShipComponentData.ShipComponentType.Wheel:
                    if(shipComponents.shipWheel != null)
                    {

                    }
                    break;

                case ShipComponentData.ShipComponentType.Sails:
                    if (shipComponents.shipSail != null)
                    {
                        shipComponents.LowerSails();
                        objectPool.ReturnObject(shipComponents.shipSail);
                    }
                    GameObject newSail = objectPool.SpawnWithParent(currentItem.name,
                                                    currentShip.GetComponent<ShipManager>().shipSailsTransform.position,
                                                    currentShip.GetComponent<ShipManager>().shipSailsTransform.rotation,
                                                    currentShip.GetComponent<ShipManager>().shipSailsTransform);
                    newSail.transform.localPosition = Vector3.zero;
                    break;

                case ShipComponentData.ShipComponentType.Flag:

                    break;

                case ShipComponentData.ShipComponentType.BoatHead:

                    break;

                default:
                    break;
            }
            currentShip.GetComponent<ShipManager>().FindComponentsInSeconds(0.5f);
        }
    }

    /*
     * Function to dock the player ship in the corresponding dock slip
     */
    public void DockPlayerShip()
    {
        if(currentShip != null && currentShip.GetComponent<LongShipController>() != null)
        {
            currentShip.GetComponent<LongShipController>().Dock(shipDockTransform);
        }
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
        if(other.gameObject.CompareTag("Player"))
        {
            UpdateCanvas(0);
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
