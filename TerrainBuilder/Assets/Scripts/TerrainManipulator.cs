using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManipulator : MonoBehaviour
{

    MeshFilter meshFilter;
    MeshCollider meshCollider;
    public bool manipulateTerrain = false;
    public bool changeVertexContinuously = true;
    public bool addRandomOffset = false;
    public bool setZero = false;
    public float randomOffset = 10f;
    public int radius = 10;
    public int strength = 10;
    private Vector3 centeredVertex;
    private Vector3 cursorPosition;
    //random number generator
    private System.Random random = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!manipulateTerrain)
            return;
        
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            Vector3 hitVertex = castRayOnCollider();
            if (hitVertex.z == float.NaN)
                return;
            if (Input.GetMouseButton(0))
            {
                leftMouseManipulation(hitVertex, vertices);
            }
            else if (Input.GetMouseButton(1))
            {
                rightMouseManipulation(hitVertex, vertices);
            }
            for(int i = 0; i< vertices.Length; i++)
            {
                if (vertices[i].z < 0f)
                {
                    vertices[i].z = 0f;
                    continue;
                }
            }
            meshFilter.sharedMesh.vertices = vertices;
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                mouseButtonReleased();
            }
        }
    }

    private Vector3 castRayOnCollider()
    {
        RaycastHit cursorPosition;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out cursorPosition, int.MaxValue))
        {
            Debug.Log("Hit detected");
            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            Debug.Log(cursorPosition.point.ToString());
            Vector3 hitVertex = retrieveHitVertex(cursorPosition.point, vertices);
            Debug.Log(hitVertex.ToString());
            Debug.DrawLine(cursorPosition.point, hitVertex);
            return hitVertex;
        }
        else
        {
            Vector3 failed = new Vector3(float.NaN, float.NaN, float.NaN);
            return failed;
        }
    }


    private void leftMouseManipulation(Vector3 hitVertex, Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = calculateDistance(hitVertex, vertices[i]);
            //when the vertex is within the defined range
            if (distance <= radius)
            {
                //Add calculations for Gauss
                if (setZero)
                {
                    vertices[i].z = 0.0f;
                    continue;
                }
                else if (!addRandomOffset)
                    vertices[i].z += strength * calculateGaussian(hitVertex, vertices[i], radius);
                else
                    vertices[i].z += strength * calculateGaussian(hitVertex, vertices[i], radius) + (((float)random.NextDouble() * 2 * randomOffset) - randomOffset);
            }

        }
    }

    private void rightMouseManipulation(Vector3 hitVertex, Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = calculateDistance(hitVertex, vertices[i]);
            //when the vertex is within the defined range
            if (distance <= radius)
            {
                
                //Add calculations for Gauss
                if (setZero)
                {
                    vertices[i].z = 0.0f;
                    continue;
                }
                if (!addRandomOffset)
                    vertices[i].z -= strength * calculateGaussian(hitVertex, vertices[i], radius);
                else
                    vertices[i].z -= strength * calculateGaussian(hitVertex, vertices[i], radius) + (((float)random.NextDouble() * 2 * randomOffset) - randomOffset);
            }
        }
    }

    private float calculateGaussian(Vector3 centerVertex, Vector3 distantVertex, float radius)
    {
        float xDiff = distantVertex.x - centerVertex.x;
        float yDiff = distantVertex.y - centerVertex.y;
        return Mathf.Exp(-((Mathf.Pow(xDiff, 2f)) + Mathf.Pow(yDiff, 2f)) / (2 * radius));
    }

    private void mouseButtonReleased()
    {
        //costly operation which will be applied after the mesh has been changed
        meshFilter.sharedMesh.RecalculateNormals();
        meshFilter.sharedMesh.RecalculateBounds();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="vertices"></param>
    /// <returns></returns>
    private Vector3 retrieveHitVertex(Vector3 point, Vector3[] vertices)
    {
        Vector3 hitVertex = new Vector3();
        float shortestDistance = float.MaxValue;
        if (point == cursorPosition)
            return centeredVertex;
        // iterate through the vertices
        for (int i = 0; i < vertices.Length; i++)
        {
            //get the distance from the hitpoint to the vertex in order to determine the hit vertex
            float distance = calculateDistance(point, transform.TransformPoint(vertices[i]));
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                hitVertex = vertices[i];
            }
        }
        Debug.Log(hitVertex.ToString());
        centeredVertex = hitVertex;
        cursorPosition = point;
        return hitVertex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="referencePoint"></param>
    /// <param name="distantPoint"></param>
    /// <returns></returns>
    private float calculateDistance(Vector3 referencePoint, Vector3 distantPoint)
    {
        Vector3 difference = referencePoint - distantPoint;
        return Mathf.Sqrt(difference.sqrMagnitude);
    }

}
