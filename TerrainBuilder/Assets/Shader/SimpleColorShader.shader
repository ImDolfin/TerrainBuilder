Shader "Unlit/SimpleColorShader"
{
	// Tutorial - Vertex und Fragment Shader examples: https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
	// DOC: How to write vertex and fragment shaders: https://docs.unity3d.com/Manual/SL-ShaderPrograms.html

	// Property Definition --> Visible in IDE
	Properties
	{
		_Color ("Albedo", Color) = (1,1,1,1)
	}

	// A Shader can contain one or more SubShaders, which are primarily used to implement shaders for different GPU capabilities
	SubShader
	{
		// Subshaders use tags to tell how and when
		// they expect to be rendered to the rendering engine.
		// https://docs.unity3d.com/Manual/SL-SubShaderTags.html
		Tags { "RenderType"="Opaque" }

		// Each SubShader is composed of a number of passes, and each Pass represents an execution of the vertex and fragment
		// code for the same object rendered with the material of the shader
		Pass
		{
			// CGPROGRAM ... ENDCG
			// These keywords surround portions of HLSL code within the vertex and fragment shaders
			CGPROGRAM

			// Definition shaders used and their function names
			#pragma vertex vert
			#pragma fragment frag

			// Builtin Includes
			// https://docs.unity3d.com/Manual/SL-BuiltinIncludes.html
			#include "UnityCG.cginc"

			// define properties
			fixed4 _Color;

			// Input Struct for Vertex
			struct appdata
			{
				float4 vertex : POSITION;
			};

			// struct to pass Data from Vertex Sahder to Fragment Shader
			struct v2f
			{
				// SV_POSITION: Shader semantic for position in Clip Space: https://docs.unity3d.com/Manual/SL-ShaderSemantics.html
				float4 vertex : SV_POSITION;
			};

			// VERTEX SHADER
			// https://docs.unity3d.com/Manual/SL-VertexProgramInputs.html
			// http://wiki.unity3d.com/index.php?title=Shader_Code
			v2f vert (appdata v)
			{
				v2f o;

				// Convert Vertex Data from Object to Clip Space
				// https://docs.unity3d.com/Manual/SL-BuiltinFunctions.html
				o.vertex = UnityObjectToClipPos(v.vertex);

				return o;
			}

			// FRAGMENT / PIXEL SHADER
			// SV_Target: Shader semantic render target (SV_Target = SV_Target0): https://docs.unity3d.com/Manual/SL-ShaderSemantics.html
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = _Color;

				return col;
			}
			ENDCG
		}
	}
}
