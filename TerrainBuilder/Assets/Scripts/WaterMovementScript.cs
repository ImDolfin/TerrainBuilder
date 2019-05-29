using UnityEngine;

public class WaterMovementScript : MonoBehaviour
{
    float xScroll1 = 0.1f;
	float yScroll1 = 0.1f;
	float xScroll2 = -0.1f;
	float yScroll2 = -0.1f;
    Renderer r;

    void Start()
    {
        r = GetComponent<Renderer> ();
    }

    void Update()
    {
        float xOffset1 = Time.time * xScroll1;
		float yOffset1 = Time.time * yScroll1;
		float xOffset2 = Time.time * xScroll2;
		float yOffset2 = Time.time * yScroll2;
        r.material.SetTextureOffset("_NormalMap1", new Vector2(xOffset1, yOffset1));
		r.material.SetTextureOffset("_NormalMap2", new Vector2(xOffset2, yOffset2));
    }
}