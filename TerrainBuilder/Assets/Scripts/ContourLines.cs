// This Script needs to be executed after the terrainbuilder. Edit->Project Settings->Script Execution Order
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
		intervall = userIntervall;
		meshRenderer = GetComponent<MeshRenderer>();
		mesh = GetComponent<MeshFilter>().sharedMesh;
		vertices = mesh.vertices;
		Texture2D texture = new Texture2D( (int)mesh.bounds.size.x, (int)mesh.bounds.size.y);	
		
		// int textureAccurancy = 10;
		// Texture2D texture = new Texture2D( (int)(mesh.bounds.size.x*textureAccurancy), (int)(mesh.bounds.size.y*textureAccurancy));
		
		Vector3 boundsMin = mesh.bounds.min;
		Vector3 boundsMax = mesh.bounds.max;
		
		// Debug.Log(boundsMax + " max -- min " + boundsMin);
		// Debug.Log("VerticesLength: " + vertices.Length);
		
		for (var i=0;i<vertices.Length;i++)
		{		
			// We don't want to color the bottom black
			if ( System.Convert.ToInt32(vertices[i].z) != 0)
			{
				// Everything divided by the intervall with modulo 0 is on the contour line
				// Convert.ToInt32 rounds to the next int (no wrong cut-off)
				if ( (System.Convert.ToInt32(vertices[i].z) % intervall) == 0)
				{
					texture.SetPixel( (int)(vertices[i].x), (int)(vertices[i].y), Color.black);
					// texture.SetPixel( (int)(vertices[i].x*textureAccurancy), (int)(vertices[i].y*textureAccurancy), Color.black);
				}
				// Check if the vertice has neighbours and is not on the edge
				else if ( vertices[i].x > boundsMin.x && vertices[i].x < boundsMax.x
					&& vertices[i].y > boundsMin.y && vertices[i].y < boundsMax.y)
				{
					// Debug.Log(vertices[i].x + "UND Y:" + vertices[i].y);Debug.Log(0);
					int left = i-1;
					int right = i+1;
					if ( (System.Convert.ToInt32(vertices[left].z) / intervall) != (System.Convert.ToInt32(vertices[i].z) / intervall) )
					{
						// float modNeighbour = vertices[left].z % intervall;
						// float modI = vertices[i].z % intervall;
						
						// float correctPoint = Mathf.Lerp(vertices[left].z, vertices[i].z, () );
						// Debug.Log(correctPoint);
						
						// Debug.Log(neighbourLeft.z + "---" + vertices[i].z);
						// texture.SetPixel( (int)vertices[i].x, (int)vertices[i].y, Color.black);
					}
				}
			}
		}  
		
		texture.Apply();
		// meshRenderer.material.mainTexture = texture;
		meshRenderer.material.SetTexture("_ContourLineTex", texture);
		 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
