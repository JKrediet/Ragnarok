using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel EquipmentPanel;
    public ItemSlot draggableItem;

    bool itemIsBeingDragged;

    //color
    private Color normalColor = Color.white;
    private Color disabledColor = new Color(1, 1, 1, 0);

    private void Update()
    {
        if(itemIsBeingDragged)
        {
            draggableItem.transform.position = Input.mousePosition;
        }
    }
    public void MoveItem(ItemSlot itemslot)
    {
        if(Input.GetKey(KeyCode.Q))
        {
            inventory.DropItem(itemslot.item);
            itemslot.item = null;
        }
        else
        {
            if (itemIsBeingDragged)
            {
                //empty slot
                if (itemslot.item == null)
                {
                    itemslot.item = draggableItem.item;
                    draggableItem.item = null;
                    draggableItem.gameObject.GetComponent<Image>().color = disabledColor;

                    //toggle
                    itemIsBeingDragged = !itemIsBeingDragged;
                }
                //slot with item in it
                else if (itemslot.item != null)
                {
                    //same item
                    if (itemslot.item.itemName == draggableItem.item.itemName)
                    {
                        //less than max stack size
                        if (itemslot.item.itemAmount + draggableItem.item.itemAmount <= itemslot.item.maxStack)
                        {
                            itemslot.item.itemAmount += draggableItem.item.itemAmount;
                            draggableItem.item = null;
                            draggableItem.gameObject.GetComponent<Image>().color = disabledColor;
                            itemIsBeingDragged = !itemIsBeingDragged;
                        }
                        //more than stack size
                        else
                        {
                            draggableItem.item.itemAmount = itemslot.item.itemAmount + draggableItem.item.itemAmount - itemslot.item.maxStack;
                            itemslot.item.itemAmount = itemslot.item.maxStack;
                        }
                    }
                    //different item
                    else
                    {
                        Item swappedItem = itemslot.item;
                        itemslot.item = draggableItem.item;
                        draggableItem.item = swappedItem;
                        draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                        draggableItem.gameObject.GetComponent<Image>().sprite = draggableItem.item.icon;
                        //do not toggle here! it will break
                    }
                }
                else
                {
                    Debug.LogError("I need more pockets!");
                }
            }
            else
            {
                //no item in slot
                if (itemslot.item != null)
                {
                    draggableItem.item = itemslot.item;
                    itemslot.item = null;
                    draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                    draggableItem.gameObject.GetComponent<Image>().sprite = draggableItem.item.icon;

                    //toggle
                    itemIsBeingDragged = !itemIsBeingDragged;
                }
            }
        }
        //add item to list of inv
        if (itemslot.chestInv != null)
        {
            //not yet inplemented
        }
        else if (itemslot.inv != null)
        {
            itemslot.inv.RefreshUI();
        }
    }
    public void CreateItem(string name, int amount, Sprite image, EquipmentType type, int maxStack)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.SetUpNewItem(name, amount, image, type, maxStack);

        inventory.AddItem(newItem);
    }
}
