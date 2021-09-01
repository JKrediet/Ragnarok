using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class HitableObject : MonoBehaviour
{
    [SerializeField] protected float health, itemTypeNeeded, minDrop, maxDrop, itemId;
    protected float maxHealth;

    protected GameObject lastPlayerThatHitTree;
    protected Rigidbody rb;

    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    protected void Start()
    {
        health *= Random.Range(0.7f, 1.3f);
        maxHealth = health;
    }
    public virtual void HitByPlayer(float _damage, GameObject _hitBy, int itemType)
    {
        if (itemType == itemTypeNeeded)
        {
            if (health > 0)
            {
                health = Mathf.Clamp(health -= _damage, 0, maxHealth);
                lastPlayerThatHitTree = _hitBy;
                if (health == 0)
                {
                    //items
                    Invoke("DropItems", 2);
                }
            }
        }
    }
    protected virtual void DropItems()
    {
        GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient);
    }
    [PunRPC]
    public void DestroyWorldItem()
    {
        PhotonNetwork.Destroy(gameObject);
        GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"Item" + itemId), transform.position + transform.forward * 2, Quaternion.identity);
        droppedItem.GetComponent<Item>().ToInventory(false);
        droppedItem.GetComponent<Item>().stackAmount = Random.Range((int)minDrop, (int)maxDrop);
    }
}
