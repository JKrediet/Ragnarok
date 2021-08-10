using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Slot : MonoBehaviour
{
    public int slotNumber;
    public GameObject item;
    public int slotType = 0;
    public int stackSize;
    public int maxStackSize;

    [SerializeField] Inventory inv;
    [SerializeField] TextMeshProUGUI stackText;

    private void Awake()
    {
        stackText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public bool CheckForItem()
    {
        return item ? true : false;
    }
    public void RecieveItem(GameObject newItem)
    {
        if (!item)
        {
            item = newItem;
            stackSize = item.GetComponent<Item>().stackAmount;
            item.transform.SetParent(transform);
            item.transform.position = transform.position;
            item.GetComponent<Item>().ToInventory(true);
        }
        stackText.text = stackSize.ToString();
    }
    public void GiveSlotnumber()
    {
        inv.GiveMouseLocationForInventory(slotNumber);
    }
    public void GiveItemToSlot()
    {
        inv.itemBeingDragged = false;
        inv.AddItemToInventoryList(-1, -1, true, -1);
    }
}
