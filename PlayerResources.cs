using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class PlayerResources : MonoBehaviour
{
    public GameObject gameManager;

    // references to the resource bars
    public Slider hungerBar;
    public Slider thirstBar;
    public Slider healthBar;
    public Slider tempBar;

    // how quickly hunger and thirst decrease, based on seconds between -1 decrease
    public float hungerLossWait;
    public float thirstLossWait;

    // how much damage player takes from hunger, thirst, and temperature, based on seconds between -1 decrease
    public float hungerDamageWait;
    public float thirstDamageWait;
    public float tempDamageWait;
    public int minTemp;

    // how much water is restored when drinking
    public float waterRestore;
    float waterValue;

    int hungerValue;
    int thirstValue;
    int healthValue;
    int tempValue;

    bool hungerCheck;
    bool thirstCheck;
    bool hungerDamage;
    bool thirstDamage;
    bool tempDamage;
    bool tempUpdate;
    public bool drinkWater;

    public bool tempZone;
    public int newTemp;

    void Start()
    {
        hungerCheck = false;
        thirstCheck = false;
        hungerDamage = false;
        thirstDamage = false;
        tempDamage = false;
        drinkWater = false;
        tempUpdate = false;
        tempZone = false;

        hungerValue = 100;
        thirstValue = 100;
        healthValue = 100;
        tempValue = 75;
    }

    void Update()
    {
        // update resource bars to match values
        hungerBar.value = hungerValue;
        thirstBar.value = thirstValue;
        healthBar.value = healthValue;
        tempBar.value = tempValue;

        if (!gameManager.GetComponent<GameManager>().gamePaused)
        {
            // decrease hunger and thirst over time
            if (!hungerCheck && hungerValue > 0)
            {
                StartCoroutine(HungerDecrease(hungerLossWait));
                hungerCheck = true;
            }
            if (!thirstCheck && !drinkWater && thirstValue > 0)
            {
                StartCoroutine(ThirstDecrease(thirstLossWait));
                thirstCheck = true;
            }

            // check if player is drinking water
            if (gameManager.GetComponent<GameManager>().inventoryOpen == false)
            {
                if (drinkWater)
                {
                    waterValue += waterRestore * Time.deltaTime;
                    if (waterValue >= 1)
                    {
                        int floor = Mathf.FloorToInt(waterValue);
                        if (thirstValue + floor > 100)
                            thirstValue = 100;
                        else
                            thirstValue += floor;
                        waterValue -= floor;
                    }
                }
            }
            else
                drinkWater = false;

            // check if health needs to decrease
            if (hungerValue == 0 && !hungerDamage)
            {
                StartCoroutine(HealthDecrease(hungerDamageWait, "hunger"));
                hungerDamage = true;
            }
            if (thirstValue == 0 && !thirstDamage)
            {
                StartCoroutine(HealthDecrease(thirstDamageWait, "thirst"));
                thirstDamage = true;
            }
            if (tempValue <= minTemp && !tempDamage)
            {
                StartCoroutine(HealthDecrease(tempDamageWait, "temp"));
                tempDamage = true;
            }

            // check if temp needs to change
            if (tempZone && !tempUpdate)
            {
                StartCoroutine(UpdateTemp(0.5f));
                tempUpdate = true;
            }

            // bring temp back to default if not in a temp zone
            if (!tempZone && tempValue != 75 && !tempUpdate)
            {
                StartCoroutine(UpdateTemp(0.5f));
                tempUpdate = true;
            }
        }
    }

    IEnumerator HungerDecrease(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (gameManager.GetComponent<GameManager>().gamePaused == false)
        {
            hungerValue -= 1;
        }

        hungerCheck = false;
    }

    IEnumerator ThirstDecrease(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (gameManager.GetComponent<GameManager>().gamePaused == false)
        {
            if (!drinkWater)
                thirstValue -= 1;
        }
        
        thirstCheck = false;
    }

    IEnumerator HealthDecrease(float wait, string damageType)
    {
        yield return new WaitForSeconds(wait);
        if (gameManager.GetComponent<GameManager>().gamePaused == false)
        {
            if (healthValue - 1 == 0)
            {
                //player dies
                healthValue = 0;

                if (damageType == "hunger")
                    gameManager.GetComponent<GameManager>().PlayerDied("You died from starvation.");
                else if (damageType == "thirst")
                    gameManager.GetComponent<GameManager>().PlayerDied("You died from dehydration.");
                else if (damageType == "temp")
                    gameManager.GetComponent<GameManager>().PlayerDied("You died from hypothermia.");
            }
            else
            {
                healthValue -= 1;

                if (damageType == "hunger")
                    hungerDamage = false;
                else if (damageType == "thirst")
                    thirstDamage = false;
                else if (damageType == "temp")
                    tempDamage = false;
            }
        }
    }

    public void EatFood(int food, int poison)
    {
        if (hungerValue + food > 100)
            hungerValue = 100;
        else
            hungerValue += food;

        if (poison != 0)
        {
            // player takes damage from poison
            if (healthValue - poison <= 0)
            {
                //player dies
                healthValue = 0;
                gameManager.GetComponent<GameManager>().PlayerDied("You died from eating poisonous mushrooms.");
            }
            else
                healthValue -= poison;
        }
    }

    IEnumerator UpdateTemp(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (tempZone)
        {
            if (tempValue > newTemp)
            {
                // temperature needs to decrease
                tempValue -= 1;
            }
            else if (tempValue < newTemp)
            {
                // temperature needs to increase
                tempValue += 1;
            }
        }
        else if (tempValue < 75)
            tempValue += 1;
        else if (tempValue > 75)
            tempValue -= 1;

        tempUpdate = false;
    }
}
