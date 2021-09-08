using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlots : ItemSlot
{
    public EquipmentType EquipmentType;

    protected override void OnValidate()
    {
        base.OnValidate();
        gameObject.name = EquipmentType.ToString() + " Slot";
    }
    public override bool CanRecieveItem(Item item)
    {
        if(item == null)
        {
            return true;
        }
        EquipableItem equipableItem = item as EquipableItem;
        return equipableItem != null && equipableItem.equipment == EquipmentType;
    }
}
