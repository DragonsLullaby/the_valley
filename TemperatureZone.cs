using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureZone : MonoBehaviour
{
    public int zoneTemp;
    public bool playerInteracting;
    public bool zoneActive = false;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player" && zoneActive)
        {
            collider.transform.GetComponent<PlayerResources>().newTemp = zoneTemp;
            collider.transform.GetComponent<PlayerResources>().tempZone = true;
            playerInteracting = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.tag == "Player")
            collider.transform.GetComponent<PlayerResources>().tempZone = false;
        playerInteracting = false;
    }
}
