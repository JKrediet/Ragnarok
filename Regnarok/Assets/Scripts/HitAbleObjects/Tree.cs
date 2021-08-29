using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class Tree : MonoBehaviour
{
    [SerializeField] float health, fallPower;
    float maxHealth;

    GameObject lastPlayerThatHitTree;
    Rigidbody rb;

    [SerializeField] GameObject treeparticle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    private void Start()
    {
        health *= Random.Range(0.7f, 1.3f);
        maxHealth = health;
    }
    public void HitByPlayer(float _damage, GameObject _hitBy, int itemType)
    {
        if (itemType == 1)
        {
            if (health > 0)
            {
                health = Mathf.Clamp(health -= _damage, 0, maxHealth);
                lastPlayerThatHitTree = _hitBy;
                if (health == 0)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    Debug.Log("Tree has been cut down!");
                    Vector3 fallDirection = new Vector3(transform.position.x - lastPlayerThatHitTree.transform.position.x, 0, transform.position.z - lastPlayerThatHitTree.transform.position.z);
                    rb.AddForce(fallDirection.normalized * fallPower);

                    //items
                    Invoke("DropItems", 10);
                }
            }
        }
    }
    void DropItems()
    {
        Instantiate(treeparticle, transform.position, Quaternion.identity);
        GetComponent<PhotonView>().RPC("DestroyWorldItem", RpcTarget.MasterClient);
    }
    [PunRPC]
    public void DestroyWorldItem()
    {
        PhotonNetwork.Destroy(gameObject);
        GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", $"Item" + 3), transform.position + transform.forward * 2, Quaternion.identity);
        droppedItem.GetComponent<Item>().ToInventory(false);
        droppedItem.GetComponent<Item>().stackAmount = Random.Range(1, 5);
    }
}
