using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel EquipmentPanel;
    [SerializeField] ItemSlot draggableItem;

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
        if (itemIsBeingDragged)
        {
            if (itemslot.item == null)
            {
                itemslot.item = draggableItem.item;
                draggableItem.item = null;
                draggableItem.gameObject.GetComponent<Image>().color = disabledColor;

                //toggle
                itemIsBeingDragged = !itemIsBeingDragged;
            }
            else if (itemslot.item != null)
            {
                Item swappedItem = itemslot.item;
                itemslot.item = draggableItem.item;
                draggableItem.item = swappedItem;

                //do not toggle here! it will break
            }
            else
            {
                Debug.LogError("yeah... something broke...");
            }
        }
        else
        {
            draggableItem.item = itemslot.item;
            itemslot.item = null;
            draggableItem.gameObject.GetComponent<Image>().color = normalColor;
            draggableItem.gameObject.GetComponent<Image>().sprite = draggableItem.item.icon;

            //toggle
            itemIsBeingDragged = !itemIsBeingDragged;
        }
    }
    public void CreateItem(string name, int amount, Sprite image, EquipmentType type)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.SetUpNewItem(name, amount, image, type);

        inventory.AddItem(newItem);
    }
}
