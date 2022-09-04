using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ProceduralEditorWindow : EditorWindow
{
    public GameObject manager;
    public TileManager tileManager;
    public bool isRunning = false;
    public bool gotPools = false;
    public bool gotData = false;

    public ProceduralObjectPool[] pools;
    public bool[] showPools;
    public int currentPoolIndex;

    ProceduralObjectPool newPool;
    ProceduralObjectPool selectedPool;

    GUIStyle titleFont = new GUIStyle();
    GUIStyle sectionFont = new GUIStyle();
    GUIStyle groupFont = new GUIStyle();
    GUIStyle bottomButtonStyle = new GUIStyle();

    public float titleHeight;
    public float toolbarHeight;
    public float poolEntryHeight;
    public float poolRectHeight;

    public float testingRectHeight;

    public Vector2 scrollPos;
    public int toolbarIndex = 0;
    public string[] toolbarStrings = { "Object Pool", "Testing", "Island Generator" };

    public Texture2D prefabPreview = null;
    public Texture tileView = null;

    public ProceduralObjectPool.ProceduralType selectedType = ProceduralObjectPool.ProceduralType.None;
    public GameObject selectedSpawnableObject = null;
    public GameObject activeGameObject = null;
    public int selectedSpawnableIndex = 0;
    public bool showPrefabs = false;
    public bool[] showIslands;

    public Vector2 newTileSpawn = new Vector2();
    public GameObject TileBasePrefab;
    public GameObject islandPrefab;
    public GameObject activeTile;
    public GameObject activeIsland;
    public Camera editorCamera;
    public float CameraDistance;

    public bool hasIslandController = false;
    public ShowData showComponents;

    [MenuItem("Tools/Procedural Generation Editor")]
    static void Init()
    {
        ProceduralEditorWindow window = (ProceduralEditorWindow)EditorWindow.GetWindow(typeof(ProceduralEditorWindow), false, "Procedural Editor");
        //window.minSize = new Vector2(1600, 1000);
        //window.maxSize = window.minSize;
        //GUILayout.ExpandWidth(false);
        window.Show();
    }

    private void OnEnable()
    {
        titleFont.fontSize = 24;
        titleFont.alignment = TextAnchor.UpperCenter;
        titleFont.fontStyle = FontStyle.Bold;

        sectionFont.fontSize = 16;
        sectionFont.alignment = TextAnchor.UpperCenter;
        sectionFont.fontStyle = FontStyle.Bold;

        groupFont.fontSize = 12;
        groupFont.alignment = TextAnchor.UpperLeft;
        groupFont.fontStyle = FontStyle.Bold;

        bottomButtonStyle.fontSize = 10;
        bottomButtonStyle.alignment = TextAnchor.LowerCenter;

        titleHeight = position.height * 0.05f;
        toolbarHeight = position.height * 0.05f;
        poolEntryHeight = position.height * 0.15f;
        poolRectHeight = position.height * 0.8f;
        testingRectHeight = position.height * 0.8f;
    }

    private void Awake()
    {
        newPool = new ProceduralObjectPool("", null, ProceduralObjectPool.ProceduralType.None, 1);
        selectedPool = new ProceduralObjectPool("", null, ProceduralObjectPool.ProceduralType.None, 1);
        GetData();
    }

    private void OnFocus()
    {
        //GetData();
    }

    private void GetData()
    {
        manager = GameObject.FindGameObjectWithTag("TileManager");
        if(manager == null)
            Debug.LogError("ProceduralEditor: Failed to get manager");

        tileManager = manager.GetComponent<TileManager>();
        if(tileManager == null)
            Debug.LogError("ProceduralEditor: Failed to get tile manager");
        else
        {
            pools = tileManager.objectPool.pools.ToArray();
            showPools = new bool[pools.Length];
            gotPools = true;
        }

        showComponents = new ShowData();
        gotData = true;
    }

    private void OnGUI()
    {
        titleHeight = position.height * 0.05f;
        toolbarHeight = position.height * 0.03f;
        poolEntryHeight = position.height * 0.15f;
        poolRectHeight = position.height * 0.8f;
        testingRectHeight = position.height * 0.9f;

        DrawTitle();
        if (gotData)
        {
            DrawTileManagerData();
            DrawToolBar();
            if (toolbarIndex == 0)
            {
                DrawNewPool();
                DrawSelectedPool();
                DrawObjectPools();
            }
            else if (toolbarIndex == 1)
            {
                DrawTestingWindow();
            }
            else if (toolbarIndex == 2)
            {
                DrawIslandGeneratorWindow();
            }
        }
    }

    private void DrawObjectPools()
    {
        pools = tileManager.objectPool.pools.ToArray();
        bool[] newPools = new bool[pools.Length];
        if (newPools.Length > showPools.Length)
        {
            for (int i = 0; i < showPools.Length; i++)
            {
                newPools[i] = showPools[i];
            }
            newPools[showPools.Length] = true;
            currentPoolIndex = showPools.Length;
            showPools = newPools;
        }

        float Width = position.width * 0.8f;
        float borderOffset = position.width * 0.004f;

        Rect bgRect = new Rect(0, titleHeight + toolbarHeight + poolEntryHeight, Width, poolRectHeight);
        Rect fgRect = new Rect(borderOffset, titleHeight + toolbarHeight + poolEntryHeight+borderOffset, Width - (borderOffset * 2), poolRectHeight - (borderOffset * 2));
        EditorGUI.DrawRect(bgRect, Color.black);
        EditorGUI.DrawRect(fgRect, Color.gray);

        GUILayout.BeginArea(fgRect);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true);
        EditorGUILayout.BeginHorizontal();

        for(int i = 1; i < (int)ProceduralObjectPool.ProceduralType.NUM_TYPES; i++)
        {
            DrawPoolType((ProceduralObjectPool.ProceduralType)i, i);
        }


        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    private void DrawPoolType(ProceduralObjectPool.ProceduralType poolType, float index)
    {
        float Width = position.width * 0.2f;
        float borderOffset = position.width * 0.004f;
        float offset = Width * (index-1);

        Rect fgRect = new Rect(offset+borderOffset, borderOffset*2, Width - (borderOffset * 2), (poolRectHeight - (borderOffset * 2))/2);
        EditorGUI.DrawRect(fgRect, new Color32(119,119,119,255));

        GUILayout.BeginArea(fgRect);
        EditorGUILayout.LabelField(((ProceduralObjectPool.ProceduralType)index).ToString(), sectionFont, GUILayout.Width(fgRect.width));

        for (int i = 0; i < pools.Length; i++)
        {
            if (pools[i].objectType == poolType)
            {
                showPools[i] = EditorGUILayout.Foldout(showPools[i], pools[i].tag, true, EditorStyles.foldout);
                if (showPools[i])
                {
                   if(i != currentPoolIndex)
                    {
                        currentPoolIndex = i;
                        for(int j = 0; j < showPools.Length; j++)
                        {
                            showPools[j] = false;
                        }
                        showPools[currentPoolIndex] = true;
                    }
                    selectedPool = pools[currentPoolIndex];
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.IntField(pools[i].size);
                    EditorGUI.indentLevel -= 2;
                    EditorGUILayout.ObjectField(pools[i].prefab, typeof(GameObject), true);
                    EditorGUILayout.EndHorizontal();
                    if (isRunning)
                    {
                        GameObject[] poolObjects = tileManager.objectPool.poolDictionary[pools[i].tag].ToArray();
                        EditorGUI.indentLevel += 3;
                        EditorGUILayout.LabelField("GameObjects", EditorStyles.boldLabel);
                        EditorGUI.indentLevel -= 3;
                        EditorGUILayout.BeginVertical();
                        for (int j = 0; j < poolObjects.Length; j++)
                        {
                            if (poolObjects[j] != null)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUI.indentLevel += 4;
                                EditorGUILayout.Toggle(poolObjects[j].activeSelf, GUILayout.Width(100));
                                EditorGUI.indentLevel -= 4;
                                EditorGUILayout.ObjectField(poolObjects[j], typeof(GameObject), true, GUILayout.Width(300));
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }
        }
        GUILayout.EndArea();
    }

    private void DrawTitle()
    {
        float Width = position.width * 0.8f;
        float borderOffset = position.width * 0.004f;

        Rect bgRect = new Rect(0, 0, Width, titleHeight);
        Rect fgRect = new Rect(borderOffset, borderOffset, Width - (borderOffset * 2), titleHeight - (borderOffset * 2));
        EditorGUI.DrawRect(bgRect, Color.black);
        EditorGUI.DrawRect(fgRect, Color.gray);

        GUILayout.BeginArea(fgRect);
        EditorGUILayout.LabelField("Procedural Generation Editor", titleFont, GUILayout.Width(fgRect.width));
        GUILayout.EndArea();
    }

    private void DrawToolBar()
    {
        float Width = position.width * 0.8f;
        float borderOffset = position.width * 0.004f;

        Rect fgRect = new Rect(0, titleHeight, Width, toolbarHeight);
        EditorGUI.DrawRect(fgRect, Color.gray);

        GUILayout.BeginArea(fgRect);
        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarStrings);
        GUILayout.EndArea();
    }

    private void DrawTileManagerData()
    {
        float Width = position.width * 0.2f;
        float Height = position.height;
        float borderOffset = position.width * 0.004f;

        Rect bgRect = new Rect(position.width*0.8f, 0, Width, Height);
        Rect fgRect = new Rect(position.width * 0.8f + borderOffset, borderOffset, Width - (borderOffset * 2), Height - (borderOffset * 2));
        EditorGUI.DrawRect(bgRect, Color.black);
        EditorGUI.DrawRect(fgRect, Color.gray);

        GUILayout.BeginArea(fgRect);
        EditorGUILayout.LabelField("Tile Manager", titleFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.ObjectField("Manager GameObject", manager, typeof(GameObject), true);
        tileManager.seed = EditorGUILayout.IntField("Seed", tileManager.seed);
        tileManager.tileSize = EditorGUILayout.IntField("Tile Size", tileManager.tileSize);
        EditorGUILayout.Space();
        editorCamera = (Camera)EditorGUILayout.ObjectField("Editor Camera", editorCamera, typeof(Camera), true);
        tileView = (Texture) EditorGUILayout.ObjectField("View Texture", tileView, typeof(Texture), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Selected Object", titleFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        selectedType = (ProceduralObjectPool.ProceduralType)EditorGUILayout.EnumPopup("Type", selectedType, GUILayout.Width(300));
        EditorGUILayout.ObjectField("Selected Prefab", selectedSpawnableObject, typeof(GameObject), true);

        showPrefabs = EditorGUILayout.Foldout(showPrefabs, selectedType.ToString(), true, EditorStyles.foldoutPreDrop);
        if (showPrefabs)
        {
            for (int i = 0; i < pools.Length; i++)
            {
                if (pools[i].objectType == selectedType)
                {
                    showPools[i] = EditorGUILayout.Foldout(showPools[i], pools[i].tag, true, EditorStyles.foldout);
                    if (showPools[i])
                    {
                        if (i != selectedSpawnableIndex)
                        {
                            selectedSpawnableIndex = i;
                            for (int j = 0; j < showPools.Length; j++)
                            {
                                showPools[j] = false;
                            }
                            showPools[selectedSpawnableIndex] = true;
                        }
                        selectedSpawnableObject = pools[selectedSpawnableIndex].prefab;
                    }
                }
            }
        }
        
        EditorGUILayout.Space();
        prefabPreview = (Texture2D)AssetPreview.GetAssetPreview(selectedSpawnableObject);
        if (prefabPreview != null)
            GUILayout.Label(prefabPreview, GUILayout.Width(fgRect.width));
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        if(GUILayout.Button("Spawn Object"))
        {
            activeGameObject = PrefabUtility.InstantiatePrefab(selectedSpawnableObject as GameObject) as GameObject;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Active Object", titleFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.ObjectField(activeGameObject, typeof(GameObject), true);
        EditorGUILayout.Space();
        if (activeGameObject != null)
        {
            activeGameObject.transform.position = EditorGUILayout.Vector3Field("Position", activeGameObject.transform.position);
            activeGameObject.transform.eulerAngles = EditorGUILayout.Vector3Field("Rotation", activeGameObject.transform.eulerAngles);
        }
        else
        {
            EditorGUILayout.Vector3Field("Position", Vector3.zero);
            EditorGUILayout.Vector3Field("Rotation", Vector3.zero);
        }

        if (activeGameObject != null &&
            activeGameObject.GetComponent<TileController>() != null)
        {
            EditorGUILayout.LabelField("TileController", sectionFont, GUILayout.Width(fgRect.width));

            showIslands = new bool[activeGameObject.GetComponent<TileController>().islands.Count];
            for(int i = 0; i < activeGameObject.GetComponent<TileController>().islands.Count; i++)
            {
                showIslands[i] = EditorGUILayout.Foldout(showIslands[i], activeGameObject.GetComponent<TileController>().islands[i].name, true, EditorStyles.foldout);
                if(showIslands[i])
                {

                }
            }
            if(GUILayout.Button("Spawn Islands"))
            {
                // TODO
            }
            if(GUILayout.Button("Remove Island"))
            {
                // TODO
            }
        }

        if (GUILayout.Button("Destroy Object"))
        {
            PrefabPlacer.SafeDestroy(activeGameObject);
        }

        if(GUILayout.Button("Get Data", bottomButtonStyle))
        {
            GetData();
        }

        GUILayout.EndArea();
    }

    private void DrawNewPool()
    {
        float Width = position.width * 0.4f;
        float borderOffset = position.width * 0.004f;

        Rect bgRect = new Rect(0, titleHeight + toolbarHeight, Width, poolEntryHeight);
        Rect fgRect = new Rect(borderOffset, titleHeight + toolbarHeight + borderOffset, Width - (borderOffset * 2), poolEntryHeight - (borderOffset * 2));
        EditorGUI.DrawRect(bgRect, Color.black);
        EditorGUI.DrawRect(fgRect, Color.gray);

        GUILayout.BeginArea(fgRect);

        if (newPool.prefab != null)
            newPool.tag = newPool.prefab.name;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("New Pool", sectionFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.Space();
        newPool.tag = EditorGUILayout.TextField("Tag", newPool.tag, GUILayout.Width(300));
        newPool.prefab = (GameObject)EditorGUILayout.ObjectField(newPool.prefab, typeof(GameObject), true, GUILayout.Width(300));
        newPool.objectType = (ProceduralObjectPool.ProceduralType)EditorGUILayout.EnumPopup("Type", newPool.objectType, GUILayout.Width(300));
        EditorGUILayout.BeginHorizontal();
        newPool.size = (int)EditorGUILayout.IntField("Size", newPool.size, GUILayout.Width(300));
        if (GUILayout.Button("Add Pool", GUILayout.Width(300)))
        {
            tileManager.objectPool.pools.Add(new ProceduralObjectPool(newPool.tag, newPool.prefab, newPool.objectType, newPool.size));
            currentPoolIndex = tileManager.objectPool.pools.Count - 1;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        GUILayout.EndArea();
    }

    private void DrawSelectedPool()
    {
        float Width = position.width * 0.4f;
        float borderOffset = position.width * 0.004f;

        Rect bgRect = new Rect(Width, titleHeight + toolbarHeight, Width, poolEntryHeight);
        Rect fgRect = new Rect(Width + borderOffset, titleHeight + toolbarHeight + borderOffset, Width - (borderOffset * 2), poolEntryHeight - (borderOffset * 2));
        EditorGUI.DrawRect(bgRect, Color.black);
        EditorGUI.DrawRect(fgRect, Color.gray);

        GUILayout.BeginArea(fgRect);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Selected Pool", sectionFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.Space();
        selectedPool.tag = EditorGUILayout.TextField("Tag", selectedPool.tag, GUILayout.Width(300));
        selectedPool.prefab = (GameObject)EditorGUILayout.ObjectField(selectedPool.prefab, typeof(GameObject), true, GUILayout.Width(300));
        EditorGUILayout.BeginHorizontal();
        selectedPool.objectType = (ProceduralObjectPool.ProceduralType)EditorGUILayout.EnumPopup("Type", selectedPool.objectType, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        selectedPool.size = (int)EditorGUILayout.IntField("Size", selectedPool.size, GUILayout.Width(300));
        if (GUILayout.Button("Remove Pool", GUILayout.Width(300)))
        {
            if (tileManager.objectPool.pools.Contains(selectedPool))
            {
                tileManager.objectPool.pools.Remove(selectedPool);
            }
            //poolManager.pools.Remove();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        GUILayout.EndArea();
    }

    private void DrawTestingWindow()
    {
        float Width = position.width * 0.8f;
        float borderOffset = position.width * 0.004f;

        Rect bgRect = new Rect(0, titleHeight + toolbarHeight, Width, testingRectHeight);
        Rect fgRect = new Rect(0 + borderOffset, titleHeight + toolbarHeight + borderOffset, Width - (borderOffset * 2), testingRectHeight - (borderOffset * 2));
        EditorGUI.DrawRect(bgRect, Color.black);
        EditorGUI.DrawRect(fgRect, Color.gray);

        GUILayout.BeginArea(fgRect);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Active Tile", sectionFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.BeginVertical();

        newTileSpawn = EditorGUILayout.Vector2Field("New Tile Position", newTileSpawn, GUILayout.Width(300));
        TileBasePrefab =(GameObject) EditorGUILayout.ObjectField("Tile Prefab", TileBasePrefab, typeof(GameObject), true);
        if (GUILayout.Button("Spawn Tile", GUILayout.Width(300)))
        {
            activeTile = Instantiate(TileBasePrefab, new Vector3(newTileSpawn.x, 0, newTileSpawn.y), Quaternion.identity);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.Width(300));
        TileBasePrefab = (GameObject)EditorGUILayout.ObjectField("Island Prefab", TileBasePrefab, typeof(GameObject), true);
        if (GUILayout.Button("Spawn Island", GUILayout.Width(300)))
        {
            if (activeTile != null)
            {
                activeTile.GetComponent<TileController>().SpawnIslands(tileManager);
            }
            else
            {
                activeIsland = Instantiate(islandPrefab, new Vector3(newTileSpawn.x, 0, newTileSpawn.y), Quaternion.identity);
                activeIsland.GetComponent<IslandController>().Initialize();
            }
        }


        CameraDistance = EditorGUILayout.Slider(CameraDistance, 0, 1000, GUILayout.Width(300));
        if(activeTile != null)
        {
            editorCamera.transform.position = new Vector3(activeTile.transform.position.x, CameraDistance, activeTile.transform.position.z);
        }
        else if(activeIsland != null)
        {
            editorCamera.transform.position = new Vector3(activeIsland.transform.position.x, CameraDistance, activeIsland.transform.position.z);
        }

        EditorGUILayout.EndVertical();

        if(tileView != null)
        {
            GUILayout.Label(tileView);
        }

        GUILayout.EndArea();
    }

    private void DrawIslandGeneratorWindow()
    {
        float Width = position.width * 0.8f;
        float borderOffset = position.width * 0.004f;

        Rect bgRect = new Rect(0, titleHeight + toolbarHeight, Width, testingRectHeight);
        Rect fgRect = new Rect(0 + borderOffset, titleHeight + toolbarHeight + borderOffset, Width - (borderOffset * 2), testingRectHeight - (borderOffset * 2));
        EditorGUI.DrawRect(bgRect, Color.black);
        EditorGUI.DrawRect(fgRect, Color.gray);

        GUILayout.BeginArea(fgRect);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Island Generator", sectionFont, GUILayout.Width(fgRect.width));
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.ObjectField("Selected Object", Selection.activeGameObject, typeof(GameObject), true, GUILayout.Width(300));

        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<IslandController>() != null)
            hasIslandController = true;
        else
            hasIslandController = false;

        if (showComponents == null)
            showComponents = new ShowData();

        if(showComponents.showComponents.Count < (int)IslandData.IslandType.NUM_ISLAND_TYPES)
        {
            int diff = (int)IslandData.IslandType.NUM_ISLAND_TYPES - showComponents.showComponents.Count;
            for (int i = 0; i < diff; i++)
            {
                showComponents.showComponents.Add(new List<bool>());
                showComponents.scrollBarPos.Add(new Vector2(0, 0));
                for(int j = 0; j < IslandController.islandComponentList.Length; j++)
                {
                    showComponents.showComponents[showComponents.showComponents.Count - 1].Add(true);
                }
            }
        }

        EditorGUILayout.Toggle("Island Controller", hasIslandController, GUILayout.Width(300));

        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < (int)IslandData.IslandType.NUM_ISLAND_TYPES; i++)
        {
            DrawIslandTypes((IslandData.IslandType)i, i, fgRect.width, fgRect.height);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    private void DrawIslandTypes(IslandData.IslandType islandType, int index, float parentWidth, float parentHeight)
    {
        float sectionWidth = parentWidth / (float)IslandData.IslandType.NUM_ISLAND_TYPES;
        float sectionHeight = parentHeight*0.7f;
        float borderOffset = position.width * 0.004f;
        float offset = sectionWidth * index;

        Rect fgRect = new Rect(offset + borderOffset, parentHeight*0.2f, sectionWidth - (borderOffset * 2), sectionHeight);
        EditorGUI.DrawRect(fgRect, new Color32(119, 119, 119, 255));

        GUILayout.BeginArea(fgRect);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(((IslandData.IslandType)index).ToString(), sectionFont, GUILayout.Width(fgRect.width));

        showComponents.scrollBarPos[index] = EditorGUILayout.BeginScrollView(showComponents.scrollBarPos[index], false, false);

        IslandController controller = null;
        if (hasIslandController)
            controller = Selection.activeGameObject.GetComponent<IslandController>();
        float originalLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.5f;

        // Draw Trees
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.25f;
        showComponents.showComponents[index][0] = EditorGUILayout.Toggle("show", showComponents.showComponents[index][0]);
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.5f;
        EditorGUILayout.LabelField(IslandController.islandComponentList[0], groupFont, GUILayout.Width(fgRect.width * 0.8f));
        EditorGUILayout.EndHorizontal();
        if (hasIslandController && showComponents.showComponents[index][0])
        {
            for (int i = 0; i < controller.islandTrees[index].components.Count; i++)
            {
                controller.islandTrees[index].components[i] = (GameObject)EditorGUILayout.ObjectField(((IslandData.IslandBiome)i).ToString(),
                                                                                                        controller.islandTrees[index].components[i],
                                                                                                        typeof(GameObject), true,
                                                                                                        GUILayout.Width(fgRect.width * 0.8f));
            }
        }

        // Draw Rocks
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.25f;
        showComponents.showComponents[index][1] = EditorGUILayout.Toggle("show", showComponents.showComponents[index][1]);
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.5f;
        EditorGUILayout.LabelField(IslandController.islandComponentList[1], groupFont, GUILayout.Width(fgRect.width * 0.8f));
        EditorGUILayout.EndHorizontal();
        if (hasIslandController && showComponents.showComponents[index][1])
        {
            for (int i = 0; i < controller.islandRocks[index].multiComponents.Count; i++)
            {
                for (int j = 0; j < controller.islandRocks[index].multiComponents[i].components.Count; j++)
                {
                    if (j == 0)
                    {
                        controller.islandRocks[index].multiComponents[i].components[j] = (GameObject)EditorGUILayout.ObjectField(controller.islandRocks[index].multiComponents[i].componentType,
                                                                                                            controller.islandRocks[index].multiComponents[i].components[j],
                                                                                                            typeof(GameObject), true,
                                                                                                            GUILayout.Width(fgRect.width * 0.8f));
                    }
                    else
                    {
                        controller.islandRocks[index].multiComponents[i].components[j] = (GameObject)EditorGUILayout.ObjectField(" ",
                                                                                                        controller.islandRocks[index].multiComponents[i].components[j],
                                                                                                        typeof(GameObject), true,
                                                                                                        GUILayout.Width(fgRect.width * 0.8f));
                    }
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New " + ((IslandData.IslandBiome)i).ToString() + " prefab"))
                {
                    controller.islandRocks[index].multiComponents[i].components.Add(null);
                }
                if (GUILayout.Button("Remove " + ((IslandData.IslandBiome)i).ToString() + " prefab"))
                {
                    controller.islandRocks[index].multiComponents[i].components.RemoveAt(controller.islandRocks[index].multiComponents[i].components.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        // Draw Grass
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.25f;
        showComponents.showComponents[index][2] = EditorGUILayout.Toggle("show", showComponents.showComponents[index][2]);
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.5f;
        EditorGUILayout.LabelField(IslandController.islandComponentList[2], groupFont, GUILayout.Width(fgRect.width * 0.8f));
        EditorGUILayout.EndHorizontal();
        if (hasIslandController && showComponents.showComponents[index][2])
        {
            for (int i = 0; i < controller.islandGrass[index].multiComponents.Count; i++)
            {
                for (int j = 0; j < controller.islandGrass[index].multiComponents[i].components.Count; j++)
                {
                    if (j == 0)
                    {
                        controller.islandGrass[index].multiComponents[i].components[j] = (GameObject)EditorGUILayout.ObjectField(controller.islandGrass[index].multiComponents[i].componentType,
                                                                                                            controller.islandGrass[index].multiComponents[i].components[j],
                                                                                                            typeof(GameObject), true,
                                                                                                            GUILayout.Width(fgRect.width * 0.8f));
                    }
                    else
                    {
                        controller.islandGrass[index].multiComponents[i].components[j] = (GameObject)EditorGUILayout.ObjectField(" ",
                                                                                                        controller.islandGrass[index].multiComponents[i].components[j],
                                                                                                        typeof(GameObject), true,
                                                                                                        GUILayout.Width(fgRect.width * 0.8f));
                    }
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New " + ((IslandData.IslandBiome)i).ToString() + " prefab"))
                {
                    controller.islandGrass[index].multiComponents[i].components.Add(null);
                }
                if (GUILayout.Button("Remove " + ((IslandData.IslandBiome)i).ToString() + " prefab"))
                {
                    controller.islandGrass[index].multiComponents[i].components.RemoveAt(controller.islandGrass[index].multiComponents[i].components.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        // Draw Docks
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.25f;
        showComponents.showComponents[index][3] = EditorGUILayout.Toggle("show", showComponents.showComponents[index][3]);
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.5f;
        EditorGUILayout.LabelField(IslandController.islandComponentList[3], groupFont, GUILayout.Width(fgRect.width * 0.8f));
        EditorGUILayout.EndHorizontal();
        if (hasIslandController && showComponents.showComponents[index][3])
        {
            for (int i = 0; i < controller.islandDocks[index].components.Count; i++)
            {
                controller.islandDocks[index].components[i] = (GameObject)EditorGUILayout.ObjectField(((IslandData.IslandBiome)i).ToString(),
                                                                                                        controller.islandDocks[index].components[i],
                                                                                                        typeof(GameObject), true,
                                                                                                        GUILayout.Width(fgRect.width * 0.8f));
            }
        }

        // Draw Fences
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.25f;
        showComponents.showComponents[index][4] = EditorGUILayout.Toggle("show", showComponents.showComponents[index][4]);
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.5f;
        EditorGUILayout.LabelField(IslandController.islandComponentList[4], groupFont, GUILayout.Width(fgRect.width * 0.8f));
        EditorGUILayout.EndHorizontal();
        if (hasIslandController && showComponents.showComponents[index][4])
        {
            for (int i = 0; i < controller.islandFences[index].components.Count; i++)
            {
                controller.islandFences[index].components[i] = (GameObject)EditorGUILayout.ObjectField(((IslandData.IslandBiome)i).ToString(),
                                                                                                        controller.islandFences[index].components[i],
                                                                                                        typeof(GameObject), true,
                                                                                                        GUILayout.Width(fgRect.width * 0.8f));
            }
        }

        // Draw Structures
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.25f;
        showComponents.showComponents[index][5] = EditorGUILayout.Toggle("show", showComponents.showComponents[index][5]);
        EditorGUIUtility.labelWidth = originalLabelWidth * 0.5f;
        EditorGUILayout.LabelField(IslandController.islandComponentList[5], groupFont, GUILayout.Width(fgRect.width * 0.8f));
        EditorGUILayout.EndHorizontal();
        if (hasIslandController && showComponents.showComponents[index][5])
        {
            for (int i = 0; i < controller.islandStructures[index].multiComponents.Count; i++)
            {
                for (int j = 0; j < controller.islandStructures[index].multiComponents[i].components.Count; j++)
                {
                    if (j == 0)
                    {
                        controller.islandStructures[index].multiComponents[i].components[j] = (GameObject)EditorGUILayout.ObjectField(controller.islandStructures[index].multiComponents[i].componentType,
                                                                                                            controller.islandStructures[index].multiComponents[i].components[j],
                                                                                                            typeof(GameObject), true,
                                                                                                            GUILayout.Width(fgRect.width * 0.8f));
                    }
                    else
                    {
                        controller.islandStructures[index].multiComponents[i].components[j] = (GameObject)EditorGUILayout.ObjectField(" ",
                                                                                                        controller.islandStructures[index].multiComponents[i].components[j],
                                                                                                        typeof(GameObject), true,
                                                                                                        GUILayout.Width(fgRect.width * 0.8f));
                    }
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New " + ((IslandData.IslandBiome)i).ToString() + " prefab"))
                {
                    controller.islandStructures[index].multiComponents[i].components.Add(null);
                }
                if (GUILayout.Button("Remove " + ((IslandData.IslandBiome)i).ToString() + " prefab"))
                {
                    controller.islandStructures[index].multiComponents[i].components.RemoveAt(controller.islandStructures[index].multiComponents[i].components.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        EditorGUIUtility.labelWidth = originalLabelWidth;
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();

    }

    private void Update()
    {
        if(GameManager.manager != null)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        Repaint();
    }
}

public class ShowData
{
    public List<List<bool>> showComponents;
    public List<Vector2> scrollBarPos;

    public ShowData()
    {
        showComponents = new List<List<bool>>();
        scrollBarPos = new List<Vector2>();
    }
}
