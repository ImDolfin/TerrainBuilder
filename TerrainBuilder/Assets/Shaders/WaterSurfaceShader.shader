Shader "Custom/WaterSurfaceShader"
{
    Properties
    {
		_MainTex ("Texture", 2D) = "white" {}
		_NormalMap1 ("Normal Map 1", 2D) = "bump" {}
		_NormalMap2 ("Normal Map 2", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _NormalMap1;
		sampler2D _NormalMap2;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_NormalMap1;
			float2 uv_NormalMap2;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {			
			float3 normalmap1 = UnpackNormal( tex2D( _NormalMap1, IN.uv_NormalMap1 ));
			float3 normalmap2 = UnpackNormal( tex2D( _NormalMap2, IN.uv_NormalMap2 ));
			o.Albedo = tex2D( _MainTex, IN.uv_MainTex ).rgb;
			o.Normal = normalize( normalmap1 + normalmap2 );
        }
        ENDCG
    }
    FallBack "Diffuse"
}
