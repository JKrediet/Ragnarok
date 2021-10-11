using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Video;
using System.IO;
using UnityEngine.SceneManagement;

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
    public ItemListScript itemlist;
    public EnemyList enemielist;

    public List<GameObject> playerObjectList;

    [Space]
    public float goldMultiplier =1;
    public void GiveStats_goldmulti(float value)
    {
        goldMultiplier = value + 1;
    }
    private void Start()
	{
        es = GetComponent<EnemySpawner>();
        Reroll();
    }
    void Reroll()
    {
        if (loadingScreen.activeSelf)
        {
            int roll = Random.Range(0, videos.Length);
            videoplayer.clip = videos[roll];
            videoplayer.Play();
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
    public void OpenChest(int id)
	{
        GetComponent<PhotonView>().RPC("OpenChestInWorld", RpcTarget.All,id);
    }
    private int amountOffDeads;
    public void CheckHp()
	{
        amountOffDeads = 0;
        for (int i = 0; i < playerObjectList.Count; i++)
		{
			if (playerObjectList[i].GetComponent<PlayerHealth>().health <= 0)
			{
                amountOffDeads++;
			}
		}
        if(amountOffDeads== playerObjectList.Count)
		{
            GetComponent<PhotonView>().RPC("DisconectAll", RpcTarget.All);
        }
	}
    [PunRPC]
    public void DisconectAll()
	{
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
    public void SpawnItem(Vector3 spawnPos,int type)
	{
		switch (type)
		{
            case 0:
                int randomComon = Random.Range(0, itemlist.common.Count);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "StackableItemPrefs", itemlist.common[randomComon].name), spawnPos, Quaternion.identity);
                break;
            case 1:
                int randomRare = Random.Range(0, itemlist.rare.Count);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "StackableItemPrefs", itemlist.rare[randomRare].name), spawnPos, Quaternion.identity);
                break;
            case 2:
                int randomEpic = Random.Range(0, itemlist.epic.Count);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "StackableItemPrefs", itemlist.epic[randomEpic].name), spawnPos, Quaternion.identity);
                break;
            case 3:
                int randomLegendary = Random.Range(0, itemlist.legendary.Count);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "StackableItemPrefs", itemlist.legendary[randomLegendary].name), spawnPos, Quaternion.identity);
                break;
            case 4:
                int randomMythic = Random.Range(0, itemlist.mythic.Count);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "StackableItemPrefs", itemlist.mythic[randomMythic].name), spawnPos, Quaternion.identity);
                break;
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
    public void SpawnEnemies(int i_i, Vector3 spawnPos, int id)
    {
        GetComponent<PhotonView>().RPC("SpawnEnemiesSyncted", RpcTarget.MasterClient, i_i, spawnPos,id);
    }
        [PunRPC]
    public void SpawnEnemiesSyncted(int i_i, Vector3 spawnPos, int id)
    {
        GameObject spawnedEnemie = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", enemielist.enemieList[i_i]), spawnPos, Quaternion.identity);
        spawnedEnemie.GetComponent<Outline>().enabled = true;
        Totem[] totems = FindObjectsOfType<Totem>();
        for (int i = 0; i < totems.Length; i++)
        {
            if (totems[i].id == id)
            {
                totems[i].enemies.Add(spawnedEnemie);
            }
        }
    }
    public void ActivatTotem(int id)
	{
        GetComponent<PhotonView>().RPC("ActivatedTotemSync", RpcTarget.All, id);
    }
    [PunRPC]
    public void ActivatedTotemSync(int id)
    {
        Totem[] totems = FindObjectsOfType<Totem>();
        for (int i = 0; i < totems.Length; i++)
        {
            if (totems[i].id == id)
            {
                totems[i].activated = true;
            }
        }
    }
    [PunRPC]
    public void OpenChestInWorld(int id)
    {
        ChestScript[] objectsFound = FindObjectsOfType<ChestScript>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].chestId == id)
            {
                objectsFound[i].anim.SetBool("OpenChest", true);
            }
        }
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
            }
        }
        ItemPickUp[] objectsFoundPickUp = FindObjectsOfType<ItemPickUp>();
        for (int i = 0; i < objectsFoundPickUp.Length; i++)
        {
            if (objectsFoundPickUp[i].itemSerialNumber == serialNumber)
            {
                Destroy(objectsFoundPickUp[i].gameObject);
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
	#region everyoneDeath
    public void EveryOneDead()
	{
        GetComponent<PhotonView>().RPC("SyncPanelLoss", RpcTarget.All);
    }
    [PunRPC]
    public void SyncPanelLoss()
	{
        
	}
	#endregion

	#region sincHealth
	public void SincHealthOfHitableObject(int _serialNumber, float _healthAmount, EquipmentType _type, Vector3 _hitlocation)
    {
        GetComponent<PhotonView>().RPC("SincHealthOnMaster", RpcTarget.MasterClient, _serialNumber, _healthAmount, _type, _hitlocation);
    }
    [PunRPC]
    public void SincHealthOnMaster(int _serialNumber, float _healthAmount, EquipmentType _type, Vector3 _hitlocation)
    {
        GetComponent<PhotonView>().RPC("SetHealth", RpcTarget.All, _serialNumber, _healthAmount, _type, _hitlocation);
    }
    [PunRPC]
    public void SetHealth(int _serialNumber, float _healthAmount, EquipmentType _type, Vector3 _hitlocation)
    {
        HitableObject[] objectsFound = FindObjectsOfType<HitableObject>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].itemSerialNumber == _serialNumber)
            {
                objectsFound[i].HitByPlayer(_healthAmount, _type, _hitlocation);
                return;
            }
        }
    }
    #endregion
    #region sinc funcace slots
    public void SincSlots(int slotNumber, string givenItem, int amount, int originFurnace)
    {
        GetComponent<PhotonView>().RPC("SincSlotsOmMaster", RpcTarget.MasterClient, slotNumber, givenItem, amount, originFurnace);
    }
    [PunRPC]
    public void SincSlotsOmMaster(int slotNumber, string givenItem, int amount, int originFurnace)
    {
        GetComponent<PhotonView>().RPC("Rpc_sincSlotsFurnace", RpcTarget.All, slotNumber, givenItem, amount, originFurnace);
    }
    [PunRPC]
    public void Rpc_sincSlotsFurnace(int slotNumber, string givenItem, int amount, int originFurnace)
    {
        PlaceAbleItemId[] objectsFound = FindObjectsOfType<PlaceAbleItemId>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].placeabelItemID == originFurnace)
            {
                objectsFound[i].GetComponent<OvenStation>().GetItemInSlot(slotNumber, givenItem, amount);
                return;
            }
        }
    }
    #endregion
    #region sinc chestincentory
    public void SincChestOnMaster(int slotId, string itemId, int itemAmount, int originChest)
    {
        GetComponent<PhotonView>().RPC("Rpc_SincChestOnMaster", RpcTarget.MasterClient, slotId, itemId, itemAmount, originChest);
    }
    [PunRPC]
    public void Rpc_SincChestOnMaster(int slotId, string itemId, int itemAmount, int originChest)
    {
        GetComponent<PhotonView>().RPC("Rpc_SincChestOnClients", RpcTarget.All, slotId, itemId, itemAmount, originChest);
    }
    [PunRPC]
    public void Rpc_SincChestOnClients(int slotId, string itemId, int itemAmount, int originChest)
    {
        PlaceAbleItemId[] objectsFound = FindObjectsOfType<PlaceAbleItemId>();
        for (int i = 0; i < objectsFound.Length; i++)
        {
            if (objectsFound[i].placeabelItemID == originChest)
            {
                objectsFound[i].GetComponent<ChestInventory>().SincSlots(slotId, itemId, itemAmount);
                return;
            }
        }
    }
    #endregion
}
