using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SmelterController : MonoBehaviour
{
    private ParticleSystem fire_ps;

    public TextMeshProUGUI temperatureText;
    public Transform barSpawnTransform;
    public Transform oreSpawnTransform;
    [Space(10)]
    public float minTemperature = 500f;
    public float maxTemperature = 3000f;
    [ShowOnly] public float currentTemperature = 0.0f;
    [ShowOnly] public float targetTemperature = 0.0f;
    public float temperatureGrowth = 10f;
    public float FuelTempGrowth = 500f;

    [Header("Temperature Colors")]
    public Color minColor;
    public Color maxColor;
    [Space(10)]
    public List<SmeltableLookupTable> smelterTable;
    //private Dictionary<string, GameObject> smelterDictionary;

    private List<GameObject> currentFuel;
    private PoolManager objectPool;

	// Use this for initialization
	void Start ()
    {
        objectPool = GameManager.manager.poolManager;
        currentFuel = new List<GameObject>();
        fire_ps = transform.GetChild(0).GetComponent<ParticleSystem>();
        if (fire_ps == null)
            Debug.LogError("Smelter: Failed to get child particle system.");
        else
        {
            var main = fire_ps.main;
            main.startColor = minColor;
        }
        currentTemperature = targetTemperature = minTemperature;
	}

    private void Update()
    {
        // adjust current temperature if current temperature is inside temperature threshold
        if (currentTemperature > minTemperature || currentTemperature < maxTemperature)
        {
            if (currentTemperature < targetTemperature)
            {
                currentTemperature += (temperatureGrowth * Time.deltaTime);        // raise temperature if lower than target
            }
            else if (currentTemperature > targetTemperature)
            {
                currentTemperature -= (temperatureGrowth * Time.deltaTime);        // lower temperature if higher than target
            }
        }
        if (currentTemperature != targetTemperature)
        {
            float lerp = (currentTemperature / maxTemperature);
            Color newColor = Color.Lerp(minColor, maxColor, lerp);
            var main = fire_ps.main;
            main.startColor = newColor;
        }
        temperatureText.SetText(((int)currentTemperature).ToString());
    }

    private void BurnFuel()
    {
        if(currentFuel.Count > 0)
        {
            objectPool.ReturnObject(currentFuel[0]);
            currentFuel.RemoveAt(0);
            targetTemperature += FuelTempGrowth;
            if (targetTemperature > maxTemperature)
                targetTemperature = maxTemperature;
        }
    }

    /*
     * Coroutine function to smelt ore thrown into the smelter
     */
    IEnumerator SmeltOre(GameObject ore, SmeltableLookupTable smeltable, float delay)
    {
        if (currentTemperature < smeltable.meltingPoint)
        {
            ore.transform.SetPositionAndRotation(barSpawnTransform.position, barSpawnTransform.rotation);
        }

        yield return new WaitForSeconds(delay);

        if (currentTemperature >= smeltable.meltingPoint)
        {
            GameObject newBar = objectPool.SpawnFromPool(smeltable.barPrefab.name, barSpawnTransform.position, barSpawnTransform.rotation);
            objectPool.ReturnObject(ore);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<ItemController>() != null)
        {
            if (other.gameObject.GetComponent<ItemController>().data.name.Equals("Wood Log"))
            {
                currentFuel.Add(other.gameObject);
                Invoke("BurnFuel", 1.0f);
            }
            else
            {
                bool isValid = false;
                for(int i = 0; i < smelterTable.Count; i++)
                {
                    if(other.gameObject.name.Equals(smelterTable[i].orePrefab.name))
                    {
                        isValid = true;
                        other.GetComponent<Rigidbody>().useGravity = false;
                        other.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        other.transform.SetPositionAndRotation(oreSpawnTransform.position, oreSpawnTransform.rotation);
                        StartCoroutine(SmeltOre(other.gameObject, smelterTable[i], 5.0f));
                        break;
                    }
                }
                if(!isValid)
                    other.transform.SetPositionAndRotation(barSpawnTransform.position, barSpawnTransform.rotation);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(barSpawnTransform.position, new Vector3(0.18f, 0.1f, 0.3f));
    }
}

[System.Serializable]
public class SmeltableLookupTable
{
    public GameObject orePrefab;
    public GameObject barPrefab;

    public string oreTag;
    public string barTag;

    public int meltingPoint;

    public SmeltableLookupTable()
    {
        orePrefab = barPrefab = null;
        oreTag = "no ore";
        barTag = "no bar";
        meltingPoint = 500;
    }
}
