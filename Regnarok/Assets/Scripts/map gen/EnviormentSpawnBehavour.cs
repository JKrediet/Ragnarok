using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviormentSpawnBehavour : MonoBehaviour
{
    public float heightOffset = 1f;
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
            transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y - heightOffset, hitInfo.point.z);
        }
        else
        {
            ray = new Ray(transform.position, transform.up);
            if (Physics.Raycast(ray, out hitInfo))
            {
                transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y - heightOffset, hitInfo.point.z);
            }
        }
    }
}