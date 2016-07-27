Shader "MSP/CountryColoredDiffuse" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _MainColor ("Country Color", Color) = (0.5,0.5,0.5,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		half3 _MainColor;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = lerp(c.rgb,c.rgb*_MainColor,c.a);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
