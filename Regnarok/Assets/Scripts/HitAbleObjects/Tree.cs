using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class Tree : HitableObject
{
    [SerializeField] float fallPower;
    [SerializeField] protected GameObject treeparticle;
    public override void HitByPlayer(float _damage, GameObject _hitBy, int itemType)
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
    protected override void DropItems()
    {
        Instantiate(treeparticle, transform.position, Quaternion.identity);
        base.DropItems();
    }
}
