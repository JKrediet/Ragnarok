using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public int itemSerialNumber;
    [SerializeField] string[] dropItemName;
    [SerializeField] protected float minDrop, maxDrop;
    [SerializeField] Vector3 dropOffset;

    //destory
    public virtual void DropItems()
    {
        for (int i = 0; i < dropItemName.Length; i++)
        {
            FindObjectOfType<GameManager>().DropItems(dropItemName[i], transform.position + dropOffset, Quaternion.identity, Random.Range((int)minDrop, (int)maxDrop), itemSerialNumber);
        }
    }
}
