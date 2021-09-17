using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChestInventory : MonoBehaviour
{
    [SerializeField] ItemSlot[] itemSlots;
    [SerializeField] List<Item> items;

    [SerializeField] Transform itemsParent;
    CharacterStats character;

    private void OnValidate()
    {
        if (itemsParent != null)
        {
            itemSlots = itemsParent.GetComponentsInChildren<ItemSlot>();
        }
        ChestRefreshUI();
    }

    private void Start()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].slotID = i;
            itemSlots[i].chestInv = this;
        }
    }
    public void ChestRefreshUI()
    {
        int i = 0;
        for (; i < items.Count && i < itemSlots.Length; i++)
        {
            items[i] = itemSlots[i].item;
            if (itemSlots[i].item != null)
            {
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
    }
    public void OpenChestInventory(CharacterStats charr)
    {
        itemsParent.gameObject.SetActive(true);
        character = charr;
    }
    public void OnSlotClick(ItemSlot slot)
    {
        character.MoveItem(slot);
        ChestRefreshUI();
    }
    public void CloseChestInventory()
    {
        itemsParent.gameObject.SetActive(false);
        character = null;
    }
}