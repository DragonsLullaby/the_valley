using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioScript : MonoBehaviour
{
    public bool wiresDone = false; // 5 wires needed
    public bool boltsDone = false; // 10 bolts needed
    public bool metalDone = false; // 2 metal plates needed
    public bool batteryDone = false; // 2 battery needed

    private void Update()
    {
        // check whether player has completed all repairs
        if (wiresDone && boltsDone && metalDone && batteryDone)
        {
            // player repaired the radio!
            // trigger end of game
            GameObject.Find("GameManager").GetComponent<GameManager>().EndGame();
        }
    }
}
