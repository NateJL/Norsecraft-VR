using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    // Save file paths
    private string saveOne = "/Saves/SaveOne";
    private string saveTwo = "/Saves/SaveTwo";
    private string saveThree = "/Saves/SaveThree";

    public GameObject playerPrefab;
    public GameObject playerShipPrefab;

    [Header("Ship Data")]
    public GameObject smallShipPrefab;
    public GameObject mediumShipPrefab;
    public GameObject largeShipPrefab;
    public GameObject longshipPrefab;
    [Space(5)]
    public Transform smallShipSpawn;
    public Transform mediumShipSpawn;
    public Transform largeShipSpawn;
    public Transform longshipSpawn;

    public QuestData[] questData;
    public Dictionary<int, QuestData> questDataCollection;

    public PoolManager objectPool;

    /*
     * Initialize:
     *      -Default Quest data
     */
    public void InitializeWorld()
    {
        objectPool = GetComponent<PoolManager>();

        InitializeQuestData();

    }

    /*
     * Initialize player prefab (character controller) before any game data (ie. health, level, reputation)
     */
    public GameObject InitializePlayer(Transform initPos)
    {
        GameObject player = Instantiate(playerPrefab, initPos.position, initPos.rotation);
        player.name = playerPrefab.name;
        return player;
    }

    /*
     * Initialize player prefab (character controller) before any game data (ie. health, level, reputation)
     */
    public GameObject InitializePlayer(Vector3 initPos, Vector3 initRot)
    {
        GameObject player = Instantiate(playerPrefab, initPos, Quaternion.Euler(initRot));
        player.name = playerPrefab.name;
        return player;
    }

    /*
     * Create quest lookup dictionary and if saved, add data from save
     */
    private void InitializeQuestData()
    {
        questDataCollection = new Dictionary<int, QuestData>();

        GameObject[] questGivers = GameObject.FindGameObjectsWithTag("Questgiver");

        for (int i = 0; i < questGivers.Length; i++)
        {
            QuestController questController = questGivers[i].GetComponent<QuestController>();
            if(questController != null)
            {
                for(int j = 0; j < questController.quests.Count; j++)
                {
                    questDataCollection.Add(questController.quests[j].saveData.questID, questController.quests[j]);
                }
            }
            else
            {
                Debug.Log("Couldnt get quest controller from: " + questGivers[i].name);
            }
        }
        questData = new QuestData[questDataCollection.Count];
        questDataCollection.Values.CopyTo(questData, 0);
    }

    /*
     * Save the current game data:
     *      - Player data
     *      - Quest save data
     */
    public void SaveGameState(int saveFileIndex, PlayerData playerData)
    {
        QuestSaveData[] questSaveData = new QuestSaveData[questData.Length];
        for(int i = 0; i < questData.Length; i++)
            questSaveData[i] = questData[i].saveData;

        SaveData saveData = new SaveData(playerData, questSaveData);
        SaveSystem.SaveGameState(saveData, saveFileIndex);
    }

    /*
     * 
     */
    public void LoadGameState(QuestSaveData[] loadQuestData)
    {
        for (int i = 0; i < questData.Length; i++)
        {
            for(int j = 0; j < loadQuestData.Length; j++)
            {
                if(loadQuestData[j].questID == questData[i].saveData.questID)
                {
                    Debug.Log("Loading Quest Data (" + questData[i].saveData.questID + "): " + questData[i].questName);
                    questData[i].saveData.isAccepted = loadQuestData[j].isAccepted;
                    questData[i].saveData.isCompleted = loadQuestData[j].isCompleted;
                }
            }
        }
    }

    /*
     * Spawn a corresponding ship to the current player data
     */
    public GameObject SpawnShip(ShipData shipData, bool loadCustomShipData)
    {
        GameObject ship = null;
        switch (shipData.Size)
        {
            case "Small":
                ship = Instantiate(smallShipPrefab, smallShipSpawn.position, smallShipSpawn.rotation) as GameObject;
                break;

            case "Medium":
                ship = Instantiate(mediumShipPrefab, mediumShipSpawn.position, mediumShipSpawn.rotation) as GameObject;
                break;

            case "Large":
                ship = Instantiate(largeShipPrefab, largeShipSpawn.position, largeShipSpawn.rotation) as GameObject;
                break;

            case "LongShip":
                ship = Instantiate(playerShipPrefab, longshipSpawn.position, longshipSpawn.rotation) as GameObject;
                ship.GetComponent<ShipManager>().FindNewComponents();

                // =============== get hull ===============
                GameObject newHull = objectPool.SpawnWithParent(shipData.HullType,
                                                                    ship.GetComponent<ShipManager>().shipHullTransform.position,
                                                                    ship.GetComponent<ShipManager>().shipHullTransform.rotation,
                                                                    ship.GetComponent<ShipManager>().shipHullTransform);
                newHull.transform.localPosition = Vector3.zero;
                if (newHull == null)
                    Debug.Log("Failed to get ship hull");

                // =============== get wheel =============== 
                GameObject newWheel = objectPool.SpawnWithParent(shipData.WheelType,
                                                                    ship.GetComponent<ShipManager>().shipWheelTransform.position,
                                                                    ship.GetComponent<ShipManager>().shipWheelTransform.rotation,
                                                                    ship.GetComponent<ShipManager>().shipWheelTransform);
                if(newWheel != null)
                {
                    newWheel.transform.localPosition = Vector3.zero;
                    ship.GetComponent<LongShipController>().shipRudder = newWheel;
                }
                else
                    Debug.Log("Failed to get ship wheel");

                // =============== get sails ===============
                GameObject newSails = objectPool.SpawnWithParent(shipData.SailType,
                                                                    ship.GetComponent<ShipManager>().shipSailsTransform.position,
                                                                    ship.GetComponent<ShipManager>().shipSailsTransform.rotation,
                                                                    ship.GetComponent<ShipManager>().shipSailsTransform);
                newSails.transform.localPosition = Vector3.zero;
                if (newSails == null)
                    Debug.Log("Failed to get ship sails");

                // =============== get flag ===============
                GameObject newFlag = objectPool.SpawnWithParent(shipData.FlagType,
                                                                    ship.GetComponent<ShipManager>().shipFlagTransform.position,
                                                                    ship.GetComponent<ShipManager>().shipFlagTransform.rotation,
                                                                    ship.GetComponent<ShipManager>().shipFlagTransform);
                if(newFlag != null)
                    newFlag.transform.localPosition = Vector3.zero;
                if (newFlag == null)
                    Debug.Log("Failed to get ship flag");
                    
                break;
                
            default:
                break;
        }
        ship.GetComponent<ShipManager>().FindComponentsInSeconds(0.5f);

        return ship;
    }

    public void LoadShipComponents(GameObject playerShip)
    {
        ShipManager ship = playerShip.GetComponent<ShipManager>();
        if (ship == null)
            return;

        if (ship.shipComponentManager.shipHull != null)
        {
            objectPool.ReturnObject(ship.shipComponentManager.shipHull);
        }
        if (ship.shipComponentManager.shipWheel != null)
        {
            objectPool.ReturnObject(ship.shipComponentManager.shipWheel);
        }
        if (ship.shipComponentManager.shipSail != null)
        {
            objectPool.ReturnObject(ship.shipComponentManager.shipWheel);
        }
        if (ship.shipComponentManager.shipFlag != null)
        {
            objectPool.ReturnObject(ship.shipComponentManager.shipFlag);
        }
    }

}
