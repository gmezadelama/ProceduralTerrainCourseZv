using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [Header("Parameters")]
    public int noiseSampleSize;
    public float scale;
    public float maxHeight = 1.0f;
    [Range(1, 10)]
    public int textureResolution = 1;

    private MeshRenderer tileMeshRenderer;
    private MeshFilter tileMeshFilter;
    private MeshCollider tileMeshCollider;

    void Start ()
    {
        // get the tile components
        tileMeshRenderer = GetComponent<MeshRenderer>();
        tileMeshFilter = GetComponent<MeshFilter>();
        tileMeshCollider = GetComponent<MeshCollider>();
        
        GenerateTile();
    }

    void GenerateTile ()
    {
        // generate a new height map
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(noiseSampleSize, scale);

        float[,] hdHeigthMap = NoiseGenerator.GenerateNoiseMap(noiseSampleSize, scale, textureResolution);

        Vector3[] verts = tileMeshFilter.mesh.vertices;

        for(int x = 0; x < noiseSampleSize; x++)
        {
            for(int z = 0; z < noiseSampleSize; z++)
            {
                int index = (x * noiseSampleSize) + z;

                verts[index].y = heightMap[x, z] * maxHeight;
            }    
        }

        tileMeshFilter.mesh.vertices = verts;
        tileMeshFilter.mesh.RecalculateBounds();
        tileMeshFilter.mesh.RecalculateNormals();

        tileMeshCollider.sharedMesh = tileMeshFilter.mesh;

        // create the height map texture
        Texture2D heightMapTexture = TextureBuilder.BuildTexture(hdHeigthMap);

        // apply the height map texture to the MeshRenderer
        tileMeshRenderer.material.mainTexture = heightMapTexture;
    }
}
