using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemType; //0=default / 1= equipment
    public int ItemId;

    private bool isInInventory, mayBePickedUp;

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
    private void OnTriggerEnter(Collider other)
    {
        if (mayBePickedUp)
        {
            if (!isInInventory)
            {
                if (other.CompareTag("Player"))
                {
                    other.GetComponent<Inventory>().AddItemFromOutsideOfInventory(gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
}
