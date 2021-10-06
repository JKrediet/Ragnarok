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
			players.Clear();

			players = new List<SeeOtherPlayerScript>(FindObjectsOfType<SeeOtherPlayerScript>());
			for (int i = 0; i < players.Count; i++)
			{
				if (players[i]==this)
				{
					players.Remove(players[i]);
				}
			}
			
			for (int i = 0; i < players.Count; i++)
			{
				Trigger(i);
			}
		}
	}
	public void Trigger(int i)
	{
		active = !active;

		players[i].GetComponent<Outline>().enabled = active;
	}
}
