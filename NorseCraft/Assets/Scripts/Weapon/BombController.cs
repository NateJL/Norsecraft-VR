using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [Header("Explosion Effect")]
    public GameObject explosion;

    private GameObject fireEffect;

	// Use this for initialization
	void Start ()
    {
        fireEffect = transform.GetChild(0).gameObject;
        fireEffect.SetActive(false);
	}
	
	public void Explode()
    {
        var explosionObj = Instantiate(explosion, transform.position, transform.rotation);
        Destroy(explosionObj, 3.0f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Fire"))
        {
            fireEffect.SetActive(true);
            Invoke("Explode", 5.0f);
        }
    }
}
