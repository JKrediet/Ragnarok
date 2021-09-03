using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapGenerator : MonoBehaviour
{
    // public
    public Gradient gradient;
    const int chuckSize = 241;
    [Range(0, 6)]
    public int editorPrevieuwLOD;
    public int ocataves;
    public int mapSeed;
    [Range(0, 1)]
    public float presitance;
    public float meshHeightMultiplier;
    public float lacunarity;
    public float noiseScale;
    public float[,] fallOffMap;
    public float loadAmount;
    public AnimationCurve meshHeightCurve;
    public Vector2 offset;
    public bool autoUpdate;
    public bool useFallOffs;
    public GameObject terrainObject;
    public MeshCollider meshCol;
    public EnviromentSpawner envSpawn;
    private Color[] colors;
    private float minTerrainheight;
    private float maxTerrainheight;
    private Mesh terrainMesh;
    
    public void StartGenerating(int seed)
	{
        mapSeed = seed;
        loadAmount = 0;
        GenerateMap();
    }
    public void GenerateMap()
    {
        terrainMesh = terrainObject.GetComponent<MeshFilter>().mesh;
        fallOffMap = FalloffGenerator.GenerateFalloffMap(chuckSize);
        float[,] noisemap = Noise.GenerateNoiseMap(chuckSize, chuckSize, mapSeed, noiseScale, ocataves, presitance, lacunarity, offset);

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
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noisemap, meshHeightMultiplier, meshHeightCurve, editorPrevieuwLOD), TextureGenerator.TextureFromColourMap(collorMap, chuckSize, chuckSize));
        AddLoadAmount();

        ColorMap();
        //Invoke("StartEnvSpawner", 1);
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
		Mesh mesh = terrainObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[vertices.Length];
        print(vertices);
        print(colors.Length);
        print(vertices.Length);
        AddLoadAmount();
        for (int i = 0; i < vertices.Length; i++)
        {
            SetMinMaxHeights(vertices[i].y);
            float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, vertices[i].y);
            colors[i] = gradient.Evaluate(height);
        }
        mesh.colors = colors;
        AddLoadAmount();
        Invoke("StartEnvSpawner", 1);
    }
    public void StartEnvSpawner()
	{
        envSpawn.Generate();
    }
    public void AddLoadAmount()
	{
        loadAmount += 20;
    }
}