using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class ItemSpawnerButton : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);
    private Color selectedColor = new Color32(80, 225, 80, 255);

    private Image buttonImage;

    private ItemSpawner itemSpawnerParent;

    public enum ButtonType
    {
        None,
        LeftScroll,
        RightScroll,
        SpawnButton
    }

    public ButtonType buttonType = ButtonType.None;

    // Use this for initialization
    void Start () {
        buttonImage = GetComponent<Image>();
        itemSpawnerParent = transform.parent.parent.parent.GetComponent<ItemSpawner>();
        if (itemSpawnerParent == null)
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
            case ButtonType.LeftScroll:
                itemSpawnerParent.LeftButtonPress();
                break;

            case ButtonType.RightScroll:
                itemSpawnerParent.RightButtonPress();
                break;

            case ButtonType.SpawnButton:
                itemSpawnerParent.SpawnItemButton();
                break;

            default:
                break;
        }
    }
}
