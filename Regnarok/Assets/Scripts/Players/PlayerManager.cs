using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        GameObject game = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Character"), Vector3.zero, Quaternion.identity);
    }
}
