Shader "Hidden/OitFullscreenRender"
{
	Properties{
		_MainTex("BackgroundTex", 2DArray) = "white" {}
	}
	SubShader
	{
		Tags{ "RenderPipeline" = "UniversalPipeline" }
		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			Blend Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma require randomwrite
			// #pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"
			#include "../LinkedListRendering.hlsl"

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			//Pixel function returns a solid color for each point.
			fixed4 frag(v2f i, uint uSampleIndex : SV_SampleIndex) : SV_Target
			{
				// Retrieve current color from background texture
				float4 col = tex2D(_MainTex, i.uv);

				return renderLinkedList(col, i.vertex.xy, uSampleIndex);
			}
			ENDHLSL
		}
	}
	SubShader {
		PackageRequirements {
			"com.unity.render-pipelines.core"
			"com.unity.render-pipelines.high-definition"
		}
		Tags{ "RenderPipeline" = "HDRenderPipeline" }
		Pass {
            Name "HDRP Order-Independent Transparency Post Process"
            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

			HLSLPROGRAM
            #pragma fragment CustomPostProcess
            #pragma vertex Vert
			#pragma target 5.0
		    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
			// #pragma enable_d3d11_debug_symbols

		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
		    #include "../LinkedListRendering.hlsl"

		    struct Attributes
		    {
		        uint vertexID : SV_VertexID;
		        UNITY_VERTEX_INPUT_INSTANCE_ID
		    };

		    struct Varyings
		    {
		        float4 positionCS : SV_POSITION;
		        float2 texcoord   : TEXCOORD0;
		        UNITY_VERTEX_OUTPUT_STEREO
		    };

		    Varyings Vert(Attributes input)
		    {
		        Varyings output;
		        UNITY_SETUP_INSTANCE_ID(input);
		        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
		        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
		        return output;
		    }

		    // List of properties to control your post process effect
		    TEXTURE2D_X(_MainTex);

		    float4 CustomPostProcess(Varyings input, uint uSampleIndex : SV_SampleIndex) : SV_Target
		    {
		        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		        // Note that if HDUtils.DrawFullScreen is used to render the post process, use ClampAndScaleUVForBilinearPostProcessTexture(input.texcoord.xy) to get the correct UVs
		        float4 col = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, input.texcoord);

		        // return renderLinkedList(col, input.positionCS.xy, uSampleIndex);
		    	return col.rgga;
		    }
			ENDHLSL
		}
	}
}