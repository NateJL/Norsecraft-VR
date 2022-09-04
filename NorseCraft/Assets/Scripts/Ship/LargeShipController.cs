using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ShipManager))]
public class LargeShipController : MonoBehaviour
{
    [Header("Movement")]
    public float currentSpeed = 0.0f;
    public float maxSpeed = 1.0f;
    public float currentRotationSpeed = 0.0f;
    public float maxRotationSpeed = 1.0f;

    [Header("Steering")]
    public GameObject shipWheel;

    [Header("Data text")]
    public GameObject dataText;

    public LargeShipSailManager sailManager;

    public List<GameObject> childCollection;

    private float wheelAngle = 0.0f;
    private Vector3 oldPosition;
    private Vector3 initialPosition;

    [Header("Float Data")]
    public float speed = 0.5f;
    public float scale = 64f;
    public float noiseStrength = 0.0f;
    public float noiseWalk = 1.0f;

    // Use this for initialization
    void Start()
    {
        childCollection = new List<GameObject>();
        oldPosition = transform.position;
        initialPosition = transform.position;
        sailManager.RaiseAllSails();
        GetComponent<ShipManager>().shipData.Size = "Large";
    }

    // Update is called once per frame
    void Update()
    {
        dataText.GetComponent<TextMeshProUGUI>().SetText("Position: " + transform.position.ToString() +
                                                        "\nVelocity: " + currentSpeed + "/" + maxSpeed +
                                                        "\nRotation: " + currentRotationSpeed + " *" +
                                                        "\nWheel Angle: " + wheelAngle + " *");


    }

    // Update is called once per frame
    void FixedUpdate ()
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
        //Debug.Log(Mathf.Sin(Time.time * speed + (transform.position.y)));
        vertex.y += Mathf.Sin(Time.time * speed + transform.position.y) * scale;
        vertex.y += Mathf.PerlinNoise(vertex.x + noiseWalk, vertex.y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
        vertex.Normalize();
        //Debug.Log(floatRotation.ToString());
        vertex.y += initialPosition.y;
        transform.position = new Vector3(transform.position.x, vertex.y, transform.position.z);
        transform.Rotate(floatRotation * Time.deltaTime);
    }

    /*
     * Calculate movement speed
     */
    private void CalculateSpeeds()
    {
        maxSpeed = 0.0f;
        maxRotationSpeed = 1f;
        if (sailManager.frontSailState)
        {
            maxSpeed += 2;
        }
        if(sailManager.midSailState)
        {
            maxSpeed += 5;
        }
        if (sailManager.backSailState)
        {
            maxSpeed += 2;
        }
        if(sailManager.headSailState)
        {
            maxSpeed += 1;
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

        Vector3 changePosition = transform.position - oldPosition + new Vector3(0.001f, 0.001f, 0.001f);
        foreach (GameObject child in childCollection)
        {
            child.transform.position += changePosition;
            //if (child.CompareTag("Player"))
            //    child.GetComponent<CharacterController>().Move(changePosition);
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
        shipWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, wheelAngle));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //other.gameObject.transform.parent = gameObject.transform;
            childCollection.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //other.gameObject.transform.parent = null;
            childCollection.Remove(other.gameObject);
        }
    }
}


[System.Serializable]
public class LargeShipSailManager
{
    [Header("Head Sails")]
    public GameObject headSailUp;
    public GameObject headSailDown;
    [HideInInspector]
    public bool headSailState;

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

    public void RaiseHeadSail()
    {
        headSailDown.SetActive(false);
        headSailUp.SetActive(true);
        headSailState = false;
    }

    public void LowerHeadSail()
    {
        headSailUp.SetActive(false);
        headSailDown.SetActive(true);
        headSailState = true;
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
        headSailDown.SetActive(false);
        frontSailUp.SetActive(true);
        midSailUp.SetActive(true);
        backSailUp.SetActive(true);
        headSailUp.SetActive(true);
        frontSailState = midSailState = backSailState = headSailState = false;
    }

    public void LowerAllSails()
    {
        frontSailUp.SetActive(false);
        midSailUp.SetActive(false);
        backSailUp.SetActive(false);
        headSailUp.SetActive(false);
        frontSailDown.SetActive(true);
        midSailDown.SetActive(true);
        backSailDown.SetActive(true);
        headSailDown.SetActive(true);
        frontSailState = midSailState = backSailState = headSailState = true;
    }
}
