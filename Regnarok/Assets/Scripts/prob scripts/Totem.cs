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
			for (int i = 0; i < amountOfEnemies; i++)
			{
				SpawnEnemies();
			}
			activated = true;
		}
	}
	void SpawnEnemies()
	{
		int randomNum = Random.Range(0, enemielist.enemieList.Count);
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
				GameObject spawnedEnemie = Instantiate(enemielist.enemieList[randomNum], hitInfo.point + spawnOffset, Quaternion.identity);
			}
			else
			{
				amountOfEnemies++;
			}
		}
		//enemies.Add(spawnedEnemie);
	}
}