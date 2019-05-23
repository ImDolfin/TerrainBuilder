using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSpecs {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Vector3[] normals;

    public int firstXPosition;
    public int firstYPosition;

    int triangleIndex;

    public MeshSpecs(int meshDimension)
    {
        vertices = new Vector3[meshDimension * meshDimension];
        triangles = new int[6 * (meshDimension - 1) * (meshDimension - 1)];
        uvs = new Vector2[meshDimension * meshDimension];
        normals = new Vector3[meshDimension * meshDimension];
        triangleIndex = 0;
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex++] = a;
        triangles[triangleIndex++] = b;
        triangles[triangleIndex++] = c;
    }

    public Mesh generateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
