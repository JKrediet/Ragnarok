using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel EquipmentPanel;
    public ItemSlot draggableItem;
    public Items[] itemList;//list of items
    bool itemIsBeingDragged;
    PlayerController playercontroller;

    //color
    [HideInInspector] public Color normalColor = Color.white;
    [HideInInspector] public Color disabledColor = new Color(1, 1, 1, 0);

    [Space]
    float endDamage, endAttackSpeed, endCritChance, endArmor, endHealth;
    public float BaseDamage, baseAttackSpeed, baseCritChance, baseArmor, baseHealth;
    float addedDamage, addedAttackSpeed, addedCritChance, addedArmor, addedHealth;
    float precentAddedDamage, precentAddedAttackSpeed, precentAddedCritChance, precentAddedArmor, precentAddedHealth;


    private void Awake()
    {
        playercontroller = GetComponent<PlayerController>();
    }
    private void Update()
    {
        if(itemIsBeingDragged)
        {
            draggableItem.transform.position = Input.mousePosition;
        }
    }
    public void CalculateOffensiveStats(Item item)
    {
        EquipableItem equipableItem = item as EquipableItem;

        endDamage = (BaseDamage + addedDamage + equipableItem.damageBonus) * (precentAddedDamage + equipableItem.damagePrecentBonus / 100 + 1);
        float tempAttackSpeed = (baseAttackSpeed + addedAttackSpeed + equipableItem.attackSpeedBonus) * (precentAddedAttackSpeed + equipableItem.attackSpeedPrecentBonus / 100 + 1);
        endAttackSpeed = tempAttackSpeed / (tempAttackSpeed * tempAttackSpeed);
        endCritChance = (baseCritChance + addedCritChance + equipableItem.critChanceBonus) * (precentAddedCritChance + equipableItem.critChancePrecentBonus / 100 + 1);

        //give stats/ offensive
        playercontroller.RecieveStats(endDamage, endAttackSpeed, endCritChance);
    }
    public void CalculateDefensiveStats()
    {
        //defensive
        endArmor = (baseArmor + addedArmor) * (precentAddedArmor / 100 + 1);
        endHealth = (baseHealth + addedHealth) * (precentAddedHealth / 100 + 1);

        //return to healthScript
    }
    public void MoveItem(ItemSlot itemslot)
    {
        if(Input.GetKey(KeyCode.Q))
        {
            inventory.DropItem(itemslot.item);
            itemslot.item = null;
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            if (!itemIsBeingDragged)
            {
                if(itemslot.item.itemAmount == 1)
                {
                    draggableItem.item = itemslot.item;
                    draggableItem.item.itemAmount = 1;
                    draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                    itemslot.item = null;
                }
                else if(itemslot.item.itemAmount > 1)
                {
                    itemslot.item.itemAmount--;
                    draggableItem.item = itemslot.item;
                    draggableItem.item.itemAmount = 1;
                    draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                }
                itemIsBeingDragged = true;
                //add item to list of inv
                if (itemslot.chestInv != null)
                {
                    //not yet inplemented
                }
                else if (itemslot.inv != null)
                {
                    itemslot.inv.RefreshUI();
                }
            }
        }
        else if(Input.GetKey(KeyCode.LeftShift))
        {
            if (!itemIsBeingDragged)
            {
                if (itemslot.item.itemAmount > 1)
                {
                    float roundedDown = 0;
                    float roundedUp = 0;
                    int half = 0;
                    if (itemslot.item.itemAmount % 2 == 0)
                    {
                        half = itemslot.item.itemAmount / 2;
                    }
                    else if (itemslot.item.itemAmount % 2 == 1)
                    {
                        roundedDown = (float)itemslot.item.itemAmount / 2 - 0.5f;
                        roundedUp = (float)itemslot.item.itemAmount / 2 + 0.5f;
                    }
                    if (half > 0)
                    {
                        itemslot.item.itemAmount = half;
                        draggableItem.item = itemslot.item;
                        draggableItem.item.itemAmount = half;
                        draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                    }
                    else
                    {
                        itemslot.item.itemAmount = (int)roundedUp;
                        draggableItem.item = itemslot.item;
                        draggableItem.item.itemAmount = (int)roundedDown;
                        draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                    }
                    itemIsBeingDragged = true;
                }
            }
        }
        else
        {
            if (itemIsBeingDragged)
            {
                //empty slot
                if (itemslot.item == null)
                {
                    itemslot.item = draggableItem.item;
                    draggableItem.item = null;
                    draggableItem.gameObject.GetComponent<Image>().color = disabledColor;

                    //toggle
                    itemIsBeingDragged = !itemIsBeingDragged;
                }
                //slot with item in it
                else if (itemslot.item != null)
                {
                    //same item
                    if (itemslot.item.itemName == draggableItem.item.itemName)
                    {
                        //less than max stack size
                        if (itemslot.item.itemAmount + draggableItem.item.itemAmount <= itemslot.item.maxStack)
                        {
                            itemslot.item.itemAmount += draggableItem.item.itemAmount;
                            draggableItem.item = null;
                            draggableItem.gameObject.GetComponent<Image>().color = disabledColor;
                            itemIsBeingDragged = !itemIsBeingDragged;
                        }
                        //more than stack size
                        else
                        {
                            draggableItem.item.itemAmount = itemslot.item.itemAmount + draggableItem.item.itemAmount - itemslot.item.maxStack;
                            itemslot.item.itemAmount = itemslot.item.maxStack;
                        }
                    }
                    //different item
                    else
                    {
                        Item swappedItem = itemslot.item;
                        itemslot.item = draggableItem.item;
                        draggableItem.item = swappedItem;
                        draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                        draggableItem.gameObject.GetComponent<Image>().sprite = draggableItem.item.icon;
                        //do not toggle here! it will break
                    }
                }
                else
                {
                    Debug.LogError("I need more pockets!");
                }
            }
            else
            {
                //no item in slot
                if (itemslot.item != null)
                {
                    draggableItem.item = itemslot.item;
                    itemslot.item = null;
                    draggableItem.gameObject.GetComponent<Image>().color = normalColor;
                    draggableItem.gameObject.GetComponent<Image>().sprite = draggableItem.item.icon;

                    //toggle
                    itemIsBeingDragged = !itemIsBeingDragged;
                }
            }
        }
        //add item to list of inv
        if (itemslot.chestInv != null)
        {
            //not yet inplemented
        }
        else if (itemslot.inv != null)
        {
            itemslot.inv.RefreshUI();
        }
    }
    public void CreateItem(string name, int amount, Sprite image, EquipmentType type, int maxStack)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.SetUpNewItem(name, amount, image, type, maxStack);

        inventory.AddItem(newItem);
    }
    [System.Serializable]
    public struct Items
    {
        public int id;
        public int amount;
        
    }
}
