// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/AlphaBlending"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Alpha("Opacity", Range(0.0, 1.0)) = 0.5
		_Color("Main Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }

		Pass
		{
			ZWrite Off
			Cull Front

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0 

			float _Alpha;
			sampler2D _MainTex;
			float4 _Color;
			
			struct VertexData {
				float4 position : POSITION;
				float2 uv :TEXCOORD0;
			};

			struct FragmentInput {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			FragmentInput vert(VertexData v)
			{
				//calculate fragment data
				FragmentInput i;
				i.position = UnityObjectToClipPos(v.position);
				i.uv = v.uv;

				return i;
			}

			float4 frag(FragmentInput i) : COLOR
			{
				float3 color = tex2D(_MainTex, i.uv).rgb * _Color.rgb;

				return float4(color, _Alpha);
			}
			ENDCG
		}

		Pass
		{
			ZWrite Off
			Cull Back

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0

			float _Alpha;
			sampler2D _MainTex;
			float4 _Color;

			struct VertexData {
				float4 position : POSITION;
				float2 uv :TEXCOORD0;
			};

			struct FragmentInput {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			FragmentInput vert(VertexData v)
			{
				FragmentInput i;
				i.position = UnityObjectToClipPos(v.position);
				i.uv = v.uv;
				return i;
			}

			float4 frag(FragmentInput i) : COLOR
			{
				float3 color = tex2D(_MainTex, i.uv).rgb * _Color.rgb;
				return float4(color, _Alpha);
			}
			ENDCG
		}
	}
}
