using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBuilder : MonoBehaviour
{
    public int mapSideLength = 9;
    public int seedValue = 3;

    DiamondSquareAlgorithm algorithm = new DiamondSquareAlgorithm();
    ColorHeightMap colorHeightMap = new ColorHeightMap();

    double[,] mapArray;

    // Start is called before the first frame update
    void Start()
    {
        generateTerrainMap();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        if (mapSideLength < 2)
        {
            mapSideLength = 2;
        }
    
        if (seedValue < 1)
        {
            seedValue = 1;
        }

    }

    public void generateMesh()
    {

    }

    public void generateTerrainMap()
    {
        mapArray =  algorithm.generateMapArray(mapSideLength, seedValue);
        colorHeightMap.createNewColorHeightMap(mapArray);
    }
}
