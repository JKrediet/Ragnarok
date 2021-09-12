using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StackableItemScript : MonoBehaviour
{
	public items[] itemlist;
	private bool cooldownBool;
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.GetComponent<StackAbleItem>())
		{
			if (!cooldownBool)
			{
				AddItem(other.transform.GetComponent<StackAbleItem>().id, other.transform.gameObject);
				other.transform.GetComponent<StackAbleItem>().Collision();
			}
		}
	}
	public void AddItem(int index, GameObject item)
	{
		cooldownBool = true;
		if (ChanceToGetDubbel())
		{
			itemlist[index].amount += 2;
		}
		else
		{
			itemlist[index].amount++;
		}
		Invoke("ResetCooldown", 0.1f);
	}
	public void ResetCooldown()
	{
		cooldownBool = false;
	}
	public int RaretyChance()
	{
		float randomNum = Random.Range(0.00f, 100.00f);
		if (randomNum <= 20)
		{
			return 1;
		}
		else if (randomNum > 20 &&randomNum <= 40)
		{
			return 2;
		}
		else if (randomNum > 40 && randomNum <= 60)
		{
			return 3;
		}
		else if (randomNum > 60 && randomNum <= 80)
		{
			return 4;
		}
		else if (randomNum >80)
		{
			return 5;
		}
		return 0;
	}
	public bool ChanceToGetDubbel()
	{
		if (Random.Range(0.00f, 100.00f) <= 1)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	[System.Serializable]
	public struct items
	{
		public string itemName;
		public int amount;
	}
}
