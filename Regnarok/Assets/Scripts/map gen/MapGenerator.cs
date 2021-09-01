using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // public
    [Header("gradient")]
    public Gradient gradient;
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
    public GameObject terrainObject;
    public MeshCollider meshCol;
    public TerrainType[] regions;
    private Color[] colors;
    private float minTerrainheight;
    private float maxTerrainheight;
    private Mesh terrainMesh;
    private void Awake()
    {
        fallOffMap = FalloffGenerator.GenerateFalloffMap(chuckSize);
    }
    private void Start()
    {
        terrainMesh = terrainObject.GetComponent<MeshFilter>().mesh;
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

        //set
		//for (int i = 0; i < terrainMesh.vertices.Length; i++)
		//{
  //          SetMinMaxHeights(terrainMesh.vertices[i].y);
  //      }
        //collor

        //ColorMap();
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
    private void SetMinMaxHeights(float noiseHeight)
    {
        // Set min and max height of map for color gradient
        if (noiseHeight > maxTerrainheight)
            maxTerrainheight = noiseHeight;
        if (noiseHeight < minTerrainheight)
            minTerrainheight = noiseHeight;
    }
    private void ColorMap()
    {
        colors = new Color[terrainMesh.vertices.Length];
        for (int i = 0, z = 0; z < terrainMesh.vertices.Length; z++)
        {

            float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, terrainMesh.vertices[i].y);
            colors[i] = gradient.Evaluate(height);
            i++;
        }
        print(colors.Length);
        print(terrainMesh.colors.Length);
        print(terrainMesh.vertices.Length);
        Mesh hold = terrainMesh;
        terrainMesh.Clear();
        terrainMesh.vertices = hold.vertices;
        terrainMesh.triangles = hold.triangles;
        terrainMesh.colors = new Color[terrainMesh.vertices.Length];
        terrainMesh.colors = colors;
        //colors.CopyTo(terrainMesh.colors, 0);
        //terrainMesh.colors[0] += colors[0];
        terrainMesh.RecalculateNormals();
        meshCol.sharedMesh = terrainMesh;
    }
}

[System.Serializable]
public struct TerrainType {
public string name;
public float baseStartHeight;
public float baseBlends;
public Color colour;
}