using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    public int itemType; //0=default / 1= axe
    public int itemId;
    public int itemDamage;
    public bool isEquipment, stackAble;
    public int stackAmount;

    private bool isInInventory, mayBePickedUp;

    public TextMeshProUGUI stackAmountText;

    private void Start()
    {
        Invoke("Cooldown", 0.5f);
    }
    public void Cooldown()
    {
        mayBePickedUp = true;
    }

    public void ToInventory(bool value)
    {
        isInInventory = value;
    }
    protected void OnTriggerEnter(Collider other)
    {
        if (mayBePickedUp)
        {
            if (!isInInventory)
            {
                if (other.CompareTag("Player"))
                {
                    other.GetComponent<Inventory>().AddItemFromOutsideOfInventory(itemId, stackAmount);
                    Destroy(gameObject);
                }
            }
        }
    }
    public void SetUp(int numberOfItems)
    {
        stackAmount = numberOfItems;
        if (stackAmountText != null)
        {
            stackAmountText.gameObject.SetActive(true);
            stackAmountText.text = stackAmount.ToString();
        }
    }
}
