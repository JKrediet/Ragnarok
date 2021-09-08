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
}
[CreateAssetMenu]
public class EquipableItem : Item
{
    [Space]
    public int damageBonus;
    public int armorBonus;
    public int speedBonus;
    public int healthBonus;
    [Space]
    public int damagePrecentBonus;
    public int armorPrecentBonus;
    public int speedPrecentBonus;
    public int healthPrecentBonus;
    [Space]
    public EquipmentType equipment;

    public override void SetUpNewItem(string _itemName, int _itemAmount, Sprite _icon, EquipmentType _type, int _maxStack)
    {
        base.SetUpNewItem(_itemName, _itemAmount, _icon, _type, _maxStack);
        equipment = _type;
    }
}
