using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBuilder : MonoBehaviour
{
    public int mapDimension = 9;
    private int subMeshSize = 0;
    public int seedValue = 3;
    public float offset = 1;
    public Material material;

    DiamondSquareAlgorithm algorithm = new DiamondSquareAlgorithm();
    //ColorHeightMap colorHeightMap = new ColorHeightMap();
    MeshSpecs meshSpecs;

    Mesh[,] meshArray;


    float[,] mapHeightsArray;


    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        generateTerrainMap();
        generateMesh();

        gameObject.transform.localScale = new Vector3(5f, 5f, 5f);
        gameObject.transform.Rotate(Vector3.right, -90);
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

    }
    /// <summary>
    /// 
    /// </summary>
    public void generateMesh()
    {
        subMeshSize = findBiggestDivider(mapDimension, 256);
        meshArray = new Mesh[mapDimension / subMeshSize, mapDimension / subMeshSize];

        //Calculate Normals by Using the surrounding heights to further predict the normal
        //courtesy to Scheitler: https://forum.unity.com/threads/how-not-to-have-visible-mesh-edges.499153/
        Vector3[,] normals = new Vector3[mapDimension, mapDimension];
        for (int y = 0; y < mapDimension; y++)
            for (int x = 0; x < mapDimension; x++)
            {
                float height = mapHeightsArray[y, x];
                float heightX = (height - mapHeightsArray[Mathf.Clamp(x - 1, 0, mapDimension - 1), y]) - (height - mapHeightsArray[Mathf.Clamp(x + 1, 0, mapDimension - 1), y]);
                float heightY = (height - mapHeightsArray[x, Mathf.Clamp(y - 1, 0, mapDimension - 1)]) - (height - mapHeightsArray[x, Mathf.Clamp(x + 1, 0, mapDimension - 1)]);
                normals[y, x] = Vector3.Cross(new Vector3(1, heightY, 0), new Vector3(0, heightX, 1));
            }
        int h = 1;

        for (int y = 0; y < mapDimension; y += subMeshSize)
            for(int x = 0; x < mapDimension; x+= subMeshSize)
            {
                GameObject meshGameObject = new GameObject();
                meshGameObject.transform.SetParent(gameObject.transform);
                meshGameObject.AddComponent<MeshRenderer>().material = material;
                meshGameObject.name = "SubMesh(" + y/subMeshSize + "," + x/subMeshSize + ")";
                MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter>();
                MeshSpecs meshSpecs = addSubmesh(x / subMeshSize, y / subMeshSize);

                for (int i = 0; i < subMeshSize; i++)
                    for (int j = 0; j < subMeshSize; j++)
                    {
                        int index = (i * subMeshSize) + j ;
                        meshSpecs.normals[index] = normals[i + meshSpecs.firstYPosition - y / subMeshSize, j + meshSpecs.firstXPosition - x / subMeshSize];
                    }

                Mesh mesh = meshSpecs.generateMesh();
                mesh.normals = meshSpecs.normals;

                meshArray[y / subMeshSize, x / subMeshSize] = mesh;

                meshFilter.mesh = mesh;
            }
    }

    private MeshSpecs addSubmesh(int xOffset, int yOffset)
    {
        meshSpecs = new MeshSpecs(subMeshSize);

        int index = 0;
        int subMeshStartX = (subMeshSize * xOffset);
        meshSpecs.firstXPosition = subMeshStartX;
        int subMeshStartY = (subMeshSize * yOffset);
        meshSpecs.firstYPosition = subMeshStartY;
        float vertexXPosition;
        float vertexYPosition;
        for (int y = 0; y < subMeshSize; y++)
        {
            for (int x = 0; x < subMeshSize; x++/*, vertexIndex++*/)
            {
                index = (y * subMeshSize) + x;
                vertexXPosition = subMeshStartX + x - xOffset;
                vertexYPosition = subMeshStartY + y - yOffset;
                meshSpecs.vertices[index] = new Vector3(vertexXPosition, vertexYPosition, mapHeightsArray[(int)vertexYPosition,(int) vertexXPosition]);
                meshSpecs.uvs[index] = new Vector2((float)(xOffset * subMeshSize + x) / (float)mapDimension, (float)(yOffset * subMeshSize + y) / (float)mapDimension);

                if (x != subMeshSize - 1 && y != subMeshSize - 1)
                {
                    meshSpecs.AddTriangle(index, index + 1, index + subMeshSize);
                    meshSpecs.AddTriangle(index + subMeshSize , index + 1, index + subMeshSize+ 1);
                }
            }
        }
        //Debug.Log(" y: " + yOffset + " x: " + xOffset);
        //Debug.Log(" 0: " + (meshSpecs.vertices[0]) + " 2: " + meshSpecs.vertices[2] + " 6: " + meshSpecs.vertices[6] + " 8: " + meshSpecs.vertices[8]);

        //meshArray[yOffset, xOffset] = meshSpecs;

        return meshSpecs;
    }

    private int findBiggestDivider(int firstNumber, int maxDivider)
    {
        for(int i = maxDivider; i > 0; i--)
        {
            if (mapDimension % i == 0)
                if(i >= 10)
                    return i;
        }
        //Well not being able to create the mesh isn't helping so in order to tackle that problem the size has to be reduced
        return findBiggestDivider(firstNumber - 1, maxDivider);

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
