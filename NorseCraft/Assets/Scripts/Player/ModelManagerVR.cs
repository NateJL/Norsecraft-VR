using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelManagerVR : MonoBehaviour
{
    [Header("OVR Tracking Components")]
    public Transform centerEyeTransform;
    [Space(10)]
    public Transform leftHandAnchor;
    public Transform rightHandAnchor;

    [Header("Base Model")]
    public Transform baseModelTransform;
    public Vector3 baseModelOffset;

    [Header("Root")]
    public Transform rootTransform;

    [Header("Head")]
    public Transform headTransform;
    public Vector3 headRotationOffset;
    public Vector3 headTransformOffset;

    [Header("Body")]
    public Transform spine03Transform;
    //public Vector3 spine03Offset;
    [Space(10)]
    public Transform spine02Transform;
    //public Vector3 spine02Offset;
    [Space(10)]
    public Transform spine01Transform;
    //public Vector3 spine01Offset;
    [Space(10)]
    public Transform upperLegLeftTransform;
    public Vector3 upperLegLeftOffset;
    [Space(10)]
    public Transform upperLegRightTransform;
    public Vector3 upperLegRightOffset;

    [Header("Left Arm")]
    public Transform leftHandTransform;
    public Vector3 leftHandRotationOffset;
    public Transform leftElbowTransform;
    public Transform leftShoulderTransform;
    public Transform leftClavicleTransform;
    
    [Header("Right Arm")]
    public Transform rightHandTransform;
    public Vector3 rightHandRotationOffset;
    public Transform rightElbowTransform;
    public Transform rightShoulderTransform;
    public Transform rightClavicleTransform;

    [Header("Initial Scale")]
    public Vector3 initialRootPosition;
    public Vector3 initialHeadPosition;
    public Vector3 initialSpine02Position;
    public Vector3 initialSpine01Position;
    public Vector3 initialUpperLegLeft;
    public Vector3 initialUpperLegRight;
    [Space(10)]
    public Vector3 initialSpine03Position;
    [Space(5)]
    public Vector3 initialLeftClavicle;
    public Vector3 initialLeftShoulder;
    public Vector3 initialLeftElbow;
    public Vector3 initialLeftHand;
    [Space(5)]
    public Vector3 initialRightClavicle;
    public Vector3 initialRightShoulder;
    public Vector3 initialRightElbow;
    public Vector3 initialRighttHand;

    [Header("Debug")]
    public float centerEyeY;
    public float initialHeadY;
    public float heightScale = 1f;

	// Use this for initialization
	void Start ()
    {
        baseModelTransform.localPosition = (new Vector3(centerEyeTransform.localPosition.x, baseModelTransform.localPosition.y, centerEyeTransform.localPosition.z) + baseModelOffset);
        initialRootPosition = rootTransform.position;
        initialHeadPosition = headTransform.position;

        initialSpine03Position = spine03Transform.position;
        initialSpine02Position = spine02Transform.position;
        initialSpine01Position = spine01Transform.position;

        initialUpperLegLeft = upperLegLeftTransform.position;
        initialUpperLegRight = upperLegRightTransform.position;
        upperLegLeftOffset = upperLegLeftTransform.localPosition;
        upperLegRightOffset = upperLegRightTransform.localPosition;

        initialLeftShoulder = leftShoulderTransform.position;
        initialLeftClavicle = leftClavicleTransform.position;
        initialLeftElbow = leftElbowTransform.position;
        initialLeftHand = leftHandTransform.position;

        initialRightShoulder = rightShoulderTransform.position;
        initialRightClavicle = rightClavicleTransform.position;
        initialRightElbow = rightElbowTransform.position;
        initialRighttHand = rightHandTransform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //baseModelTransform.localPosition = (new Vector3(centerEyeTransform.localPosition.x, baseModelTransform.localPosition.y, centerEyeTransform.localPosition.z) + baseModelOffset);
        CalculateScale();
        CalculateArms();
    }

    private void CalculateScale()
    {
        headTransform.SetPositionAndRotation(centerEyeTransform.position + headTransformOffset, (centerEyeTransform.rotation * Quaternion.Euler(headRotationOffset)));

        centerEyeY = headTransform.position.y ;
        initialHeadY = initialHeadPosition.y;
        heightScale = centerEyeY / initialHeadY;
        //spine03Offset = spine03Transform.position;
        //spine02Offset = spine02Transform.position;
        //spine01Offset = spine01Transform.position;
        
        spine03Transform.position = new Vector3(        (centerEyeTransform.position.x),        (initialSpine03Position.y * heightScale),   (centerEyeTransform.position.z));
        spine02Transform.position = new Vector3(        (centerEyeTransform.position.x),        (initialSpine02Position.y * heightScale),   (centerEyeTransform.position.z));
        spine01Transform.position = new Vector3(        (centerEyeTransform.position.x),        (initialSpine01Position.y * heightScale),   (centerEyeTransform.position.z));

        upperLegLeftTransform.position = new Vector3(   (upperLegLeftTransform.position.x),     (initialUpperLegLeft.y * heightScale),      (upperLegLeftTransform.position.z));
        upperLegRightTransform.position = new Vector3(  (upperLegRightTransform.position.x),    (initialUpperLegRight.y * heightScale),     (upperLegRightTransform.position.z));

        rootTransform.position = new Vector3(centerEyeTransform.position.x, (initialRootPosition.y * heightScale), centerEyeTransform.position.z);
    }

    private void CalculateArms()
    {
        leftHandTransform.SetPositionAndRotation(leftHandAnchor.position, (leftHandAnchor.rotation * Quaternion.Euler(leftHandRotationOffset)));
        rightHandTransform.SetPositionAndRotation(rightHandAnchor.position, (rightHandAnchor.rotation * Quaternion.Euler(rightHandRotationOffset)));

        leftClavicleTransform.position = new Vector3((leftClavicleTransform.position.x), (initialLeftClavicle.y * heightScale), (leftClavicleTransform.position.z));

        rightClavicleTransform.position = new Vector3((rightClavicleTransform.position.x), (initialRightClavicle.y * heightScale), (rightClavicleTransform.position.z));

        // Calculate left should position
        // Slerp left should rotation from left Clavicle and left Hand
        leftShoulderTransform.position = new Vector3((leftShoulderTransform.position.x), (initialLeftShoulder.y * heightScale), (leftShoulderTransform.position.z));
        leftShoulderTransform.rotation = Quaternion.Slerp(leftClavicleTransform.rotation, leftHandTransform.rotation, 0.83f);

        // Calculate right shoulder position fron height
        // Slerp right shoulder rotation from right Clavicle and right Hand
        rightShoulderTransform.position = new Vector3((rightShoulderTransform.position.x), (initialRightShoulder.y * heightScale), (rightShoulderTransform.position.z));
        rightShoulderTransform.rotation = Quaternion.Slerp(rightClavicleTransform.rotation, rightHandTransform.rotation, 0.83f);

        // Slerp left elbow position from left Shoulder and left Hand
        // Slerp left elbow rotation from left Shoulder and left Hand
        leftElbowTransform.position = Vector3.Slerp(leftShoulderTransform.position, leftHandTransform.position, 0.45f);
        leftElbowTransform.rotation = Quaternion.Slerp(leftShoulderTransform.rotation, leftHandTransform.rotation, 0.45f);

        // Slerp right elbow position from right Shoulder and right Hand
        // Slerp right elbow rotation from right Shoulder and right Hand
        rightElbowTransform.position = Vector3.Slerp(rightShoulderTransform.position, rightHandTransform.position, 0.45f);
        rightElbowTransform.rotation = Quaternion.Slerp(rightShoulderTransform.rotation, rightHandTransform.rotation, 0.45f);
    }

    /*
     * Function to invert the rotation on the y-axis
     */
    private Quaternion InverseYCoordinate(Quaternion q)
    {
        return new Quaternion(-q.x, q.y, q.z, q.w);
    }
}
