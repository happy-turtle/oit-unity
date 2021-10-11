Shader "Hidden/LinkedListRendering"
{
	Properties{
		_MainTex("BackgroundTex", 2D) = "white" {}
	}
	SubShader
	{
		// Tags{ "Queue" = "Transparent" }

		Pass {
			ZTest LEqual
			// ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			// #pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"

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
				float4 pixelColor;
				float depth;
				uint next;
			};

			StructuredBuffer<FragmentAndLinkBuffer_STRUCT> FLBuffer;
			ByteAddressBuffer StartOffsetBuffer;

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
			fixed4 frag(v2f i) : SV_Target
			{			
				float4 finalColor = float4(0, 0, 0, 0);
				// Retrieve current color from background texture 
				finalColor = tex2D(_MainTex, i.uv);

				// //ScreenParams is the display size
				//Fetch offset of first fragment for current pixel
				uint uStartOffsetAddress;
				#if UNITY_UV_STARTS_AT_TOP
					uStartOffsetAddress = 4 * ((_ScreenParams.x * (_ScreenParams.y - i.vertex.y - 0.5)) + (i.vertex.x - 0.5));
				#else
					uStartOffsetAddress = 4 * ((_ScreenParams.x * (i.vertex.y - 0.5)) + (i.vertex.x - 0.5));
				#endif
				uint uOffset = StartOffsetBuffer.Load(uStartOffsetAddress);

				static FragmentAndLinkBuffer_STRUCT SortedPixels[8];

				// Parse linked list for all pixels at this position
				// and store them into temp array for later sorting
				int nNumPixels = 0;
				while (uOffset != 0)
				{
					//Retrieve pixel at current offset
					SortedPixels[nNumPixels] = FLBuffer[uOffset];
					nNumPixels += 1;

					uOffset = (nNumPixels >= 8) ? 0 : FLBuffer[uOffset].next;
				}

				//Sort pixels in depth
				//with insertion sort
				for (int i = 0; i < nNumPixels - 1; i++)
				{
					for (int j = i + 1; j > 0; j--)
					{
						if (SortedPixels[j - 1].depth > SortedPixels[j].depth)
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
					float4 vPixColor = SortedPixels[k].pixelColor;

					// Manual blending between current fragment and previous one
					finalColor.rgb = lerp(finalColor.rgb, vPixColor.rgb, vPixColor.a);
				}

				return finalColor;
			}
			ENDCG
		}
	}
}
