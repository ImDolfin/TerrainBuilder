Shader "Unlit/LambertShader"
{
	// Property Definition --> Visible in IDE
    Properties
    {
		// The input texture
        _MainTex ("Texture", 2D) = "white" {}
	
		// Definition of the base color
		_Color ("Base Color", Color) = (1,1,1,1)
		
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
			fixed4 _Color;
			float _Ka, _Kd;

            // struct appdata
            // {
                // float4 vertex : POSITION;
                // float2 uv : TEXCOORD0;
            // };

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
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				
				// Multiplication of the basic color with the ambient and diffusion components
				col *= (_Ka*i.amb + _Kd*i.diff );
				
                return col;
            }
            ENDCG
        }
    }
}
