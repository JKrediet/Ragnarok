using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SeeOtherPlayerScript : MonoBehaviour
{
	public List<GameObject> players;
	private bool active;
	private void Start()
	{
		players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].transform.GetComponent<PhotonView>().IsMine)
			{
				players.Remove(players[i]);
			}
		}
	}
	private void Update()
	{
		if (Input.GetButtonDown("H"))
		{
			for (int i = 0; i < players.Count; i++)
			{
				Trigger(i);
			}
		}
	}
	public void Trigger(int i)
	{
		active = !active;

		players[i].GetComponent<SeeOtherPlayerScript>().enabled = active;
	}
}
