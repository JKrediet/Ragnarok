using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Result
{
    public string craftResult;
    public List<ItemAmount> itemsNeeded;
}
[Serializable]
public struct ItemAmount
{
    public string itemNeeded;
    [Range(1, 999)]
    public int Amount;
}

[CreateAssetMenu]
public class CraftingRecipe : MonoBehaviour
{
    public List<Result> craft;
    [Space]
    [SerializeField] GameObject uipanel;
    CharacterStats character;
    Inventory inventory;
    List<string> itemsInInventory;
    List<Result> craftAble;
    Result craftThis;

    public void SelectRecipe(int i)
    {
        craftThis = craft[i];
    }
    public void CanCraft()
    {
        itemsInInventory.Clear();
        for (int i = 0; i < inventory.itemSlots.Length; i++)
        {
            if(inventory.itemSlots[i].item != null)
            {
                itemsInInventory.Add(inventory.itemSlots[i].item.itemName);
            }
        }
        for (int i = 0; i < craft.Count; i++)
        {
            for (int u = 0; u < craft[i].itemsNeeded.Count; u++)
            {
                if(itemsInInventory.Contains(craft[i].itemsNeeded[u].itemNeeded))
                {

                }
            }
        }
    }
    public void Craft()
    {

    }

    public void OpenChestInventory(CharacterStats charr, Inventory inv)
    {
        uipanel.gameObject.SetActive(true);
        character = charr;
        inventory = inv;
    }
    public void CloseChestInventory()
    {
        uipanel.gameObject.SetActive(false);
        character = null;
    }
}
