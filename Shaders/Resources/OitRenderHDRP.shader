Shader "Hidden/OitRenderHDRP"
{
	Properties{
		_MainTex("Main Texture", 2DArray) = "white" {}
	}
	SubShader 
	{
		PackageRequirements {
			"com.unity.render-pipelines.high-definition"
		}
		Tags { "RenderPipeline" = "HDRenderPipeline" }
		Pass {
            Name "HDRP Order-Independent Transparency Pass"
            ZTest Always
            ZWrite Off
            Blend Off
            Cull Off

			HLSLPROGRAM
            #pragma fragment frag
			#pragma vertex Vert
			#pragma target 4.5
		    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
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

		        return renderLinkedList(col, input.positionCS.xy, uSampleIndex);
		    }
			ENDHLSL
		}
	}
}