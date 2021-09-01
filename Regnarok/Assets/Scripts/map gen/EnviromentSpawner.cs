using System.Collections;
using UnityEngine;
//using UnityEditor.AI;
public class EnviromentSpawner : MonoBehaviour
{
    public bool testing;
    public bool testEnvSpawn;
    public Objects[] spawnItems;
    [Header("Settings")]
    public Transform firstPos;
    public Transform secondPos;
    public Transform firstPosInner;
    public Transform secondPosInner;
    [Space(2)]
    public int sandIndex=0;
    [Space(2)]
    public GameObject mesh;
    public MapGenerator mapGen;
    private Vector3 spawnPoint;
    void Start()
    {
        Random.seed = mapGen.seed;
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
            //NavMeshBuilder.BuildNavMesh();
        }
    }
    public void Generate()
    {
		for (int i = 0; i < spawnItems.Length; i++)
		{
			for (int i_ = 0; i_ < spawnItems[i].amountToSpawn; i_++)
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

                        //checking collor
                        

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
                                if (spawnItems[i].randomRot)
                                {
                                    Instantiate(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                                }
                                else
                                {
                                    if (spawnItems[i].rotateWithMesh)
                                    {
                                        Instantiate(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), transform);
                                    }
                                    else
                                    {
                                        Instantiate(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, transform);
                                    }
                                }
                            }
                            else if (spawnItems[i].onlySpawnOnSand)
                            {
                                Renderer renderer = hitInfo.collider.GetComponent<MeshRenderer>();
                                Texture2D texture2D = renderer.material.mainTexture as Texture2D;
                                if (texture2D != null)
                                {
                                    Vector2 pCoord = hitInfo.textureCoord;
                                    pCoord.x *= texture2D.width;
                                    pCoord.y *= texture2D.height;
                                    Vector2 tiling = renderer.material.mainTextureScale;
                                    Color color = texture2D.GetPixel(Mathf.FloorToInt(pCoord.x * tiling.x), Mathf.FloorToInt(pCoord.y * tiling.y));

                                    if (color == mapGen.regions[sandIndex].colour)
                                    {
                                        if (spawnItems[i].randomRot)
                                        {
                                            Instantiate(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                                        }
                                        else
                                        {
                                            if (spawnItems[i].rotateWithMesh)
                                            {
                                                Instantiate(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), transform);
                                            }
                                            else
                                            {
                                                Instantiate(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, transform);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Renderer renderer = hitInfo.collider.GetComponent<MeshRenderer>();
                                Texture2D texture2D = renderer.material.mainTexture as Texture2D;
                                if (texture2D != null)
                                {
                                    Vector2 pCoord = hitInfo.textureCoord;
                                    pCoord.x *= texture2D.width;
                                    pCoord.y *= texture2D.height;
                                    Vector2 tiling = renderer.material.mainTextureScale;
                                    Color color = texture2D.GetPixel(Mathf.FloorToInt(pCoord.x * tiling.x), Mathf.FloorToInt(pCoord.y * tiling.y));

                                    if (color == mapGen.regions[sandIndex].colour)
                                    {

                                    }
                                    else
                                    {
                                        if (spawnItems[i].randomRot)
                                        {
                                            Instantiate(spawnItems[i].toSpawn, spawnPoint, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                                        }
                                        else
                                        {
                                            if (spawnItems[i].rotateWithMesh)
                                            {
                                                Instantiate(spawnItems[i].toSpawn, spawnPoint, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), transform);
                                            }
                                            else
                                            {
                                                Instantiate(spawnItems[i].toSpawn, spawnPoint, Quaternion.identity, transform);
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
        public GameObject toSpawn;
        public float startHeight;
        public int amountToSpawn;
        public bool canSpawnOnSand;
        public bool randomRot;
        public bool rotateWithMesh;
        public bool innerCircle;
        public bool onlySpawnOnSand;
    }
}