using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GameManager : MonoBehaviour
{

    [Header("Scaling")]
    public bool isDoingNight;
    public float timeForNightToEnd;
    public float scalingAmount;
    public float scalingIncreaseAmount;
    public int days;
    [Header("Player Spawn")]
    public LayerMask groundLayer;
    public float spawnRadius;
    public float spawnHeight;
    private EnemySpawner es;
	private void Start()
	{
        es = GetComponent<EnemySpawner>();
        SpawnPlayers();
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
    public void SpawnPlayers()
	{
		for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
		{
            Vector3 spawnpos = new Vector3(Random.Range(spawnRadius, -spawnRadius), spawnHeight, Random.Range(-spawnRadius, spawnRadius));


            Ray ray = new Ray(spawnpos, -transform.up);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, groundLayer))
            {
                PhotonNetwork.Instantiate("PlayerPrefab", hitInfo.transform.position, Quaternion.identity);
            }
        }
	}
}
