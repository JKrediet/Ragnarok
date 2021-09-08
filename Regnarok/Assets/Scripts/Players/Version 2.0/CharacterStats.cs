using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel EquipmentPanel;

    private void Awake()
    {
        inventory.OnItemRightClickedEvent += EquipFromInventory;
    }

    private void EquipFromInventory(Item item)
    {
        if(item is EquipableItem)
        {
            Equip((EquipableItem)item);
        }
    }
    public void Equip(EquipableItem item)
    {
        if(inventory.RemoveItem(item))
        {
            EquipableItem previousItem;
            if(EquipmentPanel.AddItem(item, out previousItem))
            {
                if(previousItem != null)
                {
                    inventory.AddItem(previousItem);
                }
            }
            else
            {
                inventory.AddItem(item);
            }
        }
    }
    void Unequip(EquipableItem item)
    {
        if(!inventory.IsFull() && EquipmentPanel.RemoveItem(item))
        {
            inventory.AddItem(item);
        }
    }
    public void CreateItem(string name, int amount, Sprite image, EquipmentType type)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.SetUpNewItem(name, amount, image, type);

        inventory.AddItem(newItem);
    }
}
