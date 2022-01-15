Shader "Hidden/MLABRendering"
{
	Properties{
		_MainTex("BackgroundTex", 2D) = "white" {}
	}
	SubShader
	{
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma require randomwrite
			// #pragma enable_d3d11_debug_symbols
			#pragma multi_compile_fragment BUILT_IN POST_PROCESSING

			#include "UnityCG.cginc"
			#include "../OitUtils.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			struct FragmentBuffer_STRUCT
			{
				uint pixelColor;
				uint uDepthCoverage;
			};

			StructuredBuffer<FragmentBuffer_STRUCT> FragmentBuffer : register(t0);
			RWTexture2D<uint> ClearMask : register(u2);

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

				// Fetch offset of first fragment for current pixel
				uint uStartOffsetAddress;
#if POST_PROCESSING
				uStartOffsetAddress = 4 * (_ScreenParams.x * (i.vertex.y - 0.5) + (i.vertex.x - 0.5));
#else
				uStartOffsetAddress = 4 * (_ScreenParams.x * (_ScreenParams.y - i.vertex.y - 0.5) + (i.vertex.x - 0.5));
#endif

				if (ClearMask[i.vertex.xy] > 0) {
					// Rendering pixels
					for (int k = 0; k < MAX_SORTED_PIXELS; k++)
					{
						// Retrieve next unblended furthermost pixel
						float4 vPixColor = UnpackRGBA(FragmentBuffer[uStartOffsetAddress + k].pixelColor);

						// Manual blending between current fragment and previous one
						col.rgb = lerp(col.rgb, vPixColor.rgb, vPixColor.a);
					}
				}

				// clear buffer mask
				ClearMask[i.vertex.xy] = 0;

				return col;
			}
			ENDCG
		}
	}
}
