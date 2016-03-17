Shader "Custom/SwayShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,0,0.5)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest LEQUAL
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			fixed4 _Color;

			fixed4 frag (v2f_img i) : SV_Target
			{
				return _Color;
			}
			ENDCG
		}
	}
}
