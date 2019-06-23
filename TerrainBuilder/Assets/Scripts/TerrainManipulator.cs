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
    /// allows the tool to continuously change the mesh
    /// if this value is false, then one has to select a vertex and manipulate the values within 
    /// the radius using the scrollwheel
    /// else, if it is true, then the vertices can be changed without having to lock on a point
    /// worth noting, if the cursor position doesn't change while continuously changing is true, 
    /// then the vertex will be locked, so it is easier to stay at a vertex
    /// </summary>
    public bool changeVertexContinuously = true;
    /// <summary>
    /// sets the tool to the createrCrater tool, which creates a crater
    /// </summary>
    public bool createCrater = false;
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
    /// Negative values will cut down into the generated terrain, 
    /// which results in a more realistic look than positive values.
    /// Positive values will create peaks that often look unrealistic.
    /// </summary>
    [Range(-20f, 20f)]
    public float randomOffset = 10f;
    /// <summary>
    /// radius of the tool
    /// </summary>
    public int radius = 10;
    /// <summary>
    /// factor by which the tool will be scaled. 
    /// This scale will be squared, so be carefull with high values
    /// </summary>
   [Range(0, 10)]
    public int scale = 10;

    private bool hasBeenCreated = false;

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
        else
            manipulateMesh();
    }

    private void OnValidate()
    {
        //do not allow a map dimension below 2 because 2^0 + 1 is technically the lowest possible number.
        if (radius < 1)
        {
            radius = 1;
        }

    }

    /// <summary>
    /// Manipulates the Mesh based on the different settings
    /// </summary>
    private void manipulateMesh()
    {
        //Do actions involving the manipulation
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            Vector3[] vertices = meshFilter.sharedMesh.vertices;

            if (changeVertexContinuously)
            {
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
            }
            else if (!changeVertexContinuously)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 hitVertex = castRayOnCollider();
                    if (hitVertex.z == float.NaN)
                        return;
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                {
                    //increase the height on left mouse button click
                    leftMouseManipulation(centeredVertex, vertices);
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                {
                    //decrease the height on left mouse button click
                    rightMouseManipulation(centeredVertex, vertices);
                }

            }
            //set all vertices heights below 0 to 0
            for (int i = 0; i < vertices.Length; i++)
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
                    if (createCrater)
                    {
                        vertices[i].z += scale * scale * calculateCraterFactor(distance, radius) + (((float)random.NextDouble() * 2 * randomOffset) - randomOffset) / 10;
                        continue;
                    }
                    vertices[i].z += scale * scale * calculateGaussian(distance, radius) + (((float)random.NextDouble() * 2 * randomOffset) - randomOffset) / 10;
                    continue;
                }
                if (createCrater)
                {
                    vertices[i].z += scale * scale * calculateCraterFactor(distance, radius);
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
                if (createCrater)
                {
                    vertices[i].z -= scale * scale * calculateCraterFactor(distance, radius);
                    continue;
                }
                vertices[i].z -= scale * scale * calculateGaussian(distance, radius);
            }
        }
    }


    /// <summary>
    /// Calculates the factor at each distance which resembles the hight increase
    /// </summary>
    /// <param name="distance">distance between the center and the point</param>
    /// <param name="radius">radius of the gaussian distribution and its mean</param>
    /// <returns>crater height scaling factor as float</returns>
    private float calculateCraterFactor(float distance, float radius)
    {
        //when the vertex is within the defined range
        float range = (radius / 3f) * (radius / 3f);
        float input = (radius - distance) * 2f;
        // the strength of the crater circle peaks
        float peakStrength = radius;
        // This is the gaussian function with some tweaks to its variables, where we use the whole Gaussian function so the distribution 
        // will be in the range of 0 to Radius, not only half of it like in the calculateGaussian method
        float craterFactor = (1 / Mathf.Sqrt(2 * Mathf.PI * range)) * Mathf.Exp(-Mathf.Pow((input - peakStrength), 2) / (2 * range));
        //*10 to increas the factor and allow better scaling between 1 and 10
        return craterFactor*10;
    }

    /// <summary>
    /// calculates the gaussion functions height factor based on the distance
    /// </summary>
    /// <param name="distance">distance between the center and the point</param>
    /// <param name="radius">radius of the gaussian distribution and its mean</param>
    /// <returns>gaussion height scaling factor as float</returns>
    private float calculateGaussian(float distance, float radius)
    {
        //variance is the (radius/4) squared because radius/4 is supposed to be the standard deviation
        float variance = (radius/4f)*(radius/4f);
        //average of the distances in 2D which is the radius
        float mean = radius;
        //input, the distance has to be changed to an actual usable x value which maps onto the function correctly
        float input = (radius-distance);
        //calculate the gaussian value
        float gaussian = (1 / Mathf.Sqrt(2 * Mathf.PI * variance)) * Mathf.Exp( - Mathf.Pow((input - mean), 2) / (2 * variance));
        //*10 to increas the factor and allow better scaling between 1 and 10
        return gaussian *10;
    }

    /// <summary>
    /// performs costly operations after release of the mouse button
    /// </summary>
    private void mouseButtonReleased()
    {
        hasBeenCreated = false;
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
