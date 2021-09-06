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
    public float    health;
    private void Start()
    {
        if (PV.IsMine)
        {
            HealthSlider.maxValue = maxHealth;
            HealthSlider.value = health;
        }
        else
        {
            //remove all obejcts that are not mine!
            Destroy(HealthSlider.gameObject);
        }
    }
    public override void Health_Damage(float damageValue)
    {
        base.Health_Damage(damageValue);
        if(HealthSlider)
        {
            HealthSlider.value = health;
        }
    }
    public override void SetMaxHealth(float extraHealth, bool gain)
    {
        base.SetMaxHealth(extraHealth, gain);
        if (HealthSlider)
        {
            HealthSlider.maxValue = maxHealth;
        }
    }
}
