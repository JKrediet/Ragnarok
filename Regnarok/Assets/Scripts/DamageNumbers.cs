using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumbers : MonoBehaviour
{
    public float damageAmount;

    //privates
    private TextMeshPro textMesh;
    private float disappearTime;
    private Color textColor;
    private Transform player;

    private void Start()
    {
        player = FindObjectOfType<PlayerHealth>().transform;
        textMesh = GetComponent<TextMeshPro>();
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        disappearTime = 1f;
    }
    void Update()
    {
        transform.LookAt(player);
        transform.rotation = Quaternion.LookRotation(player.transform.forward);
        float moveYSpeed = 2;
        transform.position += new Vector3(0, moveYSpeed, 0) * Time.deltaTime;

        disappearTime -= Time.deltaTime;
        if (disappearTime < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}