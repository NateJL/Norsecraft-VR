using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemController : MonoBehaviour
{
    public bool isGrabbable;
    public bool isGrabbed;

    public enum ItemType
    {
        None,
        Sword,
        Tool,
        Flintlock,
        Container,
        Axe,
        Resource,
        Food
    }

    public ItemType itemType = ItemType.None;

    [Tooltip("If the item is part of the object pool, the scale will be set automatically by the pool manager.")]
    public Vector3 baseScale;

    [Header("Transform Data")]
    public Vector3 grabOffset;
    public Vector3 grabRotation;
    [Space(10)]
    public Vector3 mountOffset;
    public Vector3 mountRotation;
    [Space(10)]
    public Vector3 spawnOffset;
    public Vector3 spawnRotation;

    [Header("Display Transform Data")]
    public Vector3 displayOffset;
    public Vector3 displayRotation;
    public Vector3 displayScale;

    [Space(20)]
    public ItemData data;

    [Space(20)]
    public bool isActive = true;
    public float ActionTime = 2.0f;
    public float timeToActive = 0.0f;
    private float lastActionTime = 0.0f;

    public AudioClip[] audioClips;

    private Rigidbody item_rb;

    private void Start()
    {
        isGrabbed = false;
        item_rb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform newParent)
    {
        transform.parent = newParent;
        item_rb.isKinematic = true;
        item_rb.useGravity = false;
        item_rb.freezeRotation = true;
        item_rb.velocity = Vector3.zero;
        isGrabbed = true;

        if((itemType == ItemType.Sword) || 
            (itemType == ItemType.Tool) || 
            (itemType == ItemType.Flintlock) || 
            (itemType == ItemType.Axe))
        {
            //item_rb.position = grabOffset;
            //item_rb.rotation = Quaternion.Euler(grabRotation);
            transform.localPosition = grabOffset;
            transform.localRotation = Quaternion.Euler(grabRotation);
        }

        if(itemType == ItemType.Container)
        {
            if(transform.GetChild(0).GetComponent<ContainerLidController>() != null)
            {
                transform.GetChild(0).GetComponent<ContainerLidController>().ResetAngle();
            }
        }

        lastActionTime = Time.time;
        timeToActive = 0.0f;
        isActive = true;
    }

    public void Release()
    {
        transform.parent = null;
        item_rb.useGravity = true;
        item_rb.isKinematic = false;
        item_rb.freezeRotation = false;
        isGrabbed = false;

        if (itemType == ItemType.Tool && GetComponent<FishingPoleController>() != null)
        {
            GetComponent<FishingPoleController>().ReleasePole();
        }

        isActive = false;
    }

    public void UpdateItem()
    {
        if(itemType == ItemType.Axe)
        {
            timeToActive = Time.time - lastActionTime;
            if(timeToActive > ActionTime)
            {
                lastActionTime = Time.time;
                timeToActive = 0.0f;
                isActive = true;
            }
        }
        else
        {
            isActive = true;
        }
    }

    public void UseItem()
    {
        lastActionTime = Time.time;
        timeToActive = 0.0f;
        isActive = false;

        if(audioClips.Length > 0 && audioClips[0] != null)
            GameManager.manager.soundManager.PlayAudioClip(audioClips[0].name);
    }
}
