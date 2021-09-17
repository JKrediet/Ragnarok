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
        //destory for multiplayer
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
        //hier stat calculation
        GiveStatsToStats();

        Invoke("ResetCooldown", 0.1f);
	}
    void GiveStatsToStats()
    {
        CharacterStats stats = GetComponent<CharacterStats>();
        for (int i = 0; i < itemlist.Length; i++)
        {
            if(itemlist[i].amount > 0)
            {
                if(itemlist[i].itemName == "EnergyDrink")
                {
                    stats.GiveStats_movementSpeed(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Shield")
                {
                    stats.GiveStats_addedArmor(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "IronKnuckle")
                {
                    stats.GiveStats_damageFlat(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "HeartContainer")
                {
                    stats.GiveStats_addedHealth(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "ShadowOrb")
                {
                    stats.GiveStats_healthOnKill(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Crown")
                {
                    //xp nog niet added
                    continue;
                }
                else if (itemlist[i].itemName == "Knife")
                {
                    stats.GiveStats_bleedChance(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Clock")
                {
                    stats.GiveStats_attackSpeedFlat(itemlist[i].amount * itemlist[i].value);
                }
                else if (itemlist[i].itemName == "Scouter")
                {
                    stats.GiveStats_critChanceFlat(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "GoldPouch")
                {
                    // more gold
                    continue;
                }
                else if (itemlist[i].itemName == "Plaster")
                {
                    stats.GiveStats_healthRegen(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
                else if (itemlist[i].itemName == "Tooth")
                {
                    stats.GiveStats_addLifeSteal(itemlist[i].amount * itemlist[i].value);
                    continue;
                }
            }
        }


        stats.CalculateOffensiveStats();
        stats.CalculateDefensiveStats();
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
        [TextArea]
        public string decription;
        public float value;
    }
}