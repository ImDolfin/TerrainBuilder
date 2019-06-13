Shader "Custom/ColorAndWaterShader"
{

	Properties
	{
		// Three texture inputs for the main texture and the two normal maps 
		_WaterTex("Water Texture", 2D) = "white" {} //water
        _NormalMap1 ("Normal Map 1", 2D) = "bump" {}
        _NormalMap2 ("Normal Map 2", 2D) = "bump" {}		
		// The colormap input textures
		_ColorTex("Color Texture", 2D) = "normal" {}
		_ContourLineTex("_ContourLineTex", 2D) = "normal" {}
		
		// Four scrollbar values which allow adjusting the scrolling speed of both X/Y-directions
		// from both Normal Maps, resulting in different wave speeds.
		_xScroll1 ("X Speed 1", Range(-0.5,0.5)) = 0.1
        _yScroll1 ("Y Speed 1", Range(-0.5,0.5)) = 0.09
        _xScroll2 ("X Speed 2", Range(-0.5,0.5)) = 0.08
        _yScroll2 ("Y Speed 2", Range(-0.5,0.5)) = 0.09

		// Three reflectance components which allow adjusting each component of the Phong Shading,
		// setting the share of each component in the resulting Phong Shading. 
		// See fragment shader for usage
		_Ka("Ambient Reflectance", Range(0, 1)) = 0.5
		_Kd("Diffuse Reflectance", Range(0, 1)) = 0.5
		_Ks("Specular Reflectance", Range(0, 1)) = 0.5
		// One additional component for the specular reflection, which adjusts the size of the specular
		// reflection itself, while _Ks only adjusts the share of the specular component in the Phong Shading
		_Shininess("Shininess", Range(0.1, 1000)) = 90
		
		// Maximum Height which is possible with the mesh
		_TopHeight ("Top Height", Float) =  	500		

	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			// This tag indicates forward rendering by rendering main directional light,
			// ambient, lightmaps and reflections in a single pass
			// -> passes directional light data into shader over built-in variables
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct v2f
			{
				// Clip space position
				float4 vertex : SV_POSITION;
				// Per-vertex color
				float4 color : COLOR;
				// UV coordinates (first to seventh)
				float2 uv : TEXCOORD0;
				half3 worldSpaceViewDirection : TEXCOORD1;
				float2 uv_NormalMap1 : TEXCOORD2;
                float2 uv_NormalMap2 : TEXCOORD3;
				half3 tangentX : TEXCOORD4;
				half3 tangentY : TEXCOORD5;
				half3 tangentZ : TEXCOORD6;
				float3 worldPosition : TEXCOORD7;
				float3 normal : NORMAL;
			};

			fixed4 _NormalMap1_ST, _NormalMap2_ST, _WaterTex_ST;
			float _Ka, _Kd, _Ks;
			float _Shininess;
			sampler2D _WaterTex;
			sampler2D _NormalMap1;
			sampler2D _NormalMap2;
			float _TopHeight ;
			sampler2D _ColorTex, _ContourLineTex;


			v2f vert(appdata_full v)
			{
				v2f o;
				o.color = v.color;
				o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				// Transform vertex from object space to camera clip space
				o.vertex = UnityObjectToClipPos(v.vertex);
				// Retreive world space direction from object space vertex position towards camera (and normalize it)
				o.worldSpaceViewDirection = normalize(WorldSpaceViewDir(v.vertex));
				
				// Get the vertex normal and tangent, and apply the rotation of the inverse transpose matrix to them
				half3 normal = UnityObjectToWorldNormal(v.normal);
				half3 tangent = UnityObjectToWorldNormal(v.tangent);
				// Calculate the vertex bitangent with the cross product of its tangent and normal
				half3 bitangent = cross(tangent, normal);

				// Create a tangent space matrix
				// Each of the three vectors contains the same component of the three previously
				// calculated vectors (normal, tangent, bitangent) in tangent space
				o.tangentX = half3(tangent.x, bitangent.x, normal.x);
				o.tangentY = half3(tangent.y, bitangent.y, normal.y);
				o.tangentZ = half3(tangent.z, bitangent.z, normal.z);

				// Use TRANSFORM_TEX macro from UnityCG.cginc to make sure texture scale and offset is applied correctly
				// Apply it to Normal Maps as well in order to make them shiftable by script (otherwise the Offset property won't have any effect)
				o.uv = TRANSFORM_TEX(v.texcoord, _WaterTex);
				o.uv_NormalMap1 = TRANSFORM_TEX(v.texcoord, _NormalMap1);
                o.uv_NormalMap2 = TRANSFORM_TEX(v.texcoord, _NormalMap2);
				
				o.normal = v.normal;

				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				// Get world position
				float3 worldPos = i.worldPosition;
				
				fixed4 col;
				half3 worldSpaceNormal;
				if (worldPos.y >= 0.1)
				{
					// Coloring with the color Map
					col = tex2D(_ColorTex, (worldPos.y/_TopHeight) );
					//sample the texture
					col *= tex2D(_ContourLineTex, i.uv);
					worldSpaceNormal = i.normal;
				}
				else if ((worldPos.y < 0.1) & (worldPos.y > -0.1))
				{
					// Main color from the provided texture (usually flat color, but doesn't have to be)
					col = tex2D(_WaterTex, i.uv);
					// Sample and encode Normal Maps
					half3 normal1 = UnpackNormal(tex2D(_NormalMap1, i.uv_NormalMap1));
					half3 normal2 = UnpackNormal(tex2D(_NormalMap2, i.uv_NormalMap2));
					
					// Add normal maps and normalize them
					half3 addedNormal = normalize(normal1 + normal2);
					
					// Transform normal from tangent to world space by calculating the dot 
					// product of the tangent space vectors and the combined normal maps
					worldSpaceNormal.x = dot(i.tangentX, addedNormal);
					worldSpaceNormal.y = dot(i.tangentY, addedNormal);
					worldSpaceNormal.z = dot(i.tangentZ, addedNormal);
				}
				
				// From here on, the phong shading will be calculated
				
				// The ambient component provides base illumination by applying a minimum amount of light
				float4 amb = float4(ShadeSH9(half4(worldSpaceNormal, 1)), 1);

				// nl is the dot product of the normal vector n and the vector that is pointing towards 
				// the light source l, usually with max(n dot l, 0) applied to it. Here, saturate() was
				// used instead of max() because saturate(x) works faster than max(0, x) in most cases.
				half nl = saturate(dot(worldSpaceNormal, _WorldSpaceLightPos0.xyz));
				
				// The diffuse component provides the light reflection on mat surfaces, 
				// independent from the camera's point of view.
				float4 diff = nl * _LightColor0;

				// Here the components for the specular component are calculated from the 
				// point of view, the light source direction and the normal vector
				float3 worldSpaceReflection = reflect(normalize(-_WorldSpaceLightPos0.xyz), worldSpaceNormal);
				half re = pow(max(dot(worldSpaceReflection, i.worldSpaceViewDirection), 0), _Shininess);
				
				// The specular component provides the directed reflection on specular surfaces,
				// dependent on the point of view.
				float4 spec = re * _LightColor0;

				// Adding the ambient, diffuse and specular components to the col vector in order 
				// to create the Phong Shading. The _Ka, _Kd and _Ks values are adaptable over the
				// Unity IDE which makes adjusting the dedicated components very easy.
				col *= (_Ka * amb + _Kd * diff);
				
				if ((worldPos.y < 0.1) & (worldPos.y > -0.1))
				{
					col += _Ks * spec;
				}

				return saturate(col);
			}
			ENDCG
		}

	}
}
