using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HitReceiver))]
public class QuestButtonController : MonoBehaviour
{
    private Color defaultColor = new Color32(255, 255, 255, 255);
    private Color completedColor = new Color32(128, 255, 128, 255);
    private Color incompleteColor = new Color32(255, 128, 128, 255);
    private Color hoverColor = new Color32(193, 193, 193, 255);

    private Color lastColor;

    private Image buttonImage;

    public QuestController parentQuestController;
    public QuestData questData;
    public int questIndex;

    private TextMeshProUGUI buttonText;

    private bool isSelected = false;

    public enum ButtonType
    {
        None,
        Details,
        Accept,
        Complete,
        Decline,
        Back
    }

    public ButtonType buttonType = ButtonType.None;

    // Use this for initialization
    void Awake ()
    {
        buttonImage = GetComponent<Image>();
        buttonText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        lastColor = defaultColor;
    }

    public void Select(bool selected)
    {
        isSelected = selected;
        if (questData.questType == QuestData.QuestType.Pay)
        {
            if (GameManager.manager.playerData.gold >= questData.requirements.goldRequired)
                buttonImage.color = completedColor;
            else
                buttonImage.color = incompleteColor;
        }
        else if (questData.questType == QuestData.QuestType.Talk)
        {
            buttonImage.color = completedColor;
        }
        else if (questData.questType == QuestData.QuestType.Fetch)
        {
            if (selected)
                buttonImage.color = lastColor = completedColor;
            else
                buttonImage.color = lastColor = incompleteColor;
        }
        else
            buttonImage.color = defaultColor;
    }

    public void SetButtonData(QuestController newParent,QuestData newData, bool isReturn)
    {
        questData = newData;
        parentQuestController = newParent;
        questIndex = questData.saveData.questID;
        if (isReturn)
            questIndex = -questIndex;
        buttonText.SetText(questData.questName);
        buttonType = ButtonType.Details;
    }

    public void SetButtonQuestController(QuestController newParent, int newIndex)
    {
        parentQuestController = newParent;
        questIndex = newIndex;
    }

    public void Hovering()
    {
        buttonImage.color = Color.Lerp(hoverColor, lastColor, 0.5f);
    }

    public void DoneHovering()
    {
        buttonImage.color = lastColor;
    }

    public void PressButton()
    {
        switch (buttonType)
        {
            case ButtonType.Details:
                parentQuestController.OpenQuestDetails(questIndex);
                break;

            case ButtonType.Accept:
                parentQuestController.AcceptQuest(questIndex);
                break;

            case ButtonType.Complete:
                parentQuestController.CompleteQuest(questIndex);
                break;

            case ButtonType.Decline:
                // decline
                break;

            case ButtonType.Back:
                parentQuestController.BackButtonAction();
                break;

            default:
                break;
        }
    }
}
