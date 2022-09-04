using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public enum BuildingType
    {
        House = 0,
        Tower
    }
    public BuildingType buildingType;
    [Tooltip("Number of floors to generate (1-4)")]
    public int numberOfFloors = 3;

    public Transform foundationParent;
    public Transform secondFloorParent;
    public Transform thirdFloorParent;
    public Transform fourthFloorParent;

    public GameObject currentFoundation;
    public GameObject currentSecondFloor;
    public GameObject currentThirdFloor;
    public GameObject currentFourthFloor;

    public BuildingPrefabData prefabData;

	public void GenerateBuilding()
    {
        GetObjects();
        if (currentFoundation != null)
            PrefabPlacer.SafeDestroy(currentFoundation);
        if (currentSecondFloor != null)
            PrefabPlacer.SafeDestroy(currentSecondFloor);
        if (currentThirdFloor != null)
            PrefabPlacer.SafeDestroy(currentThirdFloor);
        if (currentFourthFloor != null)
            PrefabPlacer.SafeDestroy(currentFourthFloor);

        if (buildingType == BuildingType.House)
        {
            gameObject.name = "GeneratedHouse";
        }
        else if (buildingType == BuildingType.Tower)
        {
            gameObject.name = "GeneratedTower";
        }

        GameObject[] foundations = prefabData.groups[(int)buildingType].subgroups[0].prefabs.ToArray();
        GameObject[] secondFloorPrefabs = prefabData.groups[(int)buildingType].subgroups[1].prefabs.ToArray();
        GameObject[] thirdFloorPrefabs = prefabData.groups[(int)buildingType].subgroups[2].prefabs.ToArray();
        GameObject[] fourthFloorPrefabs = prefabData.groups[(int)buildingType].subgroups[3].prefabs.ToArray();

        int prefabIndex = 0;

        // one story building
        if (numberOfFloors == 1)
        {
            prefabIndex = 2;
            currentFoundation = Instantiate(foundations[prefabIndex]);
            currentFoundation.transform.SetParent(foundationParent);
            currentFoundation.transform.localPosition = Vector3.zero;
            currentFoundation.transform.localEulerAngles = Vector3.zero;
        }

        // two story building
        else if (numberOfFloors == 2)
        {
            prefabIndex = Random.Range(0, foundations.Length);
            currentFoundation = Instantiate(foundations[prefabIndex]);
            currentFoundation.transform.SetParent(foundationParent);
            currentFoundation.transform.localPosition = Vector3.zero;
            currentFoundation.transform.localEulerAngles = Vector3.zero;

            if (prefabIndex == 2)
                prefabIndex = 1;
            else
                prefabIndex = 0;

            currentSecondFloor = Instantiate(secondFloorPrefabs[prefabIndex]);
            currentSecondFloor.transform.SetParent(secondFloorParent);
            currentSecondFloor.transform.localPosition = Vector3.zero;
            currentSecondFloor.transform.localEulerAngles = Vector3.zero;
        }

        // three story building
        else if (numberOfFloors == 3)
        {
            prefabIndex = Random.Range(0, foundations.Length);
            currentFoundation = Instantiate(foundations[prefabIndex]);
            currentFoundation.transform.SetParent(foundationParent);
            currentFoundation.transform.localPosition = Vector3.zero;
            currentFoundation.transform.localEulerAngles = Vector3.zero;

            bool isSmall = false;
            if (prefabIndex == 2)
            {
                prefabIndex = 1;
                isSmall = true;
            }
            else
                prefabIndex = 0;

            currentSecondFloor = Instantiate(secondFloorPrefabs[prefabIndex]);
            currentSecondFloor.transform.SetParent(secondFloorParent);
            currentSecondFloor.transform.localPosition = Vector3.zero;
            currentSecondFloor.transform.localEulerAngles = Vector3.zero;

            if (isSmall)
                prefabIndex = 1;
            else
                prefabIndex = Random.Range(0, thirdFloorPrefabs.Length);

            currentThirdFloor = Instantiate(thirdFloorPrefabs[prefabIndex]);
            currentThirdFloor.transform.SetParent(thirdFloorParent);
            currentThirdFloor.transform.localPosition = Vector3.zero;
            currentThirdFloor.transform.localEulerAngles = Vector3.zero;
        }

        // four story building
        else if (numberOfFloors == 4)
        {
            prefabIndex = Random.Range(0, fourthFloorPrefabs.Length);
            currentFourthFloor = Instantiate(fourthFloorPrefabs[prefabIndex]);
            currentFourthFloor.transform.SetParent(fourthFloorParent);
            currentFourthFloor.transform.localPosition = Vector3.zero;
            currentFourthFloor.transform.localEulerAngles = Vector3.zero;
        }


    }

    private void OnValidate()
    {
        GetObjects();
    }

    private void GetObjects()
    {
        foundationParent = transform.GetChild(0);
        secondFloorParent = transform.GetChild(1);
        thirdFloorParent = transform.GetChild(2);
        fourthFloorParent = transform.GetChild(3);

        if (foundationParent.childCount > 0)
            currentFoundation = foundationParent.GetChild(0).gameObject;
        if (secondFloorParent.childCount > 0)
            currentSecondFloor = secondFloorParent.GetChild(0).gameObject;
        if (thirdFloorParent.childCount > 0)
            currentThirdFloor = thirdFloorParent.GetChild(0).gameObject;
        if (fourthFloorParent.childCount > 0)
            currentFourthFloor = fourthFloorParent.GetChild(0).gameObject;
    }

}

[System.Serializable]
public class BuildingPrefabData
{
    public List<BuildingPrefabGroup> groups;
}

[System.Serializable]
public class BuildingPrefabGroup
{
    public string groupName;
    public List<BuildingPrefabSubgroup> subgroups;

    public BuildingPrefabGroup()
    {
        groupName = "New Group";
        subgroups = new List<BuildingPrefabSubgroup>();
    }
}

[System.Serializable]
public class BuildingPrefabSubgroup
{
    public string subgroupName;
    public List<GameObject> prefabs;

    public BuildingPrefabSubgroup()
    {
        subgroupName = "New Group";
        prefabs = new List<GameObject>();
    }
}
