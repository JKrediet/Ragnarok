using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviormentSpawnBehavour : MonoBehaviour
{
    public LayerMask spawnLayer;
    public float heightOffset = 1f;
    private int replace;
    void Start()
    {
        FindLand();
    }
    public void FindLand()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.transform != transform)
            {
                if (hitInfo.point != transform.position)
                {
                    transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y - heightOffset, hitInfo.point.z);
                }
            }
        }
        else
        {
            ray = new Ray(transform.position, transform.up);
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform != transform)
                {
                    transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y - heightOffset, hitInfo.point.z);
                }
            }
        }
        if (replace <= 3)
        {
            replace++;
            Invoke("FindLand", 0.15f);
        }
    }
}