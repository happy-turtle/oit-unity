Shader "Hidden/LinkedListRendering"
{
	Properties{
		_MainTex("BackgroundTex", 2D) = "white" {}
	}
	SubShader
	{
		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			Blend Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma require randomwrite
			// #pragma enable_d3d11_debug_symbols

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

			struct FragmentAndLinkBuffer_STRUCT
			{
				uint pixelColor;
				uint uDepthSampleIdx;
				uint next;
			};

			StructuredBuffer<FragmentAndLinkBuffer_STRUCT> FLBuffer : register(t0);
			ByteAddressBuffer StartOffsetBuffer : register(t1);

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
    			uint uStartOffsetAddress = 4 * (_ScreenParams.x * (i.vertex.y - 0.5) + (i.vertex.x - 0.5));
				uint uOffset = StartOffsetBuffer.Load(uStartOffsetAddress);

				FragmentAndLinkBuffer_STRUCT SortedPixels[MAX_SORTED_PIXELS];

				// Parse linked list for all pixels at this position
				// and store them into temp array for later sorting
				int nNumPixels = 0;
				while (uOffset != 0)
				{
					// Retrieve pixel at current offset
					FragmentAndLinkBuffer_STRUCT Element = FLBuffer[uOffset];
					uint uSampleIdx = UnpackSampleIdx(Element.uDepthSampleIdx);
					if (uSampleIdx == uSampleIndex)
					{
						SortedPixels[nNumPixels] = Element;
						nNumPixels += 1;
					}

					uOffset = (nNumPixels >= MAX_SORTED_PIXELS) ? 0 : FLBuffer[uOffset].next;
				}

				// Sort pixels in depth
				for (int i = 0; i < nNumPixels - 1; i++)
				{
					for (int j = i + 1; j > 0; j--)
					{
						float depth = UnpackDepth(SortedPixels[j].uDepthSampleIdx);
						float previousElementDepth = UnpackDepth(SortedPixels[j - 1].uDepthSampleIdx);
						if (previousElementDepth < depth)
						{
							FragmentAndLinkBuffer_STRUCT temp = SortedPixels[j - 1];
							SortedPixels[j - 1] = SortedPixels[j];
							SortedPixels[j] = temp;
						}
					}
				}

				// Rendering pixels
				for (int k = 0; k < nNumPixels; k++)
				{
					// Retrieve next unblended furthermost pixel
					float4 vPixColor = UnpackRGBA(SortedPixels[k].pixelColor);

					// Manual blending between current fragment and previous one
					col.rgb = lerp(col.rgb, vPixColor.rgb, vPixColor.a);
				}

				return col;
			}
			ENDCG
		}
	}
}
