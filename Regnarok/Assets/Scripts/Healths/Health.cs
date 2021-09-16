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
    [HideInInspector] public float health;

    public PhotonView PV;

    public float armor;

    protected float healthRegen;

    int BleedTicks;
    float bleedDamage;

    protected virtual void Awake()
    {
        if(GetComponent<PhotonView>())
        {
            PV = GetComponent<PhotonView>();
        }
        health = maxHealth;
    }
    public virtual void Health_Damage(float damageValue, bool bleed)
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
        if(bleed)
        {
            if(bleedDamage < damageValue * 0.25f)
            {
                bleedDamage = damageValue * 0.25f;
            }
            BleedTicks += 5;
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

    //stats
    public virtual void GiveKiller(GameObject killer)
    {
        //nothing here yet
    }
    public virtual void RecieveStats(float _health, float _armor, float _healthRegen)
    {
        maxHealth = _health;
        armor = _armor;
        healthRegen = _healthRegen;
    }

    protected virtual IEnumerator Bleed()
    {
        yield return new WaitForSeconds(1);
        if(BleedTicks > 0)
        {
            BleedTicks--;
            Health_Damage(bleedDamage, false);
            StartCoroutine("Bleed");
        }
        else if (BleedTicks == 0)
        {
            bleedDamage = 0;
        }
    }
}
