﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainVisualization
{
    Height,
    Heat,
    Moisture
}

public class TileGenerator : MonoBehaviour
{
    [Header("Parameters")]
    public int noiseSampleSize;
    public float scale;
    public float maxHeight = 1.0f;
    [Range(1, 50)]
    public int textureResolution = 1;
    public TerrainVisualization visualizationType;
    
    [HideInInspector]
    public Vector2 offset;

    [Header("TerrainTypes")]
    public TerrainType[] heightTerrainTypes;
    public TerrainType[] heatTerrainTypes;
    public TerrainType[] moistureTerrainTypes;

    [Header("Waves")]
    public Wave[] waves;
    public Wave[] heatWaves;
    public Wave[] moistureWaves;

    [Header("Curves")]
    public AnimationCurve heightCurve;

    private MeshRenderer tileMeshRenderer;
    private MeshFilter tileMeshFilter;
    private MeshCollider tileMeshCollider;

    private MeshGenerator meshGenerator;
    private MapGenerator mapGenerator;

    void Start ()
    {
        // get the tile components
        tileMeshRenderer = GetComponent<MeshRenderer>();
        tileMeshFilter = GetComponent<MeshFilter>();
        tileMeshCollider = GetComponent<MeshCollider>();
        
        meshGenerator = GetComponent<MeshGenerator>();
        mapGenerator = FindObjectOfType<MapGenerator>();

        GenerateTile();
    }

    void GenerateTile ()
    {
        // generate a new height map
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(noiseSampleSize, scale, waves, offset);

        float[,] hdHeigthMap = NoiseGenerator.GenerateNoiseMap(noiseSampleSize - 1, scale, waves, offset, textureResolution);

        Vector3[] verts = tileMeshFilter.mesh.vertices;

        for(int x = 0; x < noiseSampleSize; x++)
        {
            for(int z = 0; z < noiseSampleSize; z++)
            {
                int index = (x * noiseSampleSize) + z;

                verts[index].y = heightCurve.Evaluate(heightMap[x, z]) * maxHeight;
            }    
        }

        tileMeshFilter.mesh.vertices = verts;
        tileMeshFilter.mesh.RecalculateBounds();
        tileMeshFilter.mesh.RecalculateNormals();

        tileMeshCollider.sharedMesh = tileMeshFilter.mesh;

        // create the height map texture
        // Texture2D heightMapTexture = TextureBuilder.BuildTexture(hdHeigthMap, heightTerrainTypes);

        // apply the height map texture to the MeshRenderer
        // tileMeshRenderer.material.mainTexture = heightMapTexture;

        // Generate heatMap
        // float[,] heatMap = GenerateHeatMap(heightMap);
        // tileMeshRenderer.material.mainTexture = TextureBuilder.BuildTexture(heightMap, heatTerrainTypes);

        // Generate moistureMap
        // float[,] moistureMap = GenerateMoistureMap(heightMap);
        // tileMeshRenderer.material.mainTexture = TextureBuilder.BuildTexture(moistureMap, moistureTerrainTypes);

        float[,] heatMap = GenerateHeatMap(heightMap);
        float[,] moistureMap = GenerateMoistureMap(heightMap);

        switch(visualizationType)
        {
            case TerrainVisualization.Height:
                tileMeshRenderer.material.mainTexture = TextureBuilder.BuildTexture(hdHeigthMap, heightTerrainTypes);
                break;
            case TerrainVisualization.Heat:
                tileMeshRenderer.material.mainTexture = TextureBuilder.BuildTexture(heatMap, heatTerrainTypes);
                break;
            case TerrainVisualization.Moisture:
                tileMeshRenderer.material.mainTexture = TextureBuilder.BuildTexture(moistureMap, moistureTerrainTypes);
                break;
        }
    }

    float[,] GenerateHeatMap (float[,] heightMap)
    {
        float[,] uniformHeatMap = NoiseGenerator.GenerateUniformNoiseMap(
                                                    noiseSampleSize,
                                                    transform.position.z * (noiseSampleSize / meshGenerator.xSize),
                                                    (noiseSampleSize / 2) * mapGenerator.numX + 1
                                                );
        float[,] randomHeatMap = NoiseGenerator.GenerateNoiseMap(noiseSampleSize, scale, heatWaves, offset);

        float[,] heatMap = new float[noiseSampleSize, noiseSampleSize];

        for(int x = 0; x < noiseSampleSize; x++)
        {
            for(int z = 0; z < noiseSampleSize; z++)
            {
                heatMap[x, z] = randomHeatMap[x, z] * uniformHeatMap[x, z];
                heatMap[x, z] += 0.5f * heightMap[x, z];

                heatMap[x, z] = Mathf.Clamp(heatMap[x, z], 0.0f, 0.99f);
            }    
        }
        return heatMap;
    }

    float[,] GenerateMoistureMap (float[,] heightMap)
    {
        float[,] moistureMap = NoiseGenerator.GenerateNoiseMap(noiseSampleSize, scale, moistureWaves, offset);

        for(int x = 0; x < noiseSampleSize; x++)
        {
            for(int z = 0; z < noiseSampleSize; z++)
            {
                moistureMap[x, z] -= 0.1f * heightMap[x, z];
            }    
        }

        return moistureMap;
    }
}

[System.Serializable]
public class TerrainType
{
    [Range(0.0f, 1.0f)]
    public float threshold;
    public Gradient colorGradient;
}