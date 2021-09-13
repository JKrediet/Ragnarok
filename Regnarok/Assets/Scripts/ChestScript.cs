using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ChestScript : MonoBehaviour
{
    public int cost;
    public enum chestType { small=1,medium=2,large=3,golden=4 };
    public Transform spawnPos;
    public GameObject testObj;
    public void Interact()
	{
        RollItem();
        print("chest");
	}
    public void RollItem()
	{
        SpawnITem();
	}
    public void SpawnITem()
	{
        //Instantiate(testObj, spawnPos);
        //PhotonNetwork.Instantiate(, spawnPos.position, spawnPos.rotation);
	}
}