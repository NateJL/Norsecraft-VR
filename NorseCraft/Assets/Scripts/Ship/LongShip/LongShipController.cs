using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LongShipController : MonoBehaviour
{
    [Header("Movement")]
    [ShowOnly] public float currentSpeed = 0.0f;
    [ShowOnly] public float currentMaxSpeed = 0.0f;
    [ShowOnly] public float currentRotationSpeed = 0.0f;
    public float maxSpeed = 1.0f;
    public float rotationMultiplier = 1.0f;
    [ShowOnly] public Vector3 collisionNormals = Vector3.zero;

    [Header("Ship References")]
    public ShipManager shipManager;
    public GameObject shipRudder;

    [Header("Data text")]
    public GameObject dataText;

    [Header("Child Objects Container")]
    private GameObject childContainer;
    public List<GameObject> childCollection;

    private float rudderAngle = 0.0f;
    private Vector3 oldPosition;
    private Vector3 initialPosition;

    private ShipRaycaster raycaster;

    [Header("Float Data")]
    public float speed = 1.0f;
    public float scale = 1.0f;
    public float noiseStrength = 0.0f;
    public float noiseWalk = 1.0f;

    // Use this for initialization
    void Start ()
    {
        shipManager = GetComponent<ShipManager>();
        shipManager.FindNewComponents();
        childContainer = transform.GetChild(0).gameObject;
        childCollection = new List<GameObject>();

        oldPosition = transform.position;
        initialPosition = transform.position;

        raycaster = transform.GetChild(4).gameObject.GetComponent<ShipRaycaster>();
        if (raycaster == null)
            Debug.LogError(gameObject.name + ": Failed to get raycaster");
    }
	
	// Update is called once per frame
	void Update ()
    {
        dataText.GetComponent<TextMeshProUGUI>().SetText("Position: " + transform.position.ToString() +
                                                        "\nVelocity: " + currentSpeed + "/" + currentMaxSpeed +
                                                        "\nRotation: " + currentRotationSpeed + " *" +
                                                        "\nRudder Angle: " + rudderAngle + " *");
    }

    private void FixedUpdate()
    {
        if (shipRudder != null)
            rudderAngle = shipRudder.GetComponent<RudderController>().rudderAngle;
        else
            rudderAngle = 0;
        currentRotationSpeed = rudderAngle * rotationMultiplier;

        CalculateBuoyancy();
        CalculateVelocity();
        MoveShip();
    }

    /*
     * Calculate the height to simulate rolling of waves
     */
    private void CalculateBuoyancy()
    {
        Vector3 vertex = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 floatRotation = Vector3.right * (Mathf.Sin(Time.time * speed + (transform.position.y)) / 2);
        vertex.y += Mathf.Sin(Time.time * speed + transform.position.y) * scale;
        //vertex.y += Mathf.PerlinNoise(vertex.x + noiseWalk, vertex.y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
        vertex.Normalize();
        vertex.y += initialPosition.y;
        transform.position = new Vector3(transform.position.x, vertex.y, transform.position.z);
        transform.Rotate(floatRotation * Time.deltaTime);
    }

    /*
     * Calculate speed and direction of ship
     */
    private void CalculateVelocity()
    {
        if (shipManager.shipComponentManager.sailing)
        {
            currentMaxSpeed = maxSpeed;
        }
        else
            currentMaxSpeed = 0.0f;

        if (raycaster.frontCenterData.distance > raycaster.distanceThreshold)      // if there are no objects in front of the ship
        {
            if (currentSpeed < (currentMaxSpeed - 0.01f))
                currentSpeed += 0.01f;
            else if (currentSpeed > (currentMaxSpeed + 0.01f))
                currentSpeed -= 0.01f;
            else
                currentSpeed = currentMaxSpeed;
        }
        else
        {
            if (currentSpeed > 2)
                currentSpeed = 2;
            currentSpeed -= 0.1f;
            if (currentSpeed < -1f)
                currentSpeed = -1f;
        }

        if (raycaster.frontRightData.distance < raycaster.distanceThreshold)     // front right collision
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

    private void MoveShip()
    {
        transform.Translate((Vector3.forward + collisionNormals) * currentSpeed * Time.deltaTime, Space.Self);

        if (!(currentSpeed > 0))            // rotate slower when not moving
            currentRotationSpeed *= 0.1f;

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

    public void Dock(Transform dockTransform)
    {
        currentSpeed = 0.0f;
        shipManager.shipComponentManager.RaiseSails();
        shipRudder.GetComponent<RudderController>().ResetRudder();
        transform.SetPositionAndRotation(dockTransform.position, dockTransform.rotation);
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
        if (other.gameObject.CompareTag("Player"))
        {
            AddChildToShip(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            childCollection.Remove(other.gameObject);
        }
    }
}
