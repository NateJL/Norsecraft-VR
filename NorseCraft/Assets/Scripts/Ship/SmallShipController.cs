using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ShipManager))]
public class SmallShipController : MonoBehaviour
{
    [Header("Movement")]
    public float currentSpeed = 0.0f;
    public float maxSpeed = 1.0f;
    public float currentRotationSpeed = 0.0f;
    public float maxRotationSpeed = 1.0f;
    public Vector3 collisionNormals = Vector3.zero;

    [Space(10)]
    public Transform cannonballPoleMount;
    private GameObject cannonballPole;
    public Transform linstockMount;
    private GameObject linstock;

    [Header("Child Objects Container")]
    public GameObject childContainer;

    [Header("Data text")]
    public GameObject dataText;

    public SmallShipSailManager sailManager;
    public ShipManager shipManager;

    public List<GameObject> childCollection;
    public List<GameObject> itemCollection;

    [HideInInspector]
    public float wheelAngle = 0.0f;
    private Vector3 oldPosition;
    private Vector3 initialPosition;

    private ShipRaycaster raycaster;

    [Header("Float Data")]
    public float speed = 1.0f;
    public float scale = 1.0f;
    public float noiseStrength = 0.0f;
    public float noiseWalk = 1.0f;

    // Use this for initialization
    void Start()
    {
        shipManager = GetComponent<ShipManager>();
        shipManager.shipData.Size = "Small";
        shipManager.FindNewComponents();

        cannonballPole = cannonballPoleMount.GetChild(0).gameObject;
        linstock = linstockMount.GetChild(0).gameObject;

        childContainer = transform.GetChild(0).gameObject;
        childCollection = new List<GameObject>();
        itemCollection = new List<GameObject>();

        oldPosition = transform.position;
        initialPosition = transform.position;
        sailManager.RaiseSails();

        raycaster = transform.GetChild(4).gameObject.GetComponent<ShipRaycaster>();
        if (raycaster == null)
            Debug.LogError(gameObject.name + ": Failed to get raycaster");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!cannonballPole.GetComponent<ItemController>().isGrabbed)
            cannonballPole.transform.SetPositionAndRotation(cannonballPoleMount.transform.position, cannonballPoleMount.transform.rotation);
        if (!linstock.GetComponent<ItemController>().isGrabbed)
            linstock.transform.SetPositionAndRotation(linstockMount.transform.position, linstockMount.transform.rotation);

        foreach (GameObject item in itemCollection)
        {
            if (item.transform.parent == null)
            {
                item.transform.parent = childContainer.transform;
            }
        }

        dataText.GetComponent<TextMeshProUGUI>().SetText("Position: " + transform.position.ToString() +
                                                        "\nVelocity: " + currentSpeed + "/" + maxSpeed +
                                                        "\nRotation: " + currentRotationSpeed + " *" +
                                                        "\nWheel Angle: " + wheelAngle + " *");

        
    }

    private void FixedUpdate()
    {
        CalculateBuoyancy();
        CalculateSpeeds();
        MoveShip();
    }

    /*
     * Calculate the height relative to the waves
     */
    private void CalculateBuoyancy()
    {
        Vector3 vertex = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 floatRotation = Vector3.right * (Mathf.Sin(Time.time * speed + (transform.position.y))/2);
        vertex.y += Mathf.Sin(Time.time * speed + transform.position.y) * scale;
        vertex.y += Mathf.PerlinNoise(vertex.x + noiseWalk, vertex.y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
        vertex.Normalize();
        vertex.y += initialPosition.y;
        transform.position = new Vector3(transform.position.x, vertex.y, transform.position.z);
        //transform.Rotate(floatRotation * Time.deltaTime);
    }

    /*
     * Calculate movement speed
     */
    private void CalculateSpeeds()
    {
        wheelAngle = shipManager.shipWheelTransform.GetChild(0).GetComponent<SteeringController>().turnAngle;   // get angle of wheel
        shipManager.shipSailsTransform.localEulerAngles = new Vector3(0, wheelAngle * 0.5f, 0);                 // angle sails

        if (wheelAngle < 5.0f && wheelAngle > -5.0f)        // no angle if within threshold
            wheelAngle = 0;

        maxSpeed = 0.0f;
        maxRotationSpeed = 1f;
        if (sailManager.mainSailState)                      // add maxspeed if sail is down
        {
            maxSpeed += 15;
        }

        if (raycaster.frontCenterData.distance > raycaster.distanceThreshold)      // if there are no objects in front of the ship
        {
            if (currentSpeed < (maxSpeed - 0.01f))
                currentSpeed += 0.01f;
            else if (currentSpeed > (maxSpeed + 0.01f))
                currentSpeed -= 0.01f;
            else
                currentSpeed = maxSpeed;
        }
        else
        {
            if (currentSpeed > 2)
                currentSpeed = 2;
            currentSpeed -= 0.1f;
            if (currentSpeed < -1f)
                currentSpeed = -1f;
        }

        currentRotationSpeed = (wheelAngle / 7.5f) * maxRotationSpeed;

        if(raycaster.frontRightData.distance < raycaster.distanceThreshold)     // front right collision
        {
            collisionNormals += transform.InverseTransformDirection(raycaster.frontRightData.normal);
        }
        if (raycaster.frontLeftData.distance < raycaster.distanceThreshold)     // front left collision
        {
            collisionNormals += transform.InverseTransformDirection(raycaster.frontLeftData.normal);
        }
        if (raycaster.midRightData.distance < raycaster.distanceThreshold)      // mid right collision
        {
            collisionNormals += transform.InverseTransformDirection(raycaster.midRightData.normal);
        }
        if (raycaster.midLeftData.distance < raycaster.distanceThreshold)       // mid left collision
        {
            collisionNormals += transform.InverseTransformDirection(raycaster.midLeftData.normal);
        }
        if (raycaster.backRightData.distance < raycaster.distanceThreshold)     // back right collision
        {
            collisionNormals += transform.InverseTransformDirection(raycaster.backRightData.normal);
        }
        if (raycaster.backLeftData.distance < raycaster.distanceThreshold)      // back left collision
        {
            collisionNormals += transform.InverseTransformDirection(raycaster.backLeftData.normal);
        }
    }

    /*
     * Move the ship
     */
    public void MoveShip()
    {
        transform.Translate((Vector3.forward + collisionNormals) * currentSpeed * Time.deltaTime, Space.Self);

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






    /*
     * Add item to ship item collection
     */
    public void AddItemToShip(GameObject newItem)
    {
        itemCollection.Add(newItem);
        GameObject[] itemArray = itemCollection.ToArray();
        for (int i = 0; i < itemArray.Length - 1; i++)
        {
            for (int j = i + 1; j < itemArray.Length; j++)
            {
                if (itemArray[i].GetInstanceID() == itemArray[j].GetInstanceID())
                    itemCollection.Remove(itemArray[j]);
            }
        }
    }

    /*
     * Add child to ship child collection
     */
    public void AddChildToShip(GameObject newChild)
    {
        childCollection.Add(newChild);
        GameObject[] childArray = childCollection.ToArray();
        for (int i = 0; i < childArray.Length - 1; i++)
        {
            for (int j = i + 1; j < childArray.Length; j++)
            {
                if (childArray[i].GetInstanceID() == childArray[j].GetInstanceID())
                {
                    childCollection.Remove(childArray[j]);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Questgiver"))
        {
            AddChildToShip(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Container"))
        {
            AddItemToShip(other.gameObject);
        }
        else
        {
            ItemController item = other.gameObject.GetComponent<ItemController>();
            if ((item == null) && (other.gameObject.transform.parent != null))
            {
                item = other.gameObject.transform.parent.GetComponent<ItemController>();
                if (item != null)
                {
                    AddItemToShip(other.gameObject.transform.parent.gameObject);
                }
            }
            else if (item != null)
            {
                AddItemToShip(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Questgiver"))
        {
            childCollection.Remove(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Container"))
        {
            itemCollection.Remove(other.gameObject);
        }
        else
        {
            ItemController item = other.gameObject.GetComponent<ItemController>();
            if ((item == null) && (other.gameObject.transform.parent != null))
            {
                item = other.gameObject.transform.parent.GetComponent<ItemController>();
                if (item != null)
                {
                    itemCollection.Remove(other.gameObject.transform.parent.gameObject);
                }
            }
            else if (item != null)
            {
                itemCollection.Remove(other.gameObject);
            }
        }
    }
}

[System.Serializable]
public class SmallShipSailManager
{
    [Header("Sails")]
    public GameObject mainSailUp;
    public GameObject mainSailDown;
    [HideInInspector]
    public bool mainSailState;

    public void RaiseSails()
    {
        mainSailDown.SetActive(false);
        mainSailUp.SetActive(true);
        mainSailState = false;
    }

    public void LowerSails()
    {
        mainSailUp.SetActive(false);
        mainSailDown.SetActive(true);
        mainSailState = true;
    }
}
