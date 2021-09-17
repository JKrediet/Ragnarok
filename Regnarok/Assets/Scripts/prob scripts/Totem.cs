using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class Totem : MonoBehaviour
{
	public bool activated;
	public int amountOfEnemies;
	public float timeToCheck;
	public Vector3 spawnOffset = new Vector3(0, 1, 0);
	public GameObject[] torches;
	public EnemyList enemielist;
	public string spawnParticleName;
	public List<GameObject> enemies;
	private bool isChecking;
	private bool allEnemiesDied;

	private void Update()
	{
		if (activated)
		{
			if (!isChecking)
			{
				if (allEnemiesDied)
				{
					StartCoroutine(CheckEnemiesHp());
				}
			}
		}
	}
	public IEnumerator CheckEnemiesHp()
	{
		isChecking = true;
		enemies.RemoveAll(GameObject => GameObject == null);
		if (enemies.Count<=0)
		{
			allEnemiesDied = true;
			GiveItem();
		}
		for (int i = 0; i < torches.Length; i++)
		{
			if (i > enemies.Count)
			{
				torches[i].SetActive(false);
			}
		}
		yield return new WaitForSeconds(timeToCheck);
		isChecking = false;
	}
	public void GiveItem()
	{
		
	}
	public void Interact()
	{
		if (!activated)
		{
			for (int i = 0; i < torches.Length; i++)
			{
				torches[i].SetActive(true);
			}
			for (int i = 0; i < amountOfEnemies; i++)
			{
				SpawnEnemies();
			}
			GetComponent<PhotonView>().RPC("SyncActivated", RpcTarget.MasterClient);
		}
	}
	void SpawnEnemies()
	{
		int randomNum = Random.Range(0, enemielist.enemieList.Count-1);
		Vector3 pos = transform.position;
		pos.x += Random.Range(-6.00f, 6.00f);
		pos.z += Random.Range(-6.00f, 6.00f);

		pos.y = 100;
		Ray ray = new Ray(pos, -transform.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			if (hitInfo.transform.tag == "Mesh")
			{
				GetComponent<PhotonView>().RPC("SpawnEnemiesSyncted", RpcTarget.MasterClient,randomNum, hitInfo.point + spawnOffset);
			}
			else
			{
				amountOfEnemies++;
			}
		}
	}
	[PunRPC]
	public void SpawnEnemiesSyncted(int i,Vector3 spawnPos)
	{
		GameObject spawnedEnemie =PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",enemielist.enemieList[i]), spawnPos, Quaternion.identity);
		enemies.Add(spawnedEnemie);
	}
	[PunRPC]
	public void SyncActivated()
	{
		activated = true;
	}
}