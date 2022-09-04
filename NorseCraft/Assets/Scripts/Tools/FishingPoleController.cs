using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingPoleController : MonoBehaviour
{
    public float velocityDamping = 0.5f;
    public bool casting = false;
    public bool casted = false;
    private int lineVertices = 11;
    private Vector3[] linePositions;

    [Space(10)]
    public GameObject bobberPrefab;
    public GameObject fishPrefab;
    private GameObject currentBobber;
    private GameObject currentFish;

    private LineRenderer lineRenderer;
    public Material fishingLineMaterial;

    private Transform tipOfRod;
    private Transform reelOfRod;

    private Vector3 velocity = Vector3.zero;
    private Vector3 lastPosition;

    private PoolManager objectPool;

    public bool isFishing = false;
    public bool caughtFish = false;
    public float startCastTime = 0.0f;

	// Use this for initialization
	void Start ()
    {
        objectPool = GameManager.manager.poolManager;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = lineVertices;
        linePositions = new Vector3[lineVertices];
        for (int i = 0; i < linePositions.Length; i++)
            linePositions[i] = Vector3.zero;
        lineRenderer.SetPositions(linePositions);
        lineRenderer.startWidth = lineRenderer.endWidth = 0.01f;
        lineRenderer.material = fishingLineMaterial;
        lineRenderer.enabled = false;

        tipOfRod = transform.GetChild(0);
        reelOfRod = transform.GetChild(1);
        lastPosition = tipOfRod.position;
	}

    public void StartCasting()
    {
        lineRenderer.enabled = false;
        casted = false;
        casting = true;
        isFishing = false;
        caughtFish = false;
    }

    public void WaitForFish()
    {
        isFishing = true;
        startCastTime = Time.time;

    }
	
	public void UpdatePoleVelocity()
    {
        velocity = (tipOfRod.position - lastPosition) / Time.deltaTime;
        lastPosition = tipOfRod.position;
    }
    
    public void Cast()
    {
        casting = false;
        if (currentBobber != null)
        {
            currentBobber.GetComponent<BobberController>().ResetBobber();
            objectPool.ReturnObject(currentBobber);
        }
        GameObject bobber = objectPool.SpawnFromPool(bobberPrefab.name, tipOfRod.position, tipOfRod.rotation);
        bobber.GetComponent<Rigidbody>().AddForce(velocity*velocityDamping, ForceMode.Impulse);
        bobber.GetComponent<BobberController>().SetPoleParent(this);
        currentBobber = bobber;
        lineRenderer.enabled = true;
        casted = true;
    }

    /*
     * Update fishing line and check for time changes
     */
    public void UpdateCast()
    {
        if (!isFishing)
            startCastTime = Time.time;

        if (Time.time - startCastTime > 5.0f)
            caughtFish = true;

        if (!caughtFish)
        {
            Vector3 pos1 = tipOfRod.position;
            Vector3 pos2 = new Vector3((tipOfRod.position.x + currentBobber.transform.position.x) / 2f,
                                        currentBobber.transform.position.y,
                                        (tipOfRod.position.z + currentBobber.transform.position.z) / 2f);
            Vector3 pos3 = currentBobber.transform.position;
            float lerp = (1f / (float)lineVertices);

            lineRenderer.SetPosition(0, reelOfRod.position);
            lineRenderer.SetPosition(1, tipOfRod.position);
            for (int i = 2; i < lineVertices - 1; i++)
            {
                Vector3 linePos = Vector3.Lerp(Vector3.Lerp(pos1, pos2, lerp), Vector3.Lerp(pos2, pos3, lerp), lerp);
                lineRenderer.SetPosition(i, linePos);
                lerp += (1f / (float)lineVertices);
            }
            lineRenderer.SetPosition(lineVertices - 1, currentBobber.transform.position);
        }
        else
        {
            CatchFish();
        }
    }

    /*
     * Resets bobber and disables fishing linerenderer
     */
    public void ReleasePole()
    {
        lineRenderer.enabled = false;
        if (currentBobber != null)
        {
            currentBobber.GetComponent<BobberController>().ResetBobber();
            objectPool.ReturnObject(currentBobber);
            currentBobber = null;
        }
    }

    /*
     * Reset bobber and return line with fish
     */
    public void CatchFish()
    {
        lineRenderer.enabled = false;
        if (currentBobber != null)
        {
            currentBobber.GetComponent<BobberController>().ResetBobber();
            objectPool.ReturnObject(currentBobber);
            currentBobber = null;
        }
        Vector3 fishPos = new Vector3(tipOfRod.position.x, (tipOfRod.position.y - 1), tipOfRod.position.z);
        currentFish = objectPool.SpawnFromPool(fishPrefab.name, fishPos, Quaternion.identity);
    }
}
