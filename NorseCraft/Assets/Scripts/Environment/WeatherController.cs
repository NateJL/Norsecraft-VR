using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    // "Sun" light
    private Light sunDirectionalLight;

    [Header("Time Data")]
    [Range(0, 1)]
    public float currentTimeOfDay = 0.5f;
    public float secondsInFullDay = 30f;
    public bool timeChange;

    [Header("Fog Data")]
    public float currentFogDistance;
    public Color fogColor;
    public float currentFogColor;
    private float maxFogDistance = 300f;

    private float timeMultiplier = 1f;
    private float fogMultiplier = 1f;

    private float sunInitialIntensity;
    private float fogInitialIntensity;

	// Use this for initialization
	void Start ()
    {
        sunDirectionalLight = GetComponent<Light>();
        sunInitialIntensity = sunDirectionalLight.intensity;
        fogInitialIntensity = 128f;

        fogColor = RenderSettings.fogColor;
        currentFogColor = RenderSettings.fogEndDistance;

        UpdateSun();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (timeChange)
        {
            UpdateTime();

            UpdateSun();
            //UpdateFog();
        }
    }

    private void UpdateTime()
    {
        currentTimeOfDay += (Time.deltaTime / secondsInFullDay) * timeMultiplier;

        if (currentTimeOfDay >= 1)
        {
            currentTimeOfDay = 0;
        }

        float hour = Mathf.Floor(currentTimeOfDay * 24f);
        float minute = Mathf.RoundToInt(((currentTimeOfDay * 24f) - Mathf.Floor(currentTimeOfDay * 24f)) * 60f);
        GameData.hour = (int)hour;
        GameData.minute = (int)minute;
    }

    private void UpdateSun()
    {
        sunDirectionalLight.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        float intensityMultiplier = 1;

        if(currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0;
        }

        else if(currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }

        else if(currentTimeOfDay >= 0.73f)
        {
            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.75f) * (1 / 0.02f)));
        }

        sunDirectionalLight.intensity = sunInitialIntensity * intensityMultiplier;
    }

    private void UpdateFog()
    {
        float intensityMultiplier = 1;

        if (currentTimeOfDay <= 0.2f || currentTimeOfDay >= 0.75f)
        {
            intensityMultiplier = 0;
        }

        else if (currentTimeOfDay <= 0.25f)
        {
            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.2f) * (1 / 0.02f));
        }

        else if (currentTimeOfDay >= 0.7f)
        {
            intensityMultiplier = Mathf.Clamp01(((0.75f - currentTimeOfDay) * (1 / 0.02f)));
        }


        currentFogColor = fogInitialIntensity * intensityMultiplier;
        currentFogDistance = ((maxFogDistance - 20) * intensityMultiplier) + 20;
        fogColor = new Color32((byte)currentFogColor, (byte)currentFogColor, (byte)(currentFogColor + 2), 255);
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogEndDistance = currentFogDistance;
    }
}
