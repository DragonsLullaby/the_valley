using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayCycle : MonoBehaviour
{
    [Range(0, 24)]
    public float timeOfDay;

    public GameObject gameManager;
    
    public float orbitSpeed;
    public Light sun;
    public Light moon;
    public Volume skyVolume;
    public AnimationCurve starsCurve;

    public bool isNight;
    bool sunEnabled;
    bool moonEnabled;
    PhysicallyBasedSky sky;

    void Start()
    {
        skyVolume.profile.TryGet(out sky);
        sunEnabled = true;
        moonEnabled = false;
    }

    void Update()
    {
        if (!gameManager.GetComponent<GameManager>().gamePaused)
        {
            timeOfDay += Time.deltaTime * orbitSpeed;
            if (timeOfDay > 24)
                timeOfDay = 0;

            UpdateTime();
        }
    }

    void UpdateTime()
    {
        float alpha = timeOfDay / 24f;
        float sunRot = Mathf.Lerp(-90, 270, alpha);
        float moonRot = sunRot - 180;

        sun.transform.rotation = Quaternion.Euler(sunRot, 0, 0);
        moon.transform.rotation = Quaternion.Euler(moonRot, 0, 0);

        sky.spaceEmissionMultiplier.value = starsCurve.Evaluate(alpha) * 700f;

        // completely disable the sun/moon light component when they are out of sight
        // game starts at 8am, sun is enabled and moon is disabled
        if (timeOfDay >= 17 && timeOfDay <= 18 && !moonEnabled)
        {
            moonEnabled = true;
            moon.enabled = true;
        }
        else if (timeOfDay >= 19 && timeOfDay <= 20 && sunEnabled)
        {
            sunEnabled = false;
            sun.enabled = false;
        }
        else if (timeOfDay >= 5 && timeOfDay <= 6 && !sunEnabled)
        {
            sunEnabled = true;
            sun.enabled = true;
        }
        else if (timeOfDay >= 7 && timeOfDay <= 8 && moonEnabled)
        {
            moonEnabled = false;
            moon.enabled = false;
        }

        DayTransition();
    }

    void DayTransition()
    {
        if (isNight)
        {
            if (moon.transform.rotation.eulerAngles.x > 180)
                StartDay();
        }
        else
        {
            if (sun.transform.rotation.eulerAngles.x > 180)
                StartNight();
        }
    }

    void StartDay()
    {
        isNight = false;
        moon.shadows = LightShadows.None;
        sun.shadows = LightShadows.Soft;
    }

    void StartNight()
    {
        isNight = true;
        sun.shadows = LightShadows.None;
        moon.shadows = LightShadows.Soft;
    }

    public void Sleep()
    {
        timeOfDay = 8;
        sunEnabled = true;
        sun.enabled = true;
        StartDay();
        gameManager.GetComponent<GameManager>().EndSleep();
    }
}
