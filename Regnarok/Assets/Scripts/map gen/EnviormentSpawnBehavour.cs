using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviormentSpawnBehavour : MonoBehaviour
{
    public bool secondCheck;
    public bool isGrass;
    public float heightOffset = 1f;
    public float turnOffRb=1f;
    public bool usesRbFall;
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
                    if (isGrass)
                    {
                        transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                    }
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
                if (isGrass)
                {
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                }
            }
        }
        if (secondCheck)
        {
            Invoke("FindLand", 0.15f);
            secondCheck = false;
        }
		if (usesRbFall)
		{
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
            Invoke("TurnOffRB",turnOffRb);
        }
    }
    public void TurnOffRB()
	{
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }
}