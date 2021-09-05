using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.IO;

public class Inventory : MonoBehaviour
{
    [HideInInspector] public PhotonView pv;
    bool inventoryEnabled;
    public GameObject inventory;

    private int allSlots, allEquipmentSlots, allHotbarSlots;
    public GameObject[] slot; 

    public GameObject slotHolder, equipmentSlotHolder, hotbarSlotsHolder;
    public Item emptyItem;

    public Transform mouseItemHolder, handHolder;
    public bool itemBeingDragged;
    public int itemReleaseRange;
    private int slotNumberDragged;

    public Item draggedItem;

    //hotbar indecator
    public GameObject hotbarIndecator;
    int hotbarLocation;

    public List<int> checkTheseNumbers;
    public List<Item> inventoryContent;

    //if itemstack overflows
    int overflowAmount, overflowItemID;

    ChestInventory lastChest;

    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6
     };

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        inventory.SetActive(false);
        if (pv.IsMine)
        {
            return;
        }
        else
        {
            inventory.transform.parent.gameObject.SetActive(false);
            enabled = false;
        }
    }
    private void Start()
    {
        //lists
        allSlots = 25;
        allEquipmentSlots = 6;
        allHotbarSlots = 6;
        slot = new GameObject[allSlots + allEquipmentSlots + allHotbarSlots];
        checkTheseNumbers = new List<int>();

        for (int i = 0; i < allHotbarSlots; i++)
        {
            slot[i] = hotbarSlotsHolder.transform.GetChild(i).gameObject;
            slot[i].GetComponent<Slot>().slotNumber = i;
        }
        for (int i = 6; i < allEquipmentSlots + 6; i++)
        {
            slot[i] = equipmentSlotHolder.transform.GetChild(i - 6).gameObject;
            slot[i].GetComponent<Slot>().slotNumber = i;
        }
        for (int i = 12; i < allSlots + 12; i++)
        {
            slot[i] = slotHolder.transform.GetChild(i - 12).gameObject;
            slot[i].GetComponent<Slot>().slotNumber = i;
        }

        //put empty items in inventory list
        for (int i = 0; i < 37; i++)
        {
            GameObject yes = Instantiate(ItemList.itemListUi[0], slot[i].transform);
            yes.GetComponent<Item>().SetUp(1, i, this, default);
            inventoryContent.Add(default);
            inventoryContent[i] = yes.GetComponent<Item>();
        }
    }

    void Update()
    {
        OpenInventory();
        if (inventoryEnabled)
        {
            mouseItemHolder.position = Input.mousePosition;
        }
        ScrollHotbar();
    }
    //functions
    void ScrollHotbar()
    {
        //scroll in hotbar
        if (Input.mouseScrollDelta.y > 0 || Input.mouseScrollDelta.y < 0)
        {
            hotbarLocation -= (int)Input.mouseScrollDelta.y;
            if (hotbarLocation > allHotbarSlots - 1)
            {
                hotbarLocation = 0;
            }
            else if (hotbarLocation < 0)
            {
                hotbarLocation = allHotbarSlots - 1;
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
    void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryEnabled = !inventoryEnabled;
            inventory.SetActive(inventoryEnabled);
            GetComponent<PlayerController>().LockCamera();
            if(!inventoryEnabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
                if (mouseItemHolder.childCount > 0)
                {
                    DropItem();
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            Cursor.visible = inventoryEnabled;
        }
    }
    public void ToggleInventoryFromChest(ChestInventory chest)
    {
        lastChest = chest;
        inventoryEnabled = !inventoryEnabled;
        inventory.SetActive(inventoryEnabled);
        GetComponent<PlayerController>().LockCamera();
        if (!inventoryEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (mouseItemHolder.childCount > 0)
            {
                DropItem();
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        Cursor.visible = inventoryEnabled;
        chest.chestPanel.SetActive(chest);
    }
    public void AddItemFromOutsideOfInventory(int itemId, int itemAmount)
    {
        if(itemAmount == 0)
        {
            itemAmount = 1;
        }
        slotNumberDragged = 0;
        AddItemToInventoryList(itemId, itemAmount, false);
    }
    public void AddItemToInventoryList(int itemId, int itemAmount, bool isAlreadyInInventory)
    {
        //filter out all slots that have items in it
        if (!isAlreadyInInventory)
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
                if(inventoryContent[i].stackAmount == inventoryContent[i].maxStackAmount && inventoryContent[i].stackAmount > 0)
                {
                    checkTheseNumbers[i] = -1;
                }
                if (i > 5 && i < 12)
                {
                    //exclude equipment slots
                    checkTheseNumbers[i] = -1;
                }
                if(inventoryContent[i].stackAble == false)
                {
                    //exculde non-stable items
                    checkTheseNumbers[i] = -1;
                }
                if(inventoryContent[i].itemId == 0)
                {
                    //root out empty items
                    checkTheseNumbers[i] = i;
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
            bool swap = false;
            if (slotNumberDragged > -1)
            {
                if (mouseItemHolder.childCount > 0)
                {
                    if (slotNumberDragged > 5 && slotNumberDragged < 12)
                    {
                        if (mouseItemHolder.GetComponentInChildren<Item>().isEquipment)
                        {
                            //get item that is being held
                            int tempId = mouseItemHolder.GetChild(0).GetComponent<Item>().itemId;
                            int tempAmount = mouseItemHolder.GetChild(0).GetComponent<Item>().stackAmount;
                            int tempOld = mouseItemHolder.GetChild(0).GetComponent<Item>().oldSlotNumber;
                            Destroy(mouseItemHolder.GetChild(0).gameObject);

                            if (inventoryContent[slotNumberDragged].stackAmount > 0 && inventoryContent[slotNumberDragged].itemId != tempId)
                            {
                                inventoryContent[tempOld].itemId = inventoryContent[slotNumberDragged].itemId;
                                inventoryContent[tempOld].stackAmount = inventoryContent[slotNumberDragged].stackAmount;
                            }
                            else if (inventoryContent[slotNumberDragged].itemId == tempId)
                            {
                                if (tempOld != slotNumberDragged)
                                {
                                    if (tempAmount + inventoryContent[slotNumberDragged].stackAmount <= inventoryContent[slotNumberDragged].maxStackAmount)
                                    {
                                        tempAmount += inventoryContent[slotNumberDragged].stackAmount;
                                    }
                                    else
                                    {
                                        overflowAmount = tempAmount + inventoryContent[slotNumberDragged].stackAmount - inventoryContent[slotNumberDragged].maxStackAmount;
                                        tempAmount = inventoryContent[slotNumberDragged].maxStackAmount;
                                        overflowItemID = inventoryContent[slotNumberDragged].itemId;
                                    }
                                }
                                inventoryContent[tempOld].itemId = 0;
                                inventoryContent[tempOld].stackAmount = 0;
                            }
                            else
                            {
                                inventoryContent[tempOld].itemId = 0;
                                inventoryContent[tempOld].stackAmount = 0;
                            }
                            inventoryContent[slotNumberDragged].itemId = tempId;
                            inventoryContent[slotNumberDragged].stackAmount = tempAmount;
                            UpdateInventory();
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (inventoryContent[slotNumberDragged].stackAmount > 0 && inventoryContent[slotNumberDragged].itemId != draggedItem.itemId && inventoryContent[slotNumberDragged].itemId != 0)
                        {
                            if (draggedItem.oldSlotNumber >= 0)
                            {
                                swap = true;
                            }
                            else
                            {
                                AddItemToInventoryList(draggedItem.itemId, draggedItem.stackAmount, false);
                            }
                        }
                        else if (inventoryContent[slotNumberDragged].itemId == draggedItem.itemId)
                        {
                            if (draggedItem.oldSlotNumber != slotNumberDragged)
                            {
                                if (draggedItem.stackAmount + inventoryContent[slotNumberDragged].stackAmount <= inventoryContent[slotNumberDragged].maxStackAmount)
                                {
                                    draggedItem.stackAmount += inventoryContent[slotNumberDragged].stackAmount;
                                    inventoryContent[draggedItem.oldSlotNumber].itemId = 0;
                                    inventoryContent[draggedItem.oldSlotNumber].stackAmount = 0;
                                }
                                else
                                {
                                    overflowAmount = draggedItem.stackAmount + inventoryContent[slotNumberDragged].stackAmount - inventoryContent[slotNumberDragged].maxStackAmount;
                                    draggedItem.stackAmount = inventoryContent[slotNumberDragged].maxStackAmount;
                                    overflowItemID = inventoryContent[slotNumberDragged].itemId;
                                }
                            }
                        }
                        else
                        {
                            if (draggedItem.oldSlotNumber > -1)
                            {
                                inventoryContent[draggedItem.oldSlotNumber].itemId = 0;
                                inventoryContent[draggedItem.oldSlotNumber].stackAmount = 0;
                            }
                        }

                        int id = inventoryContent[slotNumberDragged].itemId;
                        int amount = inventoryContent[slotNumberDragged].stackAmount;
                        inventoryContent[slotNumberDragged] = draggedItem;
                        Destroy(mouseItemHolder.GetChild(0).gameObject);
                        if (swap == true)
                        {
                            GameObject temp = Instantiate(ItemList.itemListUi[id], mouseItemHolder);
                            temp.GetComponent<Item>().SetUp(amount, -1, this, default);
                            itemBeingDragged = true;
                        }

                        UpdateInventory();
                    }
                }
            }
        }
    }
    //adds items to inventory according to lists
    public void UpdateInventory()
    {
        for (int i = 0; i < inventoryContent.Count; i++)
        {
            //check if item exists
            if(inventoryContent[i].itemId >= ItemList.itemListUi.Count)
            {
                inventoryContent[i].itemId = 1;
            }

            foreach (Transform item in slot[i].transform)
            {
                Destroy(item.gameObject);
            }
            if (inventoryContent[i].stackAmount > 0)
            {
                //filter out emptyItems
                if(inventoryContent[i].itemId != 0)
                {
                    GameObject temp = Instantiate(ItemList.itemListUi[inventoryContent[i].itemId], slot[i].transform);
                    temp.GetComponent<Item>().SetUp(inventoryContent[i].stackAmount, i, this, default);
                    inventoryContent[i] = temp.GetComponent<Item>();
                }
            }
            if (slot[hotbarLocation].transform.childCount > 0)
            {
                GiveItemStats(slot[hotbarLocation].transform.GetChild(0).GetComponent<Item>());
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
    public void AddEmptyItem(int slotNumber)
    {
        inventoryContent[slotNumber] = emptyItem.GetComponent<Item>();
    }
    public Transform BeginDrag(Item dragThis)
    {
        draggedItem = dragThis;
        return mouseItemHolder;
    }
    public void GiveMouseLocationForInventory(int slotNumber)
    {
        slotNumberDragged = slotNumber;
    }
    public void DropItem()
    {
        if (mouseItemHolder.childCount > 0)
        {
            int tempId = draggedItem.itemId;
            int tempAmount = draggedItem.stackAmount;
            int tempOld = draggedItem.oldSlotNumber;
            Destroy(mouseItemHolder.GetChild(0).gameObject);
            if (tempOld > 0)
            {
                inventoryContent[tempOld].stackAmount = 0;
                inventoryContent[tempOld].itemId = 0;
            }

            itemBeingDragged = false;
            UpdateInventory();

            GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"Item" + tempId), transform.position + transform.forward * 2, Quaternion.identity);
            droppedItem.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position + transform.forward - transform.up, 2);
            droppedItem.GetComponent<Item>().ToInventory(false);
            droppedItem.GetComponent<Item>().stackAmount = tempAmount;
        }
    }
    void SelectItemInHotbar(int nummertje)
    {
        hotbarIndecator.transform.position = slot[nummertje].transform.position;
        CheckIfItemInHand(nummertje);
    }
    #region works!
    void CheckIfItemInHand(int slotNumber)
    {
        if (slot[slotNumber].transform.childCount > 0)
        {
            ShowItemInHand(slot[slotNumber].transform.GetChild(0).GetComponent<Item>().itemId);
            //hier check welk item in hand
            GiveItemStats(slot[slotNumber].transform.GetChild(0).GetComponent<Item>());
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
    #endregion
    void GiveItemStats(Item heldItem)
    {
        GetComponent<PlayerController>().GiveItemStats(heldItem);
    }
}
