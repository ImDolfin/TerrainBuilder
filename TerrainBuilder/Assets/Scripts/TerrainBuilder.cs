using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Terrain builder that builds the terrain
/// </summary>
public class TerrainBuilder : MonoBehaviour
{
    //Dimensions and input variables
    public int mapDimension = 9;
    private int subMeshSize = 0;
    public int seedValue = 3;
    public float offset = 1;
    public Material material;

    DiamondSquareAlgorithm algorithm = new DiamondSquareAlgorithm();
    //ColorHeightMap colorHeightMap = new ColorHeightMap();
    //reassigning is computationally better than creating a new object.
    MeshSpecs meshSpecs;

    Mesh[,] meshArray;
    MeshSpecs[,] meshSpecArray;


    float[,] mapHeightsArray;


    // Start is called before the first frame update
    void Start()
    {
        //retrieve the material of the parent mesh
        material = GetComponent<MeshRenderer>().material;
        //generate the terrain
        generateTerrain();
        
        //scale and rotate because we worked with x and y instead of x and z
        gameObject.transform.localScale = new Vector3(5f, 5f, 5f);
        gameObject.transform.Rotate(Vector3.right, -90);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        //do not allow a map dimension below 2 because 2^0 + 1 is technically the lowest possible number.
        if (mapDimension < 2)
        {
            mapDimension = 2;

        }

    }
    /// <summary>
    /// Generates The Whole Mesh
    /// </summary>
    /// <param name="mapHeightsArray">2D float array of each vertices height</param>
    public void generateMesh(float[,] mapHeightsArray)
    {
        this.mapHeightsArray = mapHeightsArray;
        // get biggest possible sub mesh size, divider should not exceed 256 because 256^2 - 1 is the max amount of vertices so any value above will cause artifacts.
        subMeshSize = findBiggestDivider(mapDimension, 256);
        //create meshArray to save the generated meshes to the 2D array 
        meshArray = new Mesh[mapDimension / subMeshSize, mapDimension / subMeshSize];
        //create meshSpec Array to save the meshes specifications to the 2D array of mesh specifications
        meshSpecArray = new MeshSpecs[mapDimension / subMeshSize, mapDimension / subMeshSize];

        //Calculate Normals by Using the surrounding heights to further predict the normal
        //courtesy to Scheitler: https://forum.unity.com/threads/how-not-to-have-visible-mesh-edges.499153/
        Vector3[,] normals = new Vector3[mapDimension, mapDimension];
        for (int y = 0; y < mapDimension; y++)
            for (int x = 0; x < mapDimension; x++)
            {
                //calculate the normal for each height based on the surrounding heights. te result should map 1:1 to the mapHeightsArray
                float height = this.mapHeightsArray[y, x];
                float heightX = (height - this.mapHeightsArray[Mathf.Clamp(x - 1, 0, mapDimension - 1), y]) - (height - this.mapHeightsArray[Mathf.Clamp(x + 1, 0, mapDimension - 1), y]);
                float heightY = (height - this.mapHeightsArray[x, Mathf.Clamp(y - 1, 0, mapDimension - 1)]) - (height - this.mapHeightsArray[x, Mathf.Clamp(x + 1, 0, mapDimension - 1)]);
                //normals are a cross product of two vectors and assign those
                normals[y, x] = Vector3.Cross(new Vector3(1, heightX, 0), new Vector3(0, heightX, 1));
            }

        //iterate through the submeshes and create them
        for (int y = 0; y < mapDimension; y += subMeshSize)
            for(int x = 0; x < mapDimension; x+= subMeshSize)
            {
                //generate a new child gameobject of the parent mesh, which holds the submesh
                GameObject meshGameObject = new GameObject();
                meshGameObject.transform.SetParent(gameObject.transform);
                meshGameObject.AddComponent<MeshRenderer>().material = material;
                meshGameObject.name = "SubMesh(" + y/subMeshSize + "," + x/subMeshSize + ")";
                MeshFilter meshFilter = meshGameObject.AddComponent<MeshFilter>();
                //generate and assign the new mesh
                MeshSpecs meshSpecs = addSubmesh(x / subMeshSize, y / subMeshSize);

                //assign the normals to the respective vertices
                for (int i = 0; i < subMeshSize; i++)
                    for (int j = 0; j < subMeshSize; j++)
                    {
                        int index = (i * subMeshSize) + j ;
                        meshSpecs.normals[index] = normals[i + meshSpecs.firstYPosition - y / subMeshSize, j + meshSpecs.firstXPosition - x / subMeshSize];
                    }

                //generates the mesh
                Mesh mesh = meshSpecs.generateMesh();
                //dislike reassignment, might probably remove the normals formula and simply equal the vertex normals at edges.
                mesh.normals = meshSpecs.normals;

                //save mesh to array
                meshArray[y / subMeshSize, x / subMeshSize] = mesh;
                //save spec to array
                meshSpecArray[y / subMeshSize, x / subMeshSize] = meshSpecs;
                //add mesh to meshFilter
                meshFilter.mesh = mesh;
            }
    }

    /// <summary>
    /// creates a new submesh based on the starting position and size
    /// </summary>
    /// <param name="xOffset"> submesh number in x direction</param>
    /// <param name="yOffset"> submesh number in y direction</param>
    /// <returns>MeshSpecs object of the submeshs specifications</returns>
    private MeshSpecs addSubmesh(int xOffset, int yOffset)
    {
        meshSpecs = new MeshSpecs(subMeshSize);

        // set variables.
        int index = 0;
        //find and assign starting coordinate of the mesh in x direction
        int subMeshStartX = (subMeshSize * xOffset);
        meshSpecs.firstXPosition = subMeshStartX;
        //find and assign starting coordinate of the mesh in y direction
        int subMeshStartY = (subMeshSize * yOffset);
        meshSpecs.firstYPosition = subMeshStartY;
        float vertexXPosition;
        float vertexYPosition;
        // assign each mesh variable to the respective world coordinate
        for (int y = 0; y < subMeshSize; y++)
        {
            for (int x = 0; x < subMeshSize; x++)
            {
                //calculate index position for the vector single line format
                index = (y * subMeshSize) + x;
                //valculate vertex world coordinate in x direction
                vertexXPosition = subMeshStartX + x - xOffset;
                //valculate vertex world coordinate in y direction
                vertexYPosition = subMeshStartY + y - yOffset;
                //assign vertex with the respective height to the vertex
                meshSpecs.vertices[index] = new Vector3(vertexXPosition, vertexYPosition, mapHeightsArray[(int)vertexYPosition,(int) vertexXPosition]);
                //assign uvs
                meshSpecs.uvs[index] = new Vector2((float)(xOffset * subMeshSize + x) / (float)mapDimension, (float)(yOffset * subMeshSize + y) / (float)mapDimension);
                //add triangles until they reach the submeshes corner edge
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


    /// <summary>
    /// finds the biggest divider of a number based on the maximum divider
    /// </summary>
    /// <param name="firstNumber">base number whos divider is seached for</param>
    /// <param name="maxDivider">maximum divider</param>
    /// <returns></returns>
    private int findBiggestDivider(int firstNumber, int maxDivider)
    {
        for(int i = maxDivider; i > 0; i--)
        {
            if (mapDimension % i == 0)
                //do not allow meshes smaller than cuz reasons
                if(i >= 9)
                    return i;
        }
        //Well not being able to create the mesh isn't helping so in order to tackle that problem the size has to be reduced
        return findBiggestDivider(firstNumber - 1, maxDivider);

    }

    /// <summary>
    /// generates the terrain
    /// </summary>
    public void generateTerrain()
    {
        //retrieve the heights in a 2D float array
        float[,] mapHeightsArray =  algorithm.generateMapArray(mapDimension, seedValue, offset);
        //generate the mesh using the heights array
        generateMesh(mapHeightsArray);
        //colorHeightMap.createNewColorHeightMap(mapHeightsArray);
    }
}
