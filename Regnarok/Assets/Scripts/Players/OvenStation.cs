using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OvenStation : MonoBehaviour
{
    [SerializeField] GameObject uipanel;
    CharacterStats character;
    Inventory inventory;

    [SerializeField] ItemSlot smeltSlot, fuelSlot, finishedSlot;

    float fuelTime, smeltTime, smeltProgress;
    bool canSmelt;

    [SerializeField] Slider fuelSlider;

    public void OpenCratingInventory(CharacterStats charr, Inventory inv)
    {
        uipanel.gameObject.SetActive(true);
        character = charr;
        inventory = inv;
    }
    public void CloseChestInventory()
    {
        uipanel.gameObject.SetActive(false);
        character = null;
    }
    
    public void CheckSlots()
    {
        //check if can smelt
        if(smeltSlot.item == null)
        {
            canSmelt = false;
            return;
        }
        if (finishedSlot.item != null)
        {
            if (finishedSlot.item.itemName != smeltSlot.item.itemName)
            {
                canSmelt = false;
                return;
            }
        }
        //check for fuel
        if (fuelTime == 0)
        {
            if (!NeedFuel())
            {
                canSmelt = false;
                return; //stops here: no more fuel
            }
        }
        BeginSmelt();
    }
    void BeginSmelt()
    {
        smeltSlot.item.itemAmount--;
        if (smeltSlot.item.itemAmount == 0)
        {
            smeltSlot.item = null;
        }
        UpdateSlots();
        canSmelt = true;
    }
    void FinishSmelt()
    {
        CreateItem(smeltSlot.item.itemName, 1, ItemList.SelectItem(smeltSlot.item.itemName).sprite, EquipmentType.none, ItemList.SelectItem(smeltSlot.item.itemName).maxStackSize);
        CheckSlots();
    }
    public void CreateItem(string name, int amount, Sprite image, EquipmentType type, int maxStack)
    {
        Item newItem = ScriptableObject.CreateInstance<Item>();
        newItem.SetUpNewItem(name, amount, image, type, maxStack, ItemList.SelectItem(name).foodHealAmount, ItemList.SelectItem(name).smeltTime);

        finishedSlot.item = newItem;
        UpdateSlots();
    }
    void UpdateSlots()
    {
        if(smeltSlot.item)
        {
            smeltSlot.stackAmountText.text = smeltSlot.item.itemAmount.ToString();
        }
        else
        {
            smeltSlot.stackAmountText.text = "";
        }
        if (fuelSlot.item)
        {
            fuelSlot.stackAmountText.text = fuelSlot.item.itemAmount.ToString();
        }
        else
        {
            fuelSlot.stackAmountText.text = "";
        }
        if (finishedSlot.item)
        {
            finishedSlot.stackAmountText.text = finishedSlot.item.itemAmount.ToString();
        }
        else
        {
            finishedSlot.stackAmountText.text = "";
        }
    }
    private void Update()
    {
        if(canSmelt)
        {
            if(fuelTime > 0)
            {
                if(Time.time > smeltTime)
                {
                    smeltTime = Time.time + 0.5f;
                    fuelTime -= 0.5f;
                    smeltProgress += 0.5f;
                    fuelSlider.value = fuelTime;
                    if (smeltProgress == 10)
                    {
                        smeltProgress = 0;
                        FinishSmelt();
                    }
                }
            }
            else
            {
                if(!NeedFuel())
                {
                    smeltProgress = 0;
                    canSmelt = false;
                    return; //stops here: no more fuel
                }
            }
        }
    }


    bool NeedFuel()
    {
        if (fuelSlot.item == null)
        {
            return false;
        }
        fuelSlot.item.itemAmount--;
        fuelTime = fuelSlot.item.smeltTime;
        fuelSlider.maxValue = fuelTime;
        fuelSlider.value = fuelTime;
        if (fuelSlot.item.itemAmount == 0)
        {
            fuelSlot.item = null;
        }
        UpdateSlots();
        return true;
    }

    public void MoveItem(ItemSlot slot)
    {
        character.MoveItem(slot);
    }
}