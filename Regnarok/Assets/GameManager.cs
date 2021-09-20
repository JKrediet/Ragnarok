using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Video;
using System.IO;

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

    public List<GameObject> playerObjectList;

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
        es.ClearPlayers();
        es.GetPlayers();
        es.SpawnEnemies(ScalingLeJorn());
        yield return new WaitForSeconds(timeForNightToEnd);
        scalingAmount = 1;
        isDoingNight = false;
    }
    public float ScalingLeJorn()
	{
        return scalingAmount = scalingIncreaseAmount * days;
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
    #region destroyWorldItems
    public void DropItems(string droppedItemName, Vector3 position, Quaternion rotation, int amount, int serialNumber)
    {
        GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient, droppedItemName, position, rotation, amount, serialNumber);
    }
    [PunRPC]
    public void DestroyWorldItem(string droppedItemName, Vector3 position, Quaternion rotation, int amount, int serialNumber)
    {
        GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", droppedItemName), position, rotation);
        droppedItem.GetComponent<WorldItem>().SetUp(ItemList.SelectItem(droppedItemName).name, amount, ItemList.SelectItem(droppedItemName).sprite, ItemList.SelectItem(droppedItemName).type, ItemList.SelectItem(droppedItemName).maxStackSize);
        GetComponent<PhotonView>().RPC("RemoveItemFromWorld", RpcTarget.All, serialNumber);
    }
    [PunRPC]
    public void RemoveItemFromWorld(int serialNumber)
    {
        HitableObject[] objectsFound = FindObjectsOfType<HitableObject>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].itemSerialNumber == serialNumber)
            {
                Destroy(objectsFound[i].gameObject);
                return;
            }
        }
        ItemPickUp[] objectsFoundPickUp = FindObjectsOfType<ItemPickUp>();
        for (int i = 0; i < objectsFoundPickUp.Length; i++)
        {
            if (objectsFoundPickUp[i].itemSerialNumber == serialNumber)
            {
                Destroy(objectsFoundPickUp[i].gameObject);
                return;
            }
        }
    }
    #endregion

    public bool IsThisMasterClient()
    {
        if(GetComponent<PhotonView>().Owner == PhotonNetwork.MasterClient)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #region sincHealth
    public void SincHealthOfHitableObject(int _serialNumber, float _healthAmount, EquipmentType _type)
    {
        GetComponent<PhotonView>().RPC("SincHealthOnMaster", RpcTarget.MasterClient, _serialNumber, _healthAmount, _type);
    }
    [PunRPC]
    public void SincHealthOnMaster(int _serialNumber, float _healthAmount, EquipmentType _type)
    {
        GetComponent<PhotonView>().RPC("SetHealth", RpcTarget.All, _serialNumber, _healthAmount, _type);
    }
    [PunRPC]
    public void SetHealth(int _serialNumber, float _healthAmount, EquipmentType _type)
    {
        HitableObject[] objectsFound = FindObjectsOfType<HitableObject>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].itemSerialNumber == _serialNumber)
            {
                objectsFound[i].HitByPlayer(_healthAmount, _type);
                return;
            }
        }
    }
    #endregion
}
