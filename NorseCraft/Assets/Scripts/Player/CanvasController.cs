using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [Header("Health Bar")]
    public GameObject healthBar;
    public GameObject healthBarText;
    [Space(10)]
    public GameObject hungerBar;
    public GameObject thirstBar;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        float healthBarWidth = 150f * ((float)GameManager.manager.playerData.health / 100f);
        healthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthBarWidth);
        healthBarText.GetComponent<TextMeshProUGUI>().SetText(GameManager.manager.playerData.health + "%");

        float hungerBarWidth = 150f * ((float)GameManager.manager.playerData.hunger / 100f);
        float thirstBarWidth = 150f * ((float)GameManager.manager.playerData.thirst / 100f);

        hungerBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, hungerBarWidth);
        thirstBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thirstBarWidth);
    }
}
