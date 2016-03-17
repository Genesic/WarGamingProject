Shader "Custom/TwoScreenShader"
{
	Properties
	{
		_UpperTex ("Texture", 2D) = "white" {}
		_DownTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				
				#if UNITY_UV_STARTS_AT_TOP
				o.uv.y = 1-o.uv.y;
				#endif
				
				return o;
			}
			
			sampler2D _UpperTex;
			sampler2D _DownTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 upper = tex2D(_UpperTex, float2(i.uv.x, i.uv.y - 0.5));
				fixed4 down = tex2D(_DownTex, i.uv);
				
				float Weight = saturate((i.uv.y - 0.5) * 100);
				return lerp(down, upper, Weight);
			}
			ENDCG
		}
	}
}
