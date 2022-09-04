using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class QuartermasterButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);

    private Image buttonImage;

    private AdmiraltyQuartermasterController quarterMasterParent;

    public enum ButtonType
    {
        None,
        LeftScroll,
        RightScroll,
        BuyItem
    }

    public ButtonType buttonType = ButtonType.None;

    // Use this for initialization
    void Start()
    {
        buttonImage = GetComponent<Image>();
        quarterMasterParent = transform.parent.parent.parent.GetComponent<AdmiraltyQuartermasterController>();
        if (quarterMasterParent == null)
            Debug.LogError("Failed to get parent Quartermaster");
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
            case ButtonType.LeftScroll:
                quarterMasterParent.LeftButtonPress();
                break;

            case ButtonType.RightScroll:
                quarterMasterParent.RightButtonPress();
                break;

            case ButtonType.BuyItem:
                quarterMasterParent.BuyItemAction();
                break;

            default:
                break;
        }
    }
}
