using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class QuestEditorWindow : EditorWindow
{
    GameObject[] allQuestgivers;
    GameObject selectedObj;
    List<bool> showQuestgivers;
    int selectedQuestgiverIndex;

    QuestController currentQuestgiver;
    int selectedQuestIndex;
    
    QuestData currentQuest;
    List<bool> showCurrentQuests;

    List<QuestData> allQuests;
    List<QuestData> sortedAllQuests;
    Dictionary<int, QuestData> questDictionary;
    List<bool> showAllQuests;

    bool showSortedQuests = false;

    GUIStyle titleFont = new GUIStyle();
    GUIStyle sectionFont = new GUIStyle();
    FontStyle origFontStyle;

    [MenuItem("Tools/Quest Editor")]
    public static void ShowWindow()
    {
        GetWindow<QuestEditorWindow>("Quest Editor");
    }

    private void OnEnable()
    {
        origFontStyle = EditorStyles.label.fontStyle;
        titleFont.fontSize = 24;
        titleFont.alignment = TextAnchor.UpperCenter;
        titleFont.fontStyle = FontStyle.Bold;

        sectionFont.fontSize = 16;
        sectionFont.fontStyle = FontStyle.Bold;

        questDictionary = new Dictionary<int, QuestData>();
        showQuestgivers = new List<bool>();
        allQuests = new List<QuestData>();
        sortedAllQuests = new List<QuestData>();
        showAllQuests = new List<bool>();
        showCurrentQuests = new List<bool>();
        GetData();
    }

    private void GetData()
    {
        allQuests.Clear();
        questDictionary.Clear();

        selectedObj = Selection.activeGameObject;
        if (selectedObj != null)
        {
            if (selectedObj.GetComponent<QuestController>() != null)
            {
                currentQuestgiver = selectedObj.GetComponent<QuestController>();
                //currentQuests = currentQuestgiver.quests;
            }
        }

        allQuestgivers = GameObject.FindGameObjectsWithTag("Questgiver");
        if(allQuestgivers.Length != showQuestgivers.Count)
        {
            showQuestgivers.Clear();
            for (int i = 0; i < allQuestgivers.Length; i++)
            {
                showQuestgivers.Add(false);
            }
        }
        
        for(int i = 0; i < allQuestgivers.Length; i++)
        {
            for(int j = 0; j < allQuestgivers[i].GetComponent<QuestController>().quests.Count; j++)
            {
                allQuests.Add(allQuestgivers[i].GetComponent<QuestController>().quests[j]);
                questDictionary.Add(allQuestgivers[i].GetComponent<QuestController>().quests[j].saveData.questID, allQuestgivers[i].GetComponent<QuestController>().quests[j]);
            }
        }
        
        if (showSortedQuests)
        {
            allQuests.Sort(SortByQuestID);
        }

        if(showAllQuests.Count != allQuests.Count)
        {
            showAllQuests.Clear();
            for(int i = 0; i < allQuests.Count; i++)
            {
                showAllQuests.Add(false);
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        DrawQuestgiverList();

        EditorGUILayout.BeginVertical();
        DrawSelectedQuestgiver();
        EditorGUILayout.EndVertical();

        DrawAllQuests();
        EditorGUILayout.EndHorizontal();

    }

    private void DrawAllQuests()
    {
        float Width = position.width * 0.2f;
        float Height = position.height;
        float borderOffset = Width * 0.05f;

        Rect bgRect = new Rect(position.width - Width, 0, Width, Height);
        Rect fgRect = new Rect(position.width - Width + borderOffset, borderOffset, Width - (borderOffset * 2), Height - (borderOffset * 2));
        EditorGUI.DrawRect(bgRect, Color.black);
        EditorGUI.DrawRect(fgRect, Color.gray);

        if(showAllQuests.Count != allQuests.Count)
        {
            showAllQuests.Clear();
            for(int i = 0; i < allQuests.Count; i++)
            {
                showAllQuests.Add(false);
            }
        }

        GUILayout.BeginArea(fgRect);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("All Quests", titleFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (GUILayout.Button("Refresh"))
        {
            GetData();
        }
        showSortedQuests = EditorGUILayout.Toggle("Sorted", showSortedQuests);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        for (int i = 0; i < showAllQuests.Count; i++)
        {
            EditorGUI.indentLevel += 1;
            showAllQuests[i] = EditorGUILayout.Foldout(showAllQuests[i], "(" + allQuests[i].saveData.questID + ")" + allQuests[i].questName, true, EditorStyles.foldout);
            EditorGUI.indentLevel -= 1;

            if (showAllQuests[i])
            {
                EditorGUI.indentLevel += 2;
                EditorGUILayout.LabelField("Type: " + allQuests[i].questType.ToString());
                EditorGUI.indentLevel -= 2;
            }
        }
        GUILayout.EndArea();

    }

    /*
     * Draw questgiver collection foldout
     */
    private void DrawQuestgiverList()
    {
        float Width = position.width * 0.2f;
        float Height = position.height;
        float borderOffset = Width * 0.05f;

        Rect bgRect = new Rect(0, 0, Width, Height);
        Rect fgRect = new Rect(borderOffset, borderOffset, Width - (borderOffset * 2), Height - (borderOffset * 2));
        EditorGUI.DrawRect(bgRect, Color.black);
        EditorGUI.DrawRect(fgRect, Color.gray);

        GUILayout.BeginArea(fgRect);
        EditorGUILayout.LabelField("Questgivers", titleFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        for (int i = 0; i < allQuestgivers.Length; i++)
        {
            EditorGUI.indentLevel += 1;
            showQuestgivers[i] = EditorGUILayout.Foldout(showQuestgivers[i], allQuestgivers[i].name, true, EditorStyles.foldout);
            EditorGUI.indentLevel -= 1;

            if (showQuestgivers[i])
            {
                if(i != selectedQuestgiverIndex)
                {
                    selectedQuestgiverIndex = i;
                    for (int j = 0; j < showQuestgivers.Count; j++)
                    {
                        showQuestgivers[j] = false;
                    }
                    showQuestgivers[selectedQuestgiverIndex] = true;
                }
                Selection.activeGameObject = allQuestgivers[selectedQuestgiverIndex];
                EditorGUILayout.BeginVertical();
                EditorGUI.indentLevel += 2;
                EditorGUILayout.ObjectField(allQuestgivers[i], typeof(GameObject), true);
                EditorGUILayout.LabelField(allQuestgivers[i].GetComponent<QuestController>().characterName);
                EditorGUILayout.EnumPopup(allQuestgivers[i].GetComponent<QuestController>().faction);
                EditorGUI.indentLevel -= 2;
                EditorGUILayout.EndVertical();
            }
        }
        GUILayout.EndArea();
    }

    /*
     * Draw the selected questgiver
     */
    private void DrawSelectedQuestgiver()
    {
        float Width = position.width - ((position.width * 0.2f)*2);
        float Height = position.height;
        float borderOffset = Width * 0.05f;

        Rect bgRect = new Rect(position.width * 0.2f, 0, Width, Height);
        Rect fgRect = new Rect(position.width * 0.2f + borderOffset, borderOffset, Width - (borderOffset * 2), Height - (borderOffset * 2));
        EditorGUI.DrawRect(bgRect, Color.black);
        EditorGUI.DrawRect(fgRect, Color.gray);

        GUILayout.BeginArea(fgRect);
        
        EditorGUILayout.LabelField("Selected Questgiver", titleFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        selectedObj = (GameObject)EditorGUILayout.ObjectField("Selected: ", Selection.activeGameObject, typeof(GameObject), true, GUILayout.Width(600));
        if (selectedObj != null)
        {
            if (selectedObj.GetComponent<QuestController>() != null)
            {
                currentQuestgiver = selectedObj.GetComponent<QuestController>();
                EditorGUILayout.BeginHorizontal();
                selectedObj.GetComponent<QuestController>().characterName = EditorGUILayout.TextField("Name: ", selectedObj.GetComponent<QuestController>().characterName, GUILayout.Width(300));
                selectedObj.GetComponent<QuestController>().faction = (GameData.Faction)EditorGUILayout.EnumPopup("Faction: ", selectedObj.GetComponent<QuestController>().faction, GUILayout.Width(300));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.IntField("Number of Quests: ", currentQuestgiver.quests.Count, GUILayout.Width(300));
                EditorGUILayout.IntField("Number of Quest Returns:", currentQuestgiver.questReturnIds.Count, GUILayout.Width(300));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
        }
        
        EditorGUILayout.LabelField("Quests", titleFont);
        EditorGUILayout.Space();
        if (currentQuestgiver != null)
        {
            if (currentQuestgiver.quests.Count > 0)
            {
                // Check foldout values
                if (showCurrentQuests.Count != currentQuestgiver.quests.Count)
                {
                    showCurrentQuests.Clear();
                    for (int i = 0; i < currentQuestgiver.quests.Count; i++)
                    {
                        showCurrentQuests.Add(false);
                    }
                }
                for (int i = 0; i < showCurrentQuests.Count; i++)
                {
                    EditorGUI.indentLevel += 2;
                    showCurrentQuests[i] = EditorGUILayout.Foldout(showCurrentQuests[i], currentQuestgiver.quests[i].questName, true, EditorStyles.foldout);
                    EditorGUI.indentLevel -= 2;

                    if (showCurrentQuests[i])
                    {
                        if (i != selectedQuestIndex)
                        {
                            selectedQuestIndex = i;
                            for (int j = 0; j < showCurrentQuests.Count; j++)
                            {
                                showCurrentQuests[j] = false;
                            }
                            showCurrentQuests[selectedQuestIndex] = true;
                        }

                        currentQuestgiver.quests[i].questName = EditorGUILayout.TextField("Quest Name: ", currentQuestgiver.quests[i].questName, GUILayout.Width(600));
                        currentQuestgiver.quests[i].faction = (GameData.Faction)EditorGUILayout.EnumPopup("Faction: ", currentQuestgiver.quests[i].faction, GUILayout.Width(600));

                        EditorGUILayout.LabelField("\t\t Save Data", sectionFont);
                        currentQuestgiver.quests[i].saveData.questID = EditorGUILayout.IntField("Quest ID: ", currentQuestgiver.quests[i].saveData.questID, GUILayout.Width(300));
                        currentQuestgiver.quests[i].saveData.isAccepted = EditorGUILayout.Toggle("is Accepted:", currentQuestgiver.quests[i].saveData.isAccepted);
                        currentQuestgiver.quests[i].saveData.isCompleted = EditorGUILayout.Toggle("is Completed:", currentQuestgiver.quests[i].saveData.isCompleted);

                        EditorGUILayout.Space();
                        currentQuestgiver.quests[i].questType = (QuestData.QuestType)EditorGUILayout.EnumPopup("Quest Type: ", currentQuestgiver.quests[i].questType, GUILayout.Width(600));
                        currentQuestgiver.quests[i].questDescription = EditorGUILayout.TextField("Description: ", currentQuestgiver.quests[i].questDescription, GUILayout.Height(50), GUILayout.Width(600));
                        currentQuestgiver.quests[i].questResponse = EditorGUILayout.TextField("Response: ", currentQuestgiver.quests[i].questResponse, GUILayout.Height(50), GUILayout.Width(600));
                        currentQuestgiver.quests[i].isRepeatable = EditorGUILayout.Toggle("is Repeatable", currentQuestgiver.quests[i].isRepeatable);


                        //===== Required Quests =====
                        EditorGUILayout.LabelField("\t Required Quests", sectionFont);
                        EditorGUILayout.BeginVertical();
                        for (int j = 0; j < currentQuestgiver.quests[i].requirements.prerequisiteQuests.Count; j++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            currentQuestgiver.quests[i].requirements.prerequisiteQuests[j] = EditorGUILayout.IntField(currentQuestgiver.quests[i].requirements.prerequisiteQuests[j], GUILayout.Width(50));
                            if (questDictionary.ContainsKey(currentQuestgiver.quests[i].requirements.prerequisiteQuests[j]))
                            {
                                EditorGUILayout.LabelField(questDictionary[currentQuestgiver.quests[i].requirements.prerequisiteQuests[j]].questName);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Required Quest", GUILayout.Width(200)))
                        {
                            currentQuestgiver.quests[i].requirements.prerequisiteQuests.Add(999);
                        }
                        if (GUILayout.Button("Remove Last Quest", GUILayout.Width(200)))
                        {
                            if(currentQuestgiver.quests[i].requirements.prerequisiteQuests.Count > 0)
                                currentQuestgiver.quests[i].requirements.prerequisiteQuests.RemoveAt(currentQuestgiver.quests[i].requirements.prerequisiteQuests.Count - 1);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Width(fgRect.width / 2));
                        EditorGUILayout.Space();


                        //===== Required/Given Gold =====
                        EditorGUILayout.LabelField("\t Required Gold", sectionFont);
                        currentQuestgiver.quests[i].requirements.goldGiven = EditorGUILayout.IntField("Gold Given: ", currentQuestgiver.quests[i].requirements.goldGiven, GUILayout.Width(300));
                        currentQuestgiver.quests[i].requirements.goldRequired = EditorGUILayout.IntField("Gold Required: ", currentQuestgiver.quests[i].requirements.goldRequired, GUILayout.Width(300));
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Width(fgRect.width / 2));
                        EditorGUILayout.Space();


                        //===== Required Items =====
                        EditorGUILayout.LabelField("\t Required Items", sectionFont);
                        EditorGUILayout.BeginVertical();
                        for (int j = 0; j < currentQuestgiver.quests[i].requirements.itemsRequired.Count; j++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            currentQuestgiver.quests[i].requirements.itemsRequired[j] = (GameObject)EditorGUILayout.ObjectField(currentQuestgiver.quests[i].requirements.itemsRequired[j], 
                                                                                        typeof(GameObject), true, GUILayout.Width(200));

                            if (currentQuestgiver.quests[i].requirements.itemsRequired[j] != null && currentQuestgiver.quests[i].requirements.itemsRequired[j].GetComponent<ItemController>() != null)
                            {
                                currentQuestgiver.quests[i].requirements.numberOfItemsRequired[j] = EditorGUILayout.IntField(currentQuestgiver.quests[i].requirements.itemsRequired[j].GetComponent<ItemController>().data.name,
                                                                                                    currentQuestgiver.quests[i].requirements.numberOfItemsRequired[j]);
                            }
                            else
                            {
                                currentQuestgiver.quests[i].requirements.numberOfItemsRequired[j] = EditorGUILayout.IntField("no item", currentQuestgiver.quests[i].requirements.numberOfItemsRequired[j]);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Add Required Item", GUILayout.Width(200)))
                        {
                            currentQuestgiver.quests[i].requirements.itemsRequired.Add(null);
                            currentQuestgiver.quests[i].requirements.numberOfItemsRequired.Add(0);
                        }
                        if (GUILayout.Button("Remove Last Item", GUILayout.Width(200)))
                        {
                            if (currentQuestgiver.quests[i].requirements.itemsRequired.Count > 0)
                            {
                                currentQuestgiver.quests[i].requirements.itemsRequired.RemoveAt(currentQuestgiver.quests[i].requirements.itemsRequired.Count - 1);
                                currentQuestgiver.quests[i].requirements.numberOfItemsRequired.RemoveAt(currentQuestgiver.quests[i].requirements.numberOfItemsRequired.Count - 1);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Width(fgRect.width / 2));
                        EditorGUILayout.Space();


                        //===== Reward =====
                        EditorGUILayout.LabelField("\t\t Reward", sectionFont);
                        currentQuestgiver.quests[i].reward.gold = EditorGUILayout.IntField("Gold Reward: ", currentQuestgiver.quests[i].reward.gold, GUILayout.Width(300));
                        currentQuestgiver.quests[i].reward.experience = EditorGUILayout.IntField("Experience Reward: ", currentQuestgiver.quests[i].reward.experience, GUILayout.Width(300));
                        currentQuestgiver.quests[i].reward.factionID = EditorGUILayout.IntField("Faction ID: ", currentQuestgiver.quests[i].reward.factionID, GUILayout.Width(300));
                        currentQuestgiver.quests[i].reward.reputation = EditorGUILayout.IntField("Reputation Reward: ", currentQuestgiver.quests[i].reward.reputation, GUILayout.Width(300));
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Quest"))
            {
                if (currentQuestgiver != null)
                {
                    currentQuestgiver.quests.Add(new QuestData());
                    showAllQuests.Add(false);
                    showCurrentQuests.Add(true);
                }
            }
            if (GUILayout.Button("Remove Quest at index: " + selectedQuestIndex))
            {
                if (currentQuestgiver != null)
                {
                    int oldIndex = selectedQuestIndex;
                    currentQuestgiver.quests.RemoveAt(oldIndex);
                    showCurrentQuests.RemoveAt(oldIndex);
                    selectedQuestIndex = 0;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Width(fgRect.width / 2));
        EditorGUILayout.LabelField("Quest Returns", titleFont);
        EditorGUILayout.Space();
        if (currentQuestgiver != null)
        {
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < currentQuestgiver.questReturnIds.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                currentQuestgiver.questReturnIds[i] = EditorGUILayout.IntField(currentQuestgiver.questReturnIds[i], GUILayout.Width(50));
                if (questDictionary.ContainsKey(currentQuestgiver.questReturnIds[i]))
                {
                    EditorGUILayout.LabelField(questDictionary[currentQuestgiver.questReturnIds[i]].questName);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Return Quest"))
            {
                if (currentQuestgiver != null)
                {
                    currentQuestgiver.questReturnIds.Add(999);
                }
            }
            if (GUILayout.Button("Remove Return Quest at index:"))
            {
                if (currentQuestgiver != null)
                {
                    
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndArea();
    }
    
    private void Update()
    {
        Repaint();
    }

    public static int SortByQuestID(QuestData q1, QuestData q2)
    {
        return q1.saveData.questID.CompareTo(q2.saveData.questID);
    }
}
