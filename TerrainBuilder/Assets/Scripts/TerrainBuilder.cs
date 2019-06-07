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
    public int seedValue = 3;
    public float offset = 1;

    DiamondSquareAlgorithm algorithm = new DiamondSquareAlgorithm();
    //ColorHeightMap colorHeightMap = new ColorHeightMap();
    //reassigning is computationally better than creating a new object.
    MeshSpecs meshSpecs;
    Mesh mesh;

    float[,] mapHeightsArray;


    // Start is called before the first frame update
    void Start()
    {
        //collider.convex = true;
        //collider.isTrigger = true;
        //generate the terrain
        generateTerrain();
        //gameObject.transform.localScale = new Vector3(5f, 5f, 5f);
        gameObject.transform.Rotate(Vector3.right, -90);
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * guess im stupido, y dat not work?
        GameObject gameObject = GameObject.Find("Mesh");
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = gameObject.transform.TransformPoint(vertices[i]);
            if (vertex.z < 0)
                vertices[i].z = 0;
        }
        mesh.vertices = vertices;
        
        meshFilter.mesh = mesh;*/
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
    /// generates the terrain
    /// </summary>
    public void generateTerrain()
    {
        //retrieve the heights in a 2D float array
        this.mapHeightsArray = algorithm.generateMapArray(mapDimension, seedValue, offset);
        //generate the mesh using the heights array
        generateTerrainMesh(mapHeightsArray);
        //colorHeightMap.createNewColorHeightMap(mapHeightsArray);
    }

    /// <summary>
    /// Generates The Whole Mesh
    /// </summary>
    /// <param name="mapHeightsArray">2D float array of each vertices height</param>
    public void generateTerrainMesh(float[,] mapHeightsArray)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshSpecs = new MeshSpecs(mapDimension);
        // set variables.
        int index = 0;
        // assign each mesh variable to the respective world coordinate
        for (int y = 0; y < mapDimension; y++)
        {
            for (int x = 0; x < mapDimension; x++)
            {
                //calculate index position for the vector single line format
                index = (y * mapDimension) + x;
                //assign vertex with the respective height to the vertex
                meshSpecs.vertices[index] = new Vector3(x, y, mapHeightsArray[y, x]);
                //assign uvs
                meshSpecs.uvs[index] = new Vector2((float)x / (float)mapDimension, (float)y / (float)mapDimension);

                //calculate the normal for each height based on the surrounding heights. te result should map 1:1 to the mapHeightsArray
                //Calculate Normals by Using the surrounding heights to further predict the normal
                //Sadly it is not working properly so I will just let that sit here. 
                //courtesy to Scheitler: https://forum.unity.com/threads/how-not-to-have-visible-mesh-edges.499153/
                float height = mapHeightsArray[y, x];
                float heightX = (height - mapHeightsArray[Mathf.Clamp(x - 1, 0, mapDimension - 1), y]) - (height - mapHeightsArray[Mathf.Clamp(x + 1, 0, mapDimension - 1), y]);
                float heightY = (height - mapHeightsArray[x, Mathf.Clamp(y - 1, 0, mapDimension - 1)]) - (height - mapHeightsArray[x, Mathf.Clamp(x + 1, 0, mapDimension - 1)]);
                //assign the normals to the respective vertices
                meshSpecs.normals[index] = Vector3.Cross(new Vector3(1, heightX, 0), new Vector3(0, heightX, 1));
                //add triangles until they reach the submeshes corner edge
                if (x != mapDimension - 1 && y != mapDimension - 1)
                {
                    meshSpecs.AddTriangle(index, index + 1, index + mapDimension);
                    meshSpecs.AddTriangle(index + mapDimension, index + 1, index + mapDimension + 1);
                }

            }
        }

        //generates the mesh
        this.mesh = meshSpecs.generateMesh();
        //dislike reassignment, might probably remove the normals formula and simply equal the vertex normals at edges.
        //mesh.normals = meshSpecs.normals;
        //add mesh to meshFilter
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;

    }
}
