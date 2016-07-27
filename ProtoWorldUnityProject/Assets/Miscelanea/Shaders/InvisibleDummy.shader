Shader "MSP/InvisibleDummy" {
	SubShader {
	    Pass {
	
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		
		
		float4 vert(float4 v:POSITION) : SV_POSITION {
		    return fixed4(0.0,0.0,0.0,1.0);
		}
		
		fixed4 frag() : COLOR {
			clip(-1.0);
			return fixed4(1.0,0.0,0.0,1.0);
		}
		ENDCG
	    }
	}
} 