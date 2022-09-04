using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class VendorButtonController : MonoBehaviour
{

    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);
    private Color selectedColor = new Color32(80, 225, 80, 255);

    private Image buttonImage;

    private VendorController vendorParent;

    public enum ButtonType
    {
        None,
        Back,
        BuyStoreWindow,
        SellStoreWindow,
        BuyWindowScrollLeft,
        BuyWindowScrollRight,
        BuyItem
    }

    public ButtonType buttonType = ButtonType.None;

    // Use this for initialization
    void Start()
    {
        vendorParent = transform.parent.parent.parent.gameObject.GetComponent<VendorController>();
        if (vendorParent == null)
            vendorParent = transform.parent.parent.parent.parent.parent.gameObject.GetComponent<VendorController>();
        if (vendorParent == null)
            Debug.LogError("Failed to get parent Vendor");
        buttonImage = GetComponent<Image>();
    }

    public void Hovering()
    {
        buttonImage.color = hoverColor;
    }

    public void DoneHovering()
    {
        buttonImage.color = defaultColor;
    }

    public void PressButton()
    {
        switch (buttonType)
        {
            case ButtonType.BuyStoreWindow:
                vendorParent.OpenBuyWindowButtonAction();
                break;

            case ButtonType.SellStoreWindow:
                vendorParent.OpenSellWindowButtonAction();
                break;

            case ButtonType.BuyWindowScrollLeft:
                vendorParent.UpdateBuyCanvas(-1);
                break;

            case ButtonType.BuyWindowScrollRight:
                vendorParent.UpdateBuyCanvas(1);
                break;

            case ButtonType.Back:
                vendorParent.BackButtonAction();
                break;

            case ButtonType.BuyItem:
                vendorParent.BuyItemButtonAction();
                break;

            default:
                break;
        }
    }
}
