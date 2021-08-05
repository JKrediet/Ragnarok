using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // public
    public enum DrawMode{NoiseMap,ColourMap,Mesh, FalloffMap };
    public DrawMode drawMode;
    const int chuckSize = 241;
    [Range(0,6)]
    public int editorPrevieuwLOD;
    public int ocataves;
    public int seed;
    [Range(0,1)]
    public float presitance;
    public float meshHeightMultiplier;
    public float lacunarity;
    public float noiseScale;
    public float[,] fallOffMap;
    public AnimationCurve meshHeightCurve;
    public Vector2 offset;
    public bool autoUpdate;
    public bool useFallOffs;
    public TerrainType[] regions;
    [Space(5)]
    public GameObject tree;
    public float minTreeSize;
    public float maxTreeSize;
    private Texture2D noiseImage;
    public float forestSize;
    public float treeDensity;

    private float baseDensity = 5.0f;
    private void Awake()
	{
        fallOffMap = FalloffGenerator.GenerateFalloffMap(chuckSize);
	}
    private void Start()
    {
        GenerateMap();
    }
    public void GenerateMap()
	{
        float[,] noisemap = Noise.GenerateNoiseMap(chuckSize,chuckSize,seed,noiseScale,ocataves,presitance,lacunarity, offset);

        Color[] collorMap = new Color[chuckSize * chuckSize];
        for(int y = 0; y < chuckSize; y++)
		{
            for (int x = 0; x < chuckSize; x++)
            {
                if (useFallOffs)
                {
					if (fallOffMap == null)
					{
                        fallOffMap = FalloffGenerator.GenerateFalloffMap(chuckSize);
                    }
                    noisemap[x, y] = Mathf.Clamp01(noisemap[x, y] - fallOffMap[x, y]);
                }
                float currentHeight = noisemap[x, y];
                for(int i=0;i < regions.Length; i++)
				{
					if (currentHeight <= regions[i].baseStartHeight)
					{
                        collorMap[y * chuckSize + x] = regions[i].colour;
                        break;
					}
				}
			}
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
		if (drawMode == DrawMode.NoiseMap)
		{
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noisemap));
		}
        else if (drawMode == DrawMode.ColourMap)
		{
            display.DrawTexture(TextureGenerator.TextureFromColourMap(collorMap,chuckSize,chuckSize));
        }
        else if (drawMode == DrawMode.Mesh)
		{
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noisemap, meshHeightMultiplier, meshHeightCurve,editorPrevieuwLOD), TextureGenerator.TextureFromColourMap(collorMap, chuckSize, chuckSize));
        }
        else if (drawMode == DrawMode.FalloffMap) 
        {
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(chuckSize)));
		}

     
    }
	private void OnValidate()
	{
		if (lacunarity < 1)
		{
            lacunarity = 1;
		}
		if (ocataves < 0)
		{
            ocataves = 0;
		}
	}

    // Use this for initialization
    public void GenerateTrees(Texture2D noiseValue)
    {
        print("3");
        noiseImage = noiseValue;
        for (int y = 0; y < forestSize; y++)
        {
            for (int x = 0; x < forestSize; x++)
            {
    
                float chance = noiseImage.GetPixel(x, y).r / (baseDensity / treeDensity);
                print(chance);
                if (chance == 0)
                {
                    GenerateTrees(noiseValue);
                    return;
                }
                if (ShouldPlaceTree(chance))
                {
                    print("1");
                    float size = Random.Range(minTreeSize, maxTreeSize);

                    GameObject newTree = Instantiate(tree);
                    newTree.transform.localScale = Vector3.one * size;
                    newTree.transform.position = new Vector3(x, 50, y);
                }

            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawTexture(noiseValue);
        print("5");
    }

    //Returns true or false given some chance from 0 to 1
    public bool ShouldPlaceTree(float chance)
    {
        float randomNumb = Random.Range(0.0f, 1.0f);
        print(randomNumb);
        print("7");
        if (randomNumb <= chance)
        {
            print("6");
            return true;
        }
        return false;
    }
}

[System.Serializable]
public struct TerrainType {
public string name;
public float baseStartHeight;
public float baseBlends;
public Color colour;
}