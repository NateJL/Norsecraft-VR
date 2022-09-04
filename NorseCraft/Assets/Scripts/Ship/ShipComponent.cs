using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    public Vector3 displayPositionOffset;
    public Vector3 displayRotationOffset;
    public Vector3 displayScale;

    public ShipComponentData data;
}

[System.Serializable]
public class ShipComponentData
{
    public enum ShipComponentType
    {
        None,
        Hull,
        Wheel,
        Sails,
        Flag,
        BoatHead
    }

    public string name;
    public ShipComponentType componentType = ShipComponentType.None;
    public int value;
}
