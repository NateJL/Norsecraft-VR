using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GameMonitorWindow : EditorWindow
{
    public Rect windowRect = new Rect(0, 0, 1200, 900);

    public GameObject gameManager;
    public GameObject tileManager;

    List<GameObject> tiles;

    bool[] showItems;

    public Dictionary<GameObject, GameObject> objectCollection;

    [MenuItem("Tools/Game Monitor")]
    static void Init()
    {
        GameMonitorWindow window = (GameMonitorWindow)EditorWindow.GetWindow(typeof(GameMonitorWindow), false, "Game Monitor");
        window.minSize = new Vector2(1600, 900);
        window.maxSize = window.minSize;
        GUILayout.ExpandWidth(false);
        window.Show();
    }

    private void Awake()
    {
        GetData();
    }

    private void GetData()
    {
        tiles = new List<GameObject>();

        gameManager = GameObject.FindGameObjectWithTag("GameController");
        if (gameManager == null)
            Debug.Log("Failed to get Game Manager");

        tileManager = GameObject.FindGameObjectWithTag("TileManager");
        if (tileManager == null)
            Debug.Log("Failed to get Tile Manager");
    }

    private void GetTileData()
    {
        int tileCount = tileManager.transform.childCount;
        tiles.Clear();

        // fill temp tile manager child list
        for (int i = 0; i < tileCount; i++)
        {
            tiles.Add(tileManager.transform.GetChild(i).gameObject);
        }
    }

    public void OnGUI()
    {
        if (tileManager != null)
            GetTileData();


        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawPlayerData();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        DrawTileData();
        //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

    }

    private void DrawTileData()
    {
        float Width = position.width * 0.2f;
        float Height = position.height;

        Rect tileManagerRect = new Rect(0, 0, Width, Height);
        Rect tileDataRect = new Rect(Width, 0, Width * 3, Height);
        Rect rightAreaRect = new Rect(Width * 4, 0, Width, Height);

        EditorGUI.DrawRect(tileManagerRect, Color.gray);
        EditorGUI.DrawRect(tileDataRect, Color.white);
        EditorGUI.DrawRect(rightAreaRect, Color.gray);

        GUILayout.BeginArea(tileManagerRect);
        EditorGUILayout.LabelField("Tile Manager", EditorStyles.boldLabel);
        EditorGUILayout.ObjectField(tileManager, typeof(GameObject), true);
        if(tileManager != null)
            EditorGUILayout.LabelField("Number of Children (Tiles): " + tileManager.transform.childCount);
        GUILayout.EndArea();

        Vector2 tilePosition = new Vector2(position.width * 0.2f, position.height * 0.2f);
        //DrawTile(null, tilePosition, Color.white);
        //tilePosition.x += position.width * 0.2f;
        //DrawTile(null, tilePosition, Color.blue);
    }

    private void DrawTile(GameObject tile, Vector2 tilePosition, Color bgColor)
    {
        float Width = position.width * 0.2f;
        float Height = Width;

        Rect bgTileRect = new Rect(tilePosition.x, tilePosition.y, Width, Height);

        EditorGUI.DrawRect(bgTileRect, bgColor);

        GUILayout.BeginArea(bgTileRect);
        if (tile != null)
        {
            EditorGUILayout.LabelField(tile.name);
        }
        else
        {
            EditorGUILayout.LabelField("No Tile");
        }

        GUILayout.EndArea();
    }

    private void DrawPlayerData()
    {
        EditorGUILayout.LabelField("Player Data", EditorStyles.boldLabel);
        if(GameManager.manager != null)
        {
            GameObject leftHand = GameManager.manager.player.transform.GetChild(1).GetChild(0).GetChild(5).GetChild(0).gameObject;
            GameObject rightHand = GameManager.manager.player.transform.GetChild(1).GetChild(0).GetChild(6).GetChild(0).gameObject;

            HandController leftController = leftHand.GetComponent<HandController>();
            HandController rightController = rightHand.GetComponent<HandController>();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Left Hand", EditorStyles.boldLabel);
            EditorGUILayout.ObjectField(leftHand, typeof(GameObject), true);
            EditorGUILayout.ObjectField("Grabbed Item", leftController.grabbedItem, typeof(GameObject), true);
            if(leftController.grabbedItem != null && leftController.grabbedItem.GetComponent<ItemController>() != null)
            {
                ItemController item = leftController.grabbedItem.GetComponent<ItemController>();
                EditorGUILayout.LabelField("Type", item.itemType.ToString());
                EditorGUILayout.Toggle("isActive", item.isActive);
                EditorGUILayout.LabelField("Active Timer", item.timeToActive.ToString());
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Name", item.data.name);
                EditorGUILayout.LabelField("Value", item.data.value.ToString());
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Right Hand", EditorStyles.boldLabel);
            EditorGUILayout.ObjectField(rightHand, typeof(GameObject), true);
            EditorGUILayout.ObjectField("Grabbed Item", rightController.grabbedItem, typeof(GameObject), true);
            if (rightController.grabbedItem != null && rightController.grabbedItem.GetComponent<ItemController>() != null)
            {
                ItemController item = rightController.grabbedItem.GetComponent<ItemController>();
                EditorGUILayout.LabelField("Type", item.itemType.ToString());
                EditorGUILayout.Toggle("isActive", item.isActive);
                EditorGUILayout.LabelField("Active Timer", item.timeToActive.ToString());
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Name", item.data.name);
                EditorGUILayout.LabelField("Value", item.data.value.ToString());
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void Update()
    {
        Repaint();
    }
}
