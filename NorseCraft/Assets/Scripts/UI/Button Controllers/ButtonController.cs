using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class ButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);
    private Color selectedColor = new Color32(80, 225, 80, 255);

    private Image buttonImage;

    private MenuController controller;
    private OVRPlayerController playerController;

    public enum ButtonType
    {
        None,
        Back,
        Accept,
        Options,
        Stats,
        SaveWindow,
        Exit,
        Toggle,
        Increment,
        Decrement,
        SaveGameState,
        LoadGameState
    }

    public enum IncrementType
    {
        None,
        PlayerRotationSnap
    }

    public ButtonType type = ButtonType.None;
    public IncrementType incrementType = IncrementType.None;
    public int gameFileIndex = 0;

	// Use this for initialization
	void Start ()
    {
        buttonImage = GetComponent<Image>();
        playerController = GameManager.manager.player.GetComponent<OVRPlayerController>();

        if(type == ButtonType.Increment || type == ButtonType.Decrement)
            controller = transform.parent.parent.parent.parent.GetComponent<MenuController>();
        else if(type == ButtonType.SaveGameState || type == ButtonType.LoadGameState)
            controller = transform.parent.parent.parent.GetComponent<MenuController>();
        else
            controller = transform.parent.parent.GetComponent<MenuController>();

        if (controller == null)
            Debug.LogError(gameObject.name + ": Failed to get parent MenuController.");

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
            case ButtonType.None:
                break;

            case ButtonType.Accept:
                // TODO
                break;

            case ButtonType.Options:
                controller.OptionsWindowButton();
                break;

            case ButtonType.Stats:
                controller.StatsWindowButton();
                break;

            case ButtonType.SaveWindow:
                controller.SaveLoadWindowButton();
                break;

            case ButtonType.Exit:
                // TODO: exit game
                Debug.Log("Exit game button");
                break;

            case ButtonType.Toggle:
                // TODO: toggle button
                Debug.Log("Toggle");
                break;

            case ButtonType.Back:
                controller.BackButtonAction();
                break;

            case ButtonType.Increment:
                if(incrementType == IncrementType.PlayerRotationSnap)
                {
                    playerController.RotationRatchet = playerController.RotationRatchet + 5;
                }
                break;

            case ButtonType.Decrement:
                if (incrementType == IncrementType.PlayerRotationSnap)
                {
                    playerController.RotationRatchet = playerController.RotationRatchet - 5;
                }
                break;

            case ButtonType.SaveGameState:
                controller.SaveGameStateButtonAction(gameFileIndex);
                break;

            case ButtonType.LoadGameState:
                controller.LoadGameStateButtonAction(gameFileIndex);
                break;

            default:
                break;
        }
    }

}
