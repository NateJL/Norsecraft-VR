using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class KeyboardButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);

    private Image buttonImage;

    private PlayerNameEditor nameEditor;

    public string buttonCharacter;

    public enum KeyboardButtonType
    {
        Character,
        Backspace,
        Space,
        Accept,
        Reset,
        ToggleCase
    }

    public KeyboardButtonType buttonType = KeyboardButtonType.Character;

    // Use this for initialization
    void Start ()
    {
        buttonImage = GetComponent<Image>();
        nameEditor = transform.parent.parent.GetComponent<PlayerNameEditor>();

        if (buttonType == KeyboardButtonType.Character)
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(buttonCharacter);
            nameEditor = transform.parent.parent.parent.GetComponent<PlayerNameEditor>();
        }

        if (nameEditor == null)
            Debug.LogError(gameObject.name + ": Failed to get parent NameEditor.");
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
            case KeyboardButtonType.Character:
                nameEditor.AddCharactertoName(buttonCharacter);
                break;

            case KeyboardButtonType.Space:
                nameEditor.AddCharactertoName(" ");
                break;

            case KeyboardButtonType.Backspace:
                nameEditor.RemoveCharacterFromName();
                break;

            case KeyboardButtonType.Accept:
                nameEditor.SetNewPlayerName();
                break;

            case KeyboardButtonType.Reset:
                nameEditor.ResetEditor();
                break;

            case KeyboardButtonType.ToggleCase:
                nameEditor.ToggleCase();
                break;

            default:
                break;
        }
    }
}
