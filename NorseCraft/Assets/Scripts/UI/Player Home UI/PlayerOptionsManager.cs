using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerOptionsManager : MonoBehaviour
{

    [Header("Windows")]
    public GameObject saveLoadWindow;
    public GameObject vrOptionsWindow;
    public GameObject gameDataWindow;


    [Header("Data Text")]
    public TextMeshProUGUI playerDataText;
    public TextMeshProUGUI playerShipDataText;

    [Header("Quest Text")]
    public TextMeshProUGUI factionNamesText;
    public TextMeshProUGUI factionRepsText;
    public TextMeshProUGUI factionQuestsText;

    [Header("Save/Load GameObjects")]
    public GameObject gameFileOne;
    public GameObject gameFileTwo;
    public GameObject gameFileThree;

	// Use this for initialization
	void Start ()
    {
        OpenWindow(MainMenuButtonController.ButtonType.SaveLoadWindow);
	}
	
	
    public void SaveGame(int index)
    {
        GameManager.manager.SaveGame(index);
        UpdateSaveLoadWindow();
    }

    public void LoadGame(int index)
    {
        GameManager.manager.LoadGame(index);
    }

    public void OpenWindow(MainMenuButtonController.ButtonType buttonType)
    {
        CloseAllWindows();
        switch(buttonType)
        {
            case MainMenuButtonController.ButtonType.SaveLoadWindow:
                UpdateSaveLoadWindow();
                saveLoadWindow.SetActive(true);
                break;

            case MainMenuButtonController.ButtonType.VrOptionsWindow:
                vrOptionsWindow.SetActive(true);
                break;

            case MainMenuButtonController.ButtonType.GameDataWindow:
                UpdateGameDataWindow();
                gameDataWindow.SetActive(true);
                break;

            default:
                break;
        }
    }

    private void CloseAllWindows()
    {
        saveLoadWindow.SetActive(false);
        vrOptionsWindow.SetActive(false);
        gameDataWindow.SetActive(false);
    }

    /*
     * Update the canvas for the save/load window
     */
    public void UpdateSaveLoadWindow()
    {
        SaveData data = SaveSystem.LoadGameState(1);
        UpdateGameFile(1, data);

        data = SaveSystem.LoadGameState(2);
        UpdateGameFile(2, data);

        data = SaveSystem.LoadGameState(3);
        UpdateGameFile(3, data);
    }

    private void UpdateGameFile(int index, SaveData data)
    {
        switch (index)
        {
            case 1:
                if (data != null)
                {
                    gameFileOne.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText(data.playerData.name);
                    gameFileOne.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Level: " + data.playerData.level +
                                                                                                "\nExp: " + data.playerData.experience +
                                                                                                "\nGold: " + data.playerData.gold);
                }
                else
                {
                    gameFileOne.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("No Save File");
                    gameFileOne.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Level: \nExp: \nGold: ");
                }
                break;

            case 2:
                if (data != null)
                {
                    gameFileTwo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText(data.playerData.name);
                    gameFileTwo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Level: " + data.playerData.level +
                                                                                                "\nExp: " + data.playerData.experience +
                                                                                                "\nGold: " + data.playerData.gold);
                }
                else
                {
                    gameFileTwo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("No Save File");
                    gameFileTwo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Level: \nExp: \nGold: ");
                }
                break;

            case 3:
                if (data != null)
                {
                    gameFileThree.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText(data.playerData.name);
                    gameFileThree.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Level: " + data.playerData.level +
                                                                                                "\nExp: " + data.playerData.experience +
                                                                                                "\nGold: " + data.playerData.gold);
                }
                else
                {
                    gameFileThree.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText("No Save File");
                    gameFileThree.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Level: \nExp: \nGold: ");
                }
                break;

            default:
                break;
        }
    }

    /*
     * Update the canvas for game data window
     */
    private void UpdateGameDataWindow()
    {
        string playerDataString = "";
        playerDataString += ("Name: " + GameManager.manager.playerData.name + "\n");
        playerDataString += ("Gold: " + GameManager.manager.playerData.gold + "\n");
        playerDataString += ("Level: " + GameManager.manager.playerData.level + "\n");
        playerDataString += ("Experience: " + GameManager.manager.playerData.experience);
        playerDataText.SetText(playerDataString);

        string playerShipDataString = "";
        playerShipDataString += ("Size: " + GameManager.manager.playerData.playerShipData.Size + "\n");
        playerShipDataString += ("Hull: " + GameManager.manager.playerData.playerShipData.HullType + "\n");
        playerShipDataString += ("Sails: " + GameManager.manager.playerData.playerShipData.SailType + "\n");
        playerShipDataString += ("Wheel: " + GameManager.manager.playerData.playerShipData.WheelType + "\n");
        playerShipDataString += ("Flag: " + GameManager.manager.playerData.playerShipData.FlagType + "\n");
        playerShipDataText.SetText(playerShipDataString);

        string factionNameString = "";
        factionNameString += ("Stormstad\n");
        factionNameString += ("Bjornheim\n");
        factionNamesText.SetText(factionNameString);

        string factionRepString = "";
        for(int i = 0; i < GameManager.manager.playerData.reputations.Length; i++)
        {
            factionRepString += (GameManager.manager.playerData.reputations[i] + "/" + GameData.MaxFactionReputation + "\n");
        }
        factionRepsText.SetText(factionRepString);

        int[] factionQuests = new int[GameManager.manager.playerData.reputations.Length];
        int[] completedQuests = new int[factionQuests.Length];
        foreach (QuestData quest in GameManager.manager.dataManager.questData)
        {
            switch(quest.faction)
            {
                case GameData.Faction.Stormstad:
                    factionQuests[0] += 1;
                    if (quest.saveData.isCompleted)
                        completedQuests[0] += 1;
                    break;

                case GameData.Faction.Bjornheim:
                    factionQuests[1] += 1;
                    if (quest.saveData.isCompleted)
                        completedQuests[1] += 1;
                    break;

                default:
                    break;
            }
        }

        string factionQuestString = "";
        for (int i = 0; i < factionQuests.Length; i++)
        {
            factionQuestString += (completedQuests[i] + "/" + factionQuests[i] + "\n");
        }
        factionQuestsText.SetText(factionQuestString);
        
    }
}
