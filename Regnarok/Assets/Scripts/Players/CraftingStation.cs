using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct Result
{
    public string craftResult;
    public int craftAmount;
    public List<ItemAmount> itemsNeeded;
}
[Serializable]
public struct ItemAmount
{
    public string itemNeeded;
    [Range(1, 999)]
    public int amountNeeded;
}

[CreateAssetMenu]
public class CraftingStation : MonoBehaviour
{
    public List<Result> craft;
    [Space]
    [SerializeField] GameObject uipanel, contentHolder;
    CharacterStats character;
    Inventory inventory;
    List<Item> itemsInInventory;
    Result craftThis;
    Result selectedCraft;

    public List<RecipeHolder> slots;

    private void OnValidate()
    {
        slots.Clear();
        if (contentHolder != null)
        {
            foreach (Transform item in contentHolder.transform)
            {
                slots.Add(item.GetComponent<RecipeHolder>());
            }
        }
        else
        {
            slots.Clear();
        }
        foreach (RecipeHolder holder in slots)
        {
            if (holder.recipe == null)
            {
                holder.gameObject.SetActive(false);
            }
            else
            {
                holder.gameObject.SetActive(true);
                holder.UpdateUi();
            }
        }
    }

    public CraftingRecipe CreateRecipe()
    {
        CraftingRecipe newRecipe = ScriptableObject.CreateInstance<CraftingRecipe>();
        newRecipe.SetUp(craftThis);
        return newRecipe;
    }

    private void Start()
    {
        itemsInInventory = new List<Item>();
    }

    public void SelectRecipe(RecipeHolder i)
    {
        selectedCraft = i.recipe.craft;
    }
    public void CanCraft()
    {
        //add into list for further use
        itemsInInventory.Clear();
        slots.Clear();
        for (int i = 0; i < inventory.itemSlots.Length; i++)
        {
            if(inventory.itemSlots[i].item != null)
            {
                itemsInInventory.Add(inventory.itemSlots[i].item);
            }
        }
        for (int i = 0; i < craft.Count; i++)
        {
            for (int u = 0; u < craft[i].itemsNeeded.Count; u++)
            {
                if(itemsInInventory[i].itemName == craft[i].itemsNeeded[u].itemNeeded && itemsInInventory[i].itemAmount >= craft[i].itemsNeeded[u].amountNeeded)
                {

                    craftThis = craft[i];

                    slots[i].recipe = CreateRecipe();
                }
                else
                {
                    slots[i].recipe = null;
                }
            }
        }
        foreach (RecipeHolder holder in slots)
        {
            if(holder.recipe == null)
            {
                holder.gameObject.SetActive(false);
            }
            else
            {
                holder.gameObject.SetActive(true);
                holder.UpdateUi();
            }
        }
    }
    public void Craft()
    {
        if(selectedCraft.craftResult.Length > 0)
        {
            character.CreateItem(ItemList.SelectItem(selectedCraft.craftResult).name, 1, ItemList.SelectItem(selectedCraft.craftResult).sprite, ItemList.SelectItem(selectedCraft.craftResult).type, ItemList.SelectItem(selectedCraft.craftResult).maxStackSize);
        }
    }

    public void OpenCratingInventory(CharacterStats charr, Inventory inv)
    {
        uipanel.gameObject.SetActive(true);
        character = charr;
        inventory = inv;

        CanCraft();
    }
    public void CloseChestInventory()
    {
        uipanel.gameObject.SetActive(false);
        character = null;
    }
}
