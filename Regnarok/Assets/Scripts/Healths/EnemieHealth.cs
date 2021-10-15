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
	public Renderer[] toDisolve;
	public GameObject healthbar;
	private GameManager gm;
	private StateManager sm;
	private PhotonView pv;

	public Vector3 coinDropOffset;
	public Vector3 itemDropOffset;

	private float value;
	private bool disolveCharachter;
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
	private void Update()
	{
		if (disolveCharachter)
		{
			value += 0.2f * Time.deltaTime;
			DisolveMe();
			if (value > 1)
			{
				value = 1;
				PhotonNetwork.Destroy(gameObject);
				disolveCharachter = false;
			}
		}
	}
	public void RollItem()
	{
        foreach (DropItems item in dropItemList)
        {
			float roll = Random.Range(0, 101);
			if(roll < item.ChanceOfDrop)
            {
				GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", item.nameOfItem), transform.position + itemDropOffset, Quaternion.identity);
				if(item.amountOfItem > 1)
                {
					droppedItem.GetComponent<WorldItem>().itemAmount = item.amountOfItem;
				}
			}
        }
	}
	[PunRPC]
	public void DropMoney()
	{
		coinDropOffset = new Vector3(0, 2, 0);
		float temcoins = coinDrop * (gm.days + 1) * gm.goldMultiplier;
		coinDrop = (int)temcoins;
		GameObject tempObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "GoldenCoin"), transform.position + coinDropOffset, Quaternion.identity);
		tempObject.GetComponent<WorldItem>().itemAmount = coinDrop;
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
	public void ActivateDisolve()
	{
		disolveCharachter = true;
	}
	public void DisolveMe()
	{
		for (int i = 0; i < toDisolve.Length; i++)
		{
			toDisolve[i].material.SetFloat("_Dissolve", value);
		}
	}
}