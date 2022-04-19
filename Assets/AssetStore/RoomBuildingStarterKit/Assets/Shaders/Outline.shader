Shader "RoomBuildingStarterKit/Outline/Outline"
{
	Properties
	{
		[HideInInspector] _MainTex("Main Texture", 2D) = "white" {}
		[HideInInspector] _AlphaFactor("Alpha Factor", Range(0, 1)) = 0
	}

	SubShader
	{
		CGINCLUDE

		#include "UnityCG.cginc"
		
		struct appdata_t
		{
			float4 vertex    : POSITION;
			float2 uv        : TEXCOORD0;
		};

		struct v2f
		{
			float4 position  : SV_POSITION;
			float2 uv        : TEXCOORD0;
		};

		v2f vert(appdata_t i)
		{
			v2f o;
			o.position = UnityObjectToClipPos(i.vertex);
			o.uv = i.uv;
			return o;
		}

		ENDCG

		Tags { "RenderType" = "Opaque" }

		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float2 _BlurDirection = float2(1, 0);

			static const half4 Weights[9] =
			{
				half4(0,0.0204001988,0.0204001988,0),
				half4(0,0.0577929595,0.0577929595,0),
				half4(0,0.1215916882,0.1215916882,0),
				half4(0,0.1899858519,0.1899858519,0),
				half4(1,0.2204586031,0.2204586031,1),
				half4(0,0.1899858519,0.1899858519,0),
				half4(0,0.1215916882,0.1215916882,0),
				half4(0,0.0577929595,0.0577929595,0),
				half4(0,0.0204001988,0.0204001988,0)
			};

			half4 frag(v2f i) : SV_Target
			{
				float2 step = _MainTex_TexelSize.xy * _BlurDirection;
				float2 uv = i.uv - step * 4;
				half4 c = 0;
				for (int j = 0; j < 9; ++j)
				{
					c += tex2D(_MainTex, uv) * Weights[j];
					uv += step;
				}

				return c;
			}
			
			ENDCG
		}

		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float3 _OutlineColor;
			float _AlphaFactor;

			half4 frag(v2f i) : SV_Target
			{
				half4 c = tex2D(_MainTex, i.uv);
				float alpha = saturate((c.b - c.r) * 10) * _AlphaFactor;
				return half4(_OutlineColor, alpha);
			}

			ENDCG
		}
	}
}