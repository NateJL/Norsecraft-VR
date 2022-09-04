using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachItem : MonoBehaviour
{
    public GameObject currentItem;

    public enum MountType
    {
        None,
        LeftHip,
        RightHip,
        FrontHip,
        Back
    }

    public MountType mountType = MountType.None;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	public void MountItem(GameObject item)
    {
        if(currentItem == null)
        {
            currentItem = item;
            ItemController itemData = item.GetComponent<ItemController>();
            if (itemData != null)
            {
                switch (mountType)
                {
                    case MountType.LeftHip:
                        if (!currentItem.CompareTag("Tool"))
                        {
                            currentItem.transform.SetPositionAndRotation((transform.position + itemData.mountOffset),
                                                             (transform.rotation * Quaternion.Euler(itemData.mountRotation)));
                            GameManager.manager.playerData.playerInventoryData.leftHipItem = itemData.data.name;
                        }
                        else
                        {
                            currentItem = null;
                            return;
                        }
                        break;

                    case MountType.RightHip:
                        if (!currentItem.CompareTag("Tool"))
                        {
                            currentItem.transform.SetPositionAndRotation((transform.position + itemData.mountOffset),
                                                             (transform.rotation * Quaternion.Euler(itemData.mountRotation)));
                            GameManager.manager.playerData.playerInventoryData.rightHipItem = itemData.data.name;
                        }
                        else
                        {
                            currentItem = null;
                            return;
                        }
                        break;

                    case MountType.FrontHip:
                        if (!currentItem.CompareTag("Sword") && !currentItem.CompareTag("Tool"))
                        {
                            currentItem.transform.SetPositionAndRotation((transform.position + itemData.mountOffset),
                                                             (transform.rotation * Quaternion.Euler(itemData.mountRotation)));
                            GameManager.manager.playerData.playerInventoryData.frontHipItem = itemData.data.name;
                        }
                        else
                        {
                            currentItem = null;
                            return;
                        }
                        break;

                    case MountType.Back:
                        if(currentItem.CompareTag("Tool"))
                        {
                            currentItem.transform.SetPositionAndRotation((transform.position + itemData.mountOffset),
                                                             (transform.rotation * Quaternion.Euler(itemData.mountRotation)));
                            GameManager.manager.playerData.playerInventoryData.backItem = itemData.data.name;
                        }
                        else
                        {
                            currentItem = null;
                            return;
                        }
                        break;

                    default:
                        break;
                }
            }
            else
            {
                currentItem.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }

            currentItem.GetComponent<Rigidbody>().useGravity = false;
            currentItem.GetComponent<Rigidbody>().isKinematic = true;
            currentItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            currentItem.transform.parent = gameObject.transform;
        }
    }

    public void ReleaseItem(GameObject item)
    {
        if(item.GetInstanceID() == currentItem.GetInstanceID())
        {
            currentItem.GetComponent<Rigidbody>().useGravity = true;
            currentItem.GetComponent<Rigidbody>().isKinematic = false;
            currentItem.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            currentItem.transform.parent = null;

            switch (mountType)
            {
                case MountType.LeftHip:
                    GameManager.manager.playerData.playerInventoryData.leftHipItem = "null";
                    break;

                case MountType.RightHip:
                    GameManager.manager.playerData.playerInventoryData.rightHipItem = "null";
                    break;

                case MountType.FrontHip:
                    GameManager.manager.playerData.playerInventoryData.frontHipItem = "null";
                    break;

                case MountType.Back:

                    break;

                default:
                    break;
            }

            currentItem = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(currentItem == null && !other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Container"))
        {
            if(other.gameObject.GetComponent<OVRGrabbable>() != null)
            {
                if(!other.gameObject.GetComponent<OVRGrabbable>().isGrabbed)
                {
                    MountItem(other.gameObject);
                }
            }
            else if(other.transform.parent != null && other.transform.parent.gameObject.GetComponent<OVRGrabbable>() != null)
            {
                if (!other.transform.parent.gameObject.GetComponent<OVRGrabbable>().isGrabbed)
                {
                    MountItem(other.transform.parent.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(currentItem != null)
        {
            ReleaseItem(currentItem);
        }
    }
}
