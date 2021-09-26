using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Photon.Pun;
using System.IO;
using System.Collections.Generic;

public class EnviromentSpawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    public float spawnCoolDownForEachSpawn = 0.00001f;
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
    public Transform grassHolder;
    [Space(2)]
    public GameObject mesh;
    public GameObject grassMesh;
    public MapGenerator mapGen;
    [Space(5)]
    public float minGrassHeight=1.5f;
    public float maxGrassHeight = 8f;
    private Vector3 spawnPoint;
    int serialNumberForHitableObjectsl = 0;

    public void StartGenerating()
    {
        StartCoroutine(Generate());
    }
    public IEnumerator Generate()
    {

        Random.InitState(mapGen.mapSeed);
        mesh.AddComponent<MeshCollider>();
        new WaitForSeconds(1);
        for (int i = 0; i < spawnItems.Length; i++)
        {
            for (int i_ = 0; i_ < spawnItems[i].amountToSpawn; i_++)
            {
                if (spawnItems[i].spawnItem)
                {
                    if (Chance())
                    {
                        Transform parent;
                        if (spawnItems[i].isGrass)
                        {
                            parent = grassHolder;
                        }
                        else
                        {
                            parent = transform;

                        }
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
                            else if (hitInfo.transform.tag == "Mesh")
                            {
                                if (spawnItems[i].canSpawnOnSand)
                                {
                                    if (hitInfo.point.y <= minMountenHeight)
                                    {
                                        if (spawnItems[i].randomRot)
                                        {
                                            InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), parent, i);
                                        }
                                        else
                                        {
                                            if (spawnItems[i].rotateWithMesh)
                                            {
                                                InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), parent, i);
                                            }
                                            else
                                            {
                                                InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, parent, i);
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
                                            InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), parent, i);
                                        }
                                        else
                                        {
                                            if (spawnItems[i].rotateWithMesh)
                                            {
                                                InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), parent, i);
                                            }
                                            else
                                            {
                                                InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, parent, i);
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
                                            InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), parent, i);
                                        }
                                        else
                                        {
                                            if (spawnItems[i].rotateWithMesh)
                                            {
                                                InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), parent, i);
                                            }
                                            else
                                            {
                                                InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, parent, i);
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
                                            InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), parent, i);
                                        }
                                        else
                                        {
                                            if (spawnItems[i].rotateWithMesh)
                                            {
                                                InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), parent, i);
                                            }
                                            else
                                            {
                                                InstatiateEnviorment(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, parent, i);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    new WaitForSeconds(timeBetweenSpawns);
                }
            }
            yield return new WaitForSeconds(spawnCoolDownForEachSpawn);
        }
        Invoke("BuildNavMesh", 0.5f);
    }
    public void InstatiateEnviorment(GameObject toSpawn, Vector3 location, Quaternion rotation, Transform parent, int i)
    {
        if (spawnItems[i].spawnWithPhoton)
        {
            GameObject tempObject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", spawnItems[i].name), location, rotation);
            tempObject.transform.parent = parent;
        }
        else
        {
            GameObject tempObject = Instantiate(toSpawn, location, rotation, parent);

            if (tempObject.GetComponent<HitableObject>())
            {
                tempObject.GetComponent<HitableObject>().itemSerialNumber = serialNumberForHitableObjectsl;
                serialNumberForHitableObjectsl++;
            }
            else if (tempObject.GetComponent<ItemPickUp>())
            {
                tempObject.GetComponent<ItemPickUp>().itemSerialNumber = serialNumberForHitableObjectsl;
                serialNumberForHitableObjectsl++;
            }
        }
    }
    public void BuildNavMesh()
    {
        if (canBakeNav)
        {
            mesh.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
        SpawnPlayers();
    }
    public void AddGrass()
    {
		grassMesh.GetComponent<MeshFilter>().mesh = mesh.GetComponent<MeshFilter>().mesh;
		Mesh meshGrass = grassMesh.GetComponent<MeshFilter>().mesh;
		int[] triangles = meshGrass.triangles;
		Vector3[] vertices = meshGrass.vertices;
		Vector2[] uv = meshGrass.uv;
		Vector3[] normals = meshGrass.normals;
		List<Vector3> vertList = new List<Vector3>();
		List<Vector2> uvList = new List<Vector2>();
		List<Vector3> normalsList = new List<Vector3>();
		List<int> trianglesList = new List<int>();


		int i = 0;
		while (i < grassMesh.GetComponent<MeshFilter>().mesh.vertices.Length)
		{
			vertList.Add(vertices[i]);
			uvList.Add(uv[i]);
			normalsList.Add(normals[i]);
			i++;
            new WaitForSeconds(0.01f);
        }
		for (int triCount = 0; triCount < triangles.Length; triCount += 3)
		{
			if ((transform.TransformPoint(vertices[triangles[triCount]]).y > minGrassHeight) &&
				(transform.TransformPoint(vertices[triangles[triCount + 1]]).y > minGrassHeight) &&
				(transform.TransformPoint(vertices[triangles[triCount + 2]]).y > minGrassHeight))
			{
				if ((transform.TransformPoint(vertices[triangles[triCount]]).y < maxGrassHeight) &&
				(transform.TransformPoint(vertices[triangles[triCount + 1]]).y < maxGrassHeight) &&
				(transform.TransformPoint(vertices[triangles[triCount + 2]]).y < maxGrassHeight))
				{
					trianglesList.Add(triangles[triCount]);
					trianglesList.Add(triangles[triCount + 1]);
					trianglesList.Add(triangles[triCount + 2]);

					new WaitForSeconds(0.45f);
				}
			}
		}

            
		triangles = trianglesList.ToArray();
		vertices = vertList.ToArray();
		uv = uvList.ToArray();
		normals = normalsList.ToArray();
		grassMesh.GetComponent<MeshFilter>().mesh.triangles = triangles;
		grassMesh.GetComponent<MeshFilter>().mesh.vertices = vertices;
		grassMesh.GetComponent<MeshFilter>().mesh.uv = uv;
		grassMesh.GetComponent<MeshFilter>().mesh.normals = normals;
		grassMesh.SetActive(true);
	}
    public void SpawnPlayers()
	{
        new WaitForSeconds(5);
        FindObjectOfType<GameManager>().SpawnPlayers();
        print("Spawnerd");
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
        public bool spawnItem;
        public bool spawnWithPhoton;
        public bool isGrass;
        public string name;
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