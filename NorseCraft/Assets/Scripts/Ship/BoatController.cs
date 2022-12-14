using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("Movement")]
    public float currentSpeed = 0.0f;
    public float maxSpeed = 1.0f;
    public float currentRotationSpeed = 0.0f;
    public float maxRotationSpeed = 1.0f;

    public SailController sailController;

    [Header("Steering")]
    public GameObject shipWheel;

    [Header("Data text")]
    public GameObject dataText;

    public List<GameObject> childCollection;
    private int childCollectionSize;

    [HideInInspector]
    public float wheelAngle = 0.0f;
    private Vector3 oldPosition;

	// Use this for initialization
	void Start ()
    {
        childCollection = new List<GameObject>();

        oldPosition = transform.position;
        sailController.RaiseAllSails();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        dataText.GetComponent<TextMeshProUGUI>().SetText("Position: " + transform.position.ToString() + 
                                                        "\nVelocity: " + currentSpeed + "/" + maxSpeed +
                                                        "\nRotation: " + currentRotationSpeed + " *" +
                                                        "\nWheel Angle: " + wheelAngle + " *");

        CalculateSpeeds();
        MoveShip();
    }

    private void CalculateSpeeds()
    {
        maxSpeed = 0.0f;
        maxRotationSpeed = 1f;
        if(sailController.frontSailState)
        {
            maxSpeed += 2;
        }
        if(sailController.midSailState)
        {
            maxSpeed += 4;
        }
        if(sailController.backSailState)
        {
            maxSpeed += 1;
            maxRotationSpeed += 1;
        }

        if (currentSpeed < (maxSpeed - 0.01f))
            currentSpeed += 0.01f;
        else if (currentSpeed > (maxSpeed + 0.01f))
            currentSpeed -= 0.01f;
        else
            currentSpeed = maxSpeed;
        currentRotationSpeed = (wheelAngle / 15) * maxRotationSpeed;
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
        shipWheel.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, wheelAngle));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
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
public class SailController
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
