using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeBuilder : MonoBehaviour
{
    public BiomeRow[] biomesRows;

    public static BiomeBuilder instance;

    void Awake ()
    {
        instance = this;
    }

    public Texture2D BuildTexture (TerrainType[,] heatMapTypes, TerrainType[,] moistureMapTypes)
    {
        int size = heatMapTypes.GetLength(0);
        Color[] pixels = new Color[size * size];

        for(int x = 0; x < size; x++)
        {
            for(int z = 0; z < size; z++)
            {
                int index = (x * size) + z;

                int heatMapIndex = heatMapTypes[x, z].index;
                int moistureMapIndex = moistureMapTypes[x, z].index;

                Biome biome = biomesRows[moistureMapIndex].biomes[heatMapIndex];

                pixels[index] = biome.color;
            }
        }

        Texture2D texture = new Texture2D(size, size);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }
}

[System.Serializable]
public class BiomeRow
{
    public Biome[] biomes;
}

[System.Serializable]
public class Biome
{
    public string name;
    public Color color;
}
