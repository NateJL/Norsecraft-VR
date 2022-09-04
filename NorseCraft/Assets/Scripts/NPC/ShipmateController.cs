using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipmateController : MonoBehaviour
{
    [Header("NPC Data")]
    public float speed = 1.0f;
    public float followDistance = 2.0f;

    [Space(10)]
    public GameObject player;
    public PlayerTrackingData playerTracker;

    private CharacterController controller;
    private Animator anim;

	// Use this for initialization
	void Start ()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogError(gameObject.name + ": Failed to get CharacterController");

        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.LogError(gameObject.name + ": Failed to get Animator");

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            Debug.LogError(gameObject.name + ": Failed to get Player GameObject");
		
	}

    private void FixedUpdate()
    {
        playerTracker.currentPos = player.transform.position;
        playerTracker.speed = ((playerTracker.currentPos - playerTracker.previousPos).magnitude)/Time.deltaTime;
        playerTracker.previousPos = player.transform.position;
        speed = playerTracker.speed;

        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

        if (Vector3.Distance(player.transform.position, transform.position) > followDistance)
        {
            controller.SimpleMove(transform.forward * speed);
        }
    }
}

[System.Serializable]
public class PlayerTrackingData
{
    public float speed;
    [Space(5)]
    public Vector3 previousPos;
    public Vector3 currentPos;

    public PlayerTrackingData()
    {
        previousPos = new Vector3();
        currentPos = new Vector3();
        speed = 0.0f;
    }
}
