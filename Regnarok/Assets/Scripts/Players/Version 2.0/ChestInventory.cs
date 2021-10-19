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
                    SincSlotWithMaster(i, itemSlots[i].item.itemName, itemSlots[i].item.itemAmount);
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
    void SincSlotWithMaster(int slotId, string itemId, int itemAmount)
    {
        FindObjectOfType<GameManager>().SincChestOnMaster(slotId, itemId, itemAmount, GetComponent<PlaceAbleItemId>().placeabelItemID);
    }
    public void SincSlots(int slotId, string itemId, int itemAmount)
    {
        print("created item for chest");
        itemSlots[slotId].item = character.CreateItemForChest(itemId, itemAmount, ItemList.SelectItem(itemId).sprite, ItemList.SelectItem(itemId).type, ItemList.SelectItem(itemId).maxStackSize);
        itemSlots[slotId].stackAmountText.text = itemSlots[slotId].item.itemAmount.ToString();
    }
}
