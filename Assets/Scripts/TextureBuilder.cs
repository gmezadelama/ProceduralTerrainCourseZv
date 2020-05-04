﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureBuilder
{
    // builds a texture based on the given noise map
    public static Texture2D BuildTexture (float[,] noiseMap, TerrainType[] terrainTypes)
    {
        // create color array for the pixels
        Color[] pixels = new Color[noiseMap.Length];

        // calculate the length of the texture
        int pixelLength = noiseMap.GetLength(0);

        // loop through each pixel
        for(int x = 0; x < pixelLength; x++)
        {
            for(int z = 0; z < pixelLength; z++)
            {
                // next index in the 'pixels' array
                int index = (x * pixelLength + z);
                
                for(int t = 0; t < terrainTypes.Length; t++)
                {
                    if(noiseMap[x, z] < terrainTypes[t].threshold)
                    {
                        float minVal = t == 0 ? 0 : terrainTypes[t - 1].threshold;
                        float maxVal = terrainTypes[t].threshold;

                        pixels[index] = terrainTypes[t].colorGradient.Evaluate(1.0f - (maxVal - noiseMap[x, z]) / (maxVal - minVal));
                        break;
                    }
                }
            }    
        }

        // create a new Texture2D and set it up
        Texture2D texture = new Texture2D(pixelLength, pixelLength);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    public static TerrainType[,] CreateTerrainTypeMap (float[,] noiseMap, TerrainType[] terrainTypes)
    {
        int size = noiseMap.GetLength(0);
        TerrainType[,] outputMap = new TerrainType[size, size];

        for(int x = 0; x < size; x++)
        {
            for(int z = 0; z < size; z++)
            {
                for(int t = 0; t < terrainTypes.Length; t++)
                {
                    if (noiseMap[x, z] < terrainTypes[t].threshold)
                    {
                        outputMap[x, z] = terrainTypes[t];
                        break;
                    }
                }
            }
        }

        return outputMap;
    } 
}
