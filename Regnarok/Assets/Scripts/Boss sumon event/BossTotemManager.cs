using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossTotemManager : MonoBehaviour
{
    public List<GameObject> bosTotems;
    public int amountOffBosses;
    void Update()
    {
		if (bosTotems.Count > 0)
		{
			for (int i = 0; i < bosTotems.Count; i++)
			{
				if (bosTotems[i] == null)
				{
					bosTotems.Remove(bosTotems[i]);
				}
			}
		}
		if (bosTotems.Count <=0)
		{
			//spawn dikke boss en blijf die homie zn hp checken is die dede dan doe ehm portal spawnen>?
		}
    }
}
