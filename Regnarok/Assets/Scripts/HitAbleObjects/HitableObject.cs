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
    [SerializeField] string dropItemName;
    protected GameObject lastPlayerThatHitTree;
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
    public virtual void HitByPlayer(float _damage, GameObject _hitBy, EquipmentType itemType)
    { 
        if (itemType == itemTypeNeeded)
        {
            if (health > 0)
            {
                health = Mathf.Clamp(health -= _damage, 0, maxHealth);
                lastPlayerThatHitTree = _hitBy;
                if (health == 0)
                {
                    //items
                    Invoke("DropItems", 2);
                }
            }
        }
        else
        {
            if (health > 0)
            {
                health = Mathf.Clamp(health -= 1, 0, maxHealth);
                lastPlayerThatHitTree = _hitBy;
                if (health == 0)
                {
                    //items
                    Invoke("DropItems", 2);
                }
            }
        }
    }
    protected virtual void DropItems()
    {
        FindObjectOfType<GameManager>().DropItems(dropItemName, transform.position, Quaternion.identity, Random.Range((int)minDrop, (int)maxDrop), itemSerialNumber);
    }
}
