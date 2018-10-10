Shader "Hidden/LinkedListRendering"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_BackgroundTex("BackgroundTex", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Rendering" }
		Pass {

			ZWrite Off
			ZTest Always

			CGPROGRAM
			#pragma target 5.0

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _BackgroundTex;

			struct FragmentAndLinkBuffer_STRUCT
			{
				float4 pixelColor;
				float depth;
				uint next;
			};

			RWStructuredBuffer<FragmentAndLinkBuffer_STRUCT> FLBuffer : register(u1);
			RWByteAddressBuffer StartOffsetBuffer : register(u2);

			struct vs_input {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct ps_input {
				float2 uv : TEXCOORD0;
			};

			ps_input vert(vs_input v, out float4 outpos : SV_POSITION)
			{
				ps_input o;
				outpos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			//Pixel function returns a solid color for each point.
			fixed4 frag(ps_input i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{			
				float4 finalColor = float4(0, 0, 0, 0);
				// Retrieve current color from background texture 
				finalColor = tex2D(_BackgroundTex, i.uv);

				//ScreenParams is the display size, a 480*320 sized display got
				//_ScreenParams.x = 480 and _ScreenParams.y = 320
				//Fetch offset of first fragment for current pixel
				uint uStartOffsetAddress = 4 * ((_ScreenParams.x * screenPos.y) + screenPos.x);
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
