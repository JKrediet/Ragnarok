using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{

    [Header("Scaling")]
    public bool isDoingNight;
    public float timeForNightToEnd;
    public float scalingAmount;
    public float scalingIncreaseAmount;
    public int days;
    [Header("Player Spawn")]
    public GameObject playerObject;
	public LayerMask groundLayer;
    public float spawnHeightOffset;
    public float spawnRadius;
    public float spawnHeight;
    private EnemySpawner es;
    public PlayerManager playerManager;
    public VideoClip[] videos;
    public GameObject loadingScreen, canvas;
    public VideoPlayer videoplayer;

    int loopieloop;

	private void Start()
	{
        es = GetComponent<EnemySpawner>();
        Reroll();
    }
    void Reroll()
    {
        int roll = Random.Range(0, videos.Length);
        videoplayer.clip = videos[roll];
        videoplayer.Play();
        if(loadingScreen.activeSelf)
        {
            Invoke("Reroll", 5);
        }
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
        loadingScreen.SetActive(false);
        canvas.SetActive(false);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
		{
            Vector3 spawnpos = new Vector3(Random.Range(spawnRadius, -spawnRadius), spawnHeight, Random.Range(-spawnRadius, spawnRadius));


            Ray ray = new Ray(spawnpos, -transform.up);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                spawnpos.y = hitInfo.point.y+spawnHeightOffset;
            }
            if (playerManager.pv.Owner == PhotonNetwork.PlayerList[i])
            {
                playerManager.SpawnPlayer(spawnpos);
            }
        }
	}
}
