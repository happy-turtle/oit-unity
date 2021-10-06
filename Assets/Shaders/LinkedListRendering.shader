Shader "Hidden/LinkedListRendering"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("BackgroundTex", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Rendering" }
		Pass {

			ZWrite Off
			ZTest Always

			CGPROGRAM
			#pragma target 5.0
			#pragma enable_d3d11_debug_symbols

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct FragmentAndLinkBuffer_STRUCT
			{
				float4 pixelColor;
				float depth;
				uint next;
			};

			RWStructuredBuffer<FragmentAndLinkBuffer_STRUCT> FLBuffer : register(u1);
			RWByteAddressBuffer StartOffsetBuffer : register(u2);

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

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
				// uint uStartOffsetAddress = 4 * ((_ScreenParams.x * screenPos.y) + screenPos.x);
				uint uStartOffsetAddress = 4 * ((_ScreenParams.x * (i.vertex.y - 0.5)) + (i.vertex.x - 0.5));
				uint uOffset = StartOffsetBuffer.Load(uStartOffsetAddress);

				static FragmentAndLinkBuffer_STRUCT SortedPixels[8];

				// Parse linked list for all pixels at this position
				// and store them into temp array for later sorting
				int nNumPixels = 0;
				while (uOffset != 0)
				{
					//Retrieve pixel at current offset
					SortedPixels[nNumPixels++] = FLBuffer[uOffset];

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
