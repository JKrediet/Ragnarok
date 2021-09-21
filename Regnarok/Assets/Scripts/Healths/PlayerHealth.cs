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
    public float camRotSpeed;
    public float respawnTime=15;
    public GameObject mainCam;
	public GameObject deathCam;
    private List<GameObject> players;
	public bool isDeath;
    private int camIndex;
    private void Start()
    {
        if (PV.IsMine)
        {
            HealthSlider.maxValue = maxHealth;
            HealthSlider.value = health;
            StartCoroutine("HealthRegen");
            deathCam.SetActive(false);
            players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

            for (int i = 0; i < players.Count; i++)
			{
				if (players[i] == gameObject)
				{
                    players.Remove(players[i]);
				}
			}
        }
    }
	private void Update()
	{
		if (isDeath)
		{
            deathCam.transform.Rotate(0, camRotSpeed * Time.deltaTime, 0);
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
	public override void Health_Dead()
	{
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
        players[camIndex].GetComponent<PlayerHealth>().deathCam.SetActive(true);
        mainCam.SetActive(false);
        gameObject.SetActive(false);
        isDeath = true;
        yield return new WaitForSeconds(respawnTime);
        gameObject.SetActive(true);
        mainCam.SetActive(true);
        players[camIndex].GetComponent<PlayerHealth>().deathCam.SetActive(false);
        health = 100;
        SincHealthOnMAster(health, false);
        isDeath = false;
    }
    public void CamButton(int i)
	{
        players[camIndex].GetComponent<PlayerHealth>().deathCam.SetActive(false);
        if (camIndex<players.Count&&camIndex>0)
		{          
            camIndex += i;
		}
        else if (camIndex>players.Count)
		{
            camIndex = 0;
		}
        else if (camIndex<0)
		{
            camIndex = players.Count;
		}
        players[camIndex].GetComponent<PlayerHealth>().deathCam.SetActive(true);
    }
}
