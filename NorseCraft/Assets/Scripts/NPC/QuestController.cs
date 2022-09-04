using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    public string characterName;
    public GameData.Faction faction;
    [Header("Quests")]
    public List<QuestData> quests;
    public List<int> questReturnIds;
    public List<QuestData> questReturns;

    [Header("Canvas References")]
    public GameObject mainCanvas;
    public RectTransform background;
    public GameObject mainWindow;
    public GameObject questWindowsParent;
    public GameObject questButtonsParent;

    [Header("UI Prefabs")]
    public GameObject acceptQuestWindowPrefab;
    public GameObject completeQuestWindowPrefab;
    public GameObject questButtonPrefab;
    [Space(10)]
    public Dictionary<int, ElementUI> questWindowCollection;
    public Dictionary<int, ElementUI> questButtonCollection;
    private int questCount;
    private int windowCount;

    [Space(10)]
    public List<GameObject> itemsInRange;

    private bool isAnimation;
    private float animTime = 5.0f;
    private float currentHeight;

    private float maxHeight = 180.0f;
    private float minHeight = 20.0f;
    private Vector2 defaultSize = new Vector2(128, 180);

    private PoolManager objectPool;

    // Use this for initialization
    void Start ()
    {
        objectPool = GameManager.manager.poolManager;
        itemsInRange = new List<GameObject>();
        questWindowCollection = new Dictionary<int, ElementUI>();
        questButtonCollection = new Dictionary<int, ElementUI>();

        mainCanvas.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().SetText(characterName);
        //UpdateQuestCanvas();

        /*if (faction == GameData.Faction.Stormstad)
            background.gameObject.GetComponent<Image>().color = GameManager.manager.gameData.stormstadColor;
        else if (faction == GameData.Faction.Bjornheim)
            background.gameObject.GetComponent<Image>().color = GameManager.manager.gameData.bjornheimColor;
            */
        mainCanvas.SetActive(false);
        StartAnimation();
    }

    public void UpdateQuestCanvas()
    {
        windowCount = questCount = 0;

        foreach (KeyValuePair<int, ElementUI> kvp in questWindowCollection)    // Return all windows to object pool
        {
            objectPool.ReturnObjectUI(kvp.Value);
        }
        foreach (KeyValuePair<int, ElementUI> kvp in questButtonCollection)    // Return all buttons to object pool
        {
            objectPool.ReturnObjectUI(kvp.Value);
        }

        float buttonOffset = -5.0f;
        questWindowCollection.Clear();      // Clear both window & button dictionaries
        questButtonCollection.Clear();
           
        // Loop through all quests starting at this NPC
        for (int i = 0; i < quests.Count; i++)
        {
            bool isReady = true;
            QuestData currentQuest = null;
            if (GameManager.manager.dataManager.questDataCollection.ContainsKey(quests[i].saveData.questID))        
                currentQuest = GameManager.manager.dataManager.questDataCollection[quests[i].saveData.questID];

            if (currentQuest != null)
            {
                // Loop through prerequisite quests
                if (currentQuest.requirements.prerequisiteQuests.Count > 0)
                {
                    for (int j = 0; j < currentQuest.requirements.prerequisiteQuests.Count; j++)
                    {
                        if (GameManager.manager.dataManager.questDataCollection[currentQuest.requirements.prerequisiteQuests[j]].saveData.isCompleted)  // might need to check this
                            isReady = true;
                        else
                            isReady = false;
                    }
                }
                else
                    isReady = true;

                // if the quest is not completed and isReady (all prereq. quests are completed)
                if (!currentQuest.saveData.isCompleted && isReady)
                {
                    Vector3 offset = new Vector3(0, buttonOffset, 0);
                    ElementUI newButton = objectPool.SpawnObjectUI(questButtonPrefab.name, transform.position, transform.rotation, questButtonsParent.transform);        // spawn button from pool
                    ElementUI newWindow = objectPool.SpawnObjectUI(acceptQuestWindowPrefab.name, transform.position, transform.rotation, questWindowsParent.transform);  // spawn window from pool

                    QuestButtonController buttonController = newButton.element.GetComponent<QuestButtonController>();
                    QuestWindowController windowController = newWindow.element.GetComponent<QuestWindowController>();

                    if (buttonController == null)
                        Debug.LogError(gameObject.name + "Failed to get button controller for quest: " + i);
                    if (windowController == null)
                        Debug.LogError(gameObject.name + "Failed to get window controller for quest: " + i);

                    buttonController.SetButtonData(this, currentQuest, false);
                    windowController.SetWindowData(this, currentQuest, false, itemsInRange);

                    newButton.element.transform.localPosition = offset;
                    newButton.element.transform.localEulerAngles = new Vector3(0, 180, 0);
                    newButton.element.transform.localScale = new Vector3(1, 1, 1);
                    newWindow.element.transform.localPosition = Vector3.zero;
                    newWindow.element.transform.localEulerAngles = Vector3.zero;
                    newWindow.element.transform.localScale = new Vector3(1, 1, 1);
                    buttonOffset -= 20.0f;

                    questButtonCollection.Add(currentQuest.saveData.questID, newButton);
                    questWindowCollection.Add(currentQuest.saveData.questID, newWindow);
                    
                    questWindowCollection[currentQuest.saveData.questID].element.SetActive(false);
                    windowCount++;
                    questCount++;
                }
            }
        }

        // Loop through all quests being returned to this NPC
        questReturns.Clear();
        for (int i = 0; i < questReturnIds.Count; i++)
        {
            QuestData currentQuest = null;
            if (GameManager.manager.dataManager.questDataCollection.ContainsKey(questReturnIds[i]))
                currentQuest = GameManager.manager.dataManager.questDataCollection[questReturnIds[i]];
            if (currentQuest != null)
            {
                questReturns.Add(currentQuest);
                if (currentQuest.saveData.isAccepted && !currentQuest.saveData.isCompleted)
                {
                    Vector3 offset = new Vector3(0, buttonOffset, 0);
                    ElementUI newButton = objectPool.SpawnObjectUI(questButtonPrefab.name, transform.position, transform.rotation, questButtonsParent.transform);
                    ElementUI newWindow = objectPool.SpawnObjectUI(completeQuestWindowPrefab.name, transform.position, transform.rotation, questWindowsParent.transform);

                    QuestButtonController buttonController = newButton.element.GetComponent<QuestButtonController>();
                    QuestWindowController windowController = newWindow.element.GetComponent<QuestWindowController>();

                    if (buttonController == null)
                        Debug.Log("Failed to get button controller");
                    if (windowController == null)
                        Debug.Log("Failed to get window controller");

                    buttonController.SetButtonData(this, currentQuest, true);
                    buttonController.Select(CheckItemsInRange(currentQuest));
                    windowController.SetWindowData(this, currentQuest, true, itemsInRange);

                    newButton.element.transform.localPosition = offset;
                    newButton.element.transform.localEulerAngles = new Vector3(0, 180, 0);
                    newButton.element.transform.localScale = new Vector3(1, 1, 1);
                    newWindow.element.transform.localPosition = Vector3.zero;
                    newWindow.element.transform.localEulerAngles = Vector3.zero;
                    newWindow.element.transform.localScale = new Vector3(1, 1, 1);
                    buttonOffset -= 20.0f;

                    questButtonCollection.Add(-currentQuest.saveData.questID, newButton);
                    questWindowCollection.Add(-currentQuest.saveData.questID, newWindow);
                    
                    questWindowCollection[-currentQuest.saveData.questID].element.SetActive(false);
                    windowCount++;
                }
            }
        }
    }

    private void StartAnimation()
    {
        ClearWindows();
        isAnimation = true;
        background.sizeDelta = new Vector2(background.sizeDelta.x, minHeight);
    }

    public void AcceptQuest(int index)
    {
        QuestData currentQuest = GameManager.manager.dataManager.questDataCollection[index];
        if (!currentQuest.saveData.isAccepted && !currentQuest.saveData.isCompleted)
        {
            if (currentQuest.questType == QuestData.QuestType.Buy)
            {
                GameManager.manager.playerData.gold += currentQuest.requirements.goldGiven;
                currentQuest.saveData.isAccepted = true;
                GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Accepted quest: " + currentQuest.questName + "\n+" + currentQuest.requirements.goldGiven + " gold");
            }
            if (currentQuest.questType == QuestData.QuestType.Fetch)
            {
                GameManager.manager.playerData.gold += currentQuest.requirements.goldGiven;
                currentQuest.saveData.isAccepted = true;
                if(currentQuest.requirements.goldGiven > 0)
                    GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Accepted quest: " + currentQuest.questName + "\n+" + currentQuest.requirements.goldGiven + " gold");
                else
                    GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Accepted quest: " + currentQuest.questName);
            }
            else if(currentQuest.questType == QuestData.QuestType.Escort)
            {
                currentQuest.saveData.isAccepted = true;
                GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Accepted quest: " + currentQuest.questName);
            }
            else
            {
                currentQuest.saveData.isAccepted = true;
                GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Accepted quest: " + currentQuest.questName);
            }
        }
        else
        {
            if (currentQuest.saveData.isAccepted)
                GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.red, "Already on quest!");
            if (currentQuest.saveData.isCompleted)
                GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.red, "Already completed quest!");
        }
        UpdateQuestCanvas();
        StartCoroutine("OpenQuestCanvas");
    }

    /*
     * Function called when player is attempting to turn in a quest
     */
    public void CompleteQuest(int index)
    {
        // check for requirements
        QuestData currentQuest = GameManager.manager.dataManager.questDataCollection[index];
        if (currentQuest.saveData.isAccepted && !currentQuest.saveData.isCompleted)
        {
            bool completed = false;

            if (currentQuest.questType == QuestData.QuestType.Pay)
            {
                if (GameManager.manager.playerData.gold >= currentQuest.requirements.goldRequired)
                {
                    GameManager.manager.playerData.gold -= currentQuest.requirements.goldRequired;
                    completed = true;
                    GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Completed quest: " + currentQuest.questName);
                }
                else
                {
                    GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.red, "Not Enough Gold");
                }
            }

            else if (currentQuest.questType == QuestData.QuestType.Fetch)
            {
                int[] currentItems = new int[currentQuest.requirements.numberOfItemsRequired.Count];
                List<GameObject> itemList = new List<GameObject>();
                for (int i = 0; i < itemsInRange.Count; i++)
                {
                    for (int j = 0; j < currentQuest.requirements.itemsRequired.Count; j++)
                    {
                        if (itemsInRange[i].GetComponent<ItemController>().data.name == currentQuest.requirements.itemsRequired[j].GetComponent<ItemController>().data.name)
                        {
                            if (currentItems[j] < currentQuest.requirements.numberOfItemsRequired[j])
                            {
                                currentItems[j]++;
                                itemList.Add(itemsInRange[i]);
                            }
                        }
                    }
                }
                bool isCompleted = true;
                for (int i = 0; i < currentItems.Length; i++)
                {
                    if (currentItems[i] >= currentQuest.requirements.numberOfItemsRequired[i])
                    {
                        if (isCompleted)
                            isCompleted = true;
                    }
                    else
                        isCompleted = false;
                }
                if(isCompleted)
                {
                    completed = true;
                    for(int i = 0; i < itemList.Count; i++)
                    {
                        itemsInRange.Remove(itemList[i]);
                        objectPool.ReturnObject(itemList[i]);
                    }
                    GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Completed quest: " + currentQuest.questName);
                }
                else
                {
                    GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.red, "Incomplete: " + currentQuest.questName);
                }
            }

            else if (currentQuest.questType == QuestData.QuestType.Buy)
            {
                completed = true;
                GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Completed quest: " + currentQuest.questName);
            }

            else if(currentQuest.questType == QuestData.QuestType.Escort)
            {
                completed = true;
                GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Completed quest: " + currentQuest.questName);
            }

            else if (currentQuest.questType == QuestData.QuestType.Talk)
            {
                completed = true;
                GameManager.manager.player.GetComponent<PlayerManager>().SetMessage(Color.green, "Completed quest: " + currentQuest.questName);
            }


            if(completed)
            {
                currentQuest.saveData.isCompleted = true;
                GameManager.manager.CompleteQuest(currentQuest.reward.gold, currentQuest.reward.experience, currentQuest.reward.factionID, currentQuest.reward.reputation);
                if (currentQuest.isRepeatable)
                {
                    currentQuest.saveData.isAccepted = false;
                    currentQuest.saveData.isCompleted = false;
                }
            }
        }
        UpdateQuestCanvas();
        StartCoroutine("OpenQuestCanvas");
    }

    /*
     * Function called to display quest information when button is pressed
     */
    public void OpenQuestDetails(int index)
    {
        ClearWindows();
        questWindowsParent.SetActive(true);
        questWindowCollection[index].element.SetActive(true);
        //Debug.Log("opened details of quest: " + index);
    }

    public void BackButtonAction()
    {
        ClearWindows();
        mainWindow.SetActive(true);
        questButtonsParent.SetActive(true);
    }

    private void ClearWindows()
    {
        foreach (KeyValuePair<int, ElementUI> kvp in questWindowCollection)
        {
            kvp.Value.element.SetActive(false);
        }
        questWindowsParent.SetActive(false);
        questButtonsParent.SetActive(false);
        mainWindow.SetActive(false);
    }

    /*
     * Check all items in range of the Questgiver for quest-related items
     */
    public bool CheckItemsInRange(QuestData currentQuest)
    {
        int[] currentItems = new int[currentQuest.requirements.numberOfItemsRequired.Count];
        List<GameObject> itemList = new List<GameObject>();
        for (int i = 0; i < itemsInRange.Count; i++)
        {
            for (int j = 0; j < currentQuest.requirements.itemsRequired.Count; j++)
            {
                if (itemsInRange[i].GetComponent<ItemController>().data.name == currentQuest.requirements.itemsRequired[j].GetComponent<ItemController>().data.name)
                {
                    if (currentItems[j] < currentQuest.requirements.numberOfItemsRequired[j])
                    {
                        currentItems[j]++;
                        itemList.Add(itemsInRange[i]);
                    }
                }
            }
        }
        bool isCompleted = true;
        for (int i = 0; i < currentItems.Length; i++)
        {
            if (currentItems[i] >= currentQuest.requirements.numberOfItemsRequired[i])
            {
                if (isCompleted)
                    isCompleted = true;
            }
            else
                isCompleted = false;
        }
        return isCompleted;
    }

    /*
     * Coroutine function to animate the opening of the canvas
     */
    IEnumerator OpenQuestCanvas()
    {
        StartAnimation();
        while(isAnimation)
        {
            float change = (maxHeight - minHeight) * Time.deltaTime;
            background.sizeDelta = new Vector2(background.sizeDelta.x, background.sizeDelta.y + change);

            if (background.sizeDelta.y >= maxHeight)
            {
                BackButtonAction();
                isAnimation = false;
                background.sizeDelta = defaultSize;
            }
            yield return null;
        }
    }

    /*
     * Add item to list of items in range
     */
    public void AddItemToCollection(GameObject newItem)
    {
        itemsInRange.Add(newItem);
        GameObject[] itemArray = itemsInRange.ToArray();
        for (int i = 0; i < itemArray.Length - 1; i++)              // Add item and check for duplicates
        {
            for (int j = i + 1; j < itemArray.Length; j++)
            {
                if (itemArray[i].GetInstanceID() == itemArray[j].GetInstanceID())
                    itemsInRange.Remove(itemArray[j]);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mainCanvas.SetActive(true);
            StartCoroutine("OpenQuestCanvas");
            UpdateQuestCanvas();
        }
        else
        {
            ItemController item = other.gameObject.GetComponent<ItemController>();
            if (item == null)
            {
                if(other.transform.parent != null)
                    item = other.transform.parent.gameObject.GetComponent<ItemController>();
                if(item != null)
                {
                    AddItemToCollection(other.transform.parent.gameObject);
                }
            }
            else
            {
                AddItemToCollection(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            mainCanvas.SetActive(false);
            StartAnimation();
        }
        else
        {
            ItemController item = other.gameObject.GetComponent<ItemController>();
            if (item == null)
            {
                if(other.transform.parent != null)
                    item = other.transform.parent.gameObject.GetComponent<ItemController>();
                if (item != null)
                {
                    itemsInRange.Remove(other.transform.parent.gameObject);
                }
            }
            else
            {
                itemsInRange.Remove(other.gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
    }
}
