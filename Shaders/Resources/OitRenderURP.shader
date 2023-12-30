Shader "Hidden/OitRenderURP"
{
	SubShader
	{
		PackageRequirements {
			"com.unity.render-pipelines.universal"
		}
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }
		Pass {
            Name "URP Order-Independent Transparency Pass"
			ZTest Always
			ZWrite Off
			Cull Off
			Blend Off

			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma require randomwrite
			// #pragma enable_d3d11_debug_symbols
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
			#include "../LinkedListRendering.hlsl"

			TEXTURE2D_X(_CameraOpaqueTexture);
			SAMPLER(sampler_CameraOpaqueTexture);

			//Pixel function returns a solid color for each point.
			half4 frag(Varyings input, uint uSampleIndex: SV_SampleIndex) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				// Retrieve current color from background texture
				float4 col = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, input.texcoord);

				return renderLinkedList(col, input.positionCS.xy, uSampleIndex);
			}
			ENDHLSL
		}
	}
}