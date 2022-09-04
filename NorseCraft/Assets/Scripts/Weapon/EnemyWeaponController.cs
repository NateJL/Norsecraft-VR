using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    [Header("Parent Character")]
    public GameObject parentCharacter;

    public enum EnemyType
    {
        None,
        TownGuard,
        Enemy,
        VikingGuard
    };
    public EnemyType enemyType = EnemyType.None;

    [Header("Blocking Data")]
    public float blockTimer = 1.0f;
    public bool wasBlocked;


    private float attackCooldown = 0.0f;

    // Use this for initialization
    void Start ()
    {
        //parentCharacter = transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.gameObject;
	}

    private void ResetStance()
    {
        wasBlocked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Blade"))
        {
            Debug.Log("Weapon: Hit player blade");
            wasBlocked = true;
            if (enemyType == EnemyType.Enemy)
                parentCharacter.GetComponent<EnemyController>().Blocked();
            else if (enemyType == EnemyType.TownGuard)
                parentCharacter.GetComponent<TownGuardController>().Blocked();
            else if (enemyType == EnemyType.VikingGuard)
                parentCharacter.GetComponent<VikingGuardController>().Blocked();
            Invoke("ResetStance", blockTimer);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            if ((Time.time - attackCooldown > blockTimer) && !wasBlocked)
            {
                GameManager.manager.playerData.health -= 20;
                attackCooldown = Time.time;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Blade"))
        {
            wasBlocked = false;
        }
    }
}
