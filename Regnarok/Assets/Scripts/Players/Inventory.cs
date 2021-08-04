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

    public Transform mouseItemHolder, handHolder;
    private GameObject itemBeingDragged;
    public int itemReleaseRange;
    private int itemLocationInUi;

    private int draggedStackAmount;

    //hotbar indecator
    public GameObject hotbarIndecator;
    int hotbarLocation;

    //test
    List<int> checkTheseNumbers;

    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6
     };

private void Start()
    {
        //lists
        allSlots = 25;
        allEquipmentSlots = 6;
        allHotbarSlots = 6;
        slot = new GameObject[allSlots];
        equipmentSlots = new GameObject[allEquipmentSlots];
        hotbarSlots = new GameObject[allHotbarSlots];
        checkTheseNumbers = new List<int>();

        inventory.SetActive(false);

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
        AddItemFromOutsideOfInventory(testItem, 5);
    }

    void Update()
    {
        OpenInventory();
        if (inventoryEnabled)
        {
            mouseItemHolder.position = Input.mousePosition;
        }
        if (Input.mouseScrollDelta.y > 0 || Input.mouseScrollDelta.y < 0)
        {
            hotbarLocation -= (int)Input.mouseScrollDelta.y;
            if (hotbarLocation > hotbarSlots.Length - 1)
            {
                hotbarLocation = 0;
            }
            else if (hotbarLocation < 0)
            {
                hotbarLocation = hotbarSlots.Length - 1;
            }
            SelectItemInHotbar(hotbarLocation);
        }
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                hotbarLocation = i;
                SelectItemInHotbar(hotbarLocation);
            }
        }
    }
    //functions
    void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryEnabled = !inventoryEnabled;
            inventory.SetActive(inventoryEnabled);
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
    public void AddItemFromOutsideOfInventory(GameObject itemId, int itemAmount)
    {
        if(itemAmount == 0)
        {
            itemAmount = 1;
        }
        //uses test item
        GameObject uiItem = Instantiate(ItemList.itemListUi[itemId.GetComponent<Item>().itemId]);
        print(uiItem.name);
        itemLocationInUi = 0;
        AddItemToInventory(uiItem, -1, itemAmount);
    }
    public void AddItemToInventory(GameObject newItem, int slotNumber, int itemAmount)
    {
        newItem.GetComponent<Item>().stackAmount = itemAmount;
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
                    checkTheseNumbers.Clear();
                    for (int i = 0; i < allSlots; i++)
                    {
                        checkTheseNumbers.Add(i);
                    }
                    InventorySort(newItem, checkTheseNumbers, itemAmount);
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
                    //AddItemToInventory(newItem, -1, itemAmount);
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
                    //AddItemToInventory(newItem, -1, itemAmount);
                    return;
                }
            }
        }
        else
        {
            checkTheseNumbers.Clear();
            for (int i = 0; i < allSlots; i++)
            {
                checkTheseNumbers.Add(i);
            }
            InventorySort(newItem, checkTheseNumbers, itemAmount);
        }
    }
    void InventorySort(GameObject newItem, List<int> checkTheseSlots, int itemAmount)
    {
        //filter slots on wich can be used
        for (int i = 0; i < allSlots; i++)
        {
            if (checkTheseSlots[i] == i)
            {
                if (slot[i].GetComponent<Slot>().CheckForItem() == true)
                {
                    if (slot[i].GetComponent<Slot>().item.GetComponent<Item>().itemId == newItem.GetComponent<Item>().itemId && slot[i].GetComponent<Slot>().item.GetComponent<Item>().stackAble)
                    {
                        //dummie
                    }
                    else
                    {
                        checkTheseNumbers[i] = -1;
                    }
                }
            }
        }
        for (int i = 0; i < allSlots; i++)
        {
            if(checkTheseSlots[i] == i)
            {
                if (slot[i].GetComponent<Slot>().CheckForItem() == true)
                {
                    if (slot[i].GetComponent<Slot>().item.GetComponent<Item>().itemId == newItem.GetComponent<Item>().itemId && slot[i].GetComponent<Slot>().item.GetComponent<Item>().stackAble)
                    {
                        slot[i].GetComponent<Slot>().RecieveItem(newItem);
                        return;
                    }
                    return;
                }
                else
                {
                    slot[i].GetComponent<Slot>().RecieveItem(newItem);
                    return;
                }
            }
        }
    }
    public void BeginDrag(GameObject draggedItem, int stackAmount)
    {
        draggedStackAmount = stackAmount;
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
                if (itemBeingDragged.GetComponent<Item>().isEquipment == true)
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
                else
                {
                    itemLocationInUi = -1;
                    EndDrag();
                }
            }
            else if (itemLocationInUi == 3)
            {
                //drop
                DropItem(itemBeingDragged);
                Destroy(itemBeingDragged);
                itemBeingDragged = null;
            }
            if (itemBeingDragged)
            {
                for (int i = 0; i < draggedStackAmount; i++)
                {
                    AddItemToInventory(itemBeingDragged, thisSlot, itemBeingDragged.GetComponent<Item>().stackAmount);
                }
                itemBeingDragged = null;
                draggedStackAmount = 0;
            }
            CheckIfItemInHand(hotbarLocation);
            //foreach (Transform child in mouseItemHolder)
            //{
            //    Destroy(child.gameObject);
            //}
        }
    }
    public void GiveItemLocation(int location)
    {
        //function called from ui
        //0 = default
        //1 = hotbar
        //2 = equipment
        //3 = drop
        itemLocationInUi = location;
    }
    public void DropItem(GameObject itemId)
    {
        GameObject droppedItem = Instantiate(ItemList.itemListIngame[itemId.GetComponent<Item>().itemId], transform.position + transform.forward * 2, Quaternion.identity);
        droppedItem.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position + transform.forward - transform.up, 2);
        droppedItem.GetComponent<Item>().ToInventory(false);
        droppedItem.GetComponent<Item>().stackAmount = draggedStackAmount;
        draggedStackAmount = 0;
    }
    void SelectItemInHotbar(int nummertje)
    {
        hotbarIndecator.transform.position = hotbarSlots[nummertje].transform.position;
        CheckIfItemInHand(nummertje);
    }
    void CheckIfItemInHand(int slotNumber)
    {
        if (hotbarSlots[slotNumber].GetComponent<Slot>().item)
        {
            ShowItemInHand(hotbarSlots[slotNumber].GetComponent<Slot>().item.GetComponent<Item>().itemId);
            //hier check welk item in hand
            GiveItemStats(slotNumber);
        }
        else
        {
            ShowItemInHand(-1);
        }
    }
    void ShowItemInHand(int itemId)
    {
        foreach (Transform child in handHolder)
        {
            Destroy(child.gameObject);
        }
        if(itemId >= 0)
        {
            GameObject handItem = Instantiate(ItemList.itemListIngame[itemId], handHolder.transform.position, handHolder.rotation, handHolder);
            handItem.GetComponent<Rigidbody>().isKinematic = true;
            handItem.GetComponent<Rigidbody>().useGravity = false;
            handItem.GetComponent<Collider>().enabled = false;
        }
    }
    void GiveItemStats(int nummertje)
    {
        GetComponent<PlayerController>().GiveItemStats(hotbarSlots[nummertje].GetComponent<Slot>().item.GetComponent<Item>());
    }
}
