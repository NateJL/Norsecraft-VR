using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class MenuSceneButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);

    private Image buttonImage;

    private MenuSceneController menuManager;

    public enum MenuSceneButton
    {
        None,
        Back,
        NewGameWindow,
        LoadGameWindow,
        CreateNewGame
    }

    public MenuSceneButton buttonType = MenuSceneButton.None;

    [Tooltip("Used by Load and Save buttons for corresponding game file.")]
    public int gameFileIndex = 0;

    // Use this for initialization
    void Start()
    {
        buttonImage = GetComponent<Image>();

        menuManager = transform.parent.parent.parent.GetComponent<MenuSceneController>();

        if (menuManager == null)
            Debug.LogError(gameObject.name + ": Failed to get parent MenuSceneController");
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
            case MenuSceneButton.NewGameWindow:
                menuManager.OpenNewGameWindow();
                break;

            case MenuSceneButton.LoadGameWindow:
                menuManager.OpenLoadGameWindow();
                break;

            case MenuSceneButton.Back:
                menuManager.OpenMainWindow();
                break;

            case MenuSceneButton.CreateNewGame:
                menuManager.CreateNewGameWorld();
                break;

            default:
                break;
        }
    }
}
