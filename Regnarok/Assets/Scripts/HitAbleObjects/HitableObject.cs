using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class HitableObject : MonoBehaviour
{
    public int itemSerialNumber;
    [SerializeField] protected float health;
    [SerializeField] protected EquipmentType itemTypeNeeded;
    protected float maxHealth;
    [Space]
    protected Rigidbody rb;

    GameManager manager;
    Wiggle wiggle;
    public GameObject damageNumber;
    Vector3 lastHitLocation;

    [SerializeField] List<StructDropItemsList> droppedItems;
    bool justOnce;

    [Serializable]
    public struct StructDropItemsList
    {
        public string dropItemName;
        public Vector2 dropAmounts;
    }

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        manager = FindObjectOfType<GameManager>();
        wiggle = GetComponentInChildren<Wiggle>();
    }
    protected void Start()
    {
        health *= UnityEngine.Random.Range(0.7f, 1.3f);
        maxHealth = health;
    }
    public virtual void HitByPlayer(float _damage, EquipmentType itemType, Vector3 hitlocation)
    {
        wiggle.StartWiggle();
        lastHitLocation = hitlocation;
        if (health > 0)
        {
            if (itemType == itemTypeNeeded)
            {
                if (damageNumber != null)
                {
                    GameObject damageNum = Instantiate(damageNumber, hitlocation, Quaternion.identity);
                    damageNum.GetComponent<DamageNumbers>().damageAmount = _damage;
                }
                health = Mathf.Clamp(health - _damage, 0, maxHealth);
            }
            else
            {
                if (damageNumber != null)
                {
                    GameObject damageNum = Instantiate(damageNumber, hitlocation, Quaternion.identity);
                    damageNum.GetComponent<DamageNumbers>().damageAmount = 1;
                }
                health = Mathf.Clamp(health - 1, 0, maxHealth);
            }
        }
        if (health == 0)
        {
            //items
            if (PhotonNetwork.IsMasterClient)
            {
                DropItems();
            }
        }
    }
    protected virtual void DropItems()
    {
        if (justOnce)
        {
            return;
        }
        justOnce = true;
        for (int i = 0; i < droppedItems.Count; i++)
        {
            manager.DropItems(droppedItems[i].dropItemName, lastHitLocation, Quaternion.identity, UnityEngine.Random.Range((int)droppedItems[i].dropAmounts.x, (int)droppedItems[i].dropAmounts.y), itemSerialNumber);
        }
    }
    public void TakeDamage(float _damage, EquipmentType _itemType, Vector3 hitlocation)
    {
        manager.SincHealthOfHitableObject(itemSerialNumber, _damage, _itemType, hitlocation);
    }
}
