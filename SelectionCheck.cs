using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectionCheck : MonoBehaviour
{
    public GameObject selectedObj;
    public TextMeshProUGUI InteractText;

    public GameObject player;
    public float rayDistance;

    public GameObject sky;

    int choice = 0;

    // "Selectable" layer is number 9

    void Update()
    {
        if (GetComponent<GameManager>().gamePaused == false && GetComponent<GameManager>().inventoryOpen == false)
        {
            // ----- raycast to check for a selectable object -----
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                GameObject selection = hit.transform.gameObject;
                if (selection.layer == 9)
                {
                    selectedObj = selection;
                }
                else
                {
                    selectedObj = null;
                    InteractText.gameObject.SetActive(false);
                }
            }
            else
            {
                selectedObj = null;
                InteractText.gameObject.SetActive(false);
            }

            // ----- if selectbale object is selected -----
            if (selectedObj != null)
            {
                if (selectedObj.CompareTag("Pickup"))
                {
                    // currently selected object is collectible
                    InteractText.text = "(E) Pick up " + selectedObj.GetComponent<PickupInfo>().itemName;
                    InteractText.gameObject.SetActive(true);

                    // ----- check if player presses E while object is selected -----
                    if (Input.GetKeyDown(KeyCode.E))
                        GetComponent<InventorySystem>().PickUp(selectedObj);
                }

                else if (selectedObj.CompareTag("Water"))
                {
                    // currently selected object is water
                    InteractText.text = "(E) Drink";
                    InteractText.gameObject.SetActive(true);

                    // ----- check if player holds E while object is selected -----
                    if (Input.GetKey(KeyCode.E))
                        player.GetComponent<PlayerResources>().drinkWater = true;

                    if (Input.GetKeyUp(KeyCode.E))
                        player.GetComponent<PlayerResources>().drinkWater = false;
                }

                else if (selectedObj.CompareTag("Firepit"))
                {
                    // currently selected object is the firepit

                    if (choice == 0)
                    {
                        // first choice, open crafting menu
                        InteractText.text = "(E) Open crafting menu" + "\n" + "(F) Switch choice";
                        InteractText.gameObject.SetActive(true);

                        // ----- check if player presses E while object is selected -----
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            InteractText.gameObject.SetActive(false);
                            GetComponent<GameManager>().OpenCrafting();
                            return;
                        }

                        // ----- check if player presses F while object is selected -----
                        if (Input.GetKeyDown(KeyCode.F))
                            choice = 1;
                    }

                    else if (choice == 1)
                    {
                        // second choice, add fuel (stick)
                        // check if player has stick(s) in their inventory
                        if (GetComponent<InventorySystem>().CheckForItem("Stick", 1) != null)
                        {
                            InteractText.text = "(E) Add stick to fire" + "\n" + "(F) Switch choice";
                            InteractText.gameObject.SetActive(true);

                            // ----- check if player presses E while object is selected -----
                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                GameObject slot = GetComponent<InventorySystem>().CheckForItem("Stick", 1);
                                GetComponent<InventorySystem>().DiscardItem(slot, 1);
                                selectedObj.transform.parent.GetComponent<FirepitSystem>().AddFuel("stick");

                                selectedObj.transform.parent.GetComponent<FirepitSystem>().FireOn();
                                selectedObj.transform.parent.GetComponent<TemperatureZone>().zoneActive = true;
                                player.GetComponent<PlayerResources>().newTemp = selectedObj.transform.parent.GetComponent<TemperatureZone>().zoneTemp;
                                player.GetComponent<PlayerResources>().tempZone = true;
                                selectedObj.transform.parent.GetComponent<TemperatureZone>().playerInteracting = true;
                            }

                            // ----- check if player presses F while object is selected -----
                            if (Input.GetKeyDown(KeyCode.F))
                                choice = 2;
                        }
                        else
                            choice = 2;
                    }

                    else if (choice == 2)
                    {
                        //third choice, add fuel (plank)
                        // check if player has plank(s) in their inventory
                        if (GetComponent<InventorySystem>().CheckForItem("Wooden Plank", 1) != null)
                        {
                            InteractText.text = "(E) Add plank to fire" + "\n" + "(F) Switch choice";
                            InteractText.gameObject.SetActive(true);

                            // ----- check if player presses E while object is selected -----
                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                GameObject slot = GetComponent<InventorySystem>().CheckForItem("Wooden Plank", 1);
                                GetComponent<InventorySystem>().DiscardItem(slot, 1);
                                selectedObj.transform.parent.GetComponent<FirepitSystem>().AddFuel("plank");

                                selectedObj.transform.parent.GetComponent<FirepitSystem>().FireOn();
                                selectedObj.transform.parent.GetComponent<TemperatureZone>().zoneActive = true;
                                player.GetComponent<PlayerResources>().newTemp = selectedObj.transform.parent.GetComponent<TemperatureZone>().zoneTemp;
                                player.GetComponent<PlayerResources>().tempZone = true;
                                selectedObj.transform.parent.GetComponent<TemperatureZone>().playerInteracting = true;
                            }

                            // ----- check if player presses F while object is selected -----
                            if (Input.GetKeyDown(KeyCode.F))
                                choice = 3;
                        }
                        else
                            choice = 3;
                    }

                    else if (choice == 3)
                    {
                        // fourth choice, sleep
                        // check if it is currently night
                        if (sky.GetComponent<DayCycle>().isNight)
                        {
                            InteractText.text = "(E) Sleep" + "\n" + "(F) Switch choice";
                            InteractText.gameObject.SetActive(true);

                            // ----- check if player presses E while object is selected -----
                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                //player sleeps
                                InteractText.gameObject.SetActive(false);
                                GetComponent<GameManager>().StartSleep();
                                return;
                            }

                            // ----- check if player presses F while object is selected -----
                            if (Input.GetKeyDown(KeyCode.F))
                                choice = 0;
                        }
                        else
                            choice = 0;
                    }
                }
                else if (selectedObj.CompareTag("Radio"))
                {
                    // currently selected object is the radio
                    InteractText.text = "(E) Repair Radio";
                    InteractText.gameObject.SetActive(true);

                    // ----- check if player presses E while object is selected -----
                    if (Input.GetKeyDown(KeyCode.E))
                        GetComponent<GameManager>().OpenRadio();
                }
                else
                    player.GetComponent<PlayerResources>().drinkWater = false;
            }
            else
                player.GetComponent<PlayerResources>().drinkWater = false;
        }
    }
}
