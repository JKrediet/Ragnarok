using System.Collections;
using UnityEngine;
public class EnviromentSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject firstObject;
    public GameObject secondObject;
    public GameObject thirdObject;
    public GameObject fourthObject;
    [Header("Settings")]
    public Transform firstPos;
    public Transform secondPos;
    [Space(5)]
    public bool randomRotFirst;
    public bool randomRotSecond;
    public bool randomRotThird;
    public bool randomRotFourth;
    [Space(5)]
    public int firstAmount;
    public int secondAmount;
    public int thirdAmount;
    public int fourthAmount;
    public float spawnHeight = 20;
    [Space(2)]
    public GameObject mesh;
    void Start()
    {
        mesh.AddComponent<MeshCollider>();
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
                        || hitInfo.transform.tag == "Chest"
                        || hitInfo.transform.tag == "Totem")
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
                        || hitInfo.transform.tag == "Chest"
                        || hitInfo.transform.tag == "Totem")
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
                        || hitInfo.transform.tag == "Chest"
                        || hitInfo.transform.tag == "Totem")
                    {

                    }
                    else
                    {
                        if (randomRotThird)
                        {
                            Instantiate(thirdObject, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform.parent);
                        }
                        else
                        {
                            Instantiate(thirdObject, spawnPoint, Quaternion.Euler(0, 0, 0), transform.parent);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < fourthAmount; i++)
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
                        || hitInfo.transform.tag == "Chest"
                        || hitInfo.transform.tag == "Totem")
                    {

                    }
                    else
                    {
                        if (randomRotFourth)
                        {
                            Instantiate(fourthObject, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform.parent);
                        }
                        else
                        {
                            Instantiate(fourthObject, spawnPoint, Quaternion.Euler(0, 0, 0), transform.parent);
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