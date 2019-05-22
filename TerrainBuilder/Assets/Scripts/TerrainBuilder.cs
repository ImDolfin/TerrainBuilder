using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBuilder : MonoBehaviour
{
    public int mapDimension = 9;
    public int seedValue = 3;
    public float offset = 1;

    DiamondSquareAlgorithm algorithm = new DiamondSquareAlgorithm();
    //ColorHeightMap colorHeightMap = new ColorHeightMap();
    MeshSpecs meshSpecs;


    float[,] mapHeightsArray;


    // Start is called before the first frame update
    void Start()
    {
        generateTerrainMap();
        generateMesh();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        if (mapDimension < 2)
        {
            mapDimension = 2;
        }
    
        if (seedValue < 1)
        {
            seedValue = 1;
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public void generateMesh()
    {
        meshSpecs = new MeshSpecs(mapDimension);
        int vertexIndex = 0;
        float topLeftX = (mapDimension - 1) / -2f; 
        float topLeftZ = (mapDimension - 1) / 2f;

        for (int x = 0; x < mapDimension; x++)
        {
            for (int z = 0; z < mapDimension; z++, vertexIndex++)
            {
                meshSpecs.vertices[vertexIndex] = new Vector3(topLeftX + z, mapHeightsArray[z, x], topLeftZ - x);
                meshSpecs.uvs[vertexIndex] = new Vector2(z / (float)mapDimension, x / (float)mapDimension);

                if(z < mapDimension - 1 && x < mapDimension - 1)
                {
                    meshSpecs.AddTriangle(vertexIndex, vertexIndex + mapDimension + 1, vertexIndex + mapDimension);
                    meshSpecs.AddTriangle(vertexIndex + mapDimension + 1, vertexIndex, vertexIndex + 1);
                }

            }
        }
        Mesh mesh = meshSpecs.generateMesh();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
    }

    /// <summary>
    /// 
    /// </summary>
    public void generateTerrainMap()
    {
        mapHeightsArray =  algorithm.generateMapArray(mapDimension, seedValue, offset);
        //colorHeightMap.createNewColorHeightMap(mapHeightsArray);
    }
}
