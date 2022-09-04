using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab;

    public AudioClip gunshot;
    public GameObject gunshotParticleSys;
    private AudioSource audioSource;

    private float timeAlive = 0.0f;

    [Header("Specs")]
    public float bulletVelocity = 50.0f;
    public Transform[] bulletSpawn;

    private PoolManager objectPool;
    private GameObject spawnedBullet = null;

    // Use this for initialization
    private void Start()
    {
        objectPool = GameManager.manager.poolManager;
        audioSource = GetComponent<AudioSource>();
    }

    public void FireBullet()
    {
        if (Time.time - timeAlive > 2.0f)
        {
            timeAlive = Time.time;

            foreach(Transform spawn in bulletSpawn)
            {
                //var bulletPS = (GameObject)Instantiate(gunshotParticleSys, spawn.position, spawn.rotation);

                spawnedBullet = objectPool.SpawnFromPool(bulletPrefab.name, spawn.position, spawn.rotation);

                audioSource.PlayOneShot(gunshot);

                spawnedBullet.GetComponent<Rigidbody>().velocity = spawnedBullet.transform.forward * bulletVelocity;

                Invoke("DeactivateBullet", 1.9f);
            }
        }
    }

    public void DeactivateBullet()
    {
        objectPool.ReturnObject(spawnedBullet);
        spawnedBullet = null;
    }
}
