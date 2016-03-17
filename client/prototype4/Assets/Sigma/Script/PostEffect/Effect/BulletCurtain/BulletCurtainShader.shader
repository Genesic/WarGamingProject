﻿Shader "Custom/BulletCurtainShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BulletTex ("Bullet Curtain Texture", 2D) = "white" {}
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
				
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _BulletTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainCol = tex2D(_MainTex, i.uv);
				fixed4 bulletCol = tex2D(_BulletTex, i.uv);
				
				fixed4 output = fixed4(1,1,1,1);
				
				return mainCol + bulletCol;
			}
			ENDCG
		}
	}
}
