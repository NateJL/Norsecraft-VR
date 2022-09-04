using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public Transform poolParent;

    public bool isReady = false;

    public List<Pool> pools;
    public static Dictionary<string, List<GameObject>> poolDictionary;
    public static Dictionary<string, List<ElementUI>> poolDictionaryUI;
    public static Dictionary<string, List<GameObject>> poolProcedural;
    public static Dictionary<string, GameObject> poolPrefabs;

    //public static Dictionary<string, List<GameObject>> shipPoolDictionary;
    //public static Dictionary<string, GameObject> shipPoolPrefabs;

	// Use this for initialization
	public void InitializeObjectPools ()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();
        poolDictionaryUI = new Dictionary<string, List<ElementUI>>();
        poolProcedural = new Dictionary<string, List<GameObject>>();
        poolPrefabs = new Dictionary<string, GameObject>();

        foreach(Pool pool in pools)
        {
            List<GameObject> objectPool = new List<GameObject>();
            List<ElementUI> objectPoolUI = new List<ElementUI>();

            for(int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.name = pool.prefab.name;
                if (obj.GetComponent<ItemController>() != null)
                    obj.GetComponent<ItemController>().baseScale = pool.prefab.transform.localScale;
                obj.transform.SetParent(poolParent);
                obj.SetActive(false);
                if (pool.objectType == Pool.ObjectPoolType.UI)
                    objectPoolUI.Add(new ElementUI(false, obj));
                else
                    objectPool.Add(obj);
            }

            switch(pool.objectType)
            {
                case Pool.ObjectPoolType.Item:
                case Pool.ObjectPoolType.Weapon:
                case Pool.ObjectPoolType.NPC:
                case Pool.ObjectPoolType.Ship:
                case Pool.ObjectPoolType.Tool:
                case Pool.ObjectPoolType.Container:
                case Pool.ObjectPoolType.Resource:
                case Pool.ObjectPoolType.Food:
                    poolDictionary.Add(pool.tag, objectPool);
                    poolPrefabs.Add(pool.tag, pool.prefab);
                    break;

                case Pool.ObjectPoolType.UI:
                    poolDictionaryUI.Add(pool.tag, objectPoolUI);
                    poolPrefabs.Add(pool.tag, pool.prefab);
                    break;

                case Pool.ObjectPoolType.Procedural:
                    poolProcedural.Add(pool.tag, objectPool);
                    poolPrefabs.Add(pool.tag, pool.prefab);
                    break;

                default:
                    poolDictionary.Add(pool.tag, objectPool);
                    poolPrefabs.Add(pool.tag, pool.prefab);
                    break;
            }

            //poolDictionary.Add(pool.tag, objectPool);
            //poolPrefabs.Add(pool.tag, pool.prefab);
        }
        isReady = true;
	}

    public GameObject SpawnWithParent(string tag, Vector3 position, Quaternion rotation, Transform newParent)
    {
        GameObject objectToSpawn = null;
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag: " + tag + " doesn't exist");
            return objectToSpawn;
        }

        List<GameObject> objectPool = poolDictionary[tag];

        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeSelf)
            {
                objectToSpawn = obj;
                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                objectToSpawn.transform.SetParent(newParent, false);
                return objectToSpawn;
            }
        }

        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(poolPrefabs[tag]);
            objectToSpawn.name = poolPrefabs[tag].name;
            if (objectToSpawn.GetComponent<ItemController>() != null)
                objectToSpawn.GetComponent<ItemController>().baseScale = poolPrefabs[tag].transform.localScale;
            objectPool.Add(objectToSpawn);
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.transform.SetParent(newParent, false);
            return objectToSpawn;
        }

        return objectToSpawn;
    }
	
	public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject objectToSpawn = null;
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag: " + tag + " doesn't exist");
            return objectToSpawn;
        }
        
        List<GameObject> objectPool = poolDictionary[tag];

        foreach(GameObject obj in objectPool)
        {
            if(!obj.activeSelf)
            {
                objectToSpawn = obj;
                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                return objectToSpawn;
            }
        }

        if(objectToSpawn == null)
        {
            objectToSpawn = Instantiate(poolPrefabs[tag]);
            objectToSpawn.name = poolPrefabs[tag].name;
            if (objectToSpawn.GetComponent<ItemController>() != null)
                objectToSpawn.GetComponent<ItemController>().baseScale = poolPrefabs[tag].transform.localScale;
            objectPool.Add(objectToSpawn);
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            return objectToSpawn;
        }

        return objectToSpawn;
    }

    public GameObject SpawnProceduralObject(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject objectToSpawn = null;
        if (!poolProcedural.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag: " + tag + " doesn't exist");
            return objectToSpawn;
        }

        List<GameObject> objectPool = poolProcedural[tag];

        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeSelf)
            {
                objectToSpawn = obj;
                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                return objectToSpawn;
            }
        }

        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(poolPrefabs[tag]);
            objectToSpawn.name = poolPrefabs[tag].name;
            objectPool.Add(objectToSpawn);
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            return objectToSpawn;
        }

        return objectToSpawn;
    }

    public ElementUI SpawnObjectUI(string tag, Vector3 position, Quaternion rotation, Transform newParent)
    {
        GameObject objectToSpawn = null;
        if (!poolDictionaryUI.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag: " + tag + " doesn't exist");
            return new ElementUI(false, objectToSpawn);
        }

        List<ElementUI> objectPool = poolDictionaryUI[tag];

        foreach (ElementUI obj in objectPool)
        {
            if (!obj.isCurrentlyActive)
            {
                objectToSpawn = obj.element;
                obj.isCurrentlyActive = true;
                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                objectToSpawn.transform.SetParent(newParent, false);
                return obj;
            }
        }

        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(poolPrefabs[tag]);
            objectToSpawn.name = poolPrefabs[tag].name;
            ElementUI newElement = new ElementUI(true, objectToSpawn);
            objectPool.Add(newElement);
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.transform.SetParent(newParent, false);
            return newElement;
        }

        return new ElementUI(false, objectToSpawn);
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolParent);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        if (obj.GetComponent<ItemController>() != null)
            obj.transform.localScale = obj.GetComponent<ItemController>().baseScale;
        else
            obj.transform.localScale = new Vector3(1, 1, 1);
    }

    public void ReturnProceduralObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
    }

    public void ReturnObjectUI(ElementUI obj)
    {
        obj.isCurrentlyActive = false;
        obj.element.SetActive(false);
        obj.element.transform.SetParent(poolParent);
        obj.element.transform.position = Vector3.zero;
        obj.element.transform.rotation = Quaternion.identity;
        obj.element.transform.localScale = new Vector3(1, 1, 1);
    }
}

[System.Serializable]
public class Pool
{
    public enum ObjectPoolType
    {
        None,
        Weapon,
        NPC,
        Ship,
        Item,
        UI,
        Tool,
        Container,
        Resource,
        Procedural,
        Food
    }

    public string tag;
    public GameObject prefab;
    public ObjectPoolType objectType = ObjectPoolType.None;
    public int size;

    public Pool(string newTag, GameObject newPrefab, ObjectPoolType newType, int newSize)
    {
        tag = newTag;
        prefab = newPrefab;
        objectType = newType;
        size = newSize;
    }
}

[System.Serializable]
public class ElementUI
{
    public bool isCurrentlyActive;
    public GameObject element;

    public ElementUI(bool startActive, GameObject obj)
    {
        isCurrentlyActive = startActive;
        element = obj;
    }
}
