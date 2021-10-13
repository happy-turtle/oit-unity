Shader "OrderIndependentTransparency"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }

		Pass {
			ZTest LEqual
			ZWrite Off
			ColorMask 0
			Cull Off

			CGPROGRAM
			#pragma target 5.0
			// #pragma enable_d3d11_debug_symbols

			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"

			sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _BumpMap;
			fixed4 _Color;

			struct FragmentAndLinkBuffer_STRUCT
			{
				float4 pixelColor;
				float depth;
				uint next;
				uint coverage;
			};

			RWStructuredBuffer<FragmentAndLinkBuffer_STRUCT> FLBuffer : register(u1);
			RWByteAddressBuffer StartOffsetBuffer : register(u2);

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			[earlydepthstencil]
			float4 frag(v2f i, uint uCoverage : SV_COVERAGE) : SV_Target
			{				
				// ambient lighting
				float4 albedo = tex2D(_MainTex, i.uv) * _Color;
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				float4 col = float4(ambient + albedo, albedo.a);

				//Retrieve current Pixel count and increase counter
				uint uPixelCount = FLBuffer.IncrementCounter();

				//calculate bufferAddress
				uint uStartOffsetAddress = 4 * ((_ScreenParams.x * (i.vertex.y - 0.5)) + (i.vertex.x - 0.5));
				uint uOldStartOffset;
				StartOffsetBuffer.InterlockedExchange(uStartOffsetAddress, uPixelCount, uOldStartOffset);

				//add new Fragment Entry in FragmentAndLinkBuffer
				FragmentAndLinkBuffer_STRUCT Element;
				Element.pixelColor = col;
				Element.depth = Linear01Depth(i.vertex.z);
				Element.next = uOldStartOffset;
				Element.coverage = uCoverage;
				FLBuffer[uPixelCount] = Element;

				return col;
			}
			ENDCG
		}
	}
}
