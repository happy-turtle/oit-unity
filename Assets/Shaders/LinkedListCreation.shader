Shader "Hidden/LinkedListCreation"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Creation" }
		Pass {

			ZWrite Off
			ZTest Always
			ColorMask 0
			Cull Off

			CGPROGRAM
			#pragma target 5.0
			// #pragma enable_d3d11_debug_symbols

			#pragma vertex vert
			#pragma fragment frag

			#include "Lighting.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _CameraDepthTexture;
			fixed4 _Color;

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
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float z : TEXCOORD1;
				float3 lightDir: TEXCOORD2;
				float3 viewDir : TEXCOORD3;
				float4 screenPos : TEXCOORD4;
			};

			v2f vert(vs_input v, out float4 outpos : SV_POSITION)
			{
				v2f o;
				outpos = UnityObjectToClipPos(v.vertex);
				//this is to flip the image horizontally
				//not sure why this is necessary
				if (_ProjectionParams.x < 0)
					outpos.y = 1 - outpos.y;

				o.uv = v.texcoord;

				TANGENT_SPACE_ROTATION;
				// Transform the light direction from object space to tangent space
				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
				// Transform the view direction from object space to tangent space
				o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;

				o.screenPos = ComputeScreenPos(v.vertex);


				//Camera space-depth
				o.z = abs(UnityObjectToViewPos(v.vertex).z);

				return o;
			}

			//Pixel function returns a solid color for each point.
			float4 frag(v2f i) : SV_Target
			{
				//get depth of opaque objects and fragment depth
				float depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
				float z = Linear01Depth(i.z);

				//only save fragment to buffer if nothing is in front
				if (depth < z)
				{
					//lighting calculation
					fixed3 tangentLightDir = normalize(i.lightDir);
					fixed3 tangentViewDir = normalize(i.viewDir);
					fixed3 tangentNormal = UnpackNormal(tex2D(_BumpMap, i.uv));

					fixed4 albedo = tex2D(_MainTex, i.uv) * _Color;
					fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo.rgb;
					fixed3 diffuse = _LightColor0.rgb * albedo.rgb * max(0, dot(tangentNormal, tangentLightDir));

					fixed4 col = fixed4(ambient + diffuse, albedo.a);

					//Retrieve current Pixel count and increase counter
					uint uPixelCount = FLBuffer.IncrementCounter() + 1;

					//ScreenParams ist die Groesse des Displays, ein 480*320 grosser Display hat
					//_ScreenParams.x = 480 und _ScreenParams.y
					//calculate bufferAddress
					uint uStartOffsetAddress = 4 * ((_ScreenParams.x * i.screenPos.y) + i.screenPos.x);
					uint uOldStartOffset;
					StartOffsetBuffer.InterlockedExchange(uStartOffsetAddress, uPixelCount, uOldStartOffset);

					//add new Fragment Entry in FragmentAndLinkBuffer
					FragmentAndLinkBuffer_STRUCT Element;
					Element.pixelColor = col;
					Element.depth = Linear01Depth(i.z);
					Element.next = uOldStartOffset;
					FLBuffer[uPixelCount] = Element;
				}

				//write color 0 to accumulate texture to clear it
				return float4(0, 0, 0, 0);
			}
			ENDCG
		}
	}
}
