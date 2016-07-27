Shader "MSP/LandBlend" {
	Properties {
        _ColorA ("Main Color A", Color) = (1.0,0.0,0.0,1)
        _ColorB ("Main Color B", Color) = (0.0,0.1,0.0,1)
		_MainTex ("Blend Texture (Only R used atm.)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
        half3 _ColorA;
        half3 _ColorB;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 m = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = m.r * _ColorA + (1-m.r) * _ColorB;
			o.Alpha = m.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
