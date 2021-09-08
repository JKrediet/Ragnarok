using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviour
{
	public float maxrangeFromPlayer=15;
	public float toCloseDis=5;
	public float startRaycastHeight;
	public int enemiesForPlayer;
	public int playerAmount;
	public LayerMask groundLayer;
	public GameObject testEnemie;
	private GameObject[] players;
	private void Start()
	{
		players = GameObject.FindGameObjectsWithTag("Player");
	}
	public void SpawnEnemies(float scaling)
	{
		playerAmount = 0;
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].GetComponent<MeshRenderer>())///moet hier nogg ff de float health kunnne getten jorn!
			{
				playerAmount++;
			}
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
					Instantiate(testEnemie,spawnPos, Quaternion.identity);
					//PhotonNetwork.Instantiate("string omin te spawnen", spawnPos, Quaternion.identity);
				}
			}
		}
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
		if (Chance())
		{
			spawnPos.x += Random.Range(toCloseDis, maxrangeFromPlayer);
			spawnPos.z += Random.Range(toCloseDis, maxrangeFromPlayer);
		}
		else
		{
			spawnPos.x -= Random.Range(toCloseDis, maxrangeFromPlayer);
			spawnPos.z -= Random.Range(toCloseDis, maxrangeFromPlayer);
		}

		Ray ray = new Ray(spawnPos, -transform.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, groundLayer))
		{
			spawnPos = hitInfo.transform.position;
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