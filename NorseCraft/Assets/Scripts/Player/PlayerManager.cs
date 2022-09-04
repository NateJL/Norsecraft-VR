using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Item Mounts")]
    public GameObject leftHipMount;
    public GameObject rightHipMount;
    public GameObject frontHipMount;

    [Header("Left Hand")]
    public GameObject leftHand;
    public GameObject leftHandItem;
    public HandController leftHandGrabber;

    [Header("Right Hand")]
    public GameObject rightHand;
    public GameObject rightHandItem;
    public HandController rightHandGrabber; 

    [Header("Player Body")]
    public Transform centerEyeTransform;
    public GameObject centerOfBody;

    [Header("Player UI Menu")]
    public GameObject playerMenu;

    public GameObject playerMessageCanvas;
    public TextMeshProUGUI messageText;
    private bool messageAnimation = true;
    private float messageAnimTime = 5.0f;
    private float messageDefaultAnimTime = 5.0f;
    private float timeSinceUpdate = 0.0f;

    // Use this for initialization
    void Start ()
    {
        playerMenu.SetActive(false);
        leftHandGrabber = leftHand.transform.GetChild(0).GetComponent<HandController>();
        leftHandGrabber.setParent(this);
        rightHandGrabber = rightHand.transform.GetChild(0).GetComponent<HandController>();
        rightHandGrabber.setParent(this);
    }
	
	// Update is called once per frame
	void Update ()
    {
        centerOfBody.transform.position = new Vector3(centerEyeTransform.position.x, centerOfBody.transform.position.y, centerEyeTransform.position.z);

        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            playerMenu.SetActive(!playerMenu.activeSelf);
        }

        CheckGrabs();

        //CheckRightMiddleTrigger();

        CheckLeftIndexTrigger();
        CheckRightIndexTrigger();

        CheckMessage();

        UpdatePlayerData();
    }

    private void UpdatePlayerData()
    {
        if(Time.time - timeSinceUpdate > 30.0f)
        {
            timeSinceUpdate = Time.time;
            Debug.Log("Updating data");
            if(GameManager.manager.playerData.hunger > 0)
                GameManager.manager.playerData.hunger -= 1;
            if (GameManager.manager.playerData.thirst > 0)
                GameManager.manager.playerData.thirst -= 2;
        }
    }

    /*
     * Tracks popup UI message time active
     */
    private void CheckMessage()
    {
        if(messageAnimation)
        {
            if(messageAnimTime > 0f)
            {
                messageAnimTime -= Time.deltaTime;
            }
            else
            {
                messageAnimation = false;
                messageAnimTime = 5.0f;
            }
        }
        else
        {
            playerMessageCanvas.SetActive(false);
        }
    }

    /*
     * Check grabs 
     */
    private void CheckGrabs()
    {
        CheckGrabTrigger(ref leftHandItem, ref leftHandGrabber, OVRInput.Get(OVRInput.Button.PrimaryHandTrigger));
        CheckGrabTrigger(ref rightHandItem, ref rightHandGrabber, OVRInput.Get(OVRInput.Button.SecondaryHandTrigger));
    }

    /*
     * Function to check specified hand, hand item, and hand controller
     */
    private void CheckGrabTrigger(ref GameObject handItem, ref HandController handController, bool isGrabbing)
    {
        GameObject hand = handController.gameObject;
        handController.isGrabbing = isGrabbing;
        if (isGrabbing)
        {
            if (handController.grabbedItem == null && handController.closestItem != null)
            {
                handItem = handController.grabItem();
            }
            else if(handController.grabbedItem != null)
            {
                handController.UpdateGrab();
            }
        }
        else
        {
            handController.ReleaseGrab();
            handItem = null;
        }
    }

    /*
     * LEFT INDEX (trigger)
     */
    private void CheckLeftIndexTrigger()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (leftHandGrabber.grabbedItem != null)
            {
                if (leftHandGrabber.grabbedItem.GetComponent<ItemController>().itemType == ItemController.ItemType.Flintlock)
                {
                    leftHandGrabber.grabbedItem.GetComponent<GunController>().FireBullet();
                }
                else if(leftHandGrabber.grabbedItem.GetComponent<ItemController>().itemType == ItemController.ItemType.Tool)
                {
                    if(leftHandGrabber.grabbedItem.GetComponent<FishingPoleController>() != null)
                    {
                        FishingPoleController pole = leftHandGrabber.grabbedItem.GetComponent<FishingPoleController>();
                        if (!pole.casting)
                            pole.StartCasting();
                        if (!pole.casted && pole.casting)
                            leftHandGrabber.grabbedItem.GetComponent<FishingPoleController>().UpdatePoleVelocity();
                    }
                }
            }
        }
        else
        {
            if (leftHandGrabber.grabbedItem != null && leftHandGrabber.grabbedItem.GetComponent<ItemController>() != null)
            {
                if (leftHandGrabber.grabbedItem.GetComponent<ItemController>().itemType == ItemController.ItemType.Tool)
                {
                    if (leftHandGrabber.grabbedItem.GetComponent<FishingPoleController>() != null)
                    {
                        if(leftHandGrabber.grabbedItem.GetComponent<FishingPoleController>().casting)
                            leftHandGrabber.grabbedItem.GetComponent<FishingPoleController>().Cast();
                    }
                }
            }
        }
    }

    /*
     * RIGHT INDEX (trigger)
     */
    private void CheckRightIndexTrigger()
    {
        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            if (rightHandGrabber.grabbedItem != null)
            {
                if (rightHandGrabber.grabbedItem.GetComponent<ItemController>().itemType == ItemController.ItemType.Flintlock)
                {
                    rightHandGrabber.grabbedItem.GetComponent<GunController>().FireBullet();
                }
            }
        }
    }

    public void SetMessage(Color messageColor, string message)
    {
        playerMessageCanvas.SetActive(true);
        messageAnimation = true;
        messageAnimTime = 5.0f;
        messageText.color = messageColor;
        messageText.SetText(message);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Island"))
        {
            SetMessage(Color.gray, other.gameObject.GetComponent<IslandController>().data.islandName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Island"))
        {
            SetMessage(Color.gray, "Ocean");
        }
    }
}
