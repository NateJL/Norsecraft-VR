using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CannonballCrateController : MonoBehaviour
{
    [Header("Canvas References")]
    public TextMeshProUGUI cannonballCountText;

    [Header("Crate Data")]
    public int cannonballCount;
    public int maxCannonballs = 99;

	// Use this for initialization
	void Start ()
    {
        cannonballCount = 0;
        UpdateCanvas();
	}
	
    public void UpdateCanvas()
    {
        cannonballCountText.SetText(cannonballCount + "/" + maxCannonballs);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Cannonball"))
        {
            Destroy(other.gameObject);
            cannonballCount++;
            UpdateCanvas();
        }
    }
}
