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
    public List<Item> inventoryContent;

    //if itemstack overflows
    int overflowAmount, overflowItemID;

    private int slotNumberDragged;
    public Transform mouseItemHolder;

    public GameObject chestPanel;

    public GameObject slotHolder;
    public GameObject emptyItem;

    Inventory lastInventory;

    public bool itemBeingDragged;

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
            inventoryContent.Add(Instantiate(emptyItem.GetComponent<Item>(), slot[i].transform));
        }
    }
    public void AddItemToInventoryList(int itemId, int itemAmount, bool ctrlClicked)
    {
        //filter out all slots that have items in it
        if (!ctrlClicked)
        {
            checkTheseNumbers.Clear();
            for (int i = 0; i < inventoryContent.Count; i++)
            {
                checkTheseNumbers.Add(i);
                if (inventoryContent[i].stackAmount > 0 && inventoryContent[i].itemId != itemId)
                {
                    //-1 == this slot has an item
                    checkTheseNumbers[i] = -1;
                }
                if (inventoryContent[i].stackAmount == inventoryContent[i].maxStackAmount)
                {
                    checkTheseNumbers[i] = -1;
                }
                if (i > 5 && i < 12)
                {
                    //exclude equipment slots
                    checkTheseNumbers[i] = -1;
                }
                if (inventoryContent[i].stackAble == false)
                {
                    //exculde non-stable items
                    checkTheseNumbers[i] = -1;
                }
            }
            for (int i = 0; i < inventoryContent.Count; i++)
            {
                if (checkTheseNumbers[i] == i)
                {
                    if (inventoryContent[i].itemId == itemId)
                    {
                        if (itemAmount + inventoryContent[i].stackAmount <= inventoryContent[i].maxStackAmount)
                        {
                            itemAmount += inventoryContent[i].stackAmount;
                        }
                        else
                        {
                            overflowAmount = itemAmount + inventoryContent[i].stackAmount - inventoryContent[i].maxStackAmount;
                            itemAmount = inventoryContent[i].maxStackAmount;
                            overflowItemID = inventoryContent[i].itemId;
                        }
                    }
                    inventoryContent[i].itemId = itemId;
                    inventoryContent[i].stackAmount = itemAmount;
                    UpdateInventory();
                    return;
                }
            }
        }
        else
        {
            if (slotNumberDragged > -1)
            {
                print(slotNumberDragged);
                if (mouseItemHolder.childCount > 0)
                {
                    int tempId = mouseItemHolder.GetChild(0).GetComponent<Item>().itemId;
                    int tempAmount = mouseItemHolder.GetChild(0).GetComponent<Item>().stackAmount;
                    int tempOld = mouseItemHolder.GetChild(0).GetComponent<Item>().oldSlotNumber;
                    Destroy(mouseItemHolder.GetChild(0).gameObject);

                    if (inventoryContent[slotNumberDragged].stackAmount > 0 && inventoryContent[slotNumberDragged].itemId != tempId)
                    {
                        if (tempOld >= 0)
                        {
                            inventoryContent[tempOld].itemId = inventoryContent[slotNumberDragged].itemId;
                            inventoryContent[tempOld].stackAmount = inventoryContent[slotNumberDragged].stackAmount;
                        }
                        else
                        {
                            AddItemToInventoryList(tempId, tempAmount, false);
                        }
                    }
                    else if (inventoryContent[slotNumberDragged].itemId == tempId)
                    {
                        if (tempOld != slotNumberDragged)
                        {
                            if (tempAmount + inventoryContent[slotNumberDragged].stackAmount <= inventoryContent[slotNumberDragged].maxStackAmount)
                            {
                                tempAmount += inventoryContent[slotNumberDragged].stackAmount;
                                inventoryContent[tempOld].itemId = 0;
                                inventoryContent[tempOld].stackAmount = 0;
                            }
                            else
                            {
                                overflowAmount = tempAmount + inventoryContent[slotNumberDragged].stackAmount - inventoryContent[slotNumberDragged].maxStackAmount;
                                tempAmount = inventoryContent[slotNumberDragged].maxStackAmount;
                                overflowItemID = inventoryContent[slotNumberDragged].itemId;
                            }
                        }
                    }
                    else
                    {
                        if (tempOld >= 0)
                        {
                            inventoryContent[tempOld].itemId = 0;
                            inventoryContent[tempOld].stackAmount = 0;
                        }
                    }
                    inventoryContent[slotNumberDragged].itemId = tempId;
                    inventoryContent[slotNumberDragged].stackAmount = tempAmount;

                    UpdateInventory();
                }
            }
        }
    }
    public void UpdateInventory()
    {
        for (int i = 0; i < inventoryContent.Count; i++)
        {
            //check if item exists
            if (inventoryContent[i].itemId >= ItemList.itemListUi.Count)
            {
                inventoryContent[i].itemId = 1;
            }

            foreach (Transform item in slot[i].transform)
            {
                Destroy(item.gameObject);
            }
            if (inventoryContent[i].stackAmount > 0)
            {
                GameObject temp = Instantiate(ItemList.itemListUi[inventoryContent[i].itemId], slot[i].transform);
                temp.GetComponent<Item>().SetUp(inventoryContent[i].stackAmount, i, default, this);
                inventoryContent[i] = temp.GetComponent<Item>();
            }
        }
        //adds overflow items to inventory for next slot
        if (overflowAmount > 0)
        {
            int tempAmount = overflowAmount;
            overflowAmount = 0;
            AddItemToInventoryList(overflowItemID, overflowAmount, false);
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
}
