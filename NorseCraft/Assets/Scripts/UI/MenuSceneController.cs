using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneController : MonoBehaviour
{
    public GameObject mainWindow;
    public GameObject loadGameWindow;
    public GameObject newGameWindow;

    public ToggleControllerUI[] toggles;

    public NewGameData newGameData;

	// Use this for initialization
	void Start ()
    {
        newGameData = new NewGameData();
        SetNewGameWorldSize(8);
        OpenMainWindow();
	}
	
	public void OpenLoadGameWindow()
    {
        DeactivateWindows();
        loadGameWindow.SetActive(true);
    }

    public void OpenNewGameWindow()
    {
        DeactivateWindows();
        newGameWindow.SetActive(true);
    }

    public void OpenMainWindow()
    {
        DeactivateWindows();
        mainWindow.SetActive(true);
    }

    public void DeactivateWindows()
    {
        mainWindow.SetActive(false);
        loadGameWindow.SetActive(false);
        newGameWindow.SetActive(false);
    }

    public void SetNewGameWorldSize(int size)
    {
        newGameData.worldSize = size;
        for(int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].worldSize == size)
                toggles[i].SetToggle(true);
            else
                toggles[i].SetToggle(false);
        }
    }

    public void CreateNewGameWorld()
    {
        GameManager.manager.StartNewGame(newGameData);
    }
}

[System.Serializable]
public class NewGameData
{
    public static int[] worldSizes = { 8, 16, 32 };

    public string worldName;

    public int worldSize;

    public NewGameData()
    {
        worldName = "New World";
        worldSize = 8;
    }
}
