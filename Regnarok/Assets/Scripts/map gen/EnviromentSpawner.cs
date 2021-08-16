using System.Collections;
using UnityEngine;
using UnityEditor.AI;
public class EnviromentSpawner : MonoBehaviour
{
    public bool testing;
    public bool testEnvSpawn;
    [Header("Prefabs")]
    public GameObject trees;
    public GameObject rocks;
    public GameObject chest;
    public GameObject totem;
    [Header("Settings")]
    public Transform firstPos;
    public Transform secondPos;
    [Space(5)]
    public bool randomRotTrees;
    public bool randomRotRocks;
    public bool randomRotchest;
    public bool randomRotTotem;
    [Space(5)]
    public int treeAmount;
    public int rockAmount;
    public int chestAmount;
    public int totemAmount;
    public float spawnHeight = 20;
    [Space(2)]
    public int sandIndex=0;
    [Space(2)]
    public GameObject mesh;
    public MapGenerator mapGen;
    void Start()
    {
        mesh.AddComponent<MeshCollider>();
        if (testing)
        {

        }
        else if (testEnvSpawn)
        {
            Generate();
        }
        else
        {
            Generate();
            NavMeshBuilder.BuildNavMesh();
        }
    }
    public void Generate()
    {
        for (int i = 0; i < treeAmount; i++)
        {
            if (Chance())
            {
                Vector3 spawnPoint = new Vector3(Random.Range(firstPos.position.x, secondPos.position.x), spawnHeight, Random.Range(firstPos.position.z, secondPos.position.z));
                Ray ray = new Ray(spawnPoint, -transform.up);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {

                    //checking collor
                    Renderer renderer = hitInfo.collider.GetComponent<MeshRenderer>();
                    Texture2D texture2D = renderer.material.mainTexture as Texture2D;
                    if (texture2D != null)
                    {
                        Vector2 pCoord = hitInfo.textureCoord;
                        pCoord.x *= texture2D.width;
                        pCoord.y *= texture2D.height;
                        Vector2 tiling = renderer.material.mainTextureScale;
                        Color color = texture2D.GetPixel(Mathf.FloorToInt(pCoord.x * tiling.x), Mathf.FloorToInt(pCoord.y * tiling.y));
                    
                    
                        if (hitInfo.transform.tag== "Water"
                            || hitInfo.transform.tag == "Rock"
                            || hitInfo.transform.tag == "Tree"
                            || hitInfo.transform.tag == "Chest"
                            || hitInfo.transform.tag == "Totem"
                            || color == mapGen.regions[sandIndex].colour)
                        {
       
                        }
                    else
                    {
                        if (randomRotTrees)
                        {
                            Instantiate(trees, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform.parent);
                        }
                        else
                        {
                            Instantiate(trees, spawnPoint, Quaternion.identity, transform.parent);
                        }
                    }
                    }
                }
            }
        }
        for (int i = 0; i < rockAmount; i++)
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
                        if (randomRotRocks)
                        {
                            Instantiate(rocks, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform.parent);
                        }
                        else
                        {
                            Instantiate(rocks, spawnPoint, Quaternion.identity,transform.parent);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < chestAmount; i++)
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
                        if (randomRotchest)
                        {
                            Instantiate(chest, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform.parent);
                        }
                        else
                        {
                            Instantiate(chest, spawnPoint, Quaternion.Euler(0, 0, 0), transform.parent);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < totemAmount; i++)
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
                        if (randomRotTotem)
                        {
                            Instantiate(totem, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform.parent);
                        }
                        else
                        {
                            Instantiate(totem, spawnPoint, Quaternion.Euler(0, 0, 0), transform.parent);
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