/*

This file is part of ProtoWorld.

ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer.

*/

/*
* Micro/Macro Visualization Module
*
* Furkan Sonmez
* Berend Wouda
* 
* Contains elements of a tutorial by Alan Zucconi (www.alanzucconi.com).
*/

Shader "Hidden/Heatmap" {
	Properties {
		_HeatTex("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			CGPROGRAM
				// Only supports Windows builds.
				#if SHADER_API_D3D11_9X
					#define MAXIMUM_NUMBER_OF_POINTS 13
				#elif SHADER_API_D3D9
					#define MAXIMUM_NUMBER_OF_POINTS 87
				#else
					#define MAXIMUM_NUMBER_OF_POINTS 1023
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				
				uniform uint count = 0;
				uniform float4 points[MAXIMUM_NUMBER_OF_POINTS];
				uniform float radius;

				sampler2D data;

				sampler2D _HeatTex;
				
				struct vertInput {
					float4 position : POSITION;
				};
				
				struct vertOutput {
					float4 position : POSITION;
					fixed intensity : TEXCOORD0;
				};

				vertOutput vert(vertInput input) {
					vertOutput output;

					output.position = mul(UNITY_MATRIX_MVP, input.position);
					
					float4 worldPosition = mul(unity_ObjectToWorld, input.position);

					half h = 0;
					for(uint i = 0; i < MAXIMUM_NUMBER_OF_POINTS && i < count; i++) {
						uint y = i / 1024;
						uint x = i % 1024;

						float4 value = tex2Dlod(data, fixed4(x, y, 0, 0));

						// Calculates the contribution of each point.
						half di = distance(worldPosition.xyz, value.xyz);
						half hi = 1 - saturate(di / radius);
						h += hi * value.w;
					}

					output.intensity = saturate(h);

					return output;
				}

				half4 frag(vertOutput output) : COLOR {
					// Converts [0, 1] according to the heat texture.
					return tex2D(_HeatTex, fixed2(output.intensity, 0.5));
				}
			ENDCG
		}
	}
	Fallback "Diffuse"
}