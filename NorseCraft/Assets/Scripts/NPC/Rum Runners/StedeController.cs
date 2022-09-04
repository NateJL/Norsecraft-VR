using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StedeController : MonoBehaviour
{

    private Animator anim;
    private Rigidbody character_rb;

    [Header("Movement Data")]
    public Transform parentWaypointTransform;
    public bool isMoving = false;
    public float speed = 0.75f;
    public Vector3 jumpForce = new Vector3(0.0f, 7.0f, 0.0f);
    public bool reachedDestination = false;
    public float distance = 0.0f;
    public Vector3 velocity;
    public bool isGrounded = true;
    public int[] jumpIndices;
    public Transform currentTarget;
    public int currentIndex;
    public Transform[] waypoints;

    private bool jumped = false;
    private bool started = false;

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
        if (!started && GameManager.manager.dataManager.questDataCollection[50].saveData.isAccepted)
        {
            isMoving = true;
            started = true;
        }

        velocity = character_rb.velocity;
        if (isMoving)
        {
            distance = Vector3.Distance(transform.position, currentTarget.position);
            if (distance < 0.5f)
            {
                for (int i = 0; i < jumpIndices.Length; i++)
                {
                    if (jumpIndices[i] == currentIndex && !jumped)
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
                isMoving = false;
                character_rb.velocity = Vector3.zero;
                character_rb.freezeRotation = true;
                transform.rotation = currentTarget.rotation;
                currentIndex = 0;
                waypoints = new Transform[1];
            }
        }
        else
        {
            anim.SetFloat("Forward", 0f);
        }
    }

    private void MoveToHomeTransform()
    {
        anim.SetFloat("Forward", 0f);
        isMoving = false;
        character_rb.velocity = Vector3.zero;
        character_rb.freezeRotation = true;
        transform.SetPositionAndRotation(parentWaypointTransform.position, parentWaypointTransform.rotation);
        transform.parent = parentWaypointTransform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Island"))
        {
            if(other.gameObject.GetComponent<IslandController>().data.islandID == 2)
            {
                parentWaypointTransform = other.transform.GetChild(1).GetChild(0);
                Debug.Log("Stede: Entered Shipwrecked Isle");
                currentTarget = other.transform.GetChild(1).GetChild(0);
                reachedDestination = false;
                transform.LookAt(currentTarget);
                anim.SetFloat("Forward", speed);
                character_rb.AddForce(jumpForce, ForceMode.Impulse);
                isMoving = true;
                Invoke("MoveToHomeTransform", 2.0f);
            }
        }
    }
}
