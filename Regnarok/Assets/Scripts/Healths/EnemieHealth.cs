using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class EnemieHealth : Health
{
	public int coinDrop;
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
		sm.isDead = true;
		sm.ResetAnim();
		sm.anim.SetBool("IsDying", true);
		Destroy(gameObject.GetComponent<NavMeshAgent>());
		Destroy(gameObject.GetComponent<NavMeshObstacle>());
		Destroy(healthbar);
	}
	public void RollItem()
	{

	}
	public void GiveXP()
	{

	}
	public void DropMoney()
	{
		coinDrop *= (gm.days / 100);
		//instantiate hierro
	}
	public void GivePlayerStuff()
	{
		RollItem();
		GiveXP();
		DropMoney();
		PhotonNetwork.Destroy(gameObject);
	}
}
