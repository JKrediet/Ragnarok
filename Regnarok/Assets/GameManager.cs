using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Scaling")]
    public bool isDoingNight;
    public float timeForNightToEnd;
    public float scalingAmount;
    public float scalingIncreaseAmount;
    public int days;
    private EnemySpawner es;
	private void Start()
	{
        es = GetComponent<EnemySpawner>();
	}
	public IEnumerator IsNight()
	{
        isDoingNight = true;
        days++;
        ScalingLeJorn();
        es.SpawnEnemies(scalingAmount);
        yield return new WaitForSeconds(timeForNightToEnd);
        scalingAmount = 1;
        isDoingNight = false;
    }
    public void ScalingLeJorn()
	{
        scalingAmount = scalingIncreaseAmount * days;
	}
}
