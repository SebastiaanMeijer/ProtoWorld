Shader "MSP/SolidColorUnlit" {
	Properties {
        _Color ("Main Color", Color) = (0.16,0.34,0.36,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull off
		
		CGPROGRAM
		#pragma surface surf Unlit
		
        half3 _Color;
        
        struct Input {
        	float4 color : COLOR;
        };
        
        half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten) {
			return half4(s.Albedo, s.Alpha);
		}

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color;
		}
		ENDCG
	} 
}
