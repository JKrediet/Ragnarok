using UnityEngine;

public enum EquipmentType
{
    none,
    helmet,
    chest,
    pants,
    accessory1,
    accessory2,
    accessory3,
    axe,
    pickaxe,
    weapon,
    food,
}
[CreateAssetMenu]
public class EquipableItem : Item
{
    public override void SetUpNewItem(string _itemName, int _itemAmount, Sprite _icon, EquipmentType _type, int _maxStack)
    {
        base.SetUpNewItem(_itemName, _itemAmount, _icon, _type, _maxStack);
        equipment = _type;
    }
}
