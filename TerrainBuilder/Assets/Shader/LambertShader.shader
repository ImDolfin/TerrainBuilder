Shader "Unlit/LambertShader"
{
	// Property Definition --> Visible in IDE
    Properties
    {
		// The input texture
        _MainTex ("Texture", 2D) = "white" {}
		_ContourLineTex("_ContourLineTex", 2D) = "normal" {}
		
		_TopColor ("Top Color", Color) = (1,0,0,1)				// red
		_MidColor ("Middle Color", Color) = (1,0.92,0.016,1) 	// yellow
		_BotColor ("Bottom Color", Color) = (0,1,0,1)			// green
		_WatColor ("Water Color", Color) = (0,0,1,1)			// blue
		
		_BotHeight ("Bottom Height", Float) = 	0
		_MidHeight ("Middle Height", Float) = 	250
		_TopHeight ("Top Height", Float) =  	500
		
		// increases the mid color surface
		_MidColorExtender ("Middle Color Extender Factor", Float) = 0.5
		
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
			
			// float4 _MainTex_ST;
			sampler2D _MainTex;
			float _Ka, _Kd;

			// Exchange of data between Vertex and Fragment Shader
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION; //Transfer of the converted vertex positions in homogeneous coordinates
				
				float4 amb: COLOR0;
				float4 diff: COLOR1;
            };

			
			// VERTEX SHADER
			// standard struct appdata_base: position, normal and one texture coordinate.
            v2f vert (appdata_base v)
            {
                v2f o;
				// Transformation of Vertices from Object Coordinates to Clip Coordinates
                o.vertex = UnityObjectToClipPos(v.vertex);
				
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = v.texcoord;
				
				//transformation of normal vectors into world coordinates
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				
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
				//fixed4 col = lerp(_BotColor,_TopColor, (i.uv.y / _TopHeight) );
				
				// sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				
				// Multiplication of the basic color with the ambient and diffusion components
				col *= (_Ka*i.amb + _Kd*i.diff );
				
                return col;
            }
		ENDCG
        }
    
		// Coloring the surface depending on the height
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Lambert		
	  
		fixed4 _TopColor, _MidColor, _BotColor, _WatColor;
		float _BotHeight, _MidHeight, _TopHeight, _MidColorExtender;
		sampler2D _ContourLineTex;
			
		struct Input {
			float3 worldPos;
			float2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {			 
			fixed4 col;
			
			// Water
			if(IN.worldPos.y <= 1)
				col = _WatColor;
			// lower half - transition from green to yellow
			if(IN.worldPos.y <= _MidHeight && IN.worldPos.y >= _BotHeight)
				col = lerp(_MidColor, _BotColor, (1-(IN.worldPos.y/_MidHeight))*_MidColorExtender);	// 2 + 2 thaz 4 - quick MATHS
			// upper half - transition from yellow to red
			if(IN.worldPos.y > _MidHeight)
				col = lerp(_MidColor, _TopColor, ((IN.worldPos.y - _MidHeight)/ _MidHeight)*_MidColorExtender);
		
			o.Albedo = col.rgb;
			o.Albedo *= tex2D(_ContourLineTex, IN.uv_MainTex).rgb;
			// o.Albedo += _ContourLineTex;
			}
		ENDCG
		}
	Fallback "Diffuse"
	
}
