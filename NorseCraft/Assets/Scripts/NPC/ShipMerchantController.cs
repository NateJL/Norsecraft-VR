using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMerchantController : MonoBehaviour
{
    public GameObject target;

    public Animator anim;
    public float lookRadius = 5f;

    private float health = 100;
    public GameObject healthBar;

    private bool attack;
    private float attackTimer;

    // Use this for initialization
    void Start ()
    {
        target = GameManager.manager.player;
        anim = GetComponent<Animator>();
        attack = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float distance = Vector3.Distance(target.transform.position, transform.position);

        if ((distance <= lookRadius) && (health > 0))
        {
            attack = true;
            FaceTarget();
        }
        else
        {
            attack = false;
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        if(attack)
        {

        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void Damage(float damage)
    {
        health -= damage;
        float healthBarWidth = 120f * ((float)health / 100f);
        healthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthBarWidth);

        if (health <= 0)
        {
            anim.SetBool("isDead", true);
            Destroy(gameObject, 5.0f);
        }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
