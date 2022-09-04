using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBladeController : MonoBehaviour
{
    public GameObject parentCharacter;
    public EnemyWeaponController parentWeapon;

    public float attackCooldown = 0.0f;

    private void Start()
    {
        parentWeapon = transform.parent.GetComponent<EnemyWeaponController>();
        parentCharacter = parentWeapon.parentCharacter;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Blade"))
        {
            Debug.Log("Blade: Hit player blade");
            parentCharacter.GetComponent<SkeletonController>().Blocked();
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            if (Time.time - attackCooldown > 1.0f)
            {
                GameManager.manager.playerData.health -= 20;
                attackCooldown = Time.time;
            }
        }
    }
}
