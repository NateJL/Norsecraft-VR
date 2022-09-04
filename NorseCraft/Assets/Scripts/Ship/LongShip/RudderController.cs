using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RudderController : MonoBehaviour
{
    public float minAngle = -45f;
    public float maxAngle = 45f;
    [ShowOnly] public float rudderAngle;

	public void MoveWithHand(Transform handPosition)
    {
        Vector3 lookPos = new Vector3(handPosition.position.x, transform.position.y, handPosition.position.z);
        gameObject.transform.LookAt(lookPos);

        float newAngle = gameObject.transform.localEulerAngles.y;
        if (newAngle > 180f)
            newAngle -= 360f;

        if(newAngle < minAngle)
        {
            newAngle = minAngle;
            gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localRotation.x, (360 - minAngle), gameObject.transform.localRotation.z);
        }
        else if(newAngle > maxAngle)
        {
            newAngle = maxAngle;
            gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localRotation.x, maxAngle, gameObject.transform.localRotation.z);
        }

        if ((newAngle < 1f) && (newAngle > -1f))
            newAngle = 0;

        rudderAngle = newAngle;
    }

    public void ResetRudder()
    {
        rudderAngle = 0.0f;
        gameObject.transform.localRotation = Quaternion.Euler(gameObject.transform.localRotation.x, 0, gameObject.transform.localRotation.z);
    }
}
