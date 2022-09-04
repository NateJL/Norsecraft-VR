using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonController : MonoBehaviour
{
    public float lookRadius = 8f;

    Transform target;
    //NavMeshAgent agent;
    //CharacterController controller;

    Animator anim;

    public float minDistance = 1.0f;
    public int attackType;
    public bool inRange;
    public bool walking;
    public bool attack;
    private bool attacking;
    public bool recoil;

    private float health = 100;
    public float timeBetweenDamageTaken = 2.0f;
    private float damageTimer = 0.0f;

    public GameObject healthBar;

    [Header("Loot")]
    public GameObject loot;

    // Use this for initialization
    void Start ()
    {
        target = GameManager.manager.player.transform;
        //agent = GetComponent<NavMeshAgent>();
        //controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        walking = false;
        attacking = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if((distance <= lookRadius) && (health > 0))
        {
            inRange = true;
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            //controller.Move(Vector3.forward);
            //agent.SetDestination(target.position);

            if(distance <= minDistance)
            {
                // attack
                walking = false;

                if (!attacking)
                {
                    attacking = true;
                    attack = true;
                }

                FaceTarget();
            } else
            {
                attack = false;
                walking = true;
            }
        } else
        {
            inRange = false;
        }

        /*
        Vector3 curMove = transform.position - previousPosition;
        curSpeed = curMove.magnitude / Time.deltaTime;
        previousPosition = transform.position;
        */

        anim.SetInteger("attackType", attackType);
        anim.SetBool("inRange", inRange);
        anim.SetBool("walking", walking);
        anim.SetBool("attack", attack);
        anim.SetBool("recoil", recoil);

        if (attacking)
            Invoke("Attacked", 1.0f);
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void Damage(float damage)
    {
        if (Time.time - damageTimer > timeBetweenDamageTaken)
        {
            damageTimer = Time.time;
            health -= damage;
            float healthBarWidth = 120f * ((float)health / 100f);
            healthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthBarWidth);

            if (health <= 0)
            {
                anim.SetBool("isDead", true);
                Invoke("Death", 5.0f);
            }
        }
    }

    public void Blocked()
    {
        recoil = true;
        Invoke("ResetStance", 1.0f);
    }

    private void ResetStance()
    {
        recoil = false;
    }

    private void Attacked()
    {
        attacking = false;
        attack = true;
        attackType = Random.Range(1, 6);
    }

    public void Death()
    {
        Instantiate(loot, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Blade"))
        {
            Damage(20);
        }
        else if (other.gameObject.CompareTag("Bullet"))
        {
            Damage(25);
        }
    }
}
