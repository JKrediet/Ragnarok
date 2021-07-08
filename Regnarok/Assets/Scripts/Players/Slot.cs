using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject item;
    public int slotType = 0;

    [SerializeField] Inventory inv;
    
    public bool CheckForItem()
    {
        if(item)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void RecieveItem(GameObject newItem)
    {
        item = newItem;
        item.transform.SetParent(transform);
        item.transform.position = transform.position;
        print(item.transform.position);
    }

    public void BeginDrag()
    {
        if(item)
        {
            inv.BeginDrag(item);
            item = null;
        }
        //end drag in inventory
    }
}
