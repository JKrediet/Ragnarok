using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieHealth : Health
{
	public int coinDrop;
	private GameManager gm;
	private StateManager sm;
	public void Start()
	{
		gm = FindObjectOfType<GameManager>();
		sm = GetComponent<StateManager>();
	}
	public override void Health_Dead()
	{
		RollItem();
		GiveXP();
		DropMoney();
		sm.isDead = true;
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
}
