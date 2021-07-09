using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    bool inventoryEnabled;
    public GameObject inventory;

    private int allSlots, allEquipmentSlots, allHotbarSlots;
    private int enabledSlots;
    public GameObject[] slot, equipmentSlots, hotbarSlots; // equipment / hotbar / drop

    public GameObject slotHolder, equipmentSlotHolder, hotbarSlotsHolder;
    public GameObject testItem;

    public Transform mouseItemHolder;
    private GameObject itemBeingDragged;
    public int itemReleaseRange;
    private int itemLocationInUi;

    private void Start()
    {
        //lists
        allSlots = 25;
        allEquipmentSlots = 6;
        allHotbarSlots = 6;
        slot = new GameObject[allSlots];
        equipmentSlots = new GameObject[allEquipmentSlots];
        hotbarSlots = new GameObject[allHotbarSlots];

        for (int i = 0; i < allSlots; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < allEquipmentSlots; i++)
        {
            equipmentSlots[i] = equipmentSlotHolder.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < allHotbarSlots; i++)
        {
            hotbarSlots[i] = hotbarSlotsHolder.transform.GetChild(i).gameObject;
        }

        //test
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
            if (itemLocationInUi == 0)
            {
                //inventory
                if (slotNumber > -1)
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
            else if (itemLocationInUi == 1)
            {
                //hotbar
                if (slotNumber > -1)
                {
                    if (hotbarSlots[slotNumber].GetComponent<Slot>().CheckForItem() == false)
                    {
                        hotbarSlots[slotNumber].GetComponent<Slot>().RecieveItem(newItem);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                if (hotbarSlots[i].GetComponent<Slot>().CheckForItem() == false)
                {
                    hotbarSlots[i].GetComponent<Slot>().RecieveItem(newItem);
                    return;
                }
            }
            else if (itemLocationInUi == 2)
            {
                //equipment
                if (slotNumber > -1)
                {
                    if (equipmentSlots[slotNumber].GetComponent<Slot>().CheckForItem() == false)
                    {
                        equipmentSlots[slotNumber].GetComponent<Slot>().RecieveItem(newItem);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                if (equipmentSlots[i].GetComponent<Slot>().CheckForItem() == false)
                {
                    equipmentSlots[i].GetComponent<Slot>().RecieveItem(newItem);
                    return;
                }
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
            if (itemLocationInUi == 0)
            {
                //inventory
                for (int i = 0; i < allSlots; i++)
                {
                    float distance = Vector3.Distance(slot[i].transform.position, Input.mousePosition);
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        thisSlot = i;
                    }
                }
            }
            else if (itemLocationInUi == 1)
            {
                //hotbar
                for (int i = 0; i < allHotbarSlots; i++)
                {
                    float distance = Vector3.Distance(hotbarSlots[i].transform.position, Input.mousePosition);
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        thisSlot = i;
                    }
                }
            }
            else if (itemLocationInUi == 2)
            {
                //equipment
                for (int i = 0; i < allEquipmentSlots; i++)
                {
                    float distance = Vector3.Distance(equipmentSlots[i].transform.position, Input.mousePosition);
                    if (distance < smallestDistance)
                    {
                        smallestDistance = distance;
                        thisSlot = i;
                    }
                }
            }
            else if (itemLocationInUi == 3)
            {
                //drop
            }

            AddItemToInventory(itemBeingDragged, thisSlot);
            itemBeingDragged = null;
        }
    }
    public void GiveItemLocation(int location)
    {
        //0 = default
        //1 = hotbar
        //2 = equipment
        //3 = drop
        itemLocationInUi = location;
    }
}
