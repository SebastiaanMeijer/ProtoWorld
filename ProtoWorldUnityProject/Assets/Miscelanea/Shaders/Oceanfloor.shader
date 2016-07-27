Shader "MSP/Oceanfloor" {
	Properties {
		_MainTex ("Depth (A)", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (0.51,0.76,0.85,1)
        _Sealevel ("Sealevel", Float) = -1.66
        _Seafloor ("Seafloor", Float) = -5.2
        _EdgeStength ("Edge Strength", Float) = 2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert noambient

		sampler2D _MainTex;
		half3 _MainColor;
		half _Sealevel, _Seafloor, _EdgeStength;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 m = tex2D (_MainTex, IN.uv_MainTex);
			half depthRange = saturate((IN.worldPos.y - _Seafloor) / (_Sealevel - _Seafloor));
			half edgeHighlight = (1 - m.a + depthRange) / 2;
			edgeHighlight *= edgeHighlight * 4;
			//half3 c = lerp(_DeepColor, _ShallowColor, edgeHighlight);
			half3 c = _MainColor * edgeHighlight;
			o.Albedo = c * depthRange * depthRange * depthRange * 0.5;// + depthRange - 0.3);
			o.Emission = c * (0.5 * depthRange + 0.5);// * UNITY_LIGHTMODEL_AMBIENT.rgb * 4;
			//o.Emission = c;//edgeHighlight;
			//o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
