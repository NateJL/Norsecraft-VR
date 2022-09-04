using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class CraftingButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 0);
    private Color selectedColor = new Color32(255, 255, 255, 119);
    private Color isReadyColor = new Color32(0, 255, 0, 64);
    private Color hoverColor = new Color32(119, 119, 119, 80);
    private Color darkHoverColor = new Color32(64, 64, 64, 80);

    private Color lastColor;

    private Image buttonImage;

    [HideInInspector] public CraftingAreaController parentCraftingController;
    [HideInInspector] public string craftableName;
    [HideInInspector] public int craftableIndex;

    private TextMeshProUGUI buttonText;

    private bool isSelected = false;

    public enum ButtonType
    {
        None,
        Craftable,
        CraftableCategory
    }

    public ButtonType buttonType = ButtonType.None;

    // Use this for initialization
    void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        lastColor = defaultColor;
    }

    public void Select(bool selected)
    {
        isSelected = selected;
        if (buttonType == ButtonType.Craftable)
        {
            if (isSelected)
                buttonImage.color = selectedColor;
            else
                buttonImage.color = defaultColor;
        }
        else if(buttonType == ButtonType.CraftableCategory)
        {
            if (isSelected)
                buttonImage.color = selectedColor;
            else
                buttonImage.color = hoverColor;
        }
    }

    public void SelectedIsReady()
    {
        if(isSelected)
        {
            buttonImage.color = isReadyColor;
        }
    }

    public void SetButtonData(CraftingAreaController newParent, string newName, int index)
    {
        parentCraftingController = newParent;
        craftableName = newName;
        craftableIndex = index;

        buttonText.SetText(craftableName);
    }

    public void ClearButtonData()
    {
        parentCraftingController = null;
        craftableIndex = 0;
        buttonImage.color = defaultColor;
    }

    public void Hovering()
    {
        if (buttonType == ButtonType.Craftable)
        {
            if (!isSelected)
                buttonImage.color = hoverColor;
        }
        else if(buttonType == ButtonType.CraftableCategory)
        {
            if (!isSelected)
                buttonImage.color = darkHoverColor;
        }
    }

    public void DoneHovering()
    {
        if (buttonType == ButtonType.Craftable)
        {
            if (!isSelected)
                buttonImage.color = defaultColor;
        }
        else if (buttonType == ButtonType.CraftableCategory)
        {
            if (!isSelected)
                buttonImage.color = hoverColor;
        }
    }

    public void PressButton()
    {
        switch (buttonType)
        {
            case ButtonType.Craftable:
                parentCraftingController.SelectCraftable(craftableIndex);
                break;

            case ButtonType.CraftableCategory:
                parentCraftingController.SelectCategory(craftableIndex);
                break;

            default:
                break;
        }
    }

}
