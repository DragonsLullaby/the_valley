using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotReference : MonoBehaviour
{
    public GameObject itemSlot;

    public void EatButton()
    {
        // call eat item function in the inventory system script
        GameObject.Find("GameManager").GetComponent<InventorySystem>().EatItem(itemSlot);
    }

    public void DiscardButton()
    {
        // call discard function in the inventory system script
        GameObject.Find("GameManager").GetComponent<InventorySystem>().DiscardItem(itemSlot, 1);
    }

    public void CraftButton()
    {
        // call craft item function in the inventory system script
        GameObject.Find("GameManager").GetComponent<InventorySystem>().CraftItem(itemSlot);
    }

    public void RepairButton()
    {
        // call repair radio function in the inventory system script
        GameObject.Find("GameManager").GetComponent<InventorySystem>().RepairRadio(itemSlot);
    }

    public void EquipButton()
    {
        // call equip item function in the inventory system script
        GameObject.Find("GameManager").GetComponent<InventorySystem>().EquipTool(itemSlot);
    }
}
