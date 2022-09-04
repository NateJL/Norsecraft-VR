using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OVRSimpleJSON;
using System.IO;

public class ItemDatabaseEditor : EditorWindow
{
    string databaseFilename = "ItemDatabase.json";

    int removalIndex = 0;

    DatabaseHelper databaseManager;

    GUIStyle titleFont = new GUIStyle();

    [MenuItem("Tools/Item Database")]
    public static void ShowWindow()
    {
        GetWindow<ItemDatabaseEditor>("JSON Database Editor");
    }

    private void OnEnable()
    {
        databaseManager = new DatabaseHelper();

        titleFont.fontSize = 24;
        titleFont.alignment = TextAnchor.UpperCenter;
        titleFont.fontStyle = FontStyle.Bold;
    }

    private void GetData()
    {
        databaseManager = new DatabaseHelper();

        databaseManager.LoadJsonFromFile(databaseFilename);

    }

    private void SetData()
    {
        databaseManager.SaveJsonToFile(databaseFilename);

    }

    private void OnGUI()
    {
        DrawFileReadWriteInfo();
    }

    private void DrawFileReadWriteInfo()
    {
        Rect fgRect = new Rect(0, 0, position.width, position.height);
        Rect leftRect = new Rect(position.width*0.015f, position.height * 0.1f, position.width * 0.47f, position.height - position.height * 0.15f);
        Rect rightRect = new Rect(position.width*0.515f, position.height * 0.1f, position.width * 0.47f, position.height - position.height * 0.15f);
        EditorGUI.DrawRect(leftRect, Color.white);
        EditorGUI.DrawRect(rightRect, Color.white);

        GUILayout.BeginArea(fgRect);
        EditorGUILayout.LabelField("Item Database Editor", titleFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        databaseFilename = EditorGUILayout.TextField("Filename", databaseFilename);

        if(GUILayout.Button("Load File", GUILayout.Width(200)))
        {
            GetData();
        }
        if(GUILayout.Button("Save File", GUILayout.Width(200)))
        {
            SetData();
        }
        EditorGUILayout.EndHorizontal();

        DrawAllItems(leftRect);

        DrawRawJsonText(rightRect);


        GUILayout.EndArea();
    }

    private void DrawAllItems(Rect leftRect)
    {
        GUILayout.BeginArea(leftRect);
        EditorGUILayout.LabelField("Items", titleFont, GUILayout.Width(leftRect.width));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Number of Items: " + databaseManager.data.items.Count);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name");
        EditorGUILayout.LabelField("Value");
        EditorGUILayout.LabelField("Type");
        EditorGUILayout.LabelField("IconFile");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        if (databaseManager.data.items != null)
        {
            for (int i = 0; i < databaseManager.data.items.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                databaseManager.data.items[i].itemName = EditorGUILayout.TextField(databaseManager.data.items[i].itemName);
                databaseManager.data.items[i].itemValue = EditorGUILayout.IntField(databaseManager.data.items[i].itemValue);
                databaseManager.data.items[i].itemType = EditorGUILayout.TextField(databaseManager.data.items[i].itemType);
                databaseManager.data.items[i].iconFilename = EditorGUILayout.TextField(databaseManager.data.items[i].iconFilename);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Add Item"))
        {
            databaseManager.AddItemToDatabase(new ItemInfo());
        }
        if (GUILayout.Button("Remove Item"))
        {
            databaseManager.RemoveItemFromDatabase(removalIndex);
        }
        removalIndex = EditorGUILayout.IntField("Remove At", removalIndex);
        EditorGUILayout.EndHorizontal();


        GUILayout.EndArea();
    }

    private void DrawRawJsonText(Rect rightRect)
    {
        GUILayout.BeginArea(rightRect);

        EditorGUILayout.TextArea(databaseManager.rawJson);
        EditorGUILayout.TextArea(databaseManager.prettyJson);

        GUILayout.EndArea();
    }

    private void Update()
    {
        databaseManager.UpdateText();
        Repaint();
    }
}

[System.Serializable]
public class ItemInfo
{
    public string itemName;
    public int itemValue;
    public string itemType;
    public string iconFilename;

    public ItemInfo()
    {
        itemName = "New Item";
        itemValue = 0;
        itemType = "None";
        iconFilename = "noFile.jpg";
    }
}

[System.Serializable]
public class DatabaseHelper
{
    public DatabaseInfo data;
    public string rawJson;
    public string prettyJson;

    public DatabaseHelper()
    {
        data = new DatabaseInfo();
        rawJson = "";
        prettyJson = "";
    }

    public void AddItemToDatabase(ItemInfo item)
    {
        data.items.Add(item);
        UpdateText();
    }

    public void RemoveItemFromDatabase(int index)
    {
        data.items.RemoveAt(index);
        UpdateText();
    }

    public void UpdateText()
    {
        rawJson = JsonUtility.ToJson(data, false);
        prettyJson = JsonUtility.ToJson(data, true);
    }

    public void LoadJsonFromFile(string filename)
    {
        rawJson = File.ReadAllText(Application.streamingAssetsPath + "/" + filename);
        data = JsonUtility.FromJson<DatabaseInfo>(rawJson);
        prettyJson = JsonUtility.ToJson(data, true);
    }

    public void SaveJsonToFile(string filename)
    {
        rawJson = JsonUtility.ToJson(data);

        if (rawJson != null)
            File.WriteAllText(Application.streamingAssetsPath + "/" + filename, rawJson);
        else
            Debug.LogWarning("Warning: raw JSON text is null.");
    }
}

[System.Serializable]
public class DatabaseInfo
{
    public List<ItemInfo> items;

    public DatabaseInfo()
    {
        items = new List<ItemInfo>();
    }
}











public static class DatabaseUtility
{
    /// <summary>
    /// Return a list of items stored in the JSON file
    /// </summary>
    public static List<ItemInfo> LoadDatabaseInfo(string filepath, ref string rawJsonData)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(filepath);
        if (targetFile == null)
        {
            Debug.LogWarning("Error: Failed to load file from path.");
            return null;
        }

        rawJsonData = targetFile.text;
        ItemInfo[] tempInfo = JsonHelper.FromJson<ItemInfo>(targetFile.text);

        return new List<ItemInfo>(tempInfo);
    }

    /// <summary>
    /// Save list of items to the JSON database file, given filepath and item list.
    /// </summary>
    public static void SaveDatabaseInfo(string filepath, List<ItemInfo> newInfo)
    {
        ItemInfo[] tempInfo = newInfo.ToArray();

        string tempJson = JsonHelper.ToJson(tempInfo, true);

        File.WriteAllText(filepath, tempJson);
    }
}

/// <summary>
/// A static helper class to wrap a list of objects into a json string.
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// Convert a JSON string to an array of objects
    /// </summary>
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    /// <summary>
    /// Return a JSON string from an array of objects
    /// </summary>
    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    /// <summary>
    /// Return a JSON string from an array of objects with formatting
    /// </summary>
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
