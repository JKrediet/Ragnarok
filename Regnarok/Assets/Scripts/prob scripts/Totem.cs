using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
	public bool activated;
	public int amountOfEnemies;
	public Vector3 spawnOffset = new Vector3(0, 1, 0);
	public GameObject[] torches;
	public EnemyList enemielist;
	private List<GameObject> enemies;
	public void Interact()
	{
		if (!activated)
		{
			SpawnEnemies();
			activated = true;
		}
		print("totem");
	}
	void SpawnEnemies()
	{
		for (int i = 0; i < amountOfEnemies; i++)
		{
			int randomNum = Random.Range(0, enemielist.enemieList.Count);
			Vector3 spawnPos = GetSpawnPos();
			GameObject spawnedEnemie = Instantiate(enemielist.enemieList[randomNum], spawnPos + spawnOffset, Quaternion.identity);
			enemies.Add(spawnedEnemie);
		}
	}
	public Vector3 GetSpawnPos()
	{
		Vector3 pos = transform.position;
		pos.x += Random.Range(-3.00f, 3.00f);
		pos.z += Random.Range(-3.00f, 3.00f);

		pos.y = 100;
		Ray ray = new Ray(pos, -transform.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo))
		{
			if (hitInfo.transform.tag == "Mesh")
			{

				return hitInfo.point;
			}
			else
			{
				GetSpawnPos();
			}
		}
		return new Vector3();
	}
}
