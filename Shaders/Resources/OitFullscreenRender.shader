Shader "Hidden/OitFullscreenRender"
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
			
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma require randomwrite
			// #pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"
			#include "../LinkedListRendering.hlsl"

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

				// return renderLinkedList(col, i.vertex.xy, uSampleIndex);
				return col.rgga;
			}
			ENDHLSL
		}
	}
}
