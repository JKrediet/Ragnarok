using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ItemAmount
{
    public Item item;
    [Range(1, 999)]
    public int Amount;
}

[CreateAssetMenu]
public class CraftingRecipe : ScriptableObject
{
    public List<ItemAmount> materials;
    public List<ItemAmount> result;

    public bool CanCraft(Inventory inventory)
    {
        return false;
    }
    public void Craft(Inventory inventory)
    {

    }
}
