using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the Vectors and variables which are used for mesh generation
/// it also contains the method to generate the mesh
/// </summary>
public class MeshSpecs {
    // mesh vectors
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Vector3[] normals;

    // starting position of the mesh which is the bottom right position
    public int firstXPosition;
    public int firstYPosition;

    int triangleIndex;

    /// <summary>
    /// initialze the mesh
    /// </summary>
    /// <param name="meshDimension">dimensions of the mesh</param>
    public MeshSpecs(int meshDimension)
    {
        vertices = new Vector3[meshDimension * meshDimension];
        triangles = new int[6 * (meshDimension - 1) * (meshDimension - 1)];
        uvs = new Vector2[meshDimension * meshDimension];
        normals = new Vector3[meshDimension * meshDimension];
        triangleIndex = 0;
    }

    /// <summary>
    /// Adds the given points, which create an array, to the triangles array
    /// Note: keep clockwise direction in mind
    /// </summary>
    /// <param name="a">first corner point</param>
    /// <param name="b">second corner point</param>
    /// <param name="c">third corner point</param>
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex++] = a;
        triangles[triangleIndex++] = b;
        triangles[triangleIndex++] = c;
    }

    /// <summary>
    /// Generate and return the mesh out of the given vertices, triangles, uvs and normals
    /// </summary>
    /// <returns>Mesh object that was generated out of the mesh specifications</returns>
    public Mesh generateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        //including normals and afterwards recalculating normals is not realy doing the job, might delete either the
        //Formula which perform the normal calculations or remove the recalculation
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
