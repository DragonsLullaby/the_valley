using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirepitSystem : MonoBehaviour
{
    public int fuelValue;
    public int fuelMax;
    public int stickFuel;
    public int plankFuel;

    // rate at which fuel decreases, measured in time in seconds between - 1 decrease
    public float fuelDecrease;

    bool decreaseFuel;

    void Start()
    {
        decreaseFuel = false;
    }

    void Update()
    {
        // decrease fuel over time
        if (fuelValue > 0 && !decreaseFuel)
        {
            StartCoroutine(DecreaseFuel(fuelDecrease));
            decreaseFuel = true;
        }

        // check if fuel is 0
        if (fuelValue == 0 && GetComponent<TemperatureZone>().zoneActive == true)
        {
            FireOff();
            GetComponent<TemperatureZone>().zoneActive = false;
            if (GetComponent<TemperatureZone>().playerInteracting == true)
                GameObject.Find("Player").GetComponent<PlayerResources>().tempZone = false;
        }
    }

    IEnumerator DecreaseFuel(float wait)
    {
        yield return new WaitForSeconds(wait);
        fuelValue -= 1;
        decreaseFuel = false;
    }

    public void AddFuel(string fuel)
    {
        //add fuel to the firepit
        if (fuel == "stick")
        {
            //player adds a single stick to the fuel level
            if (fuelValue + stickFuel > 100)
                fuelValue = 100;
            else
                fuelValue += stickFuel;
        }
        else if (fuel == "plank")
        {
            //player adds a single wooden plank to the fuel level
            if (fuelValue + plankFuel > 100)
                fuelValue = 100;
            else
                fuelValue += plankFuel;
        }
    }

    public void FireOn()
    {
        // activate the fire particle effect
        gameObject.transform.Find("FireParticles").GetComponent<ParticleSystem>().Play();
    }

    public void FireOff()
    {
        // deactivate the fire particle effect
        gameObject.transform.Find("FireParticles").GetComponent<ParticleSystem>().Stop();
    }
}
