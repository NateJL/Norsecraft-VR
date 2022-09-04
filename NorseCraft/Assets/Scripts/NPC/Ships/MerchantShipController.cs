using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MerchantShipController : MonoBehaviour
{
    [Header("Movement")]
    public bool onCollisionCourse = false;
    public float currentSpeed = 0.0f;
    public float currentRotationSpeed = 0.0f;
    public float maxTurningDistance = 45.0f;
    public Vector3 collisionNormals = Vector3.zero;

    [Header("Child Objects Container")]
    public GameObject childContainer;

    public List<GameObject> childCollection;
    public List<GameObject> itemCollection;

    public MerchantShipData data;

    private ShipRaycaster raycaster;
    private Vector3 oldPosition;

    // Use this for initialization
    void Start ()
    {
        data.RaiseSails();

        data.hasDestination = false;

        childContainer = transform.GetChild(0).gameObject;
        childCollection = new List<GameObject>();
        itemCollection = new List<GameObject>();

        raycaster = transform.GetChild(3).gameObject.GetComponent<ShipRaycaster>();
        if (raycaster == null)
            Debug.LogError(gameObject.name + ": Failed to get raycaster");
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        GetDestination();

        if(data.hasDestination)
        {
            MoveShip();
        }
    }

    /*
     * Function to calculate movement and move the ship
     */
    private void MoveShip()
    {
        if (!data.sailing)
        {
            data.DropSails();
        }

        if(data.sailing)
        {
            currentSpeed = 4.0f;
        }

        bool tempCrash = false;
        if(raycaster.frontCenterData.distance < maxTurningDistance)
        {
            tempCrash = true;
            currentRotationSpeed = 4f;
        }
        if (raycaster.frontRightData.distance < maxTurningDistance)     // front right collision -- TURN LEFT
        {
            tempCrash = true;
            currentRotationSpeed = -4f;
            collisionNormals += transform.InverseTransformDirection(raycaster.frontRightData.normal);
        }
        if (raycaster.frontLeftData.distance < maxTurningDistance)     // front left collision  -- TURN RIGHT
        {
            tempCrash = true;
            currentRotationSpeed = 4f;
            collisionNormals += transform.InverseTransformDirection(raycaster.frontLeftData.normal);
        }
        if (raycaster.midRightData.distance < maxTurningDistance)      // mid right collision
        {
            collisionNormals += transform.InverseTransformDirection(raycaster.midRightData.normal);
        }
        if (raycaster.midLeftData.distance < maxTurningDistance)       // mid left collision
        {
            collisionNormals += transform.InverseTransformDirection(raycaster.midLeftData.normal);
        }
        if (raycaster.backRightData.distance < maxTurningDistance)     // back right collision
        {
            collisionNormals += transform.InverseTransformDirection(raycaster.backRightData.normal);
        }
        if (raycaster.backLeftData.distance < maxTurningDistance)      // back left collision
        {
            collisionNormals += transform.InverseTransformDirection(raycaster.backLeftData.normal);
        }
        onCollisionCourse = tempCrash;

        if(!onCollisionCourse)
        {
            // point towards destionation
        }


        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);

        Vector3 eulerRotation = new Vector3(0, currentRotationSpeed * Time.deltaTime, 0);
        gameObject.transform.Rotate(eulerRotation);


        Vector3 changePosition = transform.position - oldPosition;
        foreach (GameObject child in childCollection)
        {
            child.transform.position += changePosition;
            child.transform.RotateAround(gameObject.transform.position, Vector3.up, eulerRotation.y);
        }
        oldPosition = transform.position;
        collisionNormals = Vector3.zero;

    }

    private void GetDestination()
    {
        if (!data.hasDestination)
        {
            
        }
    }
}

[System.Serializable]
public class MerchantShipData
{
    public bool hasDestination;

    public Transform destination;

    public GameObject sailsUp;
    public GameObject sailsDown;

    public bool sailing;

    public void DropSails()
    {
        sailsUp.SetActive(false);
        sailsDown.SetActive(true);
        sailing = true;
    }

    public void RaiseSails()
    {
        sailsDown.SetActive(false);
        sailsUp.SetActive(true);
        sailing = false;
    }
}
