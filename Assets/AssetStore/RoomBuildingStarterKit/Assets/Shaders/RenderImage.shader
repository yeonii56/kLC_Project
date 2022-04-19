Shader "RoomBuildingStarterKit/Outline/RenderImage"
{
	CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata_t
	{
		float4 vertex : POSITION;
		float2 uv     : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv     : TEXCOORD0;
	};

	v2f vert(appdata_t i)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(i.vertex);
		o.uv = i.uv;
		return o;
	}

	ENDCG

	SubShader
	{
		Pass
		{
			Blend One Zero
			ZTest LEqual
			Cull Off
			ZWrite Off
			Offset -0.02, 0

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			fixed4 frag(v2f i) : SV_Target
			{
				return float4(1, 1, 1, 1);
			}

			ENDCG
		}

		Pass
		{
			Blend One One
			BlendOp Max
			ZTest Always
			ZWrite Off
			Cull Off
			ColorMask GBA
			Offset -0.02, 0

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			fixed4 frag(v2f i) : SV_Target
			{
				return float4(0, 0, 1, 1);
			}

			ENDCG
		}
	}
}