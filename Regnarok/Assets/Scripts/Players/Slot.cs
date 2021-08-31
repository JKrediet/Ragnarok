using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Slot : MonoBehaviour
{
    public int slotNumber;
    public GameObject item;
    public int slotType = 0;

    [SerializeField] Inventory inv;

    public void GiveSlotnumber()
    {
        inv.GiveMouseLocationForInventory(slotNumber);
    }
    public void GiveItemToSlot()
    {
        inv.itemBeingDragged = false;
        inv.AddItemToInventoryList(-1, -1, true);
    }
}
