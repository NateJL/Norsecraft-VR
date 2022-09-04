using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerLidController : MonoBehaviour
{
    public Vector3 angle;
    private Transform frontFace;

    private void Start()
    {
        frontFace = transform.parent.GetChild(1).GetChild(0);
        angle = transform.localEulerAngles;
    }

    public void ResetAngle()
    {
        transform.localEulerAngles = Vector3.zero;
    }

    public void UpdateLidRotation(Transform handPosition)
    {
        Vector3 lookPos = new Vector3(frontFace.position.x, handPosition.position.y, frontFace.position.z);
        gameObject.transform.LookAt(lookPos);
        angle = transform.localEulerAngles;
    }
}
