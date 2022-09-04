using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ObjectPoolWindow : EditorWindow
{
    public Rect windowRect = new Rect(0, 0, 1200, 900);
    //public Rect graphArea;

    public GameObject manager;
    public PoolManager poolManager;
    public bool isRunning = false;
    
    public Pool[] pools;
    public bool[] showPools;
    public bool[] previousShowPools;
    public bool gotPool = false;

    private int maxValues = 300;
    private int currentAverage = 0;
    public List<int> framerateCollection;
    public List<int> activeObjectsCount;
    public List<int> averageFramerate;
    private int frames = 0;
    private float fps = 0;
    private float deltaTime = 0;
    private int speedScale = 1;
    private bool showFps = false;

    public Vector2 scrollPos;

    Pool addedPool;
    Pool currentPool;

    // Material used when drawing with OpenGL
    private Material material;

    public enum GraphMode
    {
        FPS,
        ActiveObjects
    }
    public GraphMode graphMode = GraphMode.FPS;

    public enum WindowMode
    {
        ObjectPool,
        IslandData
    }
    public WindowMode windowMode = WindowMode.ObjectPool;

    [MenuItem("Tools/Object Pool Manager")]
    static void Init()
    {
        ObjectPoolWindow window = (ObjectPoolWindow)EditorWindow.GetWindow(typeof(ObjectPoolWindow), false, "Pool Manager");
        window.minSize = new Vector2(1500, 900);
        window.maxSize = window.minSize;
        GUILayout.ExpandWidth(false);
        window.Show();
    }

    private void OnEnable()
    {
        // Find the "Hidden/Internal-Colored" shader, and cache it for use.
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
    }

    private void OnDisable()
    {
        DestroyImmediate(material);
    }

    private void Awake()
    {
        addedPool = new Pool("", null, Pool.ObjectPoolType.None, 1);
        currentPool = new Pool("", null, Pool.ObjectPoolType.None, 1);
        GetData();
    }

    private void GetData()
    {
        framerateCollection = new List<int>();
        activeObjectsCount = new List<int>();
        averageFramerate = new List<int>();
        manager = GameObject.FindGameObjectWithTag("GameController");
        if (manager == null)
            Debug.LogError("ObjectPoolWindow: Failed to get manager");

        poolManager = manager.GetComponent<PoolManager>();
        if (poolManager == null)
            Debug.LogError("ObjectPoolWindow: failed to get pool manager");
        else
        {
            pools = poolManager.pools.ToArray();
            showPools = new bool[pools.Length];
            previousShowPools = new bool[pools.Length];
            gotPool = true;
        }

    }

    public void OnGUI()
    {
        GameRunningSignal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        showFps = EditorGUILayout.Toggle("Show FPS", showFps);
        if (showFps)
        {
            DrawDataToggles();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            DrawGraph();
        }
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        switch(windowMode)
        {
            case WindowMode.ObjectPool:
                EditorGUILayout.BeginHorizontal();
                DrawAddPool();
                DrawCurrentPool();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                if (gotPool)
                    DrawPools();
                break;

            case WindowMode.IslandData:

                break;
        }

    }

    private void DrawDataToggles()
    {
        EditorGUILayout.BeginHorizontal();

        graphMode =(GraphMode) EditorGUILayout.EnumPopup(graphMode);

        switch(graphMode)
        {
            case GraphMode.FPS:
                if (GUILayout.Button(fps.ToString(), GUILayout.Width(300)))
                {

                }
                break;

            case GraphMode.ActiveObjects:

                break;

            default:
                if (GUILayout.Button(fps.ToString(), GUILayout.Width(300)))
                {

                }
                break;
        }
        

        //speedScale =(int) EditorGUILayout.Slider(speedScale, 1, 100);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawGraph()
    {
        Rect graphArea = GUILayoutUtility.GetRect(10, 1000, 200, 200);
        
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(graphArea);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            material.SetPass(0);

            // Draw background
            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(graphArea.width, 0, 0);
            GL.Vertex3(graphArea.width, graphArea.height, 0);
            GL.Vertex3(0, graphArea.height, 0);
            GL.End();

            GL.Begin(GL.LINES);                     // Start drawing in OpenGL Lines, to draw the lines of the grid.


            int offset = (Time.frameCount * 1) % 50;                // store measurement to determine offset (scrolling animation)
            int count = (int)(graphArea.width / 10) + 20;           //                        and line count (drawing grid)

            for (int i = 0; i < count; i++)
            {
                Color lineColour = (i % 5 == 0) ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.2f, 0.2f, 0.2f);
                GL.Color(lineColour);

                float x = i * 10 - offset;

                if (x >= 0 && x < graphArea.width)
                {
                    GL.Vertex3(x, 0, 0);
                    GL.Vertex3(x, graphArea.height, 0);
                }

                if (i < graphArea.height / 10)
                {
                    GL.Vertex3(0, i * 10, 0);
                    GL.Vertex3(graphArea.width, i * 10, 0);
                }
            }
            GL.End();


            // Draw data lines
            if (GameManager.manager != null)
            {
                GL.Begin(GL.LINES);
                GL.Color(Color.green);

                int changeX = 2;
                if (graphMode == GraphMode.FPS)
                {
                    for (int i = 1; i < framerateCollection.Count; i++)
                    {
                        GL.Color(Color.green);
                        float x = graphArea.width - ((framerateCollection.Count - (i - 1)) * changeX);
                        GL.Vertex3(x, framerateCollection[i - 1], 0);
                        x = graphArea.width - ((framerateCollection.Count - i) * changeX);
                        GL.Vertex3(x, framerateCollection[i], 0);

                        GL.Color(Color.blue);
                        x = graphArea.width - ((framerateCollection.Count - (i - 1)) * changeX);
                        GL.Vertex3(x, averageFramerate[i - 1], 0);
                        x = graphArea.width - ((framerateCollection.Count - i) * changeX);
                        GL.Vertex3(x, averageFramerate[i], 0);
                    }
                }
                else if (graphMode == GraphMode.ActiveObjects)
                {
                    for (int i = 1; i < activeObjectsCount.Count; i++)
                    {
                        GL.Color(Color.red);
                        float x = graphArea.width - ((activeObjectsCount.Count - (i - 1)) * changeX);
                        GL.Vertex3(x, activeObjectsCount[i - 1], 0);
                        x = graphArea.width - ((activeObjectsCount.Count - i) * changeX);
                        GL.Vertex3(x, activeObjectsCount[i], 0);
                    }
                }
                //GL.Vertex3(0, 0, 0);
                //GL.Vertex3(100, 100, 0);
                GL.End();
            }

            GL.PopMatrix();
            GUI.EndClip();
        }
        
    }

    /*
     * Draw the section to add a pool to the collection
     */
    private void DrawAddPool()
    {
        if (addedPool.prefab != null)
            addedPool.tag = addedPool.prefab.name;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t\t\t\t\t\t New Pool", EditorStyles.boldLabel);
        addedPool.tag = EditorGUILayout.TextField("Tag", addedPool.tag, GUILayout.Width(300));
        addedPool.prefab =(GameObject) EditorGUILayout.ObjectField(addedPool.prefab, typeof(GameObject), true, GUILayout.Width(300));
        addedPool.objectType =(Pool.ObjectPoolType) EditorGUILayout.EnumPopup("Type", addedPool.objectType, GUILayout.Width(300));
        EditorGUILayout.BeginHorizontal();
        addedPool.size =(int) EditorGUILayout.IntField("Size", addedPool.size, GUILayout.Width(300));
        if(GUILayout.Button("Add Pool", GUILayout.Width(300)))
        {
            poolManager.pools.Add(new Pool(addedPool.tag, addedPool.prefab, addedPool.objectType, addedPool.size));
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    /*
     * Draw the section displaying the current pool (and modifying)
     */
    private void DrawCurrentPool()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t\t\t\t\t Selected Pool", EditorStyles.boldLabel);
        currentPool.tag = EditorGUILayout.TextField("Tag", currentPool.tag, GUILayout.Width(300));
        currentPool.prefab = (GameObject)EditorGUILayout.ObjectField(currentPool.prefab, typeof(GameObject), true, GUILayout.Width(300));
        EditorGUILayout.BeginHorizontal();
        currentPool.objectType = (Pool.ObjectPoolType)EditorGUILayout.EnumPopup("Type", currentPool.objectType, GUILayout.Width(300));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        currentPool.size = (int)EditorGUILayout.IntField("Size", currentPool.size, GUILayout.Width(300));
        if (GUILayout.Button("Remove Pool", GUILayout.Width(300)))
        {
            // TODO: remove pool
            if(poolManager.pools.Contains(currentPool))
            {
                poolManager.pools.Remove(currentPool);
            }
            //poolManager.pools.Remove();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    /*
     * Draw all pools to window
     */
    private void DrawPools()
    {
        pools = poolManager.pools.ToArray();
        bool[] newPools = new bool[pools.Length];
        if(newPools.Length > showPools.Length)
        {
            for(int i = 0; i < showPools.Length; i++)
            {
                newPools[i] = showPools[i];
            }
            newPools[showPools.Length] = true;
            showPools = newPools;
            previousShowPools = showPools;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t Items", EditorStyles.boldLabel);
        DrawPoolSection(Pool.ObjectPoolType.Item);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t Food", EditorStyles.boldLabel);
        DrawPoolSection(Pool.ObjectPoolType.Food);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t Resources", EditorStyles.boldLabel);
        DrawPoolSection(Pool.ObjectPoolType.Resource);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t Tools", EditorStyles.boldLabel);
        DrawPoolSection(Pool.ObjectPoolType.Tool);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t Weapons", EditorStyles.boldLabel);
        DrawPoolSection(Pool.ObjectPoolType.Weapon);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t Containers", EditorStyles.boldLabel);
        DrawPoolSection(Pool.ObjectPoolType.Container);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t Ship Components", EditorStyles.boldLabel);
        DrawPoolSection(Pool.ObjectPoolType.Ship);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t NPCs", EditorStyles.boldLabel);
        DrawPoolSection(Pool.ObjectPoolType.NPC);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t UI", EditorStyles.boldLabel);
        DrawPoolSection(Pool.ObjectPoolType.UI);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("\t Procedural Components", EditorStyles.boldLabel);
        DrawPoolSection(Pool.ObjectPoolType.Procedural);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
    }

    /*
     * Draw a specified pool
     */
    private void DrawPoolSection(Pool.ObjectPoolType poolType)
    {
        for (int i = 0; i < pools.Length; i++)
        {
            if (pools[i].objectType == poolType)
            {
                showPools[i] = EditorGUILayout.Foldout(showPools[i], pools[i].tag, true, EditorStyles.foldout);
                if (showPools[i])
                {
                    if(!previousShowPools[i])
                    {
                        currentPool = pools[i];
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.IntField(pools[i].size, GUILayout.Width(100));
                    EditorGUI.indentLevel -= 2;
                    EditorGUILayout.ObjectField(pools[i].prefab, typeof(GameObject), true, GUILayout.Width(300));
                    EditorGUILayout.EndHorizontal();
                    if (isRunning)
                    {
                        GameObject[] poolObjects;
                        if (poolType == Pool.ObjectPoolType.Procedural)
                            poolObjects = PoolManager.poolProcedural[pools[i].tag].ToArray();
                        else if (poolType == Pool.ObjectPoolType.UI)
                        {
                            ElementUI[] poolElements = PoolManager.poolDictionaryUI[pools[i].tag].ToArray();
                            poolObjects = new GameObject[poolElements.Length];
                            for(int j = 0; j < poolElements.Length; j++)
                            {
                                poolObjects[j] = poolElements[j].element;
                            }
                        }
                        else
                            poolObjects = PoolManager.poolDictionary[pools[i].tag].ToArray();
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
    }

    /*
     * Checks if the game is currently running and sets corresponding values
     */
    private void GameRunningSignal()
    {
        Color defaultColor = GUI.backgroundColor;
        if (!isRunning)
            GUI.backgroundColor = Color.red;
        else
            GUI.backgroundColor = Color.green;
        GUILayout.Button("Game Running");
        GUI.backgroundColor = defaultColor;
    }

    /*
     * Updated every frame
     */
    private void Update()
    {
        if(GameManager.manager != null)
        {
            isRunning = true;
            frames++;
            if (frames % 5 == 0)
            {
                if (framerateCollection.Count >= maxValues)
                {
                    framerateCollection.RemoveAt(0);
                    averageFramerate.RemoveAt(0);
                }
                deltaTime += Time.deltaTime;
                deltaTime /= 2.0f;
                fps = 1.0f / deltaTime;
                currentAverage =(int) (((float)currentAverage + fps) / 2.0f);
                averageFramerate.Add((int)currentAverage);
                framerateCollection.Add((int)fps);
            }
        }
        else
        {
            isRunning = false;
        }

        Repaint();
    }
}
