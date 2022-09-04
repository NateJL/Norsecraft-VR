using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameEditor : MonoBehaviour
{
    public TextMeshProUGUI nameDisplayText;
    public GameObject buttonContainer;

    private string currentName;
    public GameObject[] buttons;

    private bool CapsOn = false;

	// Use this for initialization
	void Start ()
    {
        ResetEditor();

        int buttonCount = buttonContainer.transform.childCount;
        buttons = new GameObject[buttonCount];
        for(int i = 0; i < buttonCount; i++)
        {
            buttons[i] = buttonContainer.transform.GetChild(i).gameObject;
        }
	}
	
	public void AddCharactertoName(string newChar)
    {
        if (currentName.Length < GameData.MaxNameCharacters)
            currentName += newChar;
        nameDisplayText.SetText(currentName);
    }

    public void RemoveCharacterFromName()
    {
        if (currentName.Length > 0)
            currentName = currentName.Substring(0, currentName.Length - 1);
        nameDisplayText.SetText(currentName);
    }

    public void SetNewPlayerName()
    {
        GameManager.manager.playerData.name = currentName;
    }

    public void ResetEditor()
    {
        currentName = GameManager.manager.playerData.name;
        nameDisplayText.SetText(currentName);
    }

    public void ToggleCase()
    {
        if(CapsOn)
        {
            CapsOn = false;
            for(int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<KeyboardButtonController>().buttonCharacter = buttons[i].GetComponent<KeyboardButtonController>().buttonCharacter.ToLower();
                buttons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(buttons[i].GetComponent<KeyboardButtonController>().buttonCharacter);
            }
        }
        else if(!CapsOn)
        {
            CapsOn = true;
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].GetComponent<KeyboardButtonController>().buttonCharacter = buttons[i].GetComponent<KeyboardButtonController>().buttonCharacter.ToUpper();
                buttons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(buttons[i].GetComponent<KeyboardButtonController>().buttonCharacter);
            }
        }
    }
}
