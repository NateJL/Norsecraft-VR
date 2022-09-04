using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("Windows")]
    public GameObject mainMenuWindow;
    public GameObject optionsWindow;
    public GameObject statsWindow;
    public GameObject saveGameWindow;

    [Header("Background")]
    public RectTransform background;

    [Header("Game Data")]
    public TextMeshProUGUI TimeDataText;

    [Header("Stats Data")]
    public TextMeshProUGUI playerDataText;
    public TextMeshProUGUI experienceText;
    public GameObject experienceBar;

    public TextMeshProUGUI questDataText;
    public TextMeshProUGUI reputationNameText;
    public TextMeshProUGUI reputationDataText;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDataText;

    [Header("Options Data")]
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI playerRotationSnapText;

    [Header("Save Data")]
    public GameObject gameFileOne;
    public GameObject gameFileTwo;
    public GameObject gameFileThree;

    private bool isAnimation;
    private float animTime = 2.0f;
    private float currentHeight;

    private float maxHeight = 180.0f;
    private float minHeight = 20.0f;

    private float deltaTime = 0.0f;

    private OVRPlayerController playerController;

    private enum Mode
    {
        MainMenu,
        Options,
        Stats,
        Save
    }

    private enum Save
    {
        None,
        One,
        Two,
        Three
    }

    private Mode windowMode;
    private Save saveMode;

    private Vector2 defaultSize = new Vector2(120, 180);
    private Vector2 optionsWindowSize = new Vector2(180, 180);
    private Vector2 statsWindowSize = new Vector2(270, 180);
    private Vector2 saveWindowSize = new Vector2(270, 180);

	// Use this for initialization
	void Start ()
    {
        playerController = GameManager.manager.player.GetComponent<OVRPlayerController>();
        windowMode = Mode.MainMenu;
        saveMode = Save.None;
        StartAnimation();
	}

    private void Update()
    {
        if(gameObject.activeSelf)
        {
            SetGameData();
        }

        if(isAnimation)
        {
            float change = (maxHeight - minHeight) * Time.deltaTime * 2.0f;

            if (windowMode == Mode.Options)
            {
                float xSize = background.sizeDelta.x + change;
                if (xSize > optionsWindowSize.x)
                    xSize = optionsWindowSize.x;
                background.sizeDelta = new Vector2(xSize, background.sizeDelta.y + change);
            }
            else if(windowMode == Mode.Stats)
            {
                float xSize = background.sizeDelta.x + change;
                if (xSize > statsWindowSize.x)
                    xSize = statsWindowSize.x;
                background.sizeDelta = new Vector2(xSize, background.sizeDelta.y + change);
            }
            else if(windowMode == Mode.Save)
            {
                float xSize = background.sizeDelta.x + change;
                if (xSize > saveWindowSize.x)
                    xSize = saveWindowSize.x;
                background.sizeDelta = new Vector2(xSize, background.sizeDelta.y + change);
            }
            else
                background.sizeDelta = new Vector2(defaultSize.x, background.sizeDelta.y + change);

            if (background.sizeDelta.y >= maxHeight)
            {
                ActivateWindow(windowMode);
                isAnimation = false;
                if (windowMode == Mode.Options)
                    background.sizeDelta = optionsWindowSize;
                else if (windowMode == Mode.Stats)
                    background.sizeDelta = statsWindowSize;
                else if (windowMode == Mode.Save)
                    background.sizeDelta = saveWindowSize;
                else
                    background.sizeDelta = defaultSize;
            }
        }
    }

    private void SetGameData()
    {
        deltaTime += Time.deltaTime;
        deltaTime /= 2.0f;
        fpsText.SetText("fps: " + (int)(1.0 / deltaTime));
        playerRotationSnapText.SetText(playerController.RotationRatchet.ToString());

        int hour = GameData.hour;
        int minute = GameData.minute;
        if (hour < 12)
        {
            if (hour >= 10 && minute >= 10)
                TimeDataText.SetText(hour + ":" + minute + " A.M.");
            else if (hour < 10 && minute >= 10)
                TimeDataText.SetText("0" + hour + ":" + minute + " A.M.");
            else if (hour >= 10 && minute < 10)
                TimeDataText.SetText(hour + ":0" + minute + " A.M.");
            else
                TimeDataText.SetText("0" + hour + ":0" + minute + " A.M.");
        }
        else
        {
            if(hour > 12)
                hour = hour % 12;
            if (hour >= 10 && minute >= 10)
                TimeDataText.SetText(hour + ":" + minute + " P.M.");
            else if (hour < 10 && minute >= 10)
                TimeDataText.SetText("0" + hour + ":" + minute + " P.M.");
            else if (hour >= 10 && minute < 10)
                TimeDataText.SetText(hour + ":0" + minute + " P.M.");
            else
                TimeDataText.SetText("0" + hour + ":0" + minute + " P.M.");
        }
    }

    private void StartAnimation()
    {
        ClearWindows();
        isAnimation = true;
        background.sizeDelta = new Vector2(background.sizeDelta.x, minHeight);
    }

    /*
     * Function to activate the specified window
     */
    private void ActivateWindow(Mode window)
    {
        switch (window)
        {
            case Mode.MainMenu:
                mainMenuWindow.SetActive(true);
                break;

            case Mode.Options:
                optionsWindow.SetActive(true);
                break;

            case Mode.Stats:
                statsWindow.SetActive(true);
                DrawStatsData();
                break;

            case Mode.Save:
                saveGameWindow.SetActive(true);
                DrawSaveWindowData();
                break;

            default:
                mainMenuWindow.SetActive(true);
                break;
        }
    }

    /*
     * Function to clear all windows
     */
    public void ClearWindows()
    {
        mainMenuWindow.SetActive(false);
        optionsWindow.SetActive(false);
        statsWindow.SetActive(false);
        saveGameWindow.SetActive(false);
    }

    /*
     * Function called when a back button is pressed
     */
    public void BackButtonAction()
    {
        ClearWindows();
        windowMode = Mode.MainMenu;
        StartAnimation();
    }

    public void SaveGameStateButtonAction(int index)
    {
        Debug.Log("Saving Game in File: " + index);
        if (index > 0 && index <= 3)
        {
            GameManager.manager.SaveGame(index);
        }
        else
            Debug.Log("File must be saved index 1, 2, or 3. invalid: " + index);
    }

    public void LoadGameStateButtonAction(int index)
    {
        Debug.Log("Loading Game from File: " + index);
        if (index > 0 && index <= 3)
        {
            GameManager.manager.LoadGame(index);
        }
        else
            Debug.Log("File must be loaded index 1, 2, or 3. invalid: " + index);
    }

    /*
     * Function called when the save button is pressed
     */
    public void SaveLoadWindowButton()
    {
        ClearWindows();
        windowMode = Mode.Save;
        StartAnimation();
    }

    public void OptionsWindowButton()
    {
        ClearWindows();
        windowMode = Mode.Options;
        StartAnimation();
    }

    public void StatsWindowButton()
    {
        ClearWindows();
        windowMode = Mode.Stats;
        StartAnimation();
    }

    /*
     * Draw data to stats window
     */
    private void DrawStatsData()
    {
        string nameString = "";
        string dataString = "Name: " + GameManager.manager.playerData.name +
                                                                    "\nHealth: " + GameManager.manager.playerData.health +
                                                                    "\nGold: " + GameManager.manager.playerData.gold +
                                                                    "\n " + GameManager.manager.playerData.level;
        playerDataText.SetText(dataString);

        dataString = "";
        for(int i = 0; i < GameManager.manager.dataManager.questData.Length; i++)
        {
            if(GameManager.manager.dataManager.questData[i].saveData.isAccepted &&      // if accepted
              !GameManager.manager.dataManager.questData[i].saveData.isCompleted)       // and not completed
            {
                dataString += GameManager.manager.dataManager.questData[i].questName + "\n";
            }
        }
        questDataText.SetText(dataString);

        nameString = "";
        dataString = "";
        for(int i = 0; i < GameData.factionNames.Length; i++)
        {
            nameString += GameData.factionNames[i] + "\n";
            dataString += GameManager.manager.playerData.reputations[i] + "/" + GameData.MaxFactionReputation + "\n";
        }
        reputationNameText.SetText(nameString);
        reputationDataText.SetText(dataString);

        nameString = "";
        dataString = "";
        for(int i = 0; i < GameData.skillCount; i++)
        {
            nameString += GameData.craftingSkills[i] + "\n";
            dataString += GameManager.manager.playerData.skills[i] + "/" + GameData.maxSkill + "\n";
        }
        skillNameText.SetText(nameString);
        skillDataText.SetText(dataString);

        experienceText.GetComponent<TextMeshProUGUI>().SetText(GameManager.manager.playerData.experience + "/" + (GameManager.manager.playerData.level * 100));
        float experienceBarWidth = 80f * ((float)GameManager.manager.playerData.experience / (float)(GameManager.manager.playerData.level * 100));
        experienceBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, experienceBarWidth);
    }

    /*
     * Draw data for save/load window
     */
    private void DrawSaveWindowData()
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
                    gameFileOne.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Level:\t" + data.playerData.level +
                                                                                                "\nGold:\t\t" + data.playerData.gold);
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
                    gameFileTwo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Level:\t" + data.playerData.level +
                                                                                                "\nGold:\t\t" + data.playerData.gold);
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
                    gameFileThree.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Level:\t" + data.playerData.level +
                                                                                                "\nGold:\t\t" + data.playerData.gold);
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

    private void OnDisable()
    {
        windowMode = Mode.MainMenu;
        StartAnimation();
    }

}
