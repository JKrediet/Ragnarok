using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : HitableObject
{
    [SerializeField] float fallPower;
    [SerializeField] protected GameObject treeparticle;
    public override void HitByPlayer(float _damage, EquipmentType itemType)
    {
        if (itemType == itemTypeNeeded)
        {
            if (health > 0)
            {
                health = Mathf.Clamp(health -= _damage, 0, maxHealth);
                if (health == 0)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    Debug.Log("Tree has been cut down!");

                    //items
                    Invoke("DropItems", 10);
                }
            }
        }
        else
        {
            if (health > 0)
            {
                health = Mathf.Clamp(health -= 1, 0, maxHealth);
                if (health == 0)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    Debug.Log("Tree has been cut down!");
                    
                    //items
                    Invoke("DropItems", 10);
                }
            }
        }
    }
    protected override void DropItems()
    {
        Instantiate(treeparticle, transform.position, Quaternion.identity);
        base.DropItems();
    }
}
