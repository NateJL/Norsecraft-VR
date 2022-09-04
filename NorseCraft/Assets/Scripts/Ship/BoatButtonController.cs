using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);
    private Color selectedColor = new Color32(80, 225, 80, 255);

    private Image buttonImage;

    public enum ButtonType
    {
        None,
        DropSails,
        RaiseSails,
        turnLeft,
        turnRight,
        RaiseFrontSail,
        RaiseMidSail,
        RaiseBackSail,
        DropFrontSail,
        DropMidSail,
        DropBackSail,
        RaiseHeadSail,
        DropHeadSail
    }

    public ButtonType type = ButtonType.None;

    public enum ShipType
    {
        None,
        Small,
        Medium,
        Large
    }

    public ShipType shipType = ShipType.None;

    private SmallShipController smallShipController;
    private MediumShipController mediumShipController;
    private LargeShipController largeShipController;

    // Use this for initialization
    void Start ()
    {
        buttonImage = GetComponent<Image>();

        if (shipType == ShipType.Small)
        {
            smallShipController = transform.parent.parent.GetComponent<SmallShipController>();
        }
        else if(shipType == ShipType.Medium)
        {
            mediumShipController = transform.parent.parent.GetComponent<MediumShipController>();
            if (mediumShipController == null)
                mediumShipController = transform.parent.parent.parent.GetComponent<MediumShipController>();
            if (mediumShipController == null)
                Debug.LogError("Failed to get medium boat controller");
        }
        else if(shipType == ShipType.Large)
        {
            largeShipController = transform.parent.parent.GetComponent<LargeShipController>();
            if (largeShipController == null)
                largeShipController = transform.parent.parent.parent.GetComponent<LargeShipController>();
        }
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
        switch (type)
        {
            case ButtonType.DropSails:
                switch (shipType)
                {
                    case ShipType.Small:
                        smallShipController.sailManager.LowerSails();
                        break;

                    case ShipType.Medium:
                        mediumShipController.sailManager.LowerAllSails();
                        break;

                    case ShipType.Large:
                        largeShipController.sailManager.LowerAllSails();
                        break;

                    default:
                        break;
                }
                //boatController.sailController.LowerAllSails();
                break;

            case ButtonType.RaiseSails:
                switch(shipType)
                {
                    case ShipType.Small:
                        smallShipController.sailManager.RaiseSails();
                        break;

                    case ShipType.Medium:
                        mediumShipController.sailManager.RaiseAllSails();
                        break;

                    case ShipType.Large:
                        largeShipController.sailManager.RaiseAllSails();
                        break;

                    default:
                        break;
                }
                //boatController.sailController.RaiseAllSails();
                break;

            case ButtonType.RaiseFrontSail:                     // reaise front sail
                switch (shipType)
                {
                    case ShipType.Medium:
                        mediumShipController.sailManager.RaiseFrontSail();
                        break;

                    case ShipType.Large:
                        largeShipController.sailManager.RaiseFrontSail();
                        break;

                    default:
                        break;
                }
                //boatController.sailController.RaiseFrontSail();
                break;

            case ButtonType.RaiseMidSail:                       // raise mid sail
                switch (shipType)
                {
                    case ShipType.Medium:
                        mediumShipController.sailManager.RaiseMidSail();
                        break;

                    case ShipType.Large:
                        largeShipController.sailManager.RaiseMidSail();
                        break;

                    default:
                        break;
                }
                //boatController.sailController.RaiseMidSail();
                break;

            case ButtonType.RaiseBackSail:                      // raise back sail
                switch (shipType)
                {
                    case ShipType.Medium:
                        mediumShipController.sailManager.RaiseBackSail();
                        break;

                    case ShipType.Large:
                        largeShipController.sailManager.RaiseBackSail();
                        break;

                    default:
                        break;
                }
                //boatController.sailController.RaiseBackSail();
                break;

            case ButtonType.DropFrontSail:                      // drop front sail
                switch (shipType)
                {
                    case ShipType.Medium:
                        mediumShipController.sailManager.LowerFrontSail();
                        break;

                    case ShipType.Large:
                        largeShipController.sailManager.LowerFrontSail();
                        break;

                    default:
                        break;
                }
                //boatController.sailController.LowerFrontSail();
                break;

            case ButtonType.DropMidSail:                        // drop mid sail
                switch (shipType)
                {
                    case ShipType.Medium:
                        mediumShipController.sailManager.LowerMidSail();
                        break;

                    case ShipType.Large:
                        largeShipController.sailManager.LowerMidSail();
                        break;

                    default:
                        break;
                }
                //boatController.sailController.LowerMidSail();
                break;

            case ButtonType.DropBackSail:                       // drop back sail
                switch (shipType)
                {
                    case ShipType.Medium:
                        mediumShipController.sailManager.LowerBackSail();
                        break;

                    case ShipType.Large:
                        largeShipController.sailManager.LowerBackSail();
                        break;

                    default:
                        break;
                }
                //boatController.sailController.LowerBackSail();
                break;

            case ButtonType.RaiseHeadSail:
                switch (shipType)
                {
                    case ShipType.Large:
                        largeShipController.sailManager.RaiseHeadSail();
                        break;

                    default:
                        break;
                }
                break;

            case ButtonType.DropHeadSail:
                switch (shipType)
                {
                    case ShipType.Large:
                        largeShipController.sailManager.LowerHeadSail();
                        break;

                    default:
                        break;
                }
                break;

            default:
                break;
        }
    }
}
