using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleControllerUI : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);

    private Image buttonImage;
    private Toggle myToggle;

    private OVRPlayerController playerController;
    private MenuSceneController menuManager;

    public enum ToggleType
    {
        None,
        PlayerRotationSnap,
        PlayerLinearMovement,
        WorldSize
    };

    public ToggleType toggleType = ToggleType.None;
    public int worldSize = 0;

    // Use this for initialization
    void Awake()
    {
        buttonImage = transform.GetChild(0).GetComponent<Image>();
        myToggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        playerController = GameManager.manager.player.GetComponent<OVRPlayerController>();

        switch (toggleType)
        {
            case ToggleType.PlayerRotationSnap:
                myToggle.isOn = playerController.SnapRotation;
                break;

            case ToggleType.WorldSize:
                menuManager = transform.parent.parent.parent.GetComponent<MenuSceneController>();
                if (menuManager == null)
                    Debug.LogError(gameObject.name + ": Failed to get MenuSceneController.");
                else
                {
                    myToggle.isOn = false;
                }
                break;

            default:
                break;
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
        myToggle.isOn = !myToggle.isOn;
        switch(toggleType)
        {
            case ToggleType.PlayerRotationSnap:
                playerController.SnapRotation = !playerController.SnapRotation;
                break;

            case ToggleType.WorldSize:
                menuManager.SetNewGameWorldSize(worldSize);
                break;

            default:
                break;
        }
    }

    public void SetToggle(bool newValue)
    {
        myToggle.isOn = newValue;
    }
}
