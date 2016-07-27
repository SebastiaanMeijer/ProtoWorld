Shader "MSP/SolidColorDiffuse" {
	Properties {
        _Color ("Main Color", Color) = (0.16,0.34,0.36,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull off
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
		
        half3 _Color;
        
        struct Input {
        	float4 color : COLOR;
        };

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
