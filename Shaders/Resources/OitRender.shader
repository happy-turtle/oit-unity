Shader "Hidden/OitRender"
{
	Properties{
		_MainTex("Main Texture", 2DArray) = "white" {}
	}
	SubShader 
	{
		PackageRequirements {
			"org.happy-turtle.order-independent-transparency"
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
		    #include "Packages/org.happy-turtle.order-independent-transparency/Shaders/LinkedListRendering.hlsl"

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
	SubShader
	{
		PackageRequirements {
			"org.happy-turtle.order-independent-transparency"
			"com.unity.render-pipelines.universal"
		}
        Tags { "RenderPipeline" = "UniversalPipeline" }
		Pass {
            Name "URP Order-Independent Transparency Pass"
			ZTest Always
			ZWrite Off
			Cull Off
			Blend Off

			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment frag
			#pragma target 4.5
			// #pragma enable_d3d11_debug_symbols
			
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
			#include "Packages/org.happy-turtle.order-independent-transparency/Shaders/LinkedListRendering.hlsl"

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
	SubShader
	{
		PackageRequirements {
			"org.happy-turtle.order-independent-transparency"
		}
		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			Blend Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5
			// #pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"
			#include "Packages/org.happy-turtle.order-independent-transparency/Shaders/LinkedListRendering.hlsl"

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
}