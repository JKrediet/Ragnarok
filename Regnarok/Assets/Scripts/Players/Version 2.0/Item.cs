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

    [Space]
    [SerializeField] public int foodLifeRestore;
    [Space]
    [HideInInspector] public int damageBonus;
    [HideInInspector] public int armorBonus;
    [HideInInspector] public int attackSpeedBonus;
    [HideInInspector] public int critChanceBonus;
    [HideInInspector] public int healthBonus;
    [Space]
    [HideInInspector] public int damagePrecentBonus;
    [HideInInspector] public int armorPrecentBonus;
    [HideInInspector] public int attackSpeedPrecentBonus;
    [HideInInspector] public int critChancePrecentBonus;
    [HideInInspector] public int healthPrecentBonus;
    [Space]
    public EquipmentType equipment;

    public virtual void SetUpNewItem(string _itemName, int _itemAmount, Sprite _icon, EquipmentType _type, int _maxStack, int _heal)
    {
        itemName = _itemName;
        itemAmount = _itemAmount;
        icon = _icon;
        maxStack = _maxStack;
        equipment = _type;
        foodLifeRestore = _heal;
    }
}
