using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public enum ShipType
    {
        None,
        Small,
        Medium,
        Large,
        Longship
    }

    [Header("Boat Data")]
    public ShipType shipType = ShipType.None;
    public ShipData shipData;
    public ShipComponentManager shipComponentManager;

    [Header("Ship Components")]
    public Transform shipHullTransform;
    public Transform shipWheelTransform;
    public Transform shipSailsTransform;
    public Transform shipFlagTransform;

    // Use this for initialization
    void Awake ()
    {
        shipData = new ShipData(gameObject.GetInstanceID(), shipType.ToString(), 100, true);
        shipComponentManager = new ShipComponentManager();
    }

    public void FindNewComponents()
    {
        // Ship Hull
        shipHullTransform = transform.GetChild(1).GetChild(0);
        if (shipHullTransform.childCount == 1)
        {
            shipData.HullType = shipHullTransform.GetChild(0).GetComponent<ShipComponent>().data.name;
            shipComponentManager.shipHull = shipHullTransform.GetChild(0).gameObject;
        }
        else
        {
            shipData.HullType = "none";
            shipComponentManager.shipHull = null;
        }

        // Ship Wheel
        shipWheelTransform = transform.GetChild(1).GetChild(1);
        if (shipWheelTransform.childCount == 1)
        {
            shipData.WheelType = shipWheelTransform.GetChild(0).GetComponent<ShipComponent>().data.name;
            shipComponentManager.shipWheel = shipWheelTransform.GetChild(0).gameObject;
        }
        else
        {
            shipData.WheelType = "none";
            shipComponentManager.shipWheel = null;
        }

        // Ship Sails
        shipSailsTransform = transform.GetChild(1).GetChild(2);
        if (shipSailsTransform.childCount == 1)
        {
            shipData.SailType = shipSailsTransform.GetChild(0).GetComponent<ShipComponent>().data.name;
            shipComponentManager.SetSailReferences(shipSailsTransform.GetChild(0).gameObject);
        }
        else
        {
            shipData.SailType = "none";
            shipComponentManager.shipSail = null;
            shipComponentManager.sailsUp = null;
            shipComponentManager.sailsDown = null;
        }

        // Ship Flag
        shipFlagTransform = transform.GetChild(1).GetChild(3);
        if (shipFlagTransform.childCount == 1)
        {
            shipData.FlagType = shipFlagTransform.GetChild(0).GetComponent<ShipComponent>().data.name;
            shipComponentManager.shipFlag = shipFlagTransform.GetChild(0).gameObject;
        }
        else
        {
            shipData.FlagType = "none";
            shipComponentManager.shipFlag = null;
        }

        switch(shipData.Size)
        {
            case "LongShip":
                shipType = ShipType.Longship;

                break;

            case "Small":
                shipType = ShipType.Small;
                GetComponent<SmallShipController>().sailManager.mainSailUp = shipSailsTransform.GetChild(0).GetChild(0).gameObject;
                GetComponent<SmallShipController>().sailManager.mainSailDown = shipSailsTransform.GetChild(0).GetChild(1).gameObject;
                GetComponent<SmallShipController>().sailManager.RaiseSails();
                break;

            case "Medium":
                shipType = ShipType.Medium;
                GetComponent<MediumShipController>().sailManager.frontSailUp = shipSailsTransform.GetChild(0).GetChild(0).GetChild(0).gameObject;
                GetComponent<MediumShipController>().sailManager.midSailUp = shipSailsTransform.GetChild(0).GetChild(0).GetChild(1).gameObject;
                GetComponent<MediumShipController>().sailManager.backSailUp = shipSailsTransform.GetChild(0).GetChild(0).GetChild(2).gameObject;

                GetComponent<MediumShipController>().sailManager.frontSailDown = shipSailsTransform.GetChild(0).GetChild(1).GetChild(0).gameObject;
                GetComponent<MediumShipController>().sailManager.midSailDown = shipSailsTransform.GetChild(0).GetChild(1).GetChild(1).gameObject;
                GetComponent<MediumShipController>().sailManager.backSailDown = shipSailsTransform.GetChild(0).GetChild(1).GetChild(2).gameObject;

                GetComponent<MediumShipController>().sailManager.RaiseAllSails();
                break;

            case "Large":
                shipType = ShipType.Large;
                break;

            default:
                break;
        }
    }

    public void FindComponentsInSeconds(float seconds)
    {
        Invoke("FindNewComponents", seconds);
    }

    public bool ManagerSailsAction(bool raise)
    {
        switch (shipData.Size)
        {
            case "Small":
                if (raise)
                    GetComponent<SmallShipController>().sailManager.RaiseSails();
                else
                    GetComponent<SmallShipController>().sailManager.LowerSails();
                break;

            case "Medium":
                if (raise)
                    GetComponent<MediumShipController>().sailManager.RaiseAllSails();
                else
                    GetComponent<MediumShipController>().sailManager.LowerAllSails();
                break;

            case "Large":

                break;

            default:
                break;
        }
        return !raise;
    }
}

[System.Serializable]
public class ShipComponentManager
{
    [Header("Ship Components")]
    public GameObject shipHull;
    public GameObject shipWheel;
    public GameObject shipSail;
    public GameObject shipFlag;

    [Header("Ship Sails")]
    public GameObject sailsUp;
    public GameObject sailsDown;

    public bool sailing;

    public void SetSailReferences(GameObject sailParent)
    {
        shipSail = sailParent;
        sailsUp = sailParent.transform.GetChild(0).gameObject;
        sailsDown = sailParent.transform.GetChild(1).gameObject;
        RaiseSails();
    }

    public void RaiseSails()
    {
        sailsDown.SetActive(false);
        sailsUp.SetActive(true);
        sailing = false;
    }

    public void LowerSails()
    {
        sailsUp.SetActive(false);
        sailsDown.SetActive(true);
        sailing = true;
    }
}
