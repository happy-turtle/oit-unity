Shader "OrderIndependentTransparency/Unlit"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
	}
	SubShader
	{
        PackageRequirements {
			"org.happy-turtle.order-independent-transparency"
		}
		
		Tags{ "Queue" = "Geometry" }

		Pass {
			ZTest LEqual
			ZWrite Off
			ColorMask 0
			Cull Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5
			// #pragma enable_d3d11_debug_symbols

			#include "UnityCG.cginc"
    		#include "Packages/org.happy-turtle.order-independent-transparency/Shaders/LinkedListCreation.hlsl"

			sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color;

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
			float4 frag(v2f i, uint uSampleIdx : SV_SampleIndex) : SV_Target
			{
				// no lighting
				float4 col = tex2D(_MainTex, i.uv) * _Color;

				createFragmentEntry(col, i.vertex.xyz, uSampleIdx);

				return col;
			}
			ENDHLSL
		}
	}

    FallBack "Unlit/Transparent"
}