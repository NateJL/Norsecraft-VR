using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralComponent : MonoBehaviour
{
    public enum ComponentType
    {
        None = 0,
        Base,
        Prop,
        Grass,
        NUM_COMPONENT_TYPES
    }
    public ComponentType componentType = ComponentType.None;

    public Vector3 groundOffset;
    public Vector3 rotationOffset;
    [Space(15)]
    public float childWidth;
}
