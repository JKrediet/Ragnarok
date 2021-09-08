using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [Range(1, 100)]
    public int itemAmount, maxStack;

    public virtual void SetUpNewItem(string _itemName, int _itemAmount, Sprite _icon, EquipmentType _type, int _maxStack)
    {
        itemName = _itemName;
        itemAmount = _itemAmount;
        icon = _icon;
        maxStack = _maxStack;
    }
}
