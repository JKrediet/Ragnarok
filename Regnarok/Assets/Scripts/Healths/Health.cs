using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    protected float health;

    public PhotonView PV;

    float armor;


    protected virtual void Awake()
    {
        if(GetComponent<PhotonView>())
        {
            PV = GetComponent<PhotonView>();
        }
        health = maxHealth;
    }
    public virtual void Health_Damage(float damageValue)
    {
        damageValue = Mathf.Clamp(damageValue - armor, 1, maxHealth);
        if (health > 0)
        {
            health = Mathf.Clamp(health - damageValue, 0, maxHealth);
            if(health == 0)
            {
                Health_Dead();
            }
        }
    }
    public virtual void Health_Heal(float healValue)
    {
        if (health < maxHealth)
        {
            health = Mathf.Clamp(health + healValue, 0, maxHealth);
        }
    }
    public virtual void Health_Dead()
    {
        print("used base function");
        Destroy(gameObject);
    }

    //on item equip
    public virtual void SetMaxHealth(float extraHealth, bool gain)
    {
        if(!gain)
        {
            extraHealth *= -1;
        }
        maxHealth += extraHealth;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }

    //stats
    public virtual void GiveKiller(GameObject killer)
    {
        //nothing here yet
    }
    public void RecieveStats(float _health, float _armor)
    {
        maxHealth = _health;
        armor = _armor;
    }
}
