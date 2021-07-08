using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    bool inventoryEnabled;
    public GameObject inventory;

    private int allSlots;
    private int enabledSlots;
    private GameObject[] slot;

    public GameObject slotHolder;
    public GameObject testItem;

    public Transform mouseItemHolder;
    private GameObject itemBeingDragged;
    public int itemReleaseRange;

    private void Start()
    {
        allSlots = 25;
        slot = new GameObject[allSlots];
        for (int i = 0; i < allSlots; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < 5; i++)
        {
            GameObject test = Instantiate(testItem, mouseItemHolder);
            AddItemToInventory(test, -1);
        }
    }

    void Update()
    {
        OpenInventory();
        if(inventoryEnabled)
        {
            mouseItemHolder.position = Input.mousePosition;
        }
    }

    //functions
    void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryEnabled = !inventoryEnabled;
            GetComponent<PlayerController>().LockCamera();
        }
        inventory.SetActive(inventoryEnabled);
        Cursor.visible = inventoryEnabled;
        if (inventoryEnabled)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void AddItemToInventory(GameObject newItem, int slotNumber)
    {
        for (int i = 0; i < allSlots; i++)
        {
            if(slotNumber > -1)
            {
                if (slot[slotNumber].GetComponent<Slot>().CheckForItem() == false)
                {
                    slot[slotNumber].GetComponent<Slot>().RecieveItem(newItem);
                    return;
                }
                else
                {
                    return;
                }
            }
            if (slot[i].GetComponent<Slot>().CheckForItem() == false)
            {
                slot[i].GetComponent<Slot>().RecieveItem(newItem);
                return;
            }
        }
    }
    public void BeginDrag(GameObject draggedItem)
    {
        draggedItem.transform.SetParent(mouseItemHolder);
        draggedItem.transform.position = Input.mousePosition;
        itemBeingDragged = draggedItem;
    }
    public void EndDrag()
    {
        if (itemBeingDragged)
        {
            int thisSlot = -1;
            float smallestDistance = itemReleaseRange;
            for (int i = 0; i < allSlots; i++)
            {
                float distance = Vector3.Distance(slot[i].transform.position, Input.mousePosition);
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    thisSlot = i;
                }
            }
            AddItemToInventory(itemBeingDragged, thisSlot);
        }
    }
}
