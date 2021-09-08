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
    [SerializeField] List<Item> items;
    [SerializeField] Transform itemsParent;
    [SerializeField] ItemSlot[] itemSlots;
    [Space]
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] int allHotbarSlots = 6;

    bool inventoryEnabled;
    int hotbarLocation;
    private PhotonView pv;
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

    private void OnValidate()
    {
        if(itemsParent != null)
        {
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
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
        }
        for (; i < itemSlots.Length; i++)
        {
            itemSlots[i].item = null;
        }
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].item == null)
            {
                itemSlots[i].item = item;
                RefreshUI();
                return true;
            }
        }
        return false;
    }
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

    //old stuff
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
            //SelectItemInHotbar(hotbarLocation);
        }
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                hotbarLocation = i;
                //SelectItemInHotbar(hotbarLocation);
            }
        }
    }
    void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryEnabled = !inventoryEnabled;
            inventoryPanel.SetActive(inventoryEnabled);
            GetComponent<PlayerController>().LockCamera();
            if (!inventoryEnabled)
            {
                Cursor.lockState = CursorLockMode.Locked;
                //if (mouseItemHolder.childCount > 0)
                //{
                //    DropItem();
                //}
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            Cursor.visible = inventoryEnabled;
        }
    }
    public void DropItem(string name, int amount, Sprite image)
    {
        //master client needs to do this!
        GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", name), transform.position + transform.forward * 2, Quaternion.identity);
        droppedItem.GetComponent<Rigidbody>().AddExplosionForce(100, transform.position + transform.forward - transform.up, 2);
    }
}
