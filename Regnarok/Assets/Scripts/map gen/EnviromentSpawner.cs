using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class EnviromentSpawner : MonoBehaviour
{
    public bool testing;
    public bool testEnvSpawn;
    public bool canBakeNav;
    public Objects[] spawnItems;
    [Header("Height Values")]
    public float maxSandHeight;
    public float minMountenHeight;
    [Header("Settings")]
    public Transform firstPos;
    public Transform secondPos;
    public Transform firstPosInner;
    public Transform secondPosInner;
    [Space(2)]
    public GameObject mesh;
    public MapGenerator mapGen;
    private Vector3 spawnPoint;
    private PhotonView pv;
    void Start()
    {
        pv = GetComponent<PhotonView>();    
        Random.seed = mapGen.mapSeed;
    }
    public void Generate()
    {
        if (pv.Owner == PhotonNetwork.MasterClient)
        {
            mesh.AddComponent<MeshCollider>();
            for (int i = 0; i < spawnItems.Length; i++)
            {
                for (int i_ = 0; i_ < spawnItems[i].amountToSpawn; i_++)
                {
                    if (spawnItems[i].spawnItem)
                    {
                        if (Chance())
                        {
                            if (spawnItems[i].innerCircle)
                            {
                                spawnPoint = new Vector3(Random.Range(firstPosInner.position.x, secondPosInner.position.x), spawnItems[i].startHeight, Random.Range(firstPosInner.position.z, secondPosInner.position.z));
                            }
                            else
                            {
                                spawnPoint = new Vector3(Random.Range(firstPos.position.x, secondPos.position.x), spawnItems[i].startHeight, Random.Range(firstPos.position.z, secondPos.position.z));
                            }
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
                                    if (spawnItems[i].canSpawnOnSand)
                                    {
                                        if (hitInfo.point.y <= minMountenHeight)
                                        {
                                            if (spawnItems[i].randomRot)
                                            {
                                                SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), spawnItems[i].PrefabName);
                                            }
                                            else
                                            {
                                                if (spawnItems[i].rotateWithMesh)
                                                {
                                                    SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), spawnItems[i].PrefabName);
                                                }
                                                else
                                                {
                                                    SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, spawnItems[i].PrefabName);
                                                }
                                            }
                                        }
                                    }
                                    else if (spawnItems[i].onlySpawnOnSand)
                                    {
                                        if (hitInfo.point.y <= maxSandHeight)
                                        {
                                            if (spawnItems[i].randomRot)
                                            {
                                                SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), spawnItems[i].PrefabName);
                                            }
                                            else
                                            {
                                                if (spawnItems[i].rotateWithMesh)
                                                {
                                                    SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), spawnItems[i].PrefabName);
                                                }
                                                else
                                                {
                                                    SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, spawnItems[i].PrefabName);
                                                }
                                            }
                                        }
                                    }
                                    else if (spawnItems[i].onlySpawnOnMountenTop)
                                    {
                                        if (hitInfo.point.y >= minMountenHeight)
                                        {
                                            if (spawnItems[i].randomRot)
                                            {
                                                SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), spawnItems[i].PrefabName);
                                            }
                                            else
                                            {
                                                if (spawnItems[i].rotateWithMesh)
                                                {
                                                    SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), spawnItems[i].PrefabName);
                                                }
                                                else
                                                {
                                                    SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, spawnItems[i].PrefabName);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (hitInfo.point.y >= maxSandHeight && hitInfo.point.y <= minMountenHeight)
                                        {
                                            if (spawnItems[i].randomRot)
                                            {
                                                SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), spawnItems[i].PrefabName);
                                            }
                                            else
                                            {
                                                if (spawnItems[i].rotateWithMesh)
                                                {
                                                    SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), spawnItems[i].PrefabName);
                                                }
                                                else
                                                {
                                                    SpawnEnvironment(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, spawnItems[i].PrefabName);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        Invoke("BuildNavMesh", 0.5f);
        }
    }
    public void SpawnEnvironment(GameObject SpawnObject,Vector3 spawnPoint,Quaternion rotation,string ObjectName)
	{
        //PhotonNetwork.Instantiate(ObjectName, spawnPoint, rotation
        Instantiate(SpawnObject, spawnPoint, rotation, transform);
    }
    public void BuildNavMesh()
    {
		if (canBakeNav)
        { 
           // NavMeshBuilder.BuildNavMesh();
		}
        Invoke("SpawnPlayers", 1);
    }
    public void SpawnPlayers()
	{
        FindObjectOfType<GameManager>().SpawnPlayers();
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
    [System.Serializable]
    public struct Objects
    {
        public string PrefabName;
        public bool spawnItem;
        public GameObject toSpawn;
        public float startHeight;
        public int amountToSpawn;
        public bool canSpawnOnSand;
        public bool randomRot;
        public bool rotateWithMesh;
        public bool innerCircle;
        public bool onlySpawnOnSand;
        public bool onlySpawnOnMountenTop;
    }
}