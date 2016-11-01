// Original by Alan Zucconi (www.alanzucconi.com). Heavily edited to made it work with Unity 5.4 and DirectX 9.
Shader "Hidden/Heatmap" {
	Properties {
		_HeatTex("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				struct vertInput {
					float4 pos : POSITION;
				};
				
				struct vertOutput {
					float4 pos : POSITION;
					fixed3 worldPos : TEXCOORD1;
				};

				vertOutput vert(vertInput input) {
					vertOutput o;
					o.pos = mul(UNITY_MATRIX_MVP, input.pos);
					o.worldPos = mul(unity_ObjectToWorld, input.pos).xyz;
					return o;
				}

				// The number of heatmap agents.
				uniform int _Points_Length = 0;

				// The positions: (x, y, z) = position.
				uniform float3 _Points[1000];

				// The properties: x = radius, y = intensity.
				uniform float2 _Properties[1000];

				// The heatmap gradient.
				sampler2D _HeatTex;

				half4 frag(vertOutput output) : COLOR {
					half h = 0;
					
					// Loop through each section to stay under the limits imposed by Shader Model 3,
					// and calculate the contribution of each of the points per section.
					[loop] for(int index0 = 0; index0 < 100 && index0 < _Points_Length; index0++) {
						half di = distance(output.worldPos, _Points[index0].xyz);

						half ri = _Properties[index0].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[index0].y;
					}
					[loop] for(int index1 = 100; index1 < 200 && index1 < _Points_Length; index1++) {
						half di = distance(output.worldPos, _Points[index1].xyz);

						half ri = _Properties[index1].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[index1].y;
					}
					[loop] for(int index2 = 200; index2 < 300 && index2 < _Points_Length; index2++) {
						half di = distance(output.worldPos, _Points[index2].xyz);

						half ri = _Properties[index2].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[index2].y;
					}
					[loop] for(int index3 = 300; index3 < 400 && index3 < _Points_Length; index3++) {
						half di = distance(output.worldPos, _Points[index3].xyz);

						half ri = _Properties[index3].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[index3].y;
					}
					[loop] for(int index4 = 400; index4 < 500 && index4 < _Points_Length; index4++) {
						half di = distance(output.worldPos, _Points[index4].xyz);

						half ri = _Properties[index4].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[index4].y;
					}
					[loop] for (int index5 = 500; index5 < 600 && index5 < _Points_Length; index5++) {
						half di = distance(output.worldPos, _Points[index5].xyz);

						half ri = _Properties[index5].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[index5].y;
					}
					[loop] for(int index6 = 600; index6 < 700 && index6 < _Points_Length; index6++) {
						half di = distance(output.worldPos, _Points[index6].xyz);

						half ri = _Properties[index6].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[index6].y;
					}
					[loop] for(int index7 = 700; index7 < 800 && index7 < _Points_Length; index7++) {
						half di = distance(output.worldPos, _Points[index7].xyz);

						half ri = _Properties[index7].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[index7].y;
					}
					[loop] for(int index8 = 800; index8 < 900 && index8 < _Points_Length; index8++) {
						half di = distance(output.worldPos, _Points[index8].xyz);

						half ri = _Properties[index8].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[index8].y;
					}
					[loop] for(int index9 = 900; index9 < 1000 && index9 < _Points_Length; index9++) {
						half di = distance(output.worldPos, _Points[index9].xyz);

						half ri = _Properties[index9].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[index9].y;
					}

					// Convert 0-1 range into a color using the heatmap gradient.
					h = saturate(h);
					half4 color = tex2D(_HeatTex, fixed2(h, 0.5));

					return color;
				}
			ENDCG
		}
	}
	Fallback "Diffuse"
}