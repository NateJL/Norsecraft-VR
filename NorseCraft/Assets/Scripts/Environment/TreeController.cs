using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    public enum TreeType
    {
        None,
        Standing,
        Felled
    }
    public TreeType treeType = TreeType.None;
    public int hitsToFall = 3;
    [ShowOnly]public int hitsRemaining;

    public GameObject fallenTreePrefab;
    public GameObject woodPlankPrefab;
    public GameObject woodLogPrefab;

    public AudioClip treeFallAudio;

    private bool hasFallen = false;
    private float currentTime = 0.0f;
    private float timeToFall = 2.0f;

    private Vector3 targetRot = new Vector3(90, 0, 0);

    private PoolManager objectPool;

    private void Start()
    {
        hitsRemaining = hitsToFall;
        objectPool = GameManager.manager.poolManager;
    }

    public void ResetTree()
    {
        currentTime = 0.0f;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        hitsRemaining = hitsToFall;
        hasFallen = false;
    }

    private void SpawnFelledLog()
    {
        GameObject fallenTree = objectPool.SpawnFromPool(fallenTreePrefab.name, transform.GetChild(0).position, transform.GetChild(0).rotation);
        fallenTree.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        ResetTree();
        objectPool.ReturnObject(gameObject);
    }

    private void SpawnPlanks()
    {
        GameObject newPlank = objectPool.SpawnFromPool(woodPlankPrefab.name, transform.GetChild(0).position, transform.GetChild(0).rotation);
        newPlank.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        newPlank = objectPool.SpawnFromPool(woodPlankPrefab.name, transform.GetChild(1).position, transform.GetChild(1).rotation);
        newPlank.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        ResetTree();
        objectPool.ReturnObject(gameObject);
    }

    private void SpawnLogs()
    {
        GameObject newLog = objectPool.SpawnFromPool(woodLogPrefab.name, transform.GetChild(2).position, transform.GetChild(2).rotation);
        newLog.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
        newLog = objectPool.SpawnFromPool(woodLogPrefab.name, transform.GetChild(3).position, transform.GetChild(3).rotation);
        newLog.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
        newLog = objectPool.SpawnFromPool(woodLogPrefab.name, transform.GetChild(4).position, transform.GetChild(4).rotation);
        newLog.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
        ResetTree();
        objectPool.ReturnObject(gameObject);
    }

    IEnumerator ChopTreeDown()
    {
        hasFallen = true;
        while(currentTime <= timeToFall)
        {
            currentTime += Time.deltaTime;
            float lerp = currentTime / timeToFall;
            Vector3 newRot = Vector3.Lerp(Vector3.zero, targetRot, lerp);
            transform.rotation = Quaternion.Euler(newRot);
            yield return null;
        }
        while(currentTime <= treeFallAudio.length)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        SpawnFelledLog();
    }

    public void ChopTree()
    {
        if (treeType == TreeType.Standing)
        {
            if (!hasFallen && !(hitsRemaining > 1))
            {
                GameManager.manager.soundManager.PlayAudioClip(treeFallAudio.name);
                StartCoroutine("ChopTreeDown");
            }
            else if (hitsRemaining > 0)
                hitsRemaining--;
        }
        else if (treeType == TreeType.Felled)
        {
            if (!(hitsRemaining > 1))
                SpawnLogs();
            else if (hitsRemaining > 0)
                hitsRemaining--;
        }
    }

    public void SawTree()
    {
        if (treeType == TreeType.Standing)
        {
            if (!hasFallen && !(hitsRemaining > 1))
            {
                GameManager.manager.soundManager.PlayAudioClip(treeFallAudio.name);
                StartCoroutine("ChopTreeDown");
            }
            else if (hitsRemaining > 0)
                hitsRemaining--;
        }
        else if (treeType == TreeType.Felled)
        {
            if (!(hitsRemaining > 1))
                SpawnPlanks();
            else if (hitsRemaining > 0)
                hitsRemaining--;
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if(  other.gameObject.CompareTag("Blade") &&
            (other.transform.parent.GetComponent<ItemController>() != null) &&
            (other.transform.parent.GetComponent<ItemController>().itemType == ItemController.ItemType.Axe) &&
             other.transform.parent.GetComponent<ItemController>().isActive)
        {
            if (treeType == TreeType.Standing)
            {
                if(!hasFallen)
                    StartCoroutine("ChopTreeDown");
            }
            else if(treeType == TreeType.Felled)
            {
                SpawnPlanks();
            }
            other.transform.parent.GetComponent<ItemController>().UseItem();
        }
    }*/
}
