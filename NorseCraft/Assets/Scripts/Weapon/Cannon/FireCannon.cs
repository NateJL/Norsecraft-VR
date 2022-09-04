using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCannon : MonoBehaviour
{
    [Header("Cannonball Data")]
    public GameObject cannonballPrefab;
    public Transform cannonballSpawn;

    [Header("Trigger Colors")]
    public Material isReadyMat;
    public Material notReadyMat;

    public AudioClip cannonShotAudio;
    public GameObject cannonParticleSys;
    private AudioSource audioSource;

    private Renderer fireColliderMesh;

    private ReloadCannon reloader;
    private bool isLoaded;

    public float cannonballVelocity = 50.0f;

    private void Start()
    {
        fireColliderMesh = GetComponent<MeshRenderer>();
        audioSource = transform.parent.parent.GetComponent<AudioSource>();
        isLoaded = false;
        fireColliderMesh.material = notReadyMat;

        reloader = transform.GetChild(0).GetComponent<ReloadCannon>();
        if (reloader == null)
            Debug.LogError("Failed to get ReloadCannon child");
    }

    public bool IsLoaded()
    {
        return isLoaded;
    }

    public void LoadCannon()
    {
        isLoaded = true;
        fireColliderMesh.material = isReadyMat;
    }

    private void FireCannonball()
    {
        var cannonPS = Instantiate(cannonParticleSys, cannonballSpawn.position, cannonballSpawn.rotation);

        var cannonball = Instantiate(cannonballPrefab, cannonballSpawn.position, cannonballSpawn.rotation);
        cannonball.GetComponent<Rigidbody>().velocity = cannonball.transform.forward * cannonballVelocity;

        Destroy(cannonPS, 2.0f);
        Destroy(cannonball, 2.0f);
        isLoaded = false;
        fireColliderMesh.material = notReadyMat;
        reloader.FiredShot();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Fire"))
        {
            if (isLoaded)
            {
                audioSource.PlayOneShot(cannonShotAudio);
                Invoke("FireCannonball", 0.1f);
            }
        }
    }
}
