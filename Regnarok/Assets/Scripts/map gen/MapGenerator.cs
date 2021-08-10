using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // public
    public enum DrawMode { NoiseMap, ColourMap, Mesh, FalloffMap };
    public DrawMode drawMode;
    const int chuckSize = 241;
    [Range(0, 6)]
    public int editorPrevieuwLOD;
    public int ocataves;
    public int seed;
    [Range(0, 1)]
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
        float[,] noisemap = Noise.GenerateNoiseMap(chuckSize, chuckSize, seed, noiseScale, ocataves, presitance, lacunarity, offset);

        Color[] collorMap = new Color[chuckSize * chuckSize];
        for (int y = 0; y < chuckSize; y++)
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
                for (int i = 0; i < regions.Length; i++)
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
            display.DrawTexture(TextureGenerator.TextureFromColourMap(collorMap, chuckSize, chuckSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noisemap, meshHeightMultiplier, meshHeightCurve, editorPrevieuwLOD), TextureGenerator.TextureFromColourMap(collorMap, chuckSize, chuckSize));
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
}

[System.Serializable]
public struct TerrainType {
public string name;
public float baseStartHeight;
public float baseBlends;
public Color colour;
}