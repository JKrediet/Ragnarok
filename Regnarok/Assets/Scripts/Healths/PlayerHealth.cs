using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.IO;

public class PlayerHealth : Health
{
    public GameObject graveStone;
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
                else if (otherPlayersCam[i].transform.parent.transform.GetComponent<PlayerHealth>().health <= 0)
				{
                    otherPlayersCam.Remove(otherPlayersCam[i]);
                }
            }
            for (int i = 0; i < otherPlayersCam.Count; i++)
            {
				if (otherPlayersCam[i].transform.GetComponent<AudioListener>())
				{
                    Destroy(otherPlayersCam[i].transform.GetComponent<AudioListener>());

                }
            }
        }
    }
	public override void Health_Damage(float damageValue, bool bleed, float execute, Vector3 hitlocation)
    {
        base.Health_Damage(damageValue, bleed, execute, hitlocation);
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
        StartCoroutine(Dead());
    }
	IEnumerator HealthRegen()
    {
        Health_Heal(healthRegen);
        yield return new WaitForSeconds(1);
        StartCoroutine("HealthRegen");
	}
    public IEnumerator Dead()
    {
        FindObjectOfType<GameManager>().CheckHp();
        GetComponent<PhotonView>().RPC("SpawnGrave", RpcTarget.MasterClient);
        yield return new WaitForSeconds(1.5f);
        graveStone.GetComponent<GraveStoneScript>().myPlayer = transform.gameObject;
        otherPlayersCam = new List<GameObject>(GameObject.FindGameObjectsWithTag("MainCamera"));
        for (int i = 0; i < otherPlayersCam.Count; i++)
		{
			if (otherPlayersCam[i] == mainCam)
			{
				otherPlayersCam.Remove(otherPlayersCam[i]);
			}
			else if (otherPlayersCam[i].transform.parent.transform.GetComponent<PlayerHealth>().health <= 0)
			{
				otherPlayersCam.Remove(otherPlayersCam[i]);
			}
		}
		for (int i = 0; i < otherPlayersCam.Count; i++)
		{
			if (otherPlayersCam[i].transform.GetComponent<AudioListener>())
			{
				Destroy(otherPlayersCam[i].transform.GetComponent<AudioListener>());

			}
		}
		mainCam.SetActive(false);
		GetComponent<PhotonView>().RPC("SetBody", RpcTarget.All, false);
		GetComponent<PlayerController>().isDead = true;
		otherPlayersCam[index].GetComponent<Camera>().enabled = true;
	}
    [PunRPC]
    public void SpawnGrave()
	{
        graveStone = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Grave"), transform.position, transform.rotation);
    }
	public void Respawn()
	{
        SincHeal(100);
        otherPlayersCam[index].GetComponent<Camera>().enabled = false;
        otherPlayersCam.Clear();
        mainCam.SetActive(true);
        GetComponent<PlayerController>().isDead = false;
        GetComponent<PhotonView>().RPC("SetBody", RpcTarget.All,true);
        GetComponent<PhotonView>().RPC("DestroyGraveStone", RpcTarget.MasterClient);
        SincHeal(100);
    }
    [PunRPC]
    public void SetBody(bool b)
	{
		if (b)
		{
            mesh.SetActive(true);
        }
		else
		{
            mesh.SetActive(false);
        }
	}
    [PunRPC]
    public void DestroyGraveStone()
	{
        PhotonNetwork.Destroy(graveStone);
    }
}
