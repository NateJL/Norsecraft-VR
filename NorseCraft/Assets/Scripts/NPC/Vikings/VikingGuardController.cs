using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VikingGuardController : MonoBehaviour
{
    public NPCData npcData;

    [Header("Soldier Parameters")]
    public float lookRadius = 8.0f;
    public float health = 100.0f;
    public float minDistance = 1.5f;
    public float timeBetweenDamageTaken = 2.0f;
    [Space(10)]
    public GameObject myWeapon;
    public Transform wieldTransform;
    public Transform sheathTransform;
    public GameObject healthBar;

    [Header("Target Data")]
    public GameObject target;
    public bool hostile = false;

    [Header("Animator values")]
    public float attackActivateTime = 0.1f;
    public float attackDeactivateTime = 0.6f;
    public bool walk = false;
    public bool run = false;
    public bool attack = false;
    public bool recoil = false;

    private float unsheathActivateTime = 0.4f;
    private float sheathActivateTime = 0.4f;

    private bool inRange = false;
    private bool attacking = false;
    private float damageTimer = 0.0f;

    private Animator anim;

    // Use this for initialization
    void Start ()
    {
        anim = GetComponent<Animator>();
        target = null;
        DeactivateWeaponCollider();

        npcData.factionID = Array.IndexOf(Enum.GetValues(npcData.faction.GetType()), npcData.faction) - 1;

        healthBar.transform.parent.gameObject.SetActive(false);

        switch (npcData.actionType)
        {
            case NPCData.ActionType.Stand:
                //Debug.Log(gameObject.GetInstanceID() + "Standing Guard");
                break;

            case NPCData.ActionType.Path:
                //Debug.Log(gameObject.GetInstanceID() + "Pathing Guard");
                break;

            default:
                break;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {


        anim.SetBool("isHostile", hostile);
        anim.SetBool("walk", walk);
        anim.SetBool("run", run);
        anim.SetBool("attack", attack);
        anim.SetBool("recoil", recoil);

        CheckAnimations();
    }

    private void CheckAnimations()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("2H_unsheath"))
        {
            if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > unsheathActivateTime)
            {
                myWeapon.transform.SetPositionAndRotation(wieldTransform.position, wieldTransform.rotation);
                myWeapon.transform.parent = wieldTransform;
            }
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("2H_sheath"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > sheathActivateTime)
            {
                myWeapon.transform.SetPositionAndRotation(sheathTransform.position, sheathTransform.rotation);
                myWeapon.transform.parent = sheathTransform;
            }
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
        for (int i = 0; i < numberOfDrops; i++)
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
