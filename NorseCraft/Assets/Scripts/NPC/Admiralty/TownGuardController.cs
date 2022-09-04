using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownGuardController : MonoBehaviour
{
    public NPCData npcData;

    [Header("Soldier Parameters")]
    public float lookRadius = 8.0f;
    public float health = 100.0f;
    public float minDistance = 1.5f;
    public float timeBetweenDamageTaken = 2.0f;
    [Space(10)]
    public GameObject myWeapon;
    public GameObject healthBar;

    [Header("Target Data")]
    public Transform target;
    public bool hostile = false;

    [Header("Animator values")]
    public float attackActivateTime = 0.1f;
    public float attackDeactivateTime = 0.6f;
    public bool walk = false;
    public bool run = false;
    public bool attack = false;
    public bool recoil = false;

    private bool inRange = false;
    private bool attacking = false;
    private float damageTimer = 0.0f;

    private Animator anim;

    // Use this for initialization
    void Start ()
    {
        anim = GetComponent<Animator>();
        target = GameManager.manager.player.transform;
        DeactivateWeaponCollider();

        npcData.factionID = Array.IndexOf(Enum.GetValues(npcData.faction.GetType()), npcData.faction) - 1;

        healthBar.transform.parent.gameObject.SetActive(false);

        switch (npcData.actionType)
        {
            case NPCData.ActionType.Stand:
                Debug.Log(gameObject.GetInstanceID() + "Standing Guard");
                break;

            case NPCData.ActionType.Path:
                Debug.Log(gameObject.GetInstanceID() + "Pathing Guard");
                break;

            default:
                break;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        IsPlayerHostile(false);

        if (hostile)
        {
            float distance = Vector3.Distance(target.position, transform.position);

            if ((distance <= lookRadius) && (health > 0))
            {
                inRange = true;

                transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

                if (distance <= minDistance)
                {
                    // attack
                    walk = run = false;
                    attack = true;
                }
                else
                {
                    run = true;
                    walk = false;
                    attack = false;
                }
            }
            else
            {
                walk = run = false;
            }
        }
        else
        {
            switch (npcData.actionType)
            {
                case NPCData.ActionType.Stand:

                    break;

                case NPCData.ActionType.Path:
                    if (npcData.currentDestination != null)
                    {
                        transform.LookAt(new Vector3(npcData.currentDestination.transform.position.x,
                                                        transform.position.y,
                                                        npcData.currentDestination.transform.position.z));
                        walk = true;
                    }
                    else
                        walk = false;

                    if(npcData.currentDestination.GetComponent<Waypoint>().ReachedWaypoint(transform.position))
                    {
                        npcData.currentDestination = npcData.currentDestination.GetComponent<Waypoint>().GetRandomWaypoint();
                        transform.LookAt(new Vector3(npcData.currentDestination.transform.position.x, 
                                                        npcData.currentDestination.transform.position.y, 
                                                        npcData.currentDestination.transform.position.z));
                    }
                    break;

                default:
                    break;
            }
        }

        anim.SetBool("isHostile", hostile);
        anim.SetBool("walk", walk);
        anim.SetBool("run", run);
        anim.SetBool("attack", attack);
        anim.SetBool("recoil", recoil);

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("SpearAttack1"))
        {
            if(!attacking)
            {
                attacking = true;
                Invoke("ActivateWeaponCollider", attackActivateTime);
                Invoke("DeactivateWeaponCollider", attackDeactivateTime);
            }
        }
        else
        {
            attacking = false;
            myWeapon.GetComponent<BoxCollider>().enabled = false;
        }
    }

    /*
     * Function to activate the Characters weapon collider to allow collisions between 
     * other Characters and weapons.
     */
    private void ActivateWeaponCollider()
    {
        myWeapon.GetComponent<BoxCollider>().enabled = true;
    }

    /*
     * Function to deactivate the Characters weapon collider to avoid additional damage
     * during animation.
     */
    private void DeactivateWeaponCollider()
    {
        myWeapon.GetComponent<BoxCollider>().enabled = false;
    }

    /*
     * Function called when an attack is blocked
     */
    public void Blocked()
    {
        Debug.Log("Blocked");
        recoil = true;
        Invoke("ResetStance", 1.0f);
    }

    /*
     * Function to reset stance
     */
    private void ResetStance()
    {
        recoil = false;
    }

    /*
     * Function called on Character death (health <= 0)
     */
    public void Death()
    {
        int numberOfDrops = UnityEngine.Random.Range(0, npcData.loot.Length - 1);
        for(int i = 0; i < numberOfDrops; i++)
        {
            int index = UnityEngine.Random.Range(0, npcData.loot.Length - 1);
            Instantiate(npcData.loot[index], transform.position + new Vector3(0, i + 1, 0), transform.rotation);
        }
        GameManager.manager.KilledNPC(npcData.experienceReward, npcData.factionID, -npcData.reputationLoss);
        Destroy(gameObject);
    }

    /*
     * Function called to do damage to the parent Character
     */
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

    /*
     * Check if the player is hostile to the faction
     */
    public void IsPlayerHostile(bool isAttacked)
    {
        if (GameManager.manager.playerData.reputations[npcData.factionID] < GameData.FactionHostileThreshold || isAttacked)
        {
            hostile = true;
        }

        healthBar.transform.parent.gameObject.SetActive(hostile);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Blade"))
        {
            IsPlayerHostile(true);
            Damage(20);
        }
        else if (other.gameObject.CompareTag("Bullet"))
        {
            IsPlayerHostile(true);
            Damage(25);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
