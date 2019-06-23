﻿// This Script needs to be executed after the terrainbuilder. Edit->Project Settings->Script Execution Order
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContourLines : MonoBehaviour
{
	public int userIntervall;
	int intervall;
	
	Mesh mesh;
	MeshRenderer meshRenderer;
	Vector3[] vertices;
	
	
    // Start is called before the first frame update
    void Start()
    {
		 
    }
	
	// Compares two vertices and returns the one which is closer to the ContourLine
	int getVerticeNextToContourLine(int a, int b){
		  float heightA = vertices[a].z;
		  float heightB = vertices[b].z;
		  float modA = vertices[a].z % intervall;
		  float modB = vertices[b].z % intervall;
		
		  // If the height is under the ContourLine, the modulo has to be inverted
		  if ( heightA < heightB){
			  modA = intervall - modA;
		  }
		  else
		  {
		  	modB = intervall - modB;
		  }
		
		  // The smallest modulo is closest to the ContourLine
		  if (modA < modB)
		  {
		  	return a;
		  }
		  else
		  {
		  	return b;
		  }
	  }

    private void OnValidate()
    {
        //do not allow a user intervall below 0
        if (userIntervall < 0)
        {
            userIntervall = 0;

        }
    }

    // Update is called once per frame
    void Update()
    {
        drawLines()
    }

    void drawLines()
    {
        intervall = userIntervall;
		    meshRenderer = GetComponent<MeshRenderer>();
		    mesh = GetComponent<MeshFilter>().sharedMesh;
		    vertices = mesh.vertices;
		    Texture2D texture = new Texture2D( (int)mesh.bounds.size.x, (int)mesh.bounds.size.y);	
		
		    Vector3 boundsMin = mesh.bounds.min;
		    Vector3 boundsMax = mesh.bounds.max;
		    int maxX = System.Convert.ToInt32(boundsMax.x);		// Convert.ToInt32 rounds to the next int (no wrong cut-off)
		    int row = 0;
		
		    Debug.Log("length: " + vertices.Length + " maxX: " + maxX);
		    // Debug.Log(boundsMax + " max -- min " + boundsMin);
		    // Debug.Log("VerticesLength: " + vertices.Length);
		
		    for (var i=0;i<vertices.Length;i++)
		    {		
			    if (i % maxX == 0){
				    row++;
			    }
	
			    // We don't want to color the bottom black
			    if ( System.Convert.ToInt32(vertices[i].z) != 0)
			    {
				    // Everything divided by the intervall with modulo 0 is on the contour line
				    if ( (System.Convert.ToInt32(vertices[i].z) % intervall) <= 0)
				    {
					    texture.SetPixel( (int)(vertices[i].x), (int)(vertices[i].y), Color.black);
				    }
				    // Check if the vertice has neighbours and is not on the edge
				    else if ( vertices[i].x > boundsMin.x && vertices[i].x < boundsMax.x
					    && vertices[i].y > boundsMin.y && vertices[i].y < boundsMax.y)
				    {
					    // Check if there is a contour point between the neighbours
					    // left neighbour
					    if ( (System.Convert.ToInt32(vertices[i-1].z) / intervall) != (System.Convert.ToInt32(vertices[i].z) / intervall) )
					    {
						    int verticeNumber = getVerticeNextToContourLine(i, i-1);
						    texture.SetPixel( (int)(vertices[verticeNumber].x), (int)(vertices[verticeNumber].y), Color.black);
					    }
					    // right neighbour
					    if ( (System.Convert.ToInt32(vertices[i+1].z) / intervall) != (System.Convert.ToInt32(vertices[i].z) / intervall) )
					    {
						    int verticeNumber = getVerticeNextToContourLine(i, i+1);
						    texture.SetPixel( (int)(vertices[verticeNumber].x), (int)(vertices[verticeNumber].y), Color.black);
					    }
					    // front neighbour
					    if ( row > 0 && (System.Convert.ToInt32(vertices[i-maxX].z) / intervall) != (System.Convert.ToInt32(vertices[i].z) / intervall) )
					    {
						    int verticeNumber = getVerticeNextToContourLine(i, i-maxX);
						    texture.SetPixel( (int)(vertices[verticeNumber].x), (int)(vertices[verticeNumber].y), Color.black);
					    }
					    // back neighbour
					    if ( row < (vertices.Length/maxX) && (System.Convert.ToInt32(vertices[i+maxX].z) / intervall) != (System.Convert.ToInt32(vertices[i].z) / intervall) )
					    {
						    int verticeNumber = getVerticeNextToContourLine(i, i+maxX);
						    texture.SetPixel( (int)(vertices[verticeNumber].x), (int)(vertices[verticeNumber].y), Color.black);
					    }
				    }
			    }
		    }  	
		    texture.Apply();
		    meshRenderer.material.SetTexture("_ContourLineTex", texture);
}
