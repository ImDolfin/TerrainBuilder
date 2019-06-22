using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightMode : MonoBehaviour
{
	/* A script that includes a toggle in the mesh inspector, which switches
	the water texture and a color map to a lava texture and a dark, cave-like
	colormap. */
	
	public bool nightModeToggle = false;
	private Texture matTex;
	private Texture waterTex;
	private Texture lavaTex;
	private Texture dayColorMap;
	private Texture nightColorMap;
	private Renderer r;
	
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Renderer>();
		waterTex = Resources.Load<Texture2D>("Textures/WaterTexture");
		lavaTex = Resources.Load<Texture2D>("Textures/LavaTexture");
		dayColorMap = Resources.Load<Texture2D>("Textures/dayColorMap");
		nightColorMap = Resources.Load<Texture2D>("Textures/nightColorMap");
    }

    // Update is called once per frame
    void Update()
    {
		matTex = r.material.GetTexture("_WaterTex");
		
		if(nightModeToggle /*&& (matTex == waterTex)*/){
			r.material.SetTexture("_WaterTex", lavaTex);
			r.material.SetTexture("_ColorTex", nightColorMap);
			r.material.SetFloat("_xScroll1", 0.007f);
			r.material.SetFloat("_yScroll1", 0.005f);
			r.material.SetFloat("_xScroll2", 0.014f);
			r.material.SetFloat("_yScroll2", 0.01f);
		}
		else if(!nightModeToggle /*&& (matTex == lavaTex)*/){
			r.material.SetTexture("_WaterTex", waterTex);
			r.material.SetTexture("_ColorTex", dayColorMap);
			r.material.SetFloat("_xScroll1", 0.1f);
			r.material.SetFloat("_yScroll1", 0.09f);
			r.material.SetFloat("_xScroll2", 0.03f);
			r.material.SetFloat("_yScroll2", 0.04f);
		}
    }
}
