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
    public GameObject testItem, testItemWorld;

    public Transform mouseItemHolder;
    private GameObject itemBeingDragged;
    public int itemReleaseRange;
    private int itemLocationInUi;

    //nani?
    Vector3 inventoryLocation;

    private void Start()
    {
        //lists
        allSlots = 25;
        allEquipmentSlots = 6;
        allHotbarSlots = 6;
        slot = new GameObject[allSlots];
        equipmentSlots = new GameObject[allEquipmentSlots];
        hotbarSlots = new GameObject[allHotbarSlots];
        inventoryLocation = inventory.transform.position;
        inventory.transform.position = new Vector3(10000, 0, 0);

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
            AddItemFromOutsideOfInventory(0);
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
            if(inventoryEnabled)
            {
                inventory.transform.position = inventoryLocation;
            }
            else
            {
                inventory.transform.position = new Vector3(10000, 0, 0);
            }
            GetComponent<PlayerController>().LockCamera();
        }
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
    public void AddItemFromOutsideOfInventory(int itemId)
    {
        //uses test item
        GameObject test = Instantiate(testItem, mouseItemHolder);
        AddItemToInventory(test, -1);
        itemLocationInUi = 0;
        //list needs to be made
    }
    public void AddItemToInventory(GameObject newItem, int slotNumber)
    {
        if (slotNumber > -1)
        {
            if (itemLocationInUi == 0)
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
            if (itemLocationInUi == 1)
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
            if (itemLocationInUi == 2)
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
        }
        else
        {
            for (int i = 0; i < allSlots; i++)
            {
                if (slot[i].GetComponent<Slot>().CheckForItem() == false)
                {
                    slot[i].GetComponent<Slot>().RecieveItem(newItem);
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
                Destroy(itemBeingDragged);
                itemBeingDragged = null;
                DropItem();
            }
            if (itemBeingDragged)
            {
                AddItemToInventory(itemBeingDragged, thisSlot);
                itemBeingDragged = null;
            }
            foreach (Transform child in mouseItemHolder)
            {
                AddItemToInventory(child.gameObject, -1);
            }
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
    public void DropItem()
    {
        GameObject droppedItem = Instantiate(testItemWorld, transform.position + transform.forward * 2, Quaternion.identity);
        droppedItem.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position + transform.forward - transform.up, 2);
        droppedItem.GetComponent<Item>().ToInventory(false);
    }
}
