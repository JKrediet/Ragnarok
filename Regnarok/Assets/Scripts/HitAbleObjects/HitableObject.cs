using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitableObject : MonoBehaviour
{
    public int itemSerialNumber;
    [SerializeField] protected float health, minDrop, maxDrop;
    [SerializeField] protected EquipmentType itemTypeNeeded;
    protected float maxHealth;
    [Space]
    [SerializeField] string[] dropItemName;
    protected Rigidbody rb;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    protected void Start()
    {
        health *= Random.Range(0.7f, 1.3f);
        maxHealth = health;
    }
    public virtual void HitByPlayer(float _damage, EquipmentType itemType)
    { 
        if (itemType == itemTypeNeeded)
        {
            if (health > 0)
            {
                health = Mathf.Clamp(health - _damage, 1, maxHealth);
            }
        }
        else
        {
            health = Mathf.Clamp(health - 1, 1, maxHealth);
        }
        if (health == 0)
        {
            //items
            DropItems();
        }
    }
    protected virtual void DropItems()
    {
        for (int i = 0; i < dropItemName.Length; i++)
        {
            FindObjectOfType<GameManager>().DropItems(dropItemName[i], transform.position, Quaternion.identity, Random.Range((int)minDrop, (int)maxDrop), itemSerialNumber);
        }
    }
    public void TakeDamage(float _damage, EquipmentType _itemType)
    {
        FindObjectOfType<GameManager>().SincHealthOfHitableObject(itemSerialNumber, _damage, _itemType);
    }
}
