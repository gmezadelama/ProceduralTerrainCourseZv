using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public static float[,] GenerateNoiseMap (int noiseSampleSize, float scale, int resolution = 1)
    {
        
        int noiseMapLength = noiseSampleSize * resolution;
        float[,] noiseMap = new float[noiseMapLength, noiseMapLength];

        for (int x = 0; x < noiseMapLength; x++) {
            for (int y = 0; y < noiseMapLength; y++) {
                float samplePosX = (float)x / scale / (float)resolution;
                float samplePosY = (float)y / scale / (float)resolution;

                noiseMap[x, y] = Mathf.PerlinNoise(samplePosX, samplePosY);
            }    
        }

        return noiseMap;
    }
}
