using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    public GameObject mainCam;
    public GameObject mesh;
    public List<GameObject> otherPlayersCam;
    public Slider HealthSlider;
    public float respawnTime=15;
    private int index;
    private void Start()
    {
        if (PV.IsMine)
        {
            HealthSlider.maxValue = maxHealth;
            HealthSlider.value = health;
            StartCoroutine("HealthRegen");
            otherPlayersCam = new List<GameObject>(GameObject.FindGameObjectsWithTag("MainCamera"));
            for (int i = 0; i < otherPlayersCam.Count; i++)
            {
                if (otherPlayersCam[i] == mainCam)
                {
                    otherPlayersCam.Remove(otherPlayersCam[i]);
                }
            }
        }
    }
	public override void Health_Damage(float damageValue, bool bleed, Vector3 hitlocation)
    {
        base.Health_Damage(damageValue, bleed, hitlocation);
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
	public override void Health_Dead()
	{
        if (!PV.IsMine)
        {
            return;
        }
        StartCoroutine(RespawnPlayer());
    }
	IEnumerator HealthRegen()
    {
        Health_Heal(healthRegen);
        yield return new WaitForSeconds(1);
        StartCoroutine("HealthRegen");
    }
    IEnumerator RespawnPlayer()
	{
        otherPlayersCam = new List<GameObject>(GameObject.FindGameObjectsWithTag("MainCamera"));
        for (int i = 0; i < otherPlayersCam.Count; i++)
        {
            if (otherPlayersCam[i] == mainCam)
            {
                otherPlayersCam.Remove(otherPlayersCam[i]);
            }
        }
        mainCam.SetActive(false);
        mesh.SetActive(false);
        otherPlayersCam[index].GetComponent<Camera>().enabled = true;
        yield return new WaitForSeconds(respawnTime);
        SincHeal(100);
        otherPlayersCam[index].GetComponent<Camera>().enabled = false;
        mainCam.SetActive(true);
        mesh.SetActive(true);
    }
}
