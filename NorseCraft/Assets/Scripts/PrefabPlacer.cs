using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class PrefabPlacer : MonoBehaviour
{
    public GameObject parent;

    public GameObject[] roadPrefabs;
    public GameObject[] groundTiles;
    public GameObject[] dockPrefabs;
    public GameObject[] structurePrefabs;
    List<GameObject[]> allPrefabs;

    [HideInInspector]
    public PlaceableObject[] currentPrefabArray;

    private GameObject selectedPrefab;
    [Space(10)]
    public GameObject currentGameObject;
    [Space(10)]
    public Vector3 position;
    public Vector3 incrementPos;
    [Space(5)]
    public Vector3 rotation;
    public Vector3 incrementRot;

    public enum PlaceMode
    {
        Roads,
        Tiles,
        Dock,
        Structure
    }
    public PlaceMode placeMode = PlaceMode.Roads;

    /*
     * 
     */
    public GameObject SpawnPrefab(int index)
    {
        if (index < currentPrefabArray.Length)
            selectedPrefab = currentPrefabArray[index].prefab;

        if (selectedPrefab != null)
        {
            Debug.Log("Spawning Prefab: " + currentPrefabArray[index].tag);
            GameObject newPrefab = PrefabUtility.InstantiatePrefab(selectedPrefab as GameObject) as GameObject;
            currentGameObject = newPrefab;

            if (parent != null)
                newPrefab.transform.SetParent(parent.transform);
            else
                Debug.LogWarning("PrefabPlacer: no parent for spawned GameObject.");

            newPrefab.transform.localPosition = position + incrementPos;
            newPrefab.transform.localEulerAngles = rotation + incrementRot;
        }

        return currentGameObject;
    }

    public void incrementPosition()
    {
        position += incrementPos;
    }

    public void incrementRotation()
    {
        rotation += incrementRot;
    }

    public void rotateCurrentGameObject()
    {
        if(currentGameObject != null)
            currentGameObject.transform.Rotate(new Vector3(0, 90, 0));
    }


    public void GetNewPrefabs(PlaceMode newMode)
    {
        placeMode = newMode;
        switch(newMode)
        {
            case PlaceMode.Roads:
                if (roadPrefabs.Length > 0)
                {
                    currentPrefabArray = new PlaceableObject[roadPrefabs.Length];
                    {
                        for (int i = 0; i < roadPrefabs.Length; i++)
                        {
                            PlaceableObject newObj = new PlaceableObject(roadPrefabs[i].name, roadPrefabs[i]);
                            currentPrefabArray[i] = newObj;
                        }
                    }
                }
                break;

            case PlaceMode.Tiles:
                if(groundTiles.Length > 0)
                {
                    currentPrefabArray = new PlaceableObject[groundTiles.Length];
                    {
                        for(int i = 0; i < groundTiles.Length; i++)
                        {
                            PlaceableObject newObj = new PlaceableObject(groundTiles[i].name, groundTiles[i]);
                            currentPrefabArray[i] = newObj;
                        }
                    }
                }
                break;

            case PlaceMode.Dock:
                if (dockPrefabs.Length > 0)
                {
                    currentPrefabArray = new PlaceableObject[dockPrefabs.Length];
                    {
                        for (int i = 0; i < dockPrefabs.Length; i++)
                        {
                            PlaceableObject newObj = new PlaceableObject(dockPrefabs[i].name, dockPrefabs[i]);
                            currentPrefabArray[i] = newObj;
                        }
                    }
                }
                break;

            case PlaceMode.Structure:
                if (dockPrefabs.Length > 0)
                {
                    currentPrefabArray = new PlaceableObject[structurePrefabs.Length];
                    {
                        for (int i = 0; i < structurePrefabs.Length; i++)
                        {
                            PlaceableObject newObj = new PlaceableObject(structurePrefabs[i].name, structurePrefabs[i]);
                            currentPrefabArray[i] = newObj;
                        }
                    }
                }
                break;

            default:
                break;
        }
    }
	
    /*
     * Safetly destroy object in edit mode
     */
    public static T SafeDestroy<T>(T obj) where T: Object
    {
        if (Application.isEditor)
            Object.DestroyImmediate(obj);
        else
            Object.Destroy(obj);

        return null;
    }
}

[System.Serializable]
public class PlaceableObject
{
    public string tag;
    public GameObject prefab;

    public PlaceableObject(string newTag, GameObject newObj)
    {
        tag = newTag;
        prefab = newObj;
    }
}
