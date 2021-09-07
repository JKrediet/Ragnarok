using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [HideInInspector] public PhotonView pv;
    bool inventoryEnabled;
    public GameObject inventory;
    public GameObject background;

    private int allSlots, allEquipmentSlots, allHotbarSlots;
    public GameObject[] slot; 

    public GameObject slotHolder, equipmentSlotHolder, hotbarSlotsHolder;
    public GameObject emptyItem;

    public Transform mouseItemHolder, handHolder;
    public bool itemBeingDragged;
    public int itemReleaseRange;
    private int slotNumberDragged;

    public Item draggedItem;

    //hotbar indecator
    public GameObject hotbarIndecator;
    int hotbarLocation;

    public List<int> checkTheseNumbers;
    public List<GameObject> inventoryContent;

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
        background.SetActive(false);
        if (pv.IsMine)
        {
            return;
        }
        else
        {
            inventory.transform.parent.gameObject.SetActive(false);
            background.SetActive(false);
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
            //GameObject yes = Instantiate(ItemList.itemListUi[0], slot[i].transform);
            //yes.GetComponent<Item>().SetUp(1, i, this, default);
            inventoryContent.Add(default);
            //inventoryContent[i] = yes;
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
            background.SetActive(inventoryEnabled);
            GetComponent<PlayerController>().LockCamera();
            if(lastChest)
            {
                lastChest.chestPanel.SetActive(inventoryEnabled);
            }
            lastChest = null;
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
        }
    }
    public void ToggleInventoryFromChest(ChestInventory chest)
    {
        lastChest = chest;
        inventoryEnabled = !inventoryEnabled;
        inventory.SetActive(inventoryEnabled);
        background.SetActive(inventoryEnabled);
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
        chest.chestPanel.SetActive(inventoryEnabled);
    }
    public void AddItemFromOutsideOfInventory(int itemId, int itemAmount)
    {
        if(itemAmount == 0)
        {
            itemAmount = 1;
        }
        slotNumberDragged = 0;
        AddItemToInventoryList(itemId, itemAmount, false, -1);
    }
    public void AddItemToInventoryList(int itemId, int itemAmount, bool isAlreadyInInventory, int oldSlot)
    {
        if (!isAlreadyInInventory)
        {
            int useThisSlot = -1;
            for (int i = 0; i < inventoryContent.Count; i++)
            {
                if(useThisSlot > -1)
                {
                    continue;
                }
                if(inventoryContent[i] != null)
                {
                    Item item = inventoryContent[i].GetComponent<Item>();
                    if (item.itemId == itemId)
                    {
                        if(item.stackAmount + itemAmount <= item.maxStackAmount)
                        {
                            int useThis = item.stackAmount += itemAmount;
                            item.SetUp(useThis, i, this, default);
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
            if(useThisSlot > -1)
            {
                GameObject temp = Instantiate(ItemList.itemListUi[itemId], slot[useThisSlot].transform);
                temp.GetComponent<Item>().SetUp(itemAmount, useThisSlot, this, default);
                inventoryContent[useThisSlot] = temp;
                return;
            }
        }
        else
        {
            if (draggedItem != null)
            {
                if (inventoryContent[slotNumberDragged] == null)
                {
                    inventoryContent[slotNumberDragged] = draggedItem.gameObject;
                    draggedItem.transform.SetParent(slot[slotNumberDragged].transform);
                    draggedItem.transform.position = slot[slotNumberDragged].transform.position;
                    draggedItem.GetComponent<Item>().SetUp(draggedItem.stackAmount, slotNumberDragged, this, default);
                    itemBeingDragged = false;
                    draggedItem = null;
                }
                else if(inventoryContent[slotNumberDragged] != null)
                {
                    Item item = inventoryContent[slotNumberDragged].GetComponent<Item>();
                    if (item.itemId == draggedItem.itemId)
                    {
                        if (item.stackAmount + draggedItem.stackAmount <= item.maxStackAmount)
                        {
                            int useThis = item.stackAmount += draggedItem.stackAmount;
                            item.SetUp(useThis, slotNumberDragged, this, default);
                            Destroy(draggedItem.gameObject);
                            itemBeingDragged = false;
                        }
                        else
                        {
                            int useThis = item.stackAmount + draggedItem.stackAmount - item.maxStackAmount;
                            item.SetUp(item.maxStackAmount, slotNumberDragged, this, default);
                            draggedItem.SetUp(useThis, -1, this, default);
                            itemBeingDragged = true;
                            return;
                        }
                    }
                    else
                    {
                        //store slotitem
                        GameObject SwappedItem = inventoryContent[slotNumberDragged];
                        GameObject oldDraggedItem = draggedItem.gameObject;

                        draggedItem.transform.SetParent(slot[slotNumberDragged].transform);
                        draggedItem.transform.position = slot[slotNumberDragged].transform.position;
                        draggedItem.GetComponent<Item>().SetUp(draggedItem.stackAmount, slotNumberDragged, this, default);

                        itemBeingDragged = true;
                        draggedItem = null;
                        SwappedItem.GetComponent<Item>().SwapItem();
                        inventoryContent[slotNumberDragged] = oldDraggedItem;
                    }
                }
            }
        }
    }
    public void AddEmptyItem(int slotNumber)
    {
        if(slotNumber > -1)
        {
            inventoryContent[slotNumber] = null;
        }
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
                //inventoryContent[tempOld].stackAmount = 0;
                //inventoryContent[tempOld].itemId = 0;
            }

            itemBeingDragged = false;
            //UpdateInventory();

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
