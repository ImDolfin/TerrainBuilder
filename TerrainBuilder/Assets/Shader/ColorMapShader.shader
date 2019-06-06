Shader "Unlit/ColorMapShader"
{
	// Property Definition --> Visible in IDE
    Properties
    {
		// The input textures
		_ColorTex("Color Texture", 2D) = "normal" {}
		_ContourLineTex("_ContourLineTex", 2D) = "normal" {}
		
		// The Colors (if you want to create your own color gradient without color Map)
		// _TopColor ("Top Color", Color) = (1,0,0,1)				// red
		// _MidColor ("Middle Color", Color) = (1,0.92,0.016,1) 	// yellow
		// _BotColor ("Bottom Color", Color) = (0,1,0,1)			// green
		
		// Maximum Height which is possible with the mesh
		_TopHeight ("Top Height", Float) =  	500				
		
		// Reflectance of ambient light
		_Ka("Ambient Reflectance", Range(0, 1)) = 0.5

		// Reflectance of diffuse light
		_Kd("Diffuse Reflectance", Range(0, 1)) = 0.5
    }
	
    SubShader
    {
		// Shader Level Of Detail
        LOD 100

        Pass
        {
			// Subshaders use tags to tell how and when they expect to be rendered to the rendering engine.
			Tags { "RenderType"="Opaque" }
			Tags { "LightMode"="ForwardBase"}
			
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			
            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			
			float _Ka, _Kd;
			// fixed4 _TopColor, _MidColor, _BotColor ;
			float _TopHeight ;
			sampler2D _ColorTex, _ContourLineTex;
			
			// Vertex Shader inputs
			struct appdata 
			{
				float4 vertex : POSITION; // vertex position
                float2 uv : TEXCOORD0; // texture coordinate
				float3 normal : NORMAL;
			};

			// Exchange of data between Vertex and Fragment Shader
            struct v2f
            {
				float2 uv : TEXCOORD0; // texture coordinate
				float4 vertex : SV_POSITION; // clip space position
				float3 worldPos : TEXCOORD1;
				
				float4 amb: COLOR0;
				float4 diff: COLOR1;			
            };

			
			// VERTEX SHADER
            v2f vert (appdata v)
            {
                v2f o;
				o.uv = v.uv;
				
				// Transformation of Vertices from Object Coordinates to Clip Coordinates
                o.vertex = UnityObjectToClipPos(v.vertex);
				
				//transformation of normal vectors into world coordinates
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul (unity_ObjectToWorld, v.vertex);
				
				// evaluates the ambient light
				o.amb = float4(ShadeSH9(half4(worldNormal,1)),1);
				
				// Lambert Shader for diffuse light
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl* _LightColor0;
				
                return o;
            }
			
			
			// FRAGMENT SHADER
            fixed4 frag (v2f i) : COLOR
            {
				fixed4 col;

				// Coloring with the color Map
				col = tex2D(_ColorTex, (i.worldPos.y/_TopHeight) );
				
				// // Coloring without color Map
				// float _MidHeight = _TopHeight / 2;
				// float _BotHeight = 	0;
				// COLORING depending on height
				// //  lower Half  - transition from green to yellow
				// else if ( i.worldPos.y <= _MidHeight )
				// {
					// col = lerp( _MidColor, _BotColor, (1-(i.worldPos.y/_MidHeight)) );	// 2 + 2 thaz 4 - quick MATHS
				// }
				// // upper Half - transition from yellow to red
				// else if ( i.worldPos.y <= _TopHeight )
				// {
					// col = lerp( _MidColor, _TopColor, ((i.worldPos.y - _MidHeight)/ _MidHeight) );
				// }
				
				//sample the texture
                col *= tex2D(_ContourLineTex, i.uv);
				
				// Multiplication of the basic color with the ambient and diffusion components
				col *= (_Ka*i.amb + _Kd*i.diff );
				
                return col;
            }
		

		ENDCG
        }

	}	
}


