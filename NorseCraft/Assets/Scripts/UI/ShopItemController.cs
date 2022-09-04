using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemController : MonoBehaviour
{
    private Image buttonImage;

    public GameObject shopParent;

    [Header("Item Data")]
    public GameObject itemPrefab;
    [Space(10)]
    public string itemName;
    public int price;
    public bool isSelected;

    [Header("Canvas References")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemValueText;

    // Use this for initialization
    void Start ()
    {
        shopParent = transform.parent.parent.parent.parent.gameObject;
        buttonImage = GetComponent<Image>();

        var item = (GameObject)Instantiate(
                itemPrefab,
                transform.position,
                transform.rotation);
        ///item.transform.Translate(itemOffset);
        //item.transform.Rotate(itemRotation);
        //item.transform.parent = gameObject.transform;
        
        if(item.GetComponent<ShipComponent>() != null)
        {
            item.transform.Rotate(item.GetComponent<ShipComponent>().displayRotationOffset);
            item.transform.localScale = item.GetComponent<ShipComponent>().displayScale;
            item.transform.parent = gameObject.transform;
            item.transform.localPosition = item.GetComponent<ShipComponent>().displayPositionOffset;

            itemName = item.GetComponent<ShipComponent>().data.name;
            price = item.GetComponent<ShipComponent>().data.value;
        }
        else if(item.GetComponent<ItemController>() != null)
        {
            item.transform.parent = gameObject.transform;
            //item.transform.Rotate(item.GetComponent<ItemController>().itemRotation);

            item.GetComponent<Rigidbody>().useGravity = false;
            item.GetComponent<Rigidbody>().isKinematic = true;

            itemName = item.GetComponent<ItemController>().data.name;
            price = item.GetComponent<ItemController>().data.value;
        }
        else
        {
            Debug.Log("Not ship component");
        }
        
        //item.GetComponent<Rigidbody>().useGravity = false;
        //item.GetComponent<Rigidbody>().isKinematic = true;
        //item.GetComponent<OVRGrabbable>().enabled = false;

        itemNameText.SetText(itemName);
        itemValueText.SetText(price + " Gold");
    }
}
