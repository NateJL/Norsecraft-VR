using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MediumShipController : MonoBehaviour
{
    [Header("Movement")]
    public float currentSpeed = 0.0f;
    public float maxSpeed = 1.0f;
    public float currentRotationSpeed = 0.0f;
    public float maxRotationSpeed = 1.0f;

    [Header("Steering")]
    public GameObject shipWheel;

    [Header("Child Objects Container")]
    public GameObject childContainer;

    [Header("Data text")]
    public GameObject dataText;

    public MediumShipSailManager sailManager;
    public ShipManager shipManager;

    public List<GameObject> childCollection;
    public List<GameObject> itemCollection;

    [HideInInspector]
    private float wheelAngle = 0.0f;
    private Vector3 oldPosition;
    private Vector3 initialPosition;

    [Header("Float Data")]
    public float speed = 1.0f;
    public float scale = 1.0f;
    public float noiseStrength = 0.0f;
    public float noiseWalk = 1.0f;


    // Use this for initialization
    void Start ()
    {
        shipManager = GetComponent<ShipManager>();
        shipManager.shipData.Size = "Medium";
        shipManager.FindNewComponents();

        childContainer = transform.GetChild(0).gameObject;
        childCollection = new List<GameObject>();
        itemCollection = new List<GameObject>();

        oldPosition = transform.position;
        initialPosition = transform.position;
        sailManager.RaiseAllSails();
    }
	
	// Update is called once per frame
	void Update ()
    {
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

    // Update is called once per frame
    void FixedUpdate()
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
        Vector3 floatRotation = Vector3.right * Mathf.Sin(Time.time * speed + (transform.position.y));
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
        wheelAngle = shipManager.shipWheelTransform.GetChild(0).GetComponent<SteeringController>().turnAngle;

        maxSpeed = 0.0f;
        maxRotationSpeed = 1f;
        if (sailManager.frontSailState)
        {
            maxSpeed += 2;
        }
        if (sailManager.midSailState)
        {
            maxSpeed += 5;
        }
        if (sailManager.backSailState)
        {
            maxSpeed += 2;
        }

        if (currentSpeed < (maxSpeed - 0.01f))
            currentSpeed += 0.01f;
        else if (currentSpeed > (maxSpeed + 0.01f))
            currentSpeed -= 0.01f;
        else
            currentSpeed = maxSpeed;
        currentRotationSpeed = (wheelAngle / 30) * maxRotationSpeed;
    }

    public void MoveShip()
    {
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
    }

    public void TurnWheel(float addedAngle)
    {
        wheelAngle += addedAngle;
        if (wheelAngle < -180)
            wheelAngle = -180;
        else if (wheelAngle > 180)
            wheelAngle = 180;
        shipManager.shipWheelTransform.GetChild(0).transform.localRotation = Quaternion.Euler(new Vector3(0, 180, wheelAngle));
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            childCollection.Add(other.gameObject);
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
        if (other.gameObject.CompareTag("Player"))
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
public class MediumShipSailManager
{
    [Header("Front Sails")]
    public GameObject frontSailUp;
    public GameObject frontSailDown;
    [HideInInspector]
    public bool frontSailState;

    [Header("Mid Sails")]
    public GameObject midSailUp;
    public GameObject midSailDown;
    [HideInInspector]
    public bool midSailState;

    [Header("Back Sails")]
    public GameObject backSailUp;
    public GameObject backSailDown;
    [HideInInspector]
    public bool backSailState;

    public void RaiseFrontSail()
    {
        frontSailDown.SetActive(false);
        frontSailUp.SetActive(true);
        frontSailState = false;
    }

    public void LowerFrontSail()
    {
        frontSailUp.SetActive(false);
        frontSailDown.SetActive(true);
        frontSailState = true;
    }

    public void RaiseMidSail()
    {
        midSailDown.SetActive(false);
        midSailUp.SetActive(true);
        midSailState = false;
    }

    public void LowerMidSail()
    {
        midSailUp.SetActive(false);
        midSailDown.SetActive(true);
        midSailState = true;
    }

    public void RaiseBackSail()
    {
        backSailDown.SetActive(false);
        backSailUp.SetActive(true);
        backSailState = false;
    }

    public void LowerBackSail()
    {
        backSailUp.SetActive(false);
        backSailDown.SetActive(true);
        backSailState = true;
    }

    public void RaiseAllSails()
    {
        frontSailDown.SetActive(false);
        midSailDown.SetActive(false);
        backSailDown.SetActive(false);
        frontSailUp.SetActive(true);
        midSailUp.SetActive(true);
        backSailUp.SetActive(true);
        frontSailState = midSailState = backSailState = false;
    }

    public void LowerAllSails()
    {
        frontSailUp.SetActive(false);
        midSailUp.SetActive(false);
        backSailUp.SetActive(false);
        frontSailDown.SetActive(true);
        midSailDown.SetActive(true);
        backSailDown.SetActive(true);
        frontSailState = midSailState = backSailState = true;
    }
}
