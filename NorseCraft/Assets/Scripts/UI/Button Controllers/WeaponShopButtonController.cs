using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class WeaponShopButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);
    private Color selectedColor = new Color32(80, 225, 80, 255);

    private Image buttonImage;

    private WeaponShop weaponShopParent;

    public enum ButtonType
    {
        None,
        Back,
        BuyStoreWindow,
        SellStoreWindow,
        BuyWindowScrollLeft,
        BuyWindowScrollRight,
        BuyWeapon
    }

    public ButtonType buttonType = ButtonType.None;

    // Use this for initialization
    void Start ()
    {
        weaponShopParent = transform.parent.parent.parent.gameObject.GetComponent<WeaponShop>();
        if (weaponShopParent == null)
            weaponShopParent = transform.parent.parent.parent.parent.parent.gameObject.GetComponent<WeaponShop>();
        if(weaponShopParent == null)
            Debug.LogError("Failed to get parent WeaponShop");
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
                weaponShopParent.OpenBuyWindowButtonAction();
                break;

            case ButtonType.SellStoreWindow:
                weaponShopParent.OpenSellWindowButtonAction();
                break;

            case ButtonType.BuyWindowScrollLeft:
                weaponShopParent.UpdateBuyCanvas(-1);
                break;

            case ButtonType.BuyWindowScrollRight:
                weaponShopParent.UpdateBuyCanvas(1);
                break;

            case ButtonType.Back:
                weaponShopParent.BackButtonAction();
                break;

            case ButtonType.BuyWeapon:
                weaponShopParent.BuyWeaponButton();
                break;

            default:
                break;
        }
    }
}
