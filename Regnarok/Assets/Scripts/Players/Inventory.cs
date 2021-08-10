using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    bool inventoryEnabled;
    public GameObject inventory;

    private int allSlots, allEquipmentSlots, allHotbarSlots;
    public GameObject[] slot; 

    public GameObject slotHolder, equipmentSlotHolder, hotbarSlotsHolder;
    public GameObject testItem, testItemWorld;

    public Transform mouseItemHolder, handHolder;
    public bool itemBeingDragged;
    public int itemReleaseRange;
    private int itemLocationInUi;

    private int draggedStackAmount;
    private int slotNumberDragged;

    //hotbar indecator
    public GameObject hotbarIndecator;
    int hotbarLocation;

    //test
    List<int> checkTheseNumbers;
    public List<Item> inventoryContent;

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
        slot = new GameObject[allSlots + allEquipmentSlots + allHotbarSlots];
        checkTheseNumbers = new List<int>();

        inventory.SetActive(false);

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
            inventoryContent.Add(Instantiate(testItem.GetComponent<Item>(), slot[i].transform));
        }

        //test
        AddItemFromOutsideOfInventory(2, 5);
    }

    void Update()
    {
        OpenInventory();
        if (inventoryEnabled)
        {
            mouseItemHolder.position = Input.mousePosition;
        }
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
        //filter out all slots that have items in it
        if (!isAlreadyInInventory)
        {
            checkTheseNumbers.Clear();
            int temp = -1;
            for (int i = 0; i < inventoryContent.Count; i++)
            {
                checkTheseNumbers.Add(i);
                if(inventoryContent[i].itemId == itemId)
                {
                    temp = i;
                    print(temp);
                }
                if (inventoryContent[i].stackAmount > 0 && inventoryContent[i].itemId != itemId)
                {
                    //-1 == this slot has an item
                    checkTheseNumbers[i] = -1;
                }
                if (i > 5 && i < 12)
                {
                    //exclude equipment slots
                    checkTheseNumbers[i] = -1;
                }
            }
            for (int i = 0; i < inventoryContent.Count; i++)
            {
                if(temp > 0)
                {
                    itemAmount += inventoryContent[temp].stackAmount;
                    inventoryContent[temp].itemId = itemId;
                    inventoryContent[temp].stackAmount = itemAmount;
                    UpdateInventory();
                    return;
                }
                else
                {
                    if (checkTheseNumbers[i] == i)
                    {
                        if (inventoryContent[i].itemId == itemId)
                        {
                            itemAmount += inventoryContent[i].stackAmount;
                        }
                        inventoryContent[i].itemId = itemId;
                        inventoryContent[i].stackAmount = itemAmount;
                        UpdateInventory();
                        return;
                    }
                }
            }
        }
        else
        {
            if (slotNumberDragged > -1)
            {
                if (mouseItemHolder.childCount > 0)
                {
                    int tempId = mouseItemHolder.GetChild(0).GetComponent<Item>().itemId;
                    int tempAmount = mouseItemHolder.GetChild(0).GetComponent<Item>().stackAmount;
                    int tempOld = mouseItemHolder.GetChild(0).GetComponent<Item>().oldSlotNumber;
                    Destroy(mouseItemHolder.GetChild(0).gameObject);

                    if (inventoryContent[slotNumberDragged].stackAmount > 0 && inventoryContent[slotNumberDragged].itemId != tempId)
                    {
                        inventoryContent[tempOld].itemId = inventoryContent[slotNumberDragged].itemId;
                        inventoryContent[tempOld].stackAmount = inventoryContent[slotNumberDragged].stackAmount;
                    }
                    else if(inventoryContent[slotNumberDragged].itemId == tempId)
                    {
                        tempAmount += inventoryContent[slotNumberDragged].stackAmount;
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
            }
        }
    }
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
            Instantiate(ItemList.itemListUi[inventoryContent[i].itemId], slot[i].transform).GetComponent<Item>().SetUp(inventoryContent[i].stackAmount, i, this);
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
    public Transform BeginDrag()
    {
        return mouseItemHolder;
    }
    public void GiveMouseLocationForInventory(int slotNumber)
    {
        slotNumberDragged = slotNumber;
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
    #endregion
    void GiveItemStats(int nummertje)
    {
        //GetComponent<PlayerController>().GiveItemStats(hotbarSlots[nummertje].GetComponent<Slot>().item.GetComponent<Item>());
    }
}
