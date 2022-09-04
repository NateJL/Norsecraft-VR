using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Static GameManager object, perisisted through scenes
    public static GameManager manager;
    public int seed;

    public enum ActiveScene
    {
        Menu,
        Game
    }
    public ActiveScene activeScene;

    public GameObject craftingAreaPrefab;

    [Header("Zones")]
    public GameObject[] zones;
    [Header("Interiors")]
    public GameObject[] interiors;
    [Header("Ships")]
    public GameObject[] ships;

    // DataManager reference to handle data persistence
    [HideInInspector]
    public DataManager dataManager;
    public PoolManager poolManager;
    public SoundManager soundManager;
    public TileManager tileManager;

    [Header("GameObject References")]
    public GameObject player;
    public GameObject playerShip;

    [Header("Current Game Data")]
    public GameData gameData;

    [Header("Current Player Data")]
    public Transform playerSpawn;
    public PlayerData playerData;

    private void Awake ()
    {
        if (manager == null)
        {
            manager = this;
        }
        else if (manager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        dataManager = GetComponent<DataManager>();      // get data manager
        poolManager = GetComponent<PoolManager>();      // get object pool manager
        soundManager = GetComponent<SoundManager>();    // get sound manager

        if (SceneManager.GetActiveScene().name.Equals("MenuScene"))
        {
            activeScene = ActiveScene.Menu;
            StartMainMenu();
        }
        else if (SceneManager.GetActiveScene().name.Equals("MainScene"))
        {
            activeScene = ActiveScene.Game;
            StartGame(new NewGameData());
        }
    }

    private void Start()
    {
        
    }

    private void StartMainMenu()
    {
        poolManager.InitializeObjectPools();        // initialize object pool
        while (!poolManager.isReady) { }            // wait for object pool to finish initializing

        player = dataManager.InitializePlayer(new Vector3(70, 3, -15), new Vector3(0, 180, 0));     // initialize player controller

        soundManager.Initialize(player);            // initialize sound manager
    }

    public IEnumerator StartNewGame(NewGameData newGameData)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }

        StartGame(newGameData);
    }

    private void StartGame(NewGameData newGameData)
    {
        tileManager = GameObject.FindGameObjectWithTag("TileManager").GetComponent<TileManager>();
        if (tileManager == null)
            Debug.LogError("GameManager: Failed to get TileManager.");

        poolManager.InitializeObjectPools();        // initialize object pool
        while (!poolManager.isReady) { }            // wait for object pool to finish initializing

        dataManager.InitializeWorld();              // initialize world

        tileManager.Initialize(seed, newGameData.worldSize);

        player = dataManager.InitializePlayer(playerSpawn);     // initialize player controller
        StartCoroutine("WaitForGameLoad");
        
        soundManager.Initialize(player);            // initialize sound manager

        ShipData shipData = StartStandardLongShip();

        InventoryData inventoryData = new InventoryData();                                  // start with empty inventory
        playerData = new PlayerData("Player", 100, 0, 1, 50, 100, 100, shipData, inventoryData);          // set default player values
        playerShip = dataManager.SpawnShip(playerData.playerShipData, false);                      // spawn player ship
        playerData.playerShipData.id = playerShip.GetInstanceID();

        zones = GameObject.FindGameObjectsWithTag("Zone");      // get all zones
        interiors = GameObject.FindGameObjectsWithTag("Interior");  // get all interior areas
        ships = GameObject.FindGameObjectsWithTag("Ship");          // get all active ships

        for (int i = 0; i < interiors.Length; i++)           // disable all interior areas
        {
            interiors[i].SetActive(false);
        }
    }

    private IEnumerator WaitForGameLoad()
    {
        while(!tileManager.completedGeneration)
        {
            player.transform.position = playerSpawn.position;
            player.GetComponent<PlayerManager>().SetMessage(Color.white, (int)tileManager.loadingPercentage + "%");
            yield return null;
        }
        Instantiate(craftingAreaPrefab, playerSpawn.position + new Vector3(5, -4, -5), Quaternion.identity);
    }

    /*
     * Function to complete a quest and change corresponding player values
     */
    public void CompleteQuest(int addedGold, int addedExp, int factionID, int reputation)
    {
        playerData.gold += addedGold;
        playerData.experience += addedExp;
        playerData.reputations[factionID] += reputation;
        if(playerData.experience >= (playerData.level*100))
        {
            playerData.experience = playerData.experience % (playerData.level * 100);
            playerData.level++;
        }
        string factionString = "\n";
        switch (factionID)
        {
            case 0:
                factionString += "(Stormstad)";
                break;

            case 1:
                factionString += "(rep #2)";
                break;

            case 2:
                factionString += "(rep #3)";
                break;
        }
        player.GetComponent<PlayerManager>().SetMessage(Color.green, "Gold: +" + addedGold + "\nExp: +" + addedExp + "\n+" + reputation + " reputation" + factionString);
    }

    /*
     * Function called when an npc is killed by the player
     */
    public void KilledNPC(int addedExp, int factionID, int reputation)
    {
        playerData.experience += addedExp;
        playerData.reputations[factionID] += reputation;
        if (playerData.experience >= (playerData.level * 100))
        {
            playerData.experience = playerData.experience % (playerData.level * 100);
            playerData.level++;
        }
        string factionString = "\n";
        switch (factionID)
        {
            case 0:
                if(reputation > 0)
                    factionString += ("\n+");
                else
                    factionString += "\n";
                factionString += reputation + " reputation\n(stormstad)";
                break;

            case 1:
                if (reputation > 0)
                    factionString += "\n+";
                else
                    factionString += "\n";
                factionString += reputation + " reputation\n(rep #2)";
                break;

            case 2:
                if (reputation > 0)
                    factionString += "\n+";
                else
                    factionString += "\n";
                factionString += reputation + " reputation\n(rep #3)";
                break;
        }
        player.GetComponent<PlayerManager>().SetMessage(Color.green, "+" + addedExp + " experience" + factionString);
    }

    /*
     * Function to save all current game data to specified file.
     *      -Gets all current player data
     *      -Sends index & player data to dataManager.SaveGameState(int, PlayerData)
     */
    public void SaveGame(int saveFile)
    {
        playerData.playerShipData = playerShip.GetComponent<ShipManager>().shipData;
        dataManager.SaveGameState(saveFile, playerData);
    }

    /*
     * Function to load game data from specified file
     */
    public void LoadGame(int loadFile)
    {
        SaveData saveData = SaveSystem.LoadGameState(loadFile);
        dataManager.LoadGameState(saveData.questData);

        playerData = saveData.playerData;
        LoadInventory();
        
        //TODO: load new player ship
        //      return old components, spawn new
        // should be same instance ID
        //manager.playerShip = playerShip;
    }

    /*
     * Function to load the saved player inventory and place items in the corresponding mounts
     */
    public void LoadInventory()
    {
        InventoryData inventory = playerData.playerInventoryData;
        /*
        if (inventory.leftHipItem != null)
        {
            if (dataManager.itemPrefabCollection.ContainsKey(inventory.leftHipItem))
            {
                GameObject prefab = dataManager.itemPrefabCollection[inventory.leftHipItem];
                if (prefab != null)
                {
                    Transform mountTransform = player.GetComponent<PlayerManager>().leftHipMount.transform;
                }
            }
        }
        if (inventory.rightHipItem != null)
        {
            if (dataManager.itemPrefabCollection.ContainsKey(inventory.rightHipItem))
            {
                GameObject prefab = dataManager.itemPrefabCollection[inventory.rightHipItem];
                if (prefab != null)
                {
                    Transform mountTransform = player.GetComponent<PlayerManager>().rightHipMount.transform;
                }
            }
        }
        if (inventory.frontHipItem != null)
        {
            if (dataManager.itemPrefabCollection.ContainsKey(inventory.frontHipItem))
            {
                GameObject prefab = dataManager.itemPrefabCollection[inventory.frontHipItem];
                if (prefab != null)
                {
                    Transform mountTransform = player.GetComponent<PlayerManager>().frontHipMount.transform;
                }
            }
        }*/
    }

    private ShipData StartStandardLongShip()
    {
        ShipData shipData = new ShipData(0, "LongShip", 100, true);
        shipData.HullType = "LongShipStandardHull";
        shipData.WheelType = "LongShipStandardRudder";
        shipData.SailType = "LongShipSailsYellow";
        shipData.FlagType = "none";
        return shipData;
    }

    private ShipData StartSmallStandardShip()
    {
        ShipData shipData = new ShipData(0, "Small", 100, true);
        shipData.HullType = "Small Ship Hull (Gold)";
        shipData.WheelType = "Standard Ship Wheel";
        shipData.SailType = "Small Ship Sails (Brown)";
        shipData.FlagType = "none";
        return shipData;
    }

    private ShipData StartMediumStandardShip()
    {
        ShipData shipData = new ShipData(0, "Medium", 500, true);
        shipData.HullType = "Medium Ship Hull (Black)";
        shipData.WheelType = "Captain's Wheel";
        shipData.SailType = "Medium Ship Sails (Brown)";
        shipData.FlagType = "none";
        return shipData;
    }
}
