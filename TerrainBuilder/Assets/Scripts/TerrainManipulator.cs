using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManipulator : MonoBehaviour
{
    //Use mesh filter and collider as parameter to reduce the 
    //computation by avoiding to cast "GetComponent" all the time
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    //assign all the public changable values
    /// <summary>
    /// if false, then the terrain can not be manipulated.
    /// set true to manipulate the terrain
    /// </summary>
    public bool manipulateTerrain = false;
    /// <summary>
    /// currently not implemented, but if true, then one can 
    /// move the cursor, else one can only select a single point to change it
    /// </summary>
    public bool changeVertexContinuously = true;
    /// <summary>
    /// adds random offset to the tool
    /// </summary>
    public bool addRandomOffset = false;
    /// <summary>
    /// if true, then one will set all heights within the radius to 0
    /// </summary>
    public bool setZero = false;
    /// <summary>
    /// random offset of the randomizer
    /// </summary>
    public float randomOffset = 10f;
    /// <summary>
    /// radius of the tool
    /// </summary>
    public int radius = 10;
    /// <summary>
    /// factor by which the tool will be scaled. 
    /// This scale will be squared, so be carefull with high values
    /// </summary>
    public int scale = 10;

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
        //retrieve the currently selected vertex
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            Vector3 hitVertex = castRayOnCollider();
            if (hitVertex.z == float.NaN)
                return;
            if (Input.GetMouseButton(0))
            {
                //increase the height on left mouse button click
                leftMouseManipulation(hitVertex, vertices);
            }
            else if (Input.GetMouseButton(1))
            {
                //decrease the height on left mouse button click
                rightMouseManipulation(hitVertex, vertices);
            }
            //set all vertices heights below 0 to 0
            for(int i = 0; i< vertices.Length; i++)
            {
                if (vertices[i].z < 0f)
                {
                    vertices[i].z = 0f;
                    continue;
                }
            }
            meshFilter.sharedMesh.vertices = vertices;
            //meshCollider.sharedMesh = meshFilter.sharedMesh;
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                //assign updated mesh to the mesh collider and recalculate normals as well as 
                //bounds when the mouse button has been released
                mouseButtonReleased();
            }
        }
    }


    /// <summary>
    /// Casts the ray onto the collider and retrieves the closest hit vertex
    /// </summary>
    /// <returns>hit vertex in local coordinates</returns>
    private Vector3 castRayOnCollider()
    {
        RaycastHit cursorPosition;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out cursorPosition, int.MaxValue))
        {
            //Debug.Log("Hit detected");
            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            //Debug.Log(cursorPosition.point.ToString());
            Vector3 hitVertex = retrieveHitVertex(cursorPosition.point, vertices);
            //Debug.Log(hitVertex.ToString());
            Debug.DrawLine(cursorPosition.point, hitVertex);
            return hitVertex;
        }
        else
        {
            Vector3 failed = new Vector3(float.NaN, float.NaN, float.NaN);
            return failed;
        }
    }

    /// <summary>
    /// increases the height of the terrain within the provided radius
    /// </summary>
    /// <param name="hitVertex">center vertex of the radius</param>
    /// <param name="vertices">list of vertices of the certer vertexs mesh</param>
    private void leftMouseManipulation(Vector3 hitVertex, Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            //get the distance between the center and the current vertex
            float distance = calculateLocalVertexDistance(hitVertex, vertices[i]);
            //when the vertex is within the defined range
            if (distance <= radius)
            {
                
                if (setZero)
                {
                    vertices[i].z = 0.0f;
                    continue;
                }
                if (addRandomOffset)
                {
                    vertices[i].z += scale * scale * calculateGaussian(distance, radius) + (((float)random.NextDouble() * 2 * randomOffset) - randomOffset) / 10;
                    continue;
                }
                vertices[i].z += scale * scale * calculateGaussian(distance, radius);
            }

        }
    }

    /// <summary>
    /// decreases the height of the terrain within the provided radius
    /// </summary>
    /// <param name="hitVertex">center vertex of the radius</param>
    /// <param name="vertices">list of vertices of the certer vertexs mesh</param>
    private void rightMouseManipulation(Vector3 hitVertex, Vector3[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            //get the distance between the center and the current vertex
            float distance = calculateLocalVertexDistance(hitVertex, vertices[i]);
            //when the vertex is within the defined range
            if (distance <= radius)
            {

                if (setZero)
                {
                    vertices[i].z = 0.0f;
                    continue;
                }
                if (addRandomOffset)
                {
                    vertices[i].z -= scale * scale * calculateGaussian(distance, radius) + (((float)random.NextDouble() * 2 * randomOffset) - randomOffset) / 10;
                    continue;
                }
                vertices[i].z -= scale * scale * calculateGaussian(distance, radius);
            }
        }
    }

    /// <summary>
    /// calculates the gaussion functions height factor based on the distance
    /// </summary>
    /// <param name="distance">distance between the center and the point</param>
    /// <param name="radius">radius of the gaussian distribution</param>
    /// <returns>gaussion height scaling factor as float</returns>
    private float calculateGaussian(float distance, float radius)
    {
        //variance is the radius * 2 (most outer position of the gaussian function - usually 0)
        float variance = radius * 2;
        //average of the distances in 2D which is the radius
        float mean = radius;
        //input, the distance has to be changed to an actual usable x value which maps onto the function correctly
        float input = (radius-distance);
        //calculate the gaussian value
        float gaussian = (1 / Mathf.Sqrt(2 * Mathf.PI * variance)) * Mathf.Exp( - Mathf.Pow((input - mean), 2) / (2 * variance));
        //clamp the value between 0 and 1 in order to return the actual scaling factor
        return Mathf.Clamp01(gaussian);
    }

    /* USE to create craters for task number 5
     * private float calculateGaussian(float distance, float radius)
    {
        float variance = this.radius;
        float mean = (radius / 2) ;
        float input = (distance/2) ;
        float gaussian = (1 / Mathf.Sqrt(2 * Mathf.PI * variance)) * Mathf.Exp( - Mathf.Pow((input - mean), 2) / (2 * variance));
        return gaussian;
    }*/


    /// <summary>
    /// performs costly operations after release of the mouse button
    /// </summary>
    private void mouseButtonReleased()
    {
        //costly operation which will be applied after the mesh has been changed
        meshFilter.sharedMesh.RecalculateNormals();
        meshFilter.sharedMesh.RecalculateBounds();
        //problem is, that the raycast will not hit changed vertices until the colliders mesh is reassigned
        meshCollider.sharedMesh = meshFilter.sharedMesh;
    }

    /// <summary>
    /// retrieves the hit vertex based on the raycasts hitpoint
    /// </summary>
    /// <param name="point">hit point of the raycast</param>
    /// <param name="vertices">vertices of the hit mesh</param>
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
            Vector3 difference = point - transform.TransformPoint(vertices[i]);
            //different distance calculation to get the vertex from the mesh while using the map coordinates
            float distance = calculateWorldDistance(point, vertices[i]);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                hitVertex = vertices[i];
            }
        }
        //Debug.Log(hitVertex.ToString());
        centeredVertex = hitVertex;
        cursorPosition = point;
        return hitVertex;
    }

    /// <summary>
    /// calculates the distance of the local mesh vertices
    /// </summary>
    /// <param name="referencePoint">distributions center point, which acts as the reference point</param>
    /// <param name="distantPoint">point in the distance whos distance to the reference will be calculated</param>
    /// <returns>distance between the points</returns>
    private float calculateLocalVertexDistance(Vector3 referencePoint, Vector3 distantPoint)
    {
        float x = referencePoint.x - distantPoint.x;
        float y = referencePoint.y - distantPoint.y;
        Vector3 difference = referencePoint - distantPoint;
        //completely ignore the height of the vector when retrieving the distance
        //therefore only the surrounding vertices will be used, not the ones which are 
        //closer because of the height not the x/y coordinates
        difference = new Vector3(x, y, 0f);

        return Mathf.Sqrt(difference.sqrMagnitude);
    }

    /// <summary>
    /// calculates the distance of the world coordinates of the mesh vertices
    /// </summary>
    /// <param name="referencePoint">reference point</param>
    /// <param name="distantPoint">point in the distance whos distance to the reference will be calculated</param>
    /// <returns>distance between the points</returns>
    private float calculateWorldDistance(Vector3 referenceWorldPoint, Vector3 distantLocalPoint)
    {
        //simply use the difference, because this function is used for the raycast, 
        //which is supposed to get the closest point including the height
        Vector3 difference = referenceWorldPoint - transform.TransformPoint(distantLocalPoint);

        return Mathf.Sqrt(difference.sqrMagnitude);
    }
}
