using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightMode : MonoBehaviour
{
	/* A script that includes a toggle in the mesh inspector, which switches
	the water texture and a color map to a lava texture and a dark, cave-like
	colormap. */
	
	// Toggle for the Night Mode, public so it's visible in the inspector
	public bool nightModeToggle = false;
	
	// Texture variables for the currently set Texture, the Water Texture, 
	// the Lava Texture, the day Colormap and the night Colormap
	private Texture matTex;
	private Texture waterTex;
	private Texture lavaTex;
	private Texture dayColorMap;
	private Texture nightColorMap;
	
	private Renderer r;
	
    // Start is called before the first frame update
    void Start()
    {
		// Get Renderer and load the referenced Textures from Resources directory
        r = GetComponent<Renderer>();
		waterTex = Resources.Load<Texture2D>("Textures/WaterTexture");
		lavaTex = Resources.Load<Texture2D>("Textures/LavaTexture");
		dayColorMap = Resources.Load<Texture2D>("Textures/dayColorMap");
		nightColorMap = Resources.Load<Texture2D>("Textures/nightColorMap");
    }

    // Update is called once per frame
    void Update()
    {
		// Retrieve the currently set Texture from the Mesh Material
		matTex = r.material.GetTexture("_MainTex");
		
		// If the night mode is toggled and the texture hasn't been changed yet,
		// change textures and scrolling values to night mode.
		if(nightModeToggle && (matTex == waterTex)){
			r.material.SetTexture("_MainTex", lavaTex);
			r.material.SetTexture("_ColorTex", nightColorMap);
			r.material.SetFloat("_xScroll1", 0.007f);
			r.material.SetFloat("_yScroll1", 0.005f);
			r.material.SetFloat("_xScroll2", 0.014f);
			r.material.SetFloat("_yScroll2", 0.01f);
		}
		// If the night mode is not toggled and the texture hasn't been changed yet,
		// change textures and scrolling values to day mode.
		else if(!nightModeToggle && (matTex == lavaTex)){
			r.material.SetTexture("_MainTex", waterTex);
			r.material.SetTexture("_ColorTex", dayColorMap);
			r.material.SetFloat("_xScroll1", 0.1f);
			r.material.SetFloat("_yScroll1", 0.09f);
			r.material.SetFloat("_xScroll2", 0.03f);
			r.material.SetFloat("_yScroll2", 0.04f);
		}
    }
}
