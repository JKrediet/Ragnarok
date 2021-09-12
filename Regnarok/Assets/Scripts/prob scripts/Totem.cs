using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    public bool activated;
    public int amountOfEnemies;
	public Vector3 spawnOffset = new Vector3(0,1,0);
	public GameObject[] torches;
    private GameScaler gs;
	public EnemyList enemielist;
	private int enemiesToSpawn;
	private List<GameObject> enemies;
	private void Start()
	{
		gs = FindObjectOfType<GameScaler>();
		float amountOfEnemies_ = (float)amountOfEnemies;
		float resault = (amountOfEnemies_ * gs.scaling);
		enemiesToSpawn = (int)resault;
	}
	public void Interact()
	{
		if (!isActiveAndEnabled)
		{
            SpawnEnemies();
            activated = false;
		}
	}
    void SpawnEnemies()
    {
		for (int i = 0; i < enemiesToSpawn; i++)
		{
			int randomNum = Random.Range(0, enemielist.enemieList.Count);
			Vector3 pos = transform.position;
			pos.x += Random.Range(0.5f, 3);
			pos.z += Random.Range(0.5f, 3);

			pos.y = 100;
			Ray ray = new Ray(pos, -transform.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				Instantiate(enemielist.enemieList[randomNum], hitInfo.point+spawnOffset, Quaternion.identity);
			}
		}
    }
	public void TotemCheck()
	{
		for (int i = 0; i < enemies.Count; i++)
		{
			if (enemies[i].GetComponent<Health>())
			{
				
			}
			else
			{
				//kijken of de enemie nog leeft zo niet dan remove je die van de lijst
				//
			}
		}
	}
}
