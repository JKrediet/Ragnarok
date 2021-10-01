using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System.IO;

public class EnemieHealth : Health
{
	public int coinDrop;
	public float xpAmount;
	public GameObject healthbar;
	private GameManager gm;
	private StateManager sm;
	private PhotonView pv;
	public void Start()
	{
		gm = FindObjectOfType<GameManager>();
		sm = GetComponent<StateManager>();
		pv = GetComponent<PhotonView>();
	}
	public override void Health_Dead()
	{
		transform.position -= new Vector3(0, 1, 0);
		sm.isDead = true;
		sm.ResetAnim();
		sm.anim.SetBool("IsDying", true);
		Destroy(gameObject.GetComponent<NavMeshAgent>());
		Destroy(gameObject.GetComponent<NavMeshObstacle>());
		healthbar.SetActive(false);
	}
	public void RollItem()
	{

	}
	[PunRPC]
	public void DropMoney()
	{
		float temcoins = (coinDrop * gm.days) * gm.goldMultiplier;
		coinDrop = (int)temcoins;
		GameObject tempObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "GoldenCoin"), transform.position,Quaternion.identity);
		tempObject.GetComponent<CoinScript>().coinAmount = coinDrop;
	}
	[PunRPC]
	public void GiveXp()
    {
		CharacterStats[] players = FindObjectsOfType<CharacterStats>();
        foreach (CharacterStats player in players)
        {
			if(player.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
            {
				player.GainXp(xpAmount);
            }
		}
    }
	public void GivePlayerStuff()
	{
		RollItem();
		pv.RPC("DropMoney", RpcTarget.MasterClient);
		pv.RPC("GiveXp", RpcTarget.All);
		PhotonNetwork.Destroy(gameObject);
	}
}
