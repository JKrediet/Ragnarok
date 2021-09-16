using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour
{
    public List<Item> items;
    [SerializeField] Transform itemsParent, hotbarParent;
    public ItemSlot[] itemSlots, hotBarSlots;
    [Space]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject hotbarIndecator;
    [SerializeField] int allHotbarSlots = 6;

    bool inventoryEnabled;
    int hotbarLocation;
    private PhotonView pv;

    CharacterStats character;
    PlayerController controller;

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
        character = GetComponent<CharacterStats>();
        controller = GetComponent<PlayerController>();
        inventoryPanel.SetActive(false);
        if (pv.IsMine)
        {
            return;
        }
        else
        {
            inventoryPanel.transform.parent.gameObject.SetActive(false);
            enabled = false;
        }
    }
    private void Start()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].slotID = i;
            itemSlots[i].inv = this;
        }
        for (int i = 0; i < hotBarSlots.Length; i++)
        {
            hotBarSlots[i].slotID = i + 25;
            hotBarSlots[i].inv = this;
        }
    }

    private void OnValidate()
    {
        if (itemsParent != null)
        {
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
            hotBarSlots = hotbarParent.GetComponentsInChildren<ItemSlot>();
        }
        RefreshUI();
    }
    private void Update() //<----------------------------- update
    {
        OpenInventory();
        ScrollHotbar();
    }
    public void RefreshUI()
    {
        int i = 0;
        for (; i < items.Count && i < itemSlots.Length; i++)
        {
            items[i] = itemSlots[i].item;
            if (itemSlots[i].item != null)
            {
                //if 0 or less remove
                if(itemSlots[i].item.itemAmount < 1)
                {
                    itemSlots[i].stackAmountText.text = "";
                    itemSlots[i].item = null;
                    continue;
                }
                if (itemSlots[i].item.itemAmount > 1)
                {
                    itemSlots[i].stackAmountText.text = itemSlots[i].item.itemAmount.ToString();
                }
                else
                {
                    itemSlots[i].stackAmountText.text = "";
                }
            }
            else
            {
                itemSlots[i].stackAmountText.text = "";
            }
        }
        i = 0;
        for (; i < hotBarSlots.Length; i++)
        {
            if (hotBarSlots[i].item != null)
            {
                if (hotBarSlots[i].item.itemAmount > 1)
                {
                    hotBarSlots[i].stackAmountText.text = hotBarSlots[i].item.itemAmount.ToString();
                }
                else
                {
                    hotBarSlots[i].stackAmountText.text = "";
                }
            }
            else
            {
                hotBarSlots[i].stackAmountText.text = "";
            }
        }
    }
    //for world items
    public void AddItem(Item item)
    {
        if (!IsFull())
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                //empty slot
                if (itemSlots[i].item == null)
                {
                    itemSlots[i].item = item;
                    //add item to list of inv
                    RefreshUI();
                    return;
                }
                //slot with item in it
                else if (itemSlots[i].item != null)
                {
                    //same item
                    if (itemSlots[i].item.itemName == item.itemName)
                    {
                        //less than max stack size
                        if (itemSlots[i].item.itemAmount + item.itemAmount <= itemSlots[i].item.maxStack)
                        {
                            itemSlots[i].item.itemAmount += item.itemAmount;
                            //add item to list of inv
                            RefreshUI();
                            return;
                        }
                        //more than stack size
                        else
                        {
                            continue;
                        }
                    }
                    //different item
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    Debug.LogError("I need more pockets!");
                    DropItem(item);
                    //add item to list of inv
                    RefreshUI();
                    return;
                }
            }
        }
        else
        {
            Debug.LogError("I need more pockets!");
            DropItem(item);
            //add item to list of inv
            RefreshUI();
            return;
        }
    }
    //delete items only from list
    public bool RemoveItem(Item item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == item)
            {
                itemSlots[i].item = null;
                RefreshUI();
                return true;
            }
        }
        return false;
    }
    public bool IsFull()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].item == null)
            {
                return false;
            }
        }
        return true;
    }

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
            SelectItemInHotBar(hotbarLocation);
        }
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                hotbarLocation = i;
                SelectItemInHotBar(hotbarLocation);
            }
        }
    }
    void SelectItemInHotBar(int _location)
    {
        hotbarIndecator.transform.position = hotBarSlots[_location].transform.position;
        if(hotBarSlots[_location].item != null)
        {
            controller.heldItem = hotBarSlots[_location].item;
            character.CalculateOffensiveStats();
        }
        else
        {
            controller.heldItem = null;
        }
    }
    void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenActualInventory();
        }
    }
    public void OpenActualInventory()
    {
        inventoryEnabled = !inventoryEnabled;
        inventoryPanel.SetActive(inventoryEnabled);
        GetComponent<PlayerController>().LockCamera();
        if(GetComponent<PlayerController>().lastChest != null)
        {
            GetComponent<PlayerController>().lastChest.CloseChestInventory();
            GetComponent<PlayerController>().lastChest = null;
        }
        if (GetComponent<PlayerController>().lastCratingStation != null)
        {
            GetComponent<PlayerController>().lastCratingStation.CloseChestInventory();
            GetComponent<PlayerController>().lastCratingStation = null;
        }
        if (!inventoryEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (character.draggableItem.item != null)
            {
                DropItem(character.draggableItem.item);
                character.draggableItem.item = null;
                character.draggableItem.gameObject.GetComponent<Image>().color = character.disabledColor;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        Cursor.visible = inventoryEnabled;
    }
    public void DropItem(Item item)
    {
        //master client needs to do this!
        GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", item.itemName), transform.position + transform.forward * 2, Quaternion.identity);
        droppedItem.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position + transform.forward - transform.up, 2);
        droppedItem.GetComponent<WorldItem>().SetUp(item.itemName, item.itemAmount, ItemList.SelectItem(item.itemName).sprite, ItemList.SelectItem(item.itemName).type, ItemList.SelectItem(item.itemName).maxStackSize);
    }
}
