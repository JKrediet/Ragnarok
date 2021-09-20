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

    GameManager manager;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        manager = FindObjectOfType<GameManager>();
    }
    protected void Start()
    {
        health *= Random.Range(0.7f, 1.3f);
        maxHealth = health;
    }
    public virtual void HitByPlayer(float _damage, EquipmentType itemType)
    {
        if (health > 0)
        {
            if (itemType == itemTypeNeeded)
            {

                health = Mathf.Clamp(health - _damage, 0, maxHealth);
            }
            else
            {
                health = Mathf.Clamp(health - 1, 0, maxHealth);
            }
            if (health == 0)
            {
                //items
                DropItems();
            }
        }
    }
    protected virtual void DropItems()
    {
        for (int i = 0; i < dropItemName.Length; i++)
        {
            manager.DropItems(dropItemName[i], transform.position, Quaternion.identity, Random.Range((int)minDrop, (int)maxDrop), itemSerialNumber);
        }
    }
    public void TakeDamage(float _damage, EquipmentType _itemType)
    {
        print(1);
        manager.SincHealthOfHitableObject(itemSerialNumber, _damage, _itemType);
    }
}
