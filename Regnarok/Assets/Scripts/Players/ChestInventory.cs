using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChestInventory : MonoBehaviour
{
    [HideInInspector] public PhotonView pv;
    public GameObject[] slot;
    int allSlots;

    public List<int> checkTheseNumbers;
    public List<GameObject> inventoryContent;

    //if itemstack overflows
    int overflowAmount, overflowItemID;

    private int slotNumberDragged;
    public Transform mouseItemHolder;

    public GameObject chestPanel;

    public GameObject slotHolder;
    public GameObject emptyItem;

    Inventory lastInventory;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        chestPanel.SetActive(false);
    }
    private void Start()
    {
        //lists
        allSlots = 25;
        slot = new GameObject[allSlots];
        checkTheseNumbers = new List<int>();

        for (int i = 0; i < allSlots; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i).gameObject;
            slot[i].GetComponent<Slot>().slotNumber = i;
        }

        //put empty items in inventory list
        for (int i = 0; i < allSlots; i++)
        {
            inventoryContent.Add(default);
        }
    }
    public void AddItemToInventoryList(int itemId, int itemAmount, bool isAlreadyInInventory, int oldSlot)
    {
        if (!isAlreadyInInventory)
        {
            int useThisSlot = -1;
            for (int i = 0; i < inventoryContent.Count; i++)
            {
                if (useThisSlot > -1)
                {
                    continue;
                }
                if (inventoryContent[i] != null)
                {
                    Item item = inventoryContent[i].GetComponent<Item>();
                    if (item.itemId == itemId)
                    {
                        if (item.stackAmount + itemAmount <= item.maxStackAmount)
                        {
                            int useThis = item.stackAmount += itemAmount;
                            item.SetUp(useThis, i, default, this);
                            return;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (inventoryContent[i] == null)
                {
                    useThisSlot = i;
                }
            }
            if (useThisSlot > -1)
            {
                GameObject temp = Instantiate(ItemList.itemListUi[itemId], slot[useThisSlot].transform);
                temp.GetComponent<Item>().SetUp(itemAmount, useThisSlot, default, this);
                inventoryContent[useThisSlot] = temp;
                return;
            }
        }
        else
        {
            if (lastInventory.draggedItem != null)
            {
                if (inventoryContent[slotNumberDragged] == null)
                {
                    inventoryContent[slotNumberDragged] = lastInventory.draggedItem.gameObject;
                    lastInventory.draggedItem.transform.SetParent(slot[slotNumberDragged].transform);
                    lastInventory.draggedItem.transform.position = slot[slotNumberDragged].transform.position;
                    lastInventory.draggedItem.GetComponent<Item>().SetUp(lastInventory.draggedItem.stackAmount, slotNumberDragged, default, this);
                    lastInventory.itemBeingDragged = false;
                    lastInventory.draggedItem = null;
                }
                else if (inventoryContent[slotNumberDragged] != null)
                {
                    Item item = inventoryContent[slotNumberDragged].GetComponent<Item>();
                    if (item.itemId == lastInventory.draggedItem.itemId)
                    {
                        if (item.stackAmount + lastInventory.draggedItem.stackAmount <= item.maxStackAmount)
                        {
                            int useThis = item.stackAmount += lastInventory.draggedItem.stackAmount;
                            item.SetUp(useThis, slotNumberDragged, default, this);
                            Destroy(lastInventory.draggedItem.gameObject);
                            lastInventory.itemBeingDragged = false;
                        }
                        else
                        {
                            int useThis = item.stackAmount + lastInventory.draggedItem.stackAmount - item.maxStackAmount;
                            item.SetUp(item.maxStackAmount, slotNumberDragged, default, this);
                            lastInventory.draggedItem.SetUp(useThis, -1, default, this);
                            lastInventory.itemBeingDragged = true;
                            return;
                        }
                    }
                    else
                    {
                        //store slotitem
                        GameObject SwappedItem = inventoryContent[slotNumberDragged];
                        GameObject oldDraggedItem = lastInventory.draggedItem.gameObject;

                        lastInventory.draggedItem.transform.SetParent(slot[slotNumberDragged].transform);
                        lastInventory.draggedItem.transform.position = slot[slotNumberDragged].transform.position;
                        lastInventory.draggedItem.GetComponent<Item>().SetUp(lastInventory.draggedItem.stackAmount, slotNumberDragged, default, this);

                        lastInventory.itemBeingDragged = true;
                        lastInventory.draggedItem = null;
                        SwappedItem.GetComponent<Item>().SwapItem();
                        inventoryContent[slotNumberDragged] = oldDraggedItem;
                    }
                }
            }
        }
        CheckSlots();
    }
    void CheckSlots()
    {
        for (int i = 0; i < slot.Length; i++)
        {
            if(slot[i].transform.childCount == 0)
            {
                inventoryContent[i] = null;
            }
        }
    }
    public void OpenChest(Inventory inv)
    {
        lastInventory = inv;
        lastInventory.ToggleInventoryFromChest(this);
    }
    public void CloseChest()
    {
        chestPanel.SetActive(false);
        lastInventory.ToggleInventoryFromChest(this);
    }
    public void GiveMouseLocationForInventory(int slotNumber)
    {
        slotNumberDragged = slotNumber;
    }
    private void Update()
    {
        if (lastInventory)
        {
            float distance = Vector3.Distance(transform.position, lastInventory.transform.position);
            if (distance > 6)
            {
                chestPanel.SetActive(false);
                lastInventory = null;
            }
        }
    }
}
