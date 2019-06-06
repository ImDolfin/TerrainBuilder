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
        //generate the terrain
        generateTerrain();
        //scale and rotate because we worked with x and y instead of x and z
        gameObject.transform.localScale = new Vector3(5f, 5f, 5f);
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
<<<<<<< HEAD
        //retrieve the heights in a 2D float array
        this.mapHeightsArray = algorithm.generateMapArray(mapDimension, seedValue, offset);
        //generate the mesh using the heights array
        generateTerrainMesh(mapHeightsArray);
        //colorHeightMap.createNewColorHeightMap(mapHeightsArray);
=======
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
				
				GetComponent<MeshFilter>().sharedMesh = mesh;
				// Debug.Log("TerrainBuilder MeshBounds: " + mesh.bounds);
				// Debug.Log(GetComponent<MeshFilter>().sharedMesh.bounds);
            }
>>>>>>> origin/Aufgabe-3
    }

    /// <summary>
    /// Generates The Whole Mesh
    /// </summary>
    /// <param name="mapHeightsArray">2D float array of each vertices height</param>
    public void generateTerrainMesh(float[,] mapHeightsArray)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
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
        meshFilter.mesh = mesh;

    }
}
