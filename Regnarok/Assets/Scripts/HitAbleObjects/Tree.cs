using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] float health, fallPower;
    float maxHealth;

    GameObject lastPlayerThatHitTree;
    Rigidbody rb;

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
                }
            }
        }
    }
}
