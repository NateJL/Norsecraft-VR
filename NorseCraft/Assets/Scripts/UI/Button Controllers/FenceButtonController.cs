using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class FenceButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);
    private Color selectedColor = new Color32(80, 225, 80, 255);

    private Image buttonImage;

    private Fence fenceParent;

    public enum ButtonType
    {
        None,
        Sell
    }

    public ButtonType buttonType = ButtonType.None;

    // Use this for initialization
    void Start ()
    {
        buttonImage = GetComponent<Image>();
        fenceParent = transform.parent.parent.parent.GetComponent<Fence>();
        if (fenceParent == null)
            Debug.LogError("Failed to get parent spawner");
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
            case ButtonType.Sell:
                fenceParent.SellButtonAction();
                break;

            default:
                break;
        }
    }
}
