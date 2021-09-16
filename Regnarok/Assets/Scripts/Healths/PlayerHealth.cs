using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    public Slider HealthSlider;

    private void Start()
    {
        if (PV.IsMine)
        {
            HealthSlider.maxValue = maxHealth;
            HealthSlider.value = health;
            StartCoroutine("HealthRegen");
        }
    }
    public override void Health_Damage(float damageValue, bool bleed)
    {
        base.Health_Damage(damageValue, bleed);
        if(HealthSlider)
        {
            HealthSlider.value = health;
        }
    }
    public override void Health_Heal(float healValue)
    {
        base.Health_Heal(healValue);
        if(PV.IsMine)
        {
            HealthSlider.value = health;
        }
    }
    public override void RecieveStats(float _health, float _armor, float _healthRegen)
    {
        base.RecieveStats(_health, _armor, _healthRegen);
        if (PV.IsMine)
        {
            HealthSlider.maxValue = maxHealth;
        }
    }

    IEnumerator HealthRegen()
    {
        Health_Heal(healthRegen);
        yield return new WaitForSeconds(1);
        StartCoroutine("HealthRegen");
    }
}
