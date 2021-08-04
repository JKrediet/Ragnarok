//using System.Collections;
//using UnityEngine;
//public class EnviromentSpawner : MonoBehaviour
//{
//    private void MapEmbellishments(float noiseHeight, int posX, int posZ)
//    {
//        // Set min and max height of map for color gradient
//        if (noiseHeight > maxTerrainheight)
//            maxTerrainheight = noiseHeight;
//        if (noiseHeight < minTerrainheight)
//            minTerrainheight = noiseHeight;

//        if (System.Math.Abs(lastNoiseHeight - noiseHeight) < 25)
//        {
//            // Generate trees on vertices and times by scale of map
//            var bottomOfTile = posX * MESH_SCALE - 25;
//            var topOfTile = posX * MESH_SCALE + 25;
//            var rightOfTile = posZ * MESH_SCALE - 25;
//            var leftOfTile = posZ * MESH_SCALE + 25;

//            var spawnAboveTerrainBy = noiseHeight + 25;
//            if (noiseHeight > minTreeNoiseHeight && noiseHeight < maxTreeNoiseHeight)
//            {
//                for (var i = 0; i < 4; i++)
//                {
//                    if (Random.Range(1, 4) == 1)
//                    {
//                        GameObject objectToSpawn;
//                        if (Random.Range(1, 5) == 1)
//                        {
//                            objectToSpawn = rocks[Random.Range(0, rocks.Length)];
//                        }
//                        else
//                        {
//                            objectToSpawn = trees[Random.Range(0, trees.Length)];
//                        }

//                        switch (i)
//                        {
//                            case 0:
//                                Instantiate(objectToSpawn, new Vector3(topOfTile, spawnAboveTerrainBy, leftOfTile), Quaternion.identity);
//                                break;
//                            case 1:
//                                Instantiate(objectToSpawn, new Vector3(topOfTile, spawnAboveTerrainBy, rightOfTile), Quaternion.identity);
//                                break;
//                            case 2:
//                                Instantiate(objectToSpawn, new Vector3(bottomOfTile, spawnAboveTerrainBy, leftOfTile), Quaternion.identity);
//                                break;
//                            case 3:
//                                Instantiate(objectToSpawn, new Vector3(bottomOfTile, spawnAboveTerrainBy, rightOfTile), Quaternion.identity);
//                                break;
//                            default:
//                                break;
//                        }
//                    }
//                }
//            }
//        }
//        lastNoiseHeight = noiseHeight;
//    }
//}
