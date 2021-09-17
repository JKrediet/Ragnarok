using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class EnemySpawner : MonoBehaviour
{
	public float maxrangeFromPlayer=15;
	public float toCloseDis=5;
	public float startRaycastHeight;
	public int enemiesForPlayer;
	public int playerAmount;
	public Vector3 spawnOffset;
	public LayerMask groundLayer;
	public EnemyList enemielist;
	public List<GameObject> enemies;
	private GameObject[] players;
	public PhotonView pv;
	public LightingManager lm;
	public GameManager gm;
	private void Update()
	{
		if (lm.isNight)
		{
			if(enemies.Count< enemiesForPlayer * players.Length * gm.days)
			{
				SpawnEnemies(gm.ScalingLeJorn());
			}
		}
	}
	public void ClearPlayers()
	{
		players = null;
	}
	public void GetPlayers()
	{
		players = GameObject.FindGameObjectsWithTag("Player");
	}
	public void SpawnEnemies(float scaling)
	{
		playerAmount = 0;
		for (int i = 0; i < players.Length; i++)
		{
			playerAmount++;//doe hier nog health check van players of ze levend zijn zo niet doe geen ++
		}
		for (int i = 0; i < enemiesForPlayer; i++)
		{
			for (int i_i = 0; i_i < playerAmount; i_i++)
			{
				Vector3 spawnPos = GetSpawnPos(i_i);

				float dis = Vector3.Distance(spawnPos, players[i_i].transform.position);
				if(CheckDistance(dis))
				{
					spawnPos = GetSpawnPos(i_i);
					if (CheckDistance(dis))
					{
						spawnPos = GetSpawnPos(i_i);
					}
				}
				else
				{
					int random = Random.Range(0, enemielist.enemieList.Count - 1);
					pv.RPC("Spawn", RpcTarget.MasterClient, spawnPos+spawnOffset, random);
				}
			}
		}
	}
	[PunRPC]
	public void Spawn(Vector3 spawnPos,int i)
	{
		GameObject spawnedEnemie = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", enemielist.enemieList[i]), spawnPos, Quaternion.identity);
		enemies.Add(spawnedEnemie);
	}
	public bool CheckDistance(float distance)
	{
		if (distance < toCloseDis)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public Vector3 GetSpawnPos(int index)///ff fixen
	{
		Vector3 spawnPos = players[index].transform.position;
		spawnPos.y += startRaycastHeight;
		int spawnCorner = Random.Range(0, 4);
		if (spawnCorner == 1)
		{
			spawnPos.x += Random.Range(toCloseDis, maxrangeFromPlayer);
			spawnPos.z += Random.Range(toCloseDis, maxrangeFromPlayer);
		}
		else if (spawnCorner == 2)
		{
			spawnPos.x -= Random.Range(toCloseDis, maxrangeFromPlayer);
			spawnPos.z -= Random.Range(toCloseDis, maxrangeFromPlayer);
		}
		else if (spawnCorner == 3)
		{
			spawnPos.x += Random.Range(toCloseDis, maxrangeFromPlayer);
			spawnPos.z -= Random.Range(toCloseDis, maxrangeFromPlayer);
		}
		else
		{
			spawnPos.x -= Random.Range(toCloseDis, maxrangeFromPlayer);
			spawnPos.z += Random.Range(toCloseDis, maxrangeFromPlayer);
		}

		Ray ray = new Ray(spawnPos, -transform.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, groundLayer))
		{
			spawnPos = hitInfo.point+spawnOffset;
			return spawnPos;
		}
		else
		{
			GetSpawnPos(index);
		}
		return spawnPos;
	}
	public bool Chance()
	{
		if (Random.Range(0.00f, 5.00f) <= 4.00f)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}