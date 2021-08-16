using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    private void Awake()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        GameObject game = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Character"), Vector3.zero, Quaternion.identity);
    }
}
