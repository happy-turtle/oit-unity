Shader "Hidden/OitFullscreenRender"
{
	Properties{
		_MainTex("Main Texture", 2DArray) = "white" {}
	}
	SubShader {
		PackageRequirements {
			"com.unity.render-pipelines.high-definition"
		}
		Tags { "RenderPipeline" = "HighDefinitionRenderPipeline" }
		Pass {
            Name "HDRP Order-Independent Transparency Post Process"
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

			HLSLPROGRAM
            #pragma fragment frag
			#pragma vertex Vert
			#pragma target 5.0
		    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
			#pragma require randomwrite
			// #pragma enable_d3d11_debug_symbols

			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"
		    #include "../LinkedListRendering.hlsl"

		    float4 frag(Varyings input, uint uSampleIndex : SV_SampleIndex) : SV_Target
		    {
		        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		    	float4 col = float4 (0, 1, 0, 1);
				float depth = LoadCameraDepth(input.positionCS.xy);
				PositionInputs posInput = GetPositionInput(input.positionCS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
		        // Load the camera color buffer at the mip 0 if we're not at the before rendering injection point
		        if (_CustomPassInjectionPoint != CUSTOMPASSINJECTIONPOINT_BEFORE_RENDERING)
		            col = float4(CustomPassSampleCameraColor(posInput.positionNDC.xy, 0), 1);

		        return renderLinkedList(float4(col.r,col.g,col.b,1), input.positionCS.xy, uSampleIndex);
		    }
			ENDHLSL
		}
	}
	SubShader
	{
		PackageRequirements {
			"com.unity.render-pipelines.universal"
		}
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }
		Pass {
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

			//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "UnityCG.cginc"
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