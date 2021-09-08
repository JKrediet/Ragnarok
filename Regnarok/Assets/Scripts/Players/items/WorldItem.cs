using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class WorldItem : MonoBehaviour
{
    public string itemName;
    public Sprite itemImage;
    public int itemAmount;

    public bool mayBePickedUp;

    public EquipmentType equipment;

    private void Start()
    {
        mayBePickedUp = false;
        Invoke("Cooldown", 0.5f);
    }
    public void Cooldown()
    {
        mayBePickedUp = true;
    }
    protected void OnTriggerEnter(Collider other)
    {
        if (mayBePickedUp)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<CharacterStats>().CreateItem(itemName, itemAmount, itemImage, equipment);
                if (GetComponent<PhotonView>().Owner != PhotonNetwork.MasterClient)
                {
                    GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.MasterClient);
                }
                GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient);
                Invoke("SecLater", 0.1f);
            }
        }
    }
    void SecLater()
    {
        GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient);
    }
    [PunRPC]
    public void DestroyWorldItem()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
