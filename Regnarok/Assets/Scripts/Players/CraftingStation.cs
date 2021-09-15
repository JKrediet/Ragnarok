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
    [SerializeField] List<Item> itemsInInventory;
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
    void GetSlots()
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
        GetSlots();
        for (int i = 0; i < inventory.itemSlots.Length; i++)
        {
            if(inventory.itemSlots[i].item != null)
            {
                itemsInInventory.Add(inventory.itemSlots[i].item);
            }
        }
        for (int i = 0; i < craft.Count; i++)
        {
            List<int> checkList = new List<int>();
            for (int z = 0; z < itemsInInventory.Count; z++)
            {
                checkList.Clear();
                for (int u = 0; u < craft[i].itemsNeeded.Count; u++)
                {
                    checkList.Add(u);
                    if(itemsInInventory[z].itemName != craft[i].itemsNeeded[u].itemNeeded)
                    {
                        continue;
                    }
                    else if(itemsInInventory[z].itemAmount >= craft[i].itemsNeeded[u].amountNeeded)
                    {
                        checkList[u] = u;
                    }
                    else
                    {
                        checkList[u] = -1;
                    }
                    if (checkList[u] != u)
                    {
                        continue;
                    }
                }
            }
            if(checkList[i] == i)
            {
                craftThis = craft[i];

                slots[i].recipe = CreateRecipe();
            }
            else
            {
                slots[i].recipe = null;
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
        //remove needed items
        if(selectedCraft.craftResult.Length > 0)
        {
            for (int i = 0; i < inventory.itemSlots.Length; i++)
            {
                for (int y = 0; y < selectedCraft.craftResult.Length; i++)
                {
                    string neededNameItem = selectedCraft.itemsNeeded[y].itemNeeded;
                    int neededAmountItem = selectedCraft.itemsNeeded[y].amountNeeded;

                    if (inventory.itemSlots[i].item.itemName == neededNameItem && inventory.itemSlots[i].item.itemAmount >= neededAmountItem)
                    {
                        inventory.itemSlots[i].item.itemAmount -= neededAmountItem;
                    }
                    print("not enough items!: " + neededNameItem + " " + neededAmountItem);
                }
            }
            FinishCrafting();
        }
    }
    void FinishCrafting()
    {
        character.CreateItem(ItemList.SelectItem(selectedCraft.craftResult).name, 1, ItemList.SelectItem(selectedCraft.craftResult).sprite, ItemList.SelectItem(selectedCraft.craftResult).type, ItemList.SelectItem(selectedCraft.craftResult).maxStackSize);
        inventory.RefreshUI();
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
