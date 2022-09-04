using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class ShipWrightButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);
    private Color selectedColor = new Color32(80, 225, 80, 255);

    private Image buttonImage;

    private ShipWright shipWrightParent;

    public enum ButtonType
    {
        None,
        Back,
        Accept,
        StoreButton,
        ScrollLeft,
        ScrollRight,
        Buy,
        DockShip
    }

    public ButtonType buttonType = ButtonType.None;

    // Use this for initialization
    void Start()
    {
        shipWrightParent = transform.parent.parent.parent.gameObject.GetComponent<ShipWright>();
        if (shipWrightParent == null)
            Debug.LogError("Failed to get parent ShipWright.");
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
            case ButtonType.Accept:
                
                break;

            case ButtonType.StoreButton:
                shipWrightParent.StoreButtonAction();
                break;

            case ButtonType.Back:
                shipWrightParent.BackButtonAction();
                break;

            case ButtonType.ScrollLeft:
                shipWrightParent.UpdateCanvas(-1);
                break;

            case ButtonType.ScrollRight:
                shipWrightParent.UpdateCanvas(1);
                break;

            case ButtonType.Buy:
                shipWrightParent.BuyShipComponentAction();
                break;

            case ButtonType.DockShip:
                shipWrightParent.DockPlayerShip();
                break;

            default:
                break;
        }
    }
}
