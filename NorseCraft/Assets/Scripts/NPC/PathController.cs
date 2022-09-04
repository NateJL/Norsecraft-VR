using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody character_rb;

    [Header("Movement Data")]
    public Transform parentWaypointTransform;
    public bool isMoving = false;
    public float speed = 1f;
    public Vector3 jumpForce = new Vector3(1.0f, 7.0f, 0.0f);
    public bool reachedDestination = false;
    public float distance = 0.0f;
    public Vector3 velocity;
    public bool isGrounded = true;
    public int[] jumpIndices;
    public Transform currentTarget;
    public int currentIndex;
    public Transform[] waypoints;

    private bool jumped = false;

    // Use this for initialization
    void Start()
    {
        character_rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        waypoints = new Transform[parentWaypointTransform.childCount];

        int counter = 0;
        foreach (Transform child in parentWaypointTransform)
        {
            waypoints[counter] = parentWaypointTransform.GetChild(counter);
            counter++;
        }

        currentIndex = 0;
        currentTarget = waypoints[currentIndex];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.manager.dataManager.questDataCollection[50].saveData.isAccepted)
        {
            isMoving = true;
        }

        distance = Vector3.Distance(transform.position, currentTarget.position);
        velocity = character_rb.velocity;
        if (isMoving)
        {
            if (distance < 0.5f)
            {
                for(int i = 0; i < jumpIndices.Length; i++)
                {
                    if(jumpIndices[i] == currentIndex && !jumped)
                    {
                        jumped = true;
                        isGrounded = false;
                        character_rb.AddForce(jumpForce, ForceMode.Impulse);
                        break;
                    }
                }
                reachedDestination = true;
                if (currentIndex < waypoints.Length - 1)
                {
                    currentIndex++;
                    currentTarget = waypoints[currentIndex];
                    reachedDestination = false;
                }
            }
            
            if (!reachedDestination)
            {
                //anim.SetBool("OnGround", isGrounded);
                anim.SetFloat("Forward", speed);
                anim.SetFloat("Jump", velocity.y);

                transform.LookAt(currentTarget);
            }
            else
            {
                anim.SetFloat("Forward", 0f);
                transform.rotation = currentTarget.rotation;
            }
        }
        else
        {
            anim.SetFloat("Forward", 0f);
        }
    }
}
