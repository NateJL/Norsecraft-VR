using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCollider : MonoBehaviour
{
    public bool isParent = false;

	// Use this for initialization
	void Start ()
    {

	}

    private void OnDrawGizmos()
    {
        if (isParent)
        {
            Gizmos.color = new Color32(0, 255, 0, 119);
            foreach (Transform obj in transform)
            {
                if (obj.gameObject.GetComponent<BoxCollider>() != null)
                {
                    Matrix4x4 rotationMatrix = Matrix4x4.TRS(obj.position, obj.rotation, obj.lossyScale);
                    Gizmos.matrix = rotationMatrix;
                    Gizmos.DrawCube(Vector3.zero, obj.gameObject.GetComponent<BoxCollider>().size);
                }
            }
        }
        else
        {
            if (GetComponent<BoxCollider>() != null)
            {
                Gizmos.color = new Color32(0, 255, 0, 119);
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(transform.position, GetComponent<BoxCollider>().size);
            }
        }
    }
}
