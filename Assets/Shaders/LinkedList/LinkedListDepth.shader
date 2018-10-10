Shader "Hidden/LinkedListDepth" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	SubShader{

		Tags{ "RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderType" = "Depth" }

		Pass{

			Cull Front

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _LastCameraDepthTexture;

			struct a2v
			{
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				float z : TEXCOORD1;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;

				if (_ProjectionParams.x < 0)
					o.pos.y = 1 - o.pos.y;

				//Camera space-depth
				o.z = abs(UnityObjectToViewPos(v.pos).z);

				return o;
			}

			float4 frag(v2f i) : SV_Target{
				//write z value to render texture
				float z = Linear01Depth(i.z);
				return z;
			}
			ENDCG
		}
	}
}
