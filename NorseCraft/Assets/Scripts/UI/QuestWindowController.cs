using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestWindowController : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public QuestController parentQuestController;

    public QuestButtonController acceptButtonController;
    public QuestButtonController backButtonController;

    public QuestData questData;

    private string displayText;

	// Use this for initialization
	void Awake ()
    {
        titleText = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        descriptionText = transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        acceptButtonController = transform.GetChild(2).GetComponent<QuestButtonController>();
        backButtonController = transform.GetChild(3).GetComponent<QuestButtonController>();
	}

    public void SetWindowData(QuestController newParent, QuestData newData, bool isReturn, List<GameObject> itemsInRange)
    {
        questData = newData;
        parentQuestController = newParent;
        acceptButtonController.SetButtonQuestController(newParent, newData.saveData.questID);
        backButtonController.SetButtonQuestController(newParent, newData.saveData.questID);
        titleText.SetText(newData.questName);
        displayText = "";
        string newText = "";
        if (isReturn)
            displayText = newData.questResponse;
        else
            displayText = newData.questDescription;
        if(questData.questType == QuestData.QuestType.Pay)
        {
            newText += "\nRequired: " + questData.requirements.goldRequired + " Gold";
        }
        else if(questData.questType == QuestData.QuestType.Fetch)
        {
            // if the quest is already accepted (turn in window)
            if (isReturn)
            {
                // calculate items needed for quest
                int[] currentItems = new int[questData.requirements.numberOfItemsRequired.Count];
                
                // Loop through all items in range
                for (int i = 0; i < itemsInRange.Count; i++)
                {
                    // Loop through all required items
                    for (int j = 0; j < questData.requirements.itemsRequired.Count; j++)
                    {
                        // if there is a match
                        if (itemsInRange[i].GetComponent<ItemController>().data.name == questData.requirements.itemsRequired[j].GetComponent<ItemController>().data.name)
                        {
                            // Add matched item to item count 
                            currentItems[j]++;
                        }
                    }
                }

                // Loop through all required item groups (not individually)
                for (int i = 0; i < currentItems.Length; i++)
                {
                    // Append item name, number of current items, and number of required items
                    newText += ("\n" + questData.requirements.itemsRequired[i].GetComponent<ItemController>().data.name + ": " + currentItems[i] + "/" + questData.requirements.numberOfItemsRequired[i]);
                }
            }
            else
            {
                for (int i = 0; i < questData.requirements.itemsRequired.Count; i++)
                {
                    newText += ("\n" + questData.requirements.itemsRequired[i].GetComponent<ItemController>().data.name + ": x" + questData.requirements.numberOfItemsRequired[i]);
                }
            }
        }
        descriptionText.SetText(displayText + newText);
    }

    public void addItemsToText(string dataText)
    {
        descriptionText.SetText(displayText + dataText);
    }
	
}
