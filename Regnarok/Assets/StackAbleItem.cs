using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StackAbleItem : MonoBehaviour
{
	public int id;
	public void Collision()
	{
		//PhotonNetwork.Destroy(gameObject);
		Destroy(gameObject);
	}
}
