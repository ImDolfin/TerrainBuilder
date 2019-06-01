using UnityEngine;

public class WaterMovementScript : MonoBehaviour
{
    float xScroll1;
	float yScroll1;
	float xScroll2;
	float yScroll2;
    Renderer r;

    void Start()
    {
        r = GetComponent<Renderer> ();
    }

    void Update()
    {
		// Get the X and Y setting of both Normal Maps from the shader 
		// (adjustable over the Unity IDE)
		xScroll1 = r.material.GetFloat("_xScroll1");
		yScroll1 = r.material.GetFloat("_yScroll1");
		xScroll2 = r.material.GetFloat("_xScroll2");
		yScroll2 = r.material.GetFloat("_yScroll2");
		
		// Calculate the X/Y-Offset of both Normal Maps by multiplying 
		// the time that passed since the beginning with the currently
		// set scrolling speeds
        float xOffset1 = Time.time * xScroll1;
		float yOffset1 = Time.time * yScroll1;
		float xOffset2 = Time.time * xScroll2;
		float yOffset2 = Time.time * yScroll2;
		
		// Apply the calculated offset to both Normal Maps which creates
		// a wave animation
        r.material.SetTextureOffset("_NormalMap1", new Vector2(xOffset1, yOffset1));
		r.material.SetTextureOffset("_NormalMap2", new Vector2(xOffset2, yOffset2));
    }
}