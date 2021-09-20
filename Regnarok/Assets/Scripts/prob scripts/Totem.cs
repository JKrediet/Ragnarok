using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
public class Totem : MonoBehaviour
{
	public bool activated;
	public bool gaveItem;
	public int amountOfEnemies;
	public float timeToCheck;
	public Vector3 itemSpawnOffset = new Vector3(0, 1.5f, 0);
	public Vector3 spawnOffset = new Vector3(0, 1, 0);
	public GameObject[] torches;
	public EnemyList enemielist;
	public  ItemListScript itemlist;
	public string spawnParticleName;
	public List<GameObject> enemies;
	private bool isChecking;
	private bool allEnemiesDied;
	private int type;
	private void Start()
	{
		type = Random.Range(1, 4);
		amountOfEnemies *=type;
	}
	private void Update()
	{
		if (!gaveItem)
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
	}
	public IEnumerator CheckEnemiesHp()
	{
		isChecking = true;
		for (var i = enemies.Count - 1; i > -1; i--)
		{
			if (enemies[i] == null)
			{
				enemies.RemoveAt(i);
			}
		}
		if (enemies.Count<=0)
		{
			allEnemiesDied = true;
			if (!gaveItem)
			{
				GiveItem();
			}
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
		gaveItem = true;
		int rarity = RaretyChance();
		string prefabName;
		if (rarity == 0)
		{
			int random = Random.Range(0, itemlist.common.Count);
			prefabName = itemlist.common[random].name;

			Ray ray = new Ray(GetPos(), -transform.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				GetComponent<PhotonView>().RPC("SpawnItem", RpcTarget.MasterClient, hitInfo.point + itemSpawnOffset, prefabName);
			}
		}
		else if (rarity==1)
		{
			int random = Random.Range(0, itemlist.rare.Count);
			prefabName = itemlist.rare[random].name;
			Ray ray = new Ray(GetPos(), -transform.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				GetComponent<PhotonView>().RPC("SpawnItem", RpcTarget.MasterClient, hitInfo.point + itemSpawnOffset, prefabName);
			}
		}
		else if (rarity == 2)
		{
			int random = Random.Range(0, itemlist.epic.Count);
			prefabName = itemlist.epic[random].name;
			Ray ray = new Ray(GetPos(), -transform.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				GetComponent<PhotonView>().RPC("SpawnItem", RpcTarget.MasterClient, hitInfo.point + itemSpawnOffset, prefabName);
			}
		}
		else if (rarity == 3)
		{
			int random = Random.Range(0, itemlist.legendary.Count);
			prefabName = itemlist.legendary[random].name;
			Ray ray = new Ray(GetPos(), -transform.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				GetComponent<PhotonView>().RPC("SpawnItem", RpcTarget.MasterClient, hitInfo.point + itemSpawnOffset, prefabName);
			}
		}
		else if (rarity == 4)
		{
			int random = Random.Range(0, itemlist.mythic.Count);
			prefabName = itemlist.mythic[random].name;
			Ray ray = new Ray(GetPos(), -transform.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				GetComponent<PhotonView>().RPC("SpawnItem", RpcTarget.MasterClient, hitInfo.point + itemSpawnOffset, prefabName);
			}
		}
	}
	[PunRPC]
	public void SpawnItem(Vector3 pos,string name)
	{
		PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", name), pos, Quaternion.identity);
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
	public Vector3 GetPos()
	{
		Vector3 pos = transform.position;
		pos.x += Random.Range(-6.00f, 6.00f);
		pos.z += Random.Range(-6.00f, 6.00f);

		pos.y = 100;
		return pos;
	}
	void SpawnEnemies()
	{
		int randomNum = Random.Range(0, enemielist.enemieList.Count-1);
		Ray ray = new Ray(GetPos(), -transform.up);
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
	public int RaretyChance()
	{
		float randomNum = Random.Range(0.00f, 100.00f);
		int itemRarity = 0;

		if (type == 1)
		{
			if (randomNum <= 68.5f)
			{//common
				itemRarity = 0;
			}
			else if (randomNum > 68.5f && randomNum <= 93.5f)
			{//rare
				itemRarity = 1;
			}
			else if (randomNum > 93.5f && randomNum <= 98.5f)
			{//epic
				itemRarity = 2;
			}
			else if (randomNum > 98.5f && randomNum <= 99.5f)
			{//legendary
				itemRarity = 3;
			}
			else if (randomNum > 99.5f)
			{//mythic
				itemRarity = 4;
			}
		}
		else if (type == 2)
		{
			if (randomNum <= 27.5f)
			{//common
				itemRarity = 0;
			}
			else if (randomNum > 27.5f && randomNum <= 82.5f)
			{//rare
				itemRarity = 1;
			}
			else if (randomNum > 82.5f && randomNum <= 92.5f)
			{//epic
				itemRarity = 2;
			}
			else if (randomNum > 92.5f && randomNum <= 97.5f)
			{//legendary
				itemRarity = 3;
			}
			else if (randomNum > 97.5f)
			{//mythic
				itemRarity = 4;
			}
		}
		else if (type == 3)
		{
			if (randomNum <= 24.2f)
			{//common
				itemRarity = 0;
			}
			else if (randomNum > 24.2f && randomNum <= 60.5f)
			{//rare
				itemRarity = 1;
			}
			else if (randomNum > 60.5f && randomNum <= 80)
			{//epic
				itemRarity = 2;
			}
			else if (randomNum > 80 && randomNum <= 94)
			{//legendary
				itemRarity = 3;
			}
			else if (randomNum > 94)
			{//mythic
				itemRarity = 4;
			}
		}
		else if (type == 4)
		{
			if (randomNum <= 5)
			{//common
				itemRarity = 0;
			}
			else if (randomNum > 5 && randomNum <= 25)
			{//rare
				itemRarity = 1;
			}
			else if (randomNum > 25 && randomNum <= 65)
			{//epic
				itemRarity = 2;
			}
			else if (randomNum > 65 && randomNum <= 85)
			{//legendary
				itemRarity = 3;
			}
			else if (randomNum > 85)
			{//mythic
				itemRarity = 4;
			}
		}
		if (itemRarity == 0)
		{
			int roll = Random.Range(0, 5);
			return roll;
		}
		else if (itemRarity == 1)
		{
			int roll = Random.Range(6, 11);
			return roll;
		}
		else if (itemRarity == 2)
		{
			int roll = Random.Range(12, 17);
			return roll;
		}
		else if (itemRarity == 3)
		{
			int roll = Random.Range(18, 22);
			return roll;
		}
		else if (itemRarity == 4)
		{
			int roll = Random.Range(23, 25);
			return roll;
		}
		return 0;
	}
}