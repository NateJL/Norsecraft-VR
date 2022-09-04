using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class MainMenuButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);

    private Image buttonImage;

    private PlayerOptionsManager menuManager;

    public enum ButtonType
    {
        None,
        SaveLoadWindow,
        VrOptionsWindow,
        GameDataWindow,
        SaveGame,
        LoadGame
    }

    public ButtonType buttonType = ButtonType.None;

    [Tooltip("Used by Load and Save buttons for corresponding game file.")]
    public int gameFileIndex = 0;

    // Use this for initialization
    void Start()
    {
        buttonImage = GetComponent<Image>();

        if (buttonType == ButtonType.SaveLoadWindow || buttonType == ButtonType.VrOptionsWindow || buttonType == ButtonType.GameDataWindow)
            menuManager = transform.parent.parent.GetComponent<PlayerOptionsManager>();
        else if (buttonType == ButtonType.SaveGame || buttonType == ButtonType.LoadGame)
            menuManager = transform.parent.parent.parent.GetComponent<PlayerOptionsManager>();

        if (menuManager == null)
            Debug.LogError(gameObject.name + ": Failed to get parent PlayerOptionsManager");
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
            case ButtonType.SaveLoadWindow:
            case ButtonType.VrOptionsWindow:
            case ButtonType.GameDataWindow:
                menuManager.OpenWindow(buttonType);
                break;

            case ButtonType.SaveGame:
                menuManager.SaveGame(gameFileIndex);
                break;

            case ButtonType.LoadGame:
                menuManager.LoadGame(gameFileIndex);
                break;

            default:
                break;
        }
    }
}
