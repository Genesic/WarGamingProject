Shader "Custom/ShockShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always Blend Off
		Fog { Mode off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			half4 _ShockCenter;
			half _ShockTime;

			fixed4 frag (v2f_img i) : SV_Target
			{
				half dist = distance(i.uv, _ShockCenter.xy);
				
				//dist = abs(dist - _SinTime.w);	// diff of wave cemter
				
				//half weight = saturate(1 - pow(dist * 10, 2)) * 0.1; // clamp 0 ~ 1
				
				//half2 diff = normalize(i.uv - half2(0.5,0.5));  
				
				//i.uv += ((diff * weight) / (dist * 40.0));
				
				//half4 col = tex2D(_MainTex, i.uv); 
				//col += ((col * weight) / (dist * 40.0)); 
				
				half shock = _ShockTime * _ShockCenter.z;
				half diff = dist - shock;	// diff of wave cemter
				half weight = saturate(1 - abs(diff) * 10); // clamp 0 ~ 0.1
				half scale = (1.0 - pow(abs(diff * 10), 2)); 
				
				//half2 dir = normalize(i.uv - _ShockCenter.xy);  
				half2 dir = normalize(_ShockCenter.xy - i.uv);  
				half coefficient = shock * dist * 40 + 0.01F;
				
				i.uv += ((dir * weight * diff * scale) / coefficient);
				
				half4 col = tex2D(_MainTex, i.uv); 
				
				col	 += ((col * weight * scale) / coefficient);
				
				return col; 
			}
			ENDCG
		}
	}
}
