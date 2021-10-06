using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class SeeOtherPlayerScript : MonoBehaviour
{
	public List<SeeOtherPlayerScript> players;
	private bool active;
	private void Start()
	{
		players = new List<SeeOtherPlayerScript>(FindObjectsOfType<SeeOtherPlayerScript>());
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
		if (Input.GetKeyDown(KeyCode.H))
		{
			print("1");
			if (players.Count <= 0)
			{
				print("5");
				players = new List<SeeOtherPlayerScript>(FindObjectsOfType<SeeOtherPlayerScript>());
				for (int i = 0; i < players.Count; i++)
				{
					print("4");
					if (players[i]==this)
					{
						players.Remove(players[i]);
						print("3");
					}
				}
			}
			for (int i = 0; i < players.Count; i++)
			{
				Trigger(i);
				print("2");
			}
		}
	}
	public void Trigger(int i)
	{
		active = !active;

		players[i].GetComponent<Outline>().enabled = active;
	}
}
