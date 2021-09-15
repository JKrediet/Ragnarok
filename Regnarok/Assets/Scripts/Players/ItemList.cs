using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public struct ItemContent
{
    public string name;
    public GameObject worldItem;
    [Space]
    public Sprite sprite;
    public EquipmentType type;
    public int maxStackSize;
}

public class ItemList : MonoBehaviour
{
    public ItemContent[] itemContents;
    public static List<ItemContent> staticItemContents;

    private void Awake()
    {
        staticItemContents = new List<ItemContent>(itemContents);
    }

    public static ItemContent SelectItem(string itemName)
    {
        for (int i = 0; i < staticItemContents.Count; i++)
        {
            if(staticItemContents[i].name == itemName)
            {
                return staticItemContents[i];
            }
        }
        Debug.LogError("Specified item not found!");
        return default;
    }
}
