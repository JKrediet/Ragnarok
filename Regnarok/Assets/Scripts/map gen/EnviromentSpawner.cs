using System.Collections;
using UnityEngine;
public class EnviromentSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject firstObject;
    public GameObject secondObject;
    public GameObject thirdObject;
    [Header("Settings")]
    public Transform firstPos;
    public Transform secondPos;
    [Space(5)]
    public bool randomRotFirst;
    public bool randomRotSecond;
    public bool randomRotThird;
    [Space(5)]
    public int firstAmount;
    public int secondAmount;
    public int thirdAmount;
    public float spawnHeight = 20;
    void Start()
    {
        Generate();
    }
    public void Generate()
    {
        for (int i = 0; i < firstAmount; i++)
        {
            if (Chance())
            {
                Vector3 spawnPoint = new Vector3(Random.Range(firstPos.position.x, secondPos.position.x), spawnHeight, Random.Range(firstPos.position.z, secondPos.position.z));
                Ray ray = new Ray(spawnPoint, -transform.up);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.transform.tag== "Water"
                        || hitInfo.transform.tag == "Rock"
                        || hitInfo.transform.tag == "Tree"
                        || hitInfo.transform.tag == "Chest")
                    {
       
                    }
                    else
                    {
                        if (randomRotFirst)
                        {
                            Instantiate(firstObject, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform.parent);
                        }
                        else
                        {
                            Instantiate(firstObject, spawnPoint, Quaternion.identity, transform.parent);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < secondAmount; i++)
        {
            if (Chance())
            {
                Vector3 spawnPoint = new Vector3(Random.Range(firstPos.position.x, secondPos.position.x), spawnHeight, Random.Range(firstPos.position.z, secondPos.position.z));
                Ray ray = new Ray(spawnPoint, -transform.up);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.transform.tag == "Water"
                        || hitInfo.transform.tag == "Rock"
                        || hitInfo.transform.tag == "Tree"
                        || hitInfo.transform.tag == "Chest")
                    {

                    }
                    else
                    {
                        if (randomRotSecond)
                        {
                            Instantiate(secondObject, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform.parent);
                        }
                        else
                        {
                            Instantiate(secondObject, spawnPoint, Quaternion.identity,transform.parent);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < thirdAmount; i++)
        {
            if (Chance())
            {
                Vector3 spawnPoint = new Vector3(Random.Range(firstPos.position.x, secondPos.position.x), spawnHeight, Random.Range(firstPos.position.z, secondPos.position.z));
                Ray ray = new Ray(spawnPoint, -transform.up);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.transform.tag == "Water"
                        || hitInfo.transform.tag == "Rock"
                        || hitInfo.transform.tag == "Tree"
                        || hitInfo.transform.tag == "Chest")
                    {

                    }
                    else
                    {
                        if (randomRotThird)
                        {
                            Instantiate(thirdObject, spawnPoint, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), transform.parent);
                        }
                        else
                        {
                            Instantiate(thirdObject, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform.parent);
                        }
                    }
                }
            }
        }
    }
    public bool Chance()
    {
        if (Random.Range(0.00f,5.00f) <= 4.00f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}