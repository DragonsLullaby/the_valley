using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    // ----- Inventory Slots -----
    public int[] slotCount;
    public GameObject[] itemSlot;
    public GameObject itemPreview;
    public GameObject cookingPreview;
    public GameObject craftingPreview;
    public GameObject radioPreview;
    public GameObject craftingObj;
    public GameObject cookingObj;
    public GameObject craftingTabObj;
    public GameObject cookingTabObj;
    public GameObject toolSlot1;
    public GameObject toolSlot2;

    public GameObject makeshiftAxe;
    public GameObject makeshiftPickaxe;
    public GameObject metalAxe;
    public GameObject metalPickaxe;

    public GameObject player;

    public bool craftingTab = true;
    public bool radioScreen = false;

    void Start()
    {
        slotCount = new int[9];
    }

    public void PickUp(GameObject obj)
    {
        // ----- check if player has some of the same item in their inventory already -----
        for (int i = 0; i < 9; i++)
        {
            if (itemSlot[i].GetComponent<PickupInfo>().itemName == obj.GetComponent<PickupInfo>().itemName)
            {
                // found matching item in inventory
                if (slotCount[i] < 20)
                {
                    // less than 20 items in this stack, can pick up
                    slotCount[i] += 1;
                    itemSlot[i].transform.Find("ItemCount").gameObject.GetComponent<TextMeshProUGUI>().text = slotCount[i].ToString();
                    Destroy(obj);
                    return;
                }
            }
        }

        // ----- else, find empty slot to place item -----
        for (int i = 0; i < 9; i++)
        {
            if (itemSlot[i].GetComponent<PickupInfo>().itemName == "")
            {
                // inventory slot is empty, item can be placed there
                itemSlot[i].GetComponent<PickupInfo>().itemName = obj.GetComponent<PickupInfo>().itemName;
                string description = obj.GetComponent<PickupInfo>().itemDescription.Replace("NEWLINE", "\n");
                itemSlot[i].GetComponent<PickupInfo>().itemDescription = description;
                itemSlot[i].GetComponent<PickupInfo>().foodRestore = obj.GetComponent<PickupInfo>().foodRestore;
                itemSlot[i].GetComponent<PickupInfo>().poisonDamage = obj.GetComponent<PickupInfo>().poisonDamage;
                itemSlot[i].GetComponent<PickupInfo>().itemSprite = obj.GetComponent<PickupInfo>().itemSprite;

                Destroy(obj);
                slotCount[i] = 1;

                itemSlot[i].transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = itemSlot[i].GetComponent<PickupInfo>().itemSprite;
                itemSlot[i].transform.Find("ItemIcon").gameObject.SetActive(true);
                itemSlot[i].transform.Find("ItemCount").gameObject.GetComponent<TextMeshProUGUI>().text = slotCount[i].ToString();
                itemSlot[i].transform.Find("ItemCount").gameObject.SetActive(true);

                itemSlot[i].GetComponent<Button>().interactable = true;
                return;
            }
        }

        // ----- else, no available inventory slots -----
        Debug.Log("no available space in inventory");
    }

    public void PreviewItem(GameObject slot)
    {
        if (GetComponent<GameManager>().craftingOpen == true)
        {
            // crafting menu is open, uses different item preview object
            if (craftingTab)
            {
                craftingPreview.transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = slot.GetComponent<PickupInfo>().itemSprite;
                craftingPreview.transform.Find("ItemIcon").gameObject.SetActive(true);
                craftingPreview.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().itemName;
                string description = slot.GetComponent<PickupInfo>().itemDescription.Replace("NEWLINE", "\n");
                craftingPreview.transform.Find("ItemDescription").gameObject.GetComponent<TextMeshProUGUI>().text = description;
                craftingPreview.transform.Find("RequiredItems1").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().craftingMaterial1;
                craftingPreview.transform.Find("RequiredItems2").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().craftingMaterial2;
                craftingPreview.transform.Find("RequiredItems3").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().craftingMaterial3;
                craftingPreview.transform.Find("Quantity1").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().craftingQuantity1.ToString();
                craftingPreview.transform.Find("Quantity2").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().craftingQuantity2.ToString();
                craftingPreview.transform.Find("Quantity3").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().craftingQuantity3.ToString();

                // check if player has the necessary items for crafting this item
                GameObject foundSlot1 = CheckForItem(slot.GetComponent<PickupInfo>().craftingMaterial1, slot.GetComponent<PickupInfo>().craftingQuantity1);
                GameObject foundSlot2 = CheckForItem(slot.GetComponent<PickupInfo>().craftingMaterial2, slot.GetComponent<PickupInfo>().craftingQuantity2);
                GameObject foundSlot3 = CheckForItem(slot.GetComponent<PickupInfo>().craftingMaterial3, slot.GetComponent<PickupInfo>().craftingQuantity3);
                if (foundSlot1 != null && foundSlot2 != null && foundSlot3 != null)
                {
                    // player has all the required items for crafting this item
                    craftingPreview.transform.Find("CraftButton").gameObject.GetComponent<Button>().interactable = true;
                }
                else
                    craftingPreview.transform.Find("CraftButton").gameObject.GetComponent<Button>().interactable = false;

                craftingPreview.GetComponent<SlotReference>().itemSlot = slot;
            }
            else if (!craftingTab)
            {
                cookingPreview.transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = slot.GetComponent<PickupInfo>().itemSprite;
                cookingPreview.transform.Find("ItemIcon").gameObject.SetActive(true);
                cookingPreview.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().itemName;
                string description = slot.GetComponent<PickupInfo>().itemDescription.Replace("NEWLINE", "\n");
                cookingPreview.transform.Find("ItemDescription").gameObject.GetComponent<TextMeshProUGUI>().text = description;
                cookingPreview.transform.Find("RequiredItems1").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().craftingMaterial1;

                // check if player has the necessary items for cooking this food item
                GameObject foundSlot = CheckForItem(slot.GetComponent<PickupInfo>().craftingMaterial1, 1);
                if (foundSlot != null)
                {
                    // player has the required items for cooking
                    cookingPreview.transform.Find("CookButton").gameObject.GetComponent<Button>().interactable = true;
                }
                else
                    cookingPreview.transform.Find("CookButton").gameObject.GetComponent<Button>().interactable = false;

                cookingPreview.GetComponent<SlotReference>().itemSlot = slot;
            }
        }
        else if (radioScreen)
        {
            // the radio inventory screen is open
            radioPreview.transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = slot.GetComponent<PickupInfo>().itemSprite;
            radioPreview.transform.Find("ItemIcon").gameObject.SetActive(true);
            radioPreview.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().itemName;
            string description = slot.GetComponent<PickupInfo>().itemDescription.Replace("NEWLINE", "\n");
            radioPreview.transform.Find("ItemDescription").gameObject.GetComponent<TextMeshProUGUI>().text = description;
            radioPreview.transform.Find("RequiredQuantity").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().craftingQuantity1.ToString();

            // check if player has the necessary items for repairing this part of the radio
            GameObject foundSlot = CheckForItem(slot.GetComponent<PickupInfo>().itemName, slot.GetComponent<PickupInfo>().craftingQuantity1);
            if (foundSlot != null)
            {
                // player has the required items for repairing
                radioPreview.transform.Find("RepairButton").gameObject.GetComponent<Button>().interactable = true;
            }
            else
                radioPreview.transform.Find("RepairButton").gameObject.GetComponent<Button>().interactable = false;

            radioPreview.GetComponent<SlotReference>().itemSlot = slot;
        }
        else
        {
            itemPreview.transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = slot.GetComponent<PickupInfo>().itemSprite;
            itemPreview.transform.Find("ItemIcon").gameObject.SetActive(true);
            itemPreview.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = slot.GetComponent<PickupInfo>().itemName;
            string description = slot.GetComponent<PickupInfo>().itemDescription.Replace("NEWLINE", "\n");
            itemPreview.transform.Find("ItemDescription").gameObject.GetComponent<TextMeshProUGUI>().text = description;

            if (slot.GetComponent<PickupInfo>().foodRestore != 0)
                itemPreview.transform.Find("EatButton").gameObject.GetComponent<Button>().interactable = true;
            else
                itemPreview.transform.Find("EatButton").gameObject.GetComponent<Button>().interactable = false;

            if (slot.GetComponent<PickupInfo>().isTool == true)
                itemPreview.transform.Find("EquipButton").gameObject.GetComponent<Button>().interactable = true;
            else
                itemPreview.transform.Find("EquipButton").gameObject.GetComponent<Button>().interactable = false;

            itemPreview.transform.Find("DiscardButton").gameObject.GetComponent<Button>().interactable = true;

            itemPreview.GetComponent<SlotReference>().itemSlot = slot;
        }
    }

    public void EatItem(GameObject slot)
    {
        // restore hunger
        int food = slot.GetComponent<PickupInfo>().foodRestore;
        int poison = slot.GetComponent<PickupInfo>().poisonDamage;

        player.GetComponent<PlayerResources>().EatFood(food, poison);

        DiscardItem(slot, 1);
    }

    public void DiscardItem(GameObject slot, int num)
    {
        // discard "num" of the items in selected slot
        // get slot number from object name
        char c = slot.name[13];
        int slotNum = (int)char.GetNumericValue(c) - 1;

        // remove item(s) from stack
        slotCount[slotNum] -= num;

        if (slotCount[slotNum] > 0)
            slot.transform.Find("ItemCount").gameObject.GetComponent<TextMeshProUGUI>().text = slotCount[slotNum].ToString();
        else
        {
            // slot is now empty
            // reset item preview to be blank
            ClearPreviewSlot("inventory");

            // empty the item slot script info
            slot.GetComponent<PickupInfo>().itemName = "";
            slot.GetComponent<PickupInfo>().itemDescription = "";
            slot.GetComponent<PickupInfo>().foodRestore = 0;
            slot.GetComponent<PickupInfo>().poisonDamage = 0;
            slot.GetComponent<PickupInfo>().itemSprite = null;

            // empty the item slot display
            slot.transform.Find("ItemIcon").gameObject.SetActive(false);
            slot.transform.Find("ItemCount").gameObject.SetActive(false);
            slot.GetComponent<Button>().interactable = false;
        }
    }

    public void ClearPreviewSlot(string screen)
    {
        if (screen == "inventory")
        {
            itemPreview.GetComponent<SlotReference>().itemSlot = null;
            itemPreview.transform.Find("ItemIcon").gameObject.SetActive(false);
            itemPreview.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            itemPreview.transform.Find("ItemDescription").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            itemPreview.transform.Find("EatButton").gameObject.GetComponent<Button>().interactable = false;
            itemPreview.transform.Find("DiscardButton").gameObject.GetComponent<Button>().interactable = false;
            itemPreview.transform.Find("EquipButton").gameObject.GetComponent<Button>().interactable = false;
        }
        else if (screen == "crafting")
        {
            craftingPreview.GetComponent<SlotReference>().itemSlot = null;
            craftingPreview.transform.Find("ItemIcon").gameObject.SetActive(false);
            craftingPreview.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            craftingPreview.transform.Find("ItemDescription").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            craftingPreview.transform.Find("RequiredItems1").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            craftingPreview.transform.Find("RequiredItems2").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            craftingPreview.transform.Find("RequiredItems3").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            craftingPreview.transform.Find("Quantity1").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            craftingPreview.transform.Find("Quantity2").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            craftingPreview.transform.Find("Quantity3").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            craftingPreview.transform.Find("CraftButton").gameObject.GetComponent<Button>().interactable = false;
        }
        else if (screen == "cooking")
        {
            cookingPreview.GetComponent<SlotReference>().itemSlot = null;
            cookingPreview.transform.Find("ItemIcon").gameObject.SetActive(false);
            cookingPreview.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            cookingPreview.transform.Find("ItemDescription").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            cookingPreview.transform.Find("RequiredItems1").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            cookingPreview.transform.Find("CookButton").gameObject.GetComponent<Button>().interactable = false;
        }
        else if (screen == "radio")
        {
            radioPreview.GetComponent<SlotReference>().itemSlot = null;
            radioPreview.transform.Find("ItemIcon").gameObject.SetActive(false);
            radioPreview.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            radioPreview.transform.Find("ItemDescription").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            radioPreview.transform.Find("RequiredQuantity").gameObject.GetComponent<TextMeshProUGUI>().text = "";
            radioPreview.transform.Find("RepairButton").gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public GameObject CheckForItem(string itemName, int num)
    {
        for (int i = 0; i < 9; i++)
        {
            if (itemSlot[i].GetComponent<PickupInfo>().itemName == itemName)
            {
                // found matching item in inventory
                //check if there are the right number of items in this stack
                if (slotCount[i] >= num)
                {
                    // this slot has the right number of items
                    return itemSlot[i];
                }
            }
        }

        // no matching item in inventory or not enough in stack
        return null;
    }

    GameObject CheckForEmptySlot()
    {
        for (int i = 0; i < 9; i++)
        {
            if (itemSlot[i].GetComponent<PickupInfo>().itemName == "")
            {
                // found empty slot
                return itemSlot[i];
            }
        }

        // no empty slots in inventory
        return null;
    }

    public void CraftingTab()
    {
        // player opens the crafting tab in crafting menu
        craftingTab = true;
        craftingTabObj.GetComponent<Button>().interactable = false;
        cookingTabObj.GetComponent<Button>().interactable = true;
        craftingObj.SetActive(true);
        cookingObj.SetActive(false);
    }

    public void CookingTab()
    {
        // player opens the cooking tab in crafting menu
        craftingTab = false;
        craftingTabObj.GetComponent<Button>().interactable = true;
        cookingTabObj.GetComponent<Button>().interactable = false;
        craftingObj.SetActive(false);
        cookingObj.SetActive(true);
    }

    public void CraftItem(GameObject slot)
    {
        GameObject newSlot;
        GameObject requiredItemSlot1;
        GameObject requiredItemSlot2 = null;
        GameObject requiredItemSlot3 = null;
        bool emptySlot = false;

        // craft a new item and discard consumed items from player's inventory
        // check if player already has some of this crafted item in their inventory
        newSlot = CheckForItem(slot.GetComponent<PickupInfo>().itemName, 1);
        if (newSlot == null)
        {
            // no current items, check for an empty slot for crafted item to go instead
            emptySlot = true;
            newSlot = CheckForEmptySlot();
            if (newSlot == null)
            {
                // no empty slots in the inventory
                Debug.Log("No Empty Inventory Slots!");
                return;
            }
        }

        // check for first crafting material
        requiredItemSlot1 = CheckForItem(slot.GetComponent<PickupInfo>().craftingMaterial1, slot.GetComponent<PickupInfo>().craftingQuantity1);
        if (requiredItemSlot1 == null)
        {
            // player does not have first crafting material in inventory
            Debug.Log("Missing Crafting Material(s)!");
            return;
        }

        // check for second crafting material
        if (slot.GetComponent<PickupInfo>().craftingMaterial2 != "")
        {
            requiredItemSlot2 = CheckForItem(slot.GetComponent<PickupInfo>().craftingMaterial2, slot.GetComponent<PickupInfo>().craftingQuantity2);
            if (requiredItemSlot2 == null)
            {
                // player does not have second crafting material in inventory
                Debug.Log("Missing Crafting Material(s)!");
                return;
            }

            requiredItemSlot3 = CheckForItem(slot.GetComponent<PickupInfo>().craftingMaterial3, slot.GetComponent<PickupInfo>().craftingQuantity3);
            if (requiredItemSlot3 == null)
            {
                // player does not have third crafting material in inventory
                Debug.Log("Missing Crafting Material(s)!");
                return;
            }
        }

        // player has all necessary things to craft the item
        // discard crafting materials
        DiscardItem(requiredItemSlot1, slot.GetComponent<PickupInfo>().craftingQuantity1);
        if (requiredItemSlot2 != null)
            DiscardItem(requiredItemSlot2, slot.GetComponent<PickupInfo>().craftingQuantity2);
        if (requiredItemSlot3 != null)
            DiscardItem(requiredItemSlot3, slot.GetComponent<PickupInfo>().craftingQuantity3);

        // give player new crafted item in empty slot
        if (emptySlot)
        {
            newSlot.GetComponent<PickupInfo>().itemName = slot.GetComponent<PickupInfo>().itemName;
            string description = slot.GetComponent<PickupInfo>().itemDescription.Replace("NEWLINE", "\n");
            newSlot.GetComponent<PickupInfo>().itemDescription = description;
            newSlot.GetComponent<PickupInfo>().foodRestore = slot.GetComponent<PickupInfo>().foodRestore;
            newSlot.GetComponent<PickupInfo>().itemSprite = slot.GetComponent<PickupInfo>().itemSprite;
            newSlot.GetComponent<PickupInfo>().isTool = slot.GetComponent<PickupInfo>().isTool;
            newSlot.transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = slot.GetComponent<PickupInfo>().itemSprite;
            newSlot.transform.Find("ItemIcon").gameObject.SetActive(true);
            newSlot.GetComponent<Button>().interactable = true;
        }

        // set number of items in this slot
        char c = newSlot.name[13];
        int slotNum = (int)char.GetNumericValue(c) - 1;
        slotCount[slotNum] += 1;
        newSlot.transform.Find("ItemCount").gameObject.GetComponent<TextMeshProUGUI>().text = slotCount[slotNum].ToString();
        newSlot.transform.Find("ItemCount").gameObject.SetActive(true);

        // re-check whether player can craft another item and set button accordingly
        GameObject foundSlot = CheckForItem(slot.GetComponent<PickupInfo>().craftingMaterial1, slot.GetComponent<PickupInfo>().craftingQuantity1);
        if (foundSlot != null)
        {
            // player has first required material
            if (slot.GetComponent<PickupInfo>().craftingMaterial2 != "")
            {
                foundSlot = CheckForItem(slot.GetComponent<PickupInfo>().craftingMaterial2, slot.GetComponent<PickupInfo>().craftingQuantity2);
                if (foundSlot != null)
                {
                    // player has second required material
                    foundSlot = CheckForItem(slot.GetComponent<PickupInfo>().craftingMaterial3, slot.GetComponent<PickupInfo>().craftingQuantity3);
                    if (foundSlot != null)
                    {
                        // player has third required material
                        craftingPreview.transform.Find("CraftButton").gameObject.GetComponent<Button>().interactable = true;
                    }
                    else
                        craftingPreview.transform.Find("CraftButton").gameObject.GetComponent<Button>().interactable = false;
                }
                else
                    craftingPreview.transform.Find("CraftButton").gameObject.GetComponent<Button>().interactable = false;
            }
            else
                cookingPreview.transform.Find("CookButton").gameObject.GetComponent<Button>().interactable = true;
        }
        else
        {
            cookingPreview.transform.Find("CookButton").gameObject.GetComponent<Button>().interactable = false;
            craftingPreview.transform.Find("CraftButton").gameObject.GetComponent<Button>().interactable = false;
        }
    }

    public void RepairRadio(GameObject slot)
    {
        // repair the radio with the supplied item slot info
        // check for required items for repair
        GameObject requiredItemSlot = CheckForItem(slot.GetComponent<PickupInfo>().itemName, slot.GetComponent<PickupInfo>().craftingQuantity1);
        if (requiredItemSlot == null)
        {
            // player does not have the required material in inventory
            Debug.Log("Missing Crafting Material(s)!");
            return;
        }

        // player has the necessary item to repair
        // discard crafting materials
        DiscardItem(requiredItemSlot, slot.GetComponent<PickupInfo>().craftingQuantity1);

        // set button and item slot to be inactive
        slot.GetComponent<Button>().interactable = false;
        radioPreview.transform.Find("RepairButton").gameObject.GetComponent<Button>().interactable = false;

        // set corresponding repair item to be marked as done
        switch (slot.GetComponent<PickupInfo>().itemName)
        {
            case "Wires":
                GameObject.Find("Radio").GetComponent<RadioScript>().wiresDone = true;
                break;
            case "Bolts":
                GameObject.Find("Radio").GetComponent<RadioScript>().boltsDone = true;
                break;
            case "Metal Plate":
                GameObject.Find("Radio").GetComponent<RadioScript>().metalDone = true;
                break;
            case "Battery":
                GameObject.Find("Radio").GetComponent<RadioScript>().batteryDone = true;
                break;
        }
    }

    public void EquipTool(GameObject slot)
    {
        // equip the currently previewed item to the player's hotbar and make tool appear
        switch (slot.gameObject.GetComponent<PickupInfo>().itemName)
        {
            case "Makeshift Axe":
                // goes in slot 1
                toolSlot1.transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = slot.GetComponent<PickupInfo>().itemSprite;
                toolSlot1.transform.Find("ItemIcon").gameObject.SetActive(true);
                makeshiftAxe.SetActive(true);
                break;
            case "Makeshift Pickaxe":
                // goes in slot 2
                toolSlot2.transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = slot.GetComponent<PickupInfo>().itemSprite;
                toolSlot2.transform.Find("ItemIcon").gameObject.SetActive(true);
                makeshiftPickaxe.SetActive(true);
                break;
            case "Metal Axe":
                // goes in slot 1
                toolSlot1.transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = slot.GetComponent<PickupInfo>().itemSprite;
                toolSlot1.transform.Find("ItemIcon").gameObject.SetActive(true);
                makeshiftAxe.SetActive(false);
                metalAxe.SetActive(true);
                break;
            case "Metal Pickaxe":
                // goes in slot 2
                toolSlot2.transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = slot.GetComponent<PickupInfo>().itemSprite;
                toolSlot2.transform.Find("ItemIcon").gameObject.SetActive(true);
                makeshiftPickaxe.SetActive(false);
                metalPickaxe.SetActive(true);
                break;
        }

        // remove item from player's inventory
        DiscardItem(slot, 1);

        // deactivate the equip button
        itemPreview.transform.Find("EquipButton").gameObject.GetComponent<Button>().interactable = false;
    }
}
