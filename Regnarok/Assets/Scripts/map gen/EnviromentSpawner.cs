using System.Collections;
using UnityEngine;
public class EnviromentSpawner : MonoBehaviour
{
    public GameObject tree;
    public float minTreeSize;
    public float maxTreeSize;
    public Texture2D noiseImage;
    public float forestSize;
    public float treeDensity;

    private float baseDensity = 5.0f;

    // Use this for initialization
    void Start()
    {
        Generate();
    }

    public void Generate()
    {

        for (int y = 0; y < forestSize; y++)
        {

            for (int x = 0; x < forestSize; x++)
            {

                float chance = noiseImage.GetPixel(x, y).r / (baseDensity / treeDensity);

                if (ShouldPlaceTree(chance))
                {
                    float size = Random.Range(minTreeSize, maxTreeSize);

                    GameObject newTree = Instantiate(tree);
                    newTree.transform.localScale = Vector3.one * size;
                    newTree.transform.position = new Vector3(x, 0, y);
                    newTree.transform.parent = transform;
                }
            }
        }
    }

    //Returns true or false given some chance from 0 to 1
    public bool ShouldPlaceTree(float chance)
    {
        if (Random.Range(0.0f, 1.0f) <= chance)
        {
            return true;
        }
        return false;
    }
}





























    //public GameObject mesh;
    //[Space(5)]
    //public float maxTerrainheight;
    //public float minTerrainheight;
    //[Space(5)]
    //public float lastNoiseHeight;

    //[Header("Prefabs")]
    //public GameObject[] trees;
    //public GameObject[] rocks;
    //public GameObject[] chests;
    //private void MapEmbellishments(float noiseHeight, int posX, int posZ)
    //{
    //    Set min and max height of map for color gradient


    //    if (System.Math.Abs(lastNoiseHeight - noiseHeight) < 25)
    //        {
    //            Generate trees on vertices and times by scale of map
    //        var bottomOfTile = posX * mesh.transform.localScale.x - 25;
    //            var topOfTile = posX * mesh.transform.localScale.x + 25;
    //            var rightOfTile = posZ * mesh.transform.localScale.x - 25;
    //            var leftOfTile = posZ * mesh.transform.localScale.x + 25;

    //            var spawnAboveTerrainBy = noiseHeight + 25;

    //            for (var i = 0; i < 4; i++)
    //            {
    //                if (Random.Range(1, 4) == 1)
    //                {
    //                    GameObject objectToSpawn;
    //                    if (Random.Range(1, 5) == 1)
    //                    {
    //                        objectToSpawn = rocks[Random.Range(0, rocks.Length)];
    //                    }
    //                    else if (Random.Range(1, 5) == 2)
    //                    {
    //                        objectToSpawn = chests[Random.Range(0, chests.Length)];
    //                    }
    //                    else
    //                    {
    //                        objectToSpawn = trees[Random.Range(0, trees.Length)];
    //                    }

    //                    switch (i)
    //                    {
    //                        case 0:
    //                            Instantiate(objectToSpawn, new Vector3(topOfTile, spawnAboveTerrainBy, leftOfTile), Quaternion.identity);
    //                            break;
    //                        case 1:
    //                            Instantiate(objectToSpawn, new Vector3(topOfTile, spawnAboveTerrainBy, rightOfTile), Quaternion.identity);
    //                            break;
    //                        case 2:
    //                            Instantiate(objectToSpawn, new Vector3(bottomOfTile, spawnAboveTerrainBy, leftOfTile), Quaternion.identity);
    //                            break;
    //                        case 3:
    //                            Instantiate(objectToSpawn, new Vector3(bottomOfTile, spawnAboveTerrainBy, rightOfTile), Quaternion.identity);
    //                            break;
    //                        default:
    //                            break;
    //                    }
    //                }
    //            }

    //        }
    //    lastNoiseHeight = noiseHeight;
    //}
