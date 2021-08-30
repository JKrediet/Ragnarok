using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    public bool activated;
    public int amountOfEnemies;
    private GameScaler gs;
	public EnemyList enemielist;
	private int enemiesToSpawn;
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
			Instantiate(enemielist.enemieList[randomNum], pos, Quaternion.identity);
		}
    }
}
