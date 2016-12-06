/*

This file is part of ProtoWorld.

ProtoWorld is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library. If not, see <http://www.gnu.org/licenses/>.

Authors of ProtoWorld: Miguel Ramos Carretero, Jayanth Raghothama, Aram Azhari, Johnson Ho and Sebastiaan Meijer.

*/

/*
* Heatmap Module
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
				#pragma target 5.0
				
				#pragma vertex vert
				#pragma fragment frag
				
				uniform uint count;
				uniform float radius;

				StructuredBuffer<float4> points;

				sampler2D _HeatTex;
				
				struct vertInput {
					float4 position : POSITION;
				};
				
				struct vertOutput {
					float4 position : POSITION;
					float intensity : TEXCOORD0;
				};

				vertOutput vert(vertInput input) {
					vertOutput output;

					output.position = mul(UNITY_MATRIX_MVP, input.position);
					
					float4 worldPosition = mul(unity_ObjectToWorld, input.position);

					float intensity = 0;

					for(uint index = 0; index < count; index++) {
						// Calculate the contribution of each point.
						float distanz0r = distance(worldPosition.xyz, points[index].xyz);
						float contribution = 1 - saturate(distanz0r / radius);
						intensity += contribution * points[index].w;
					}

					output.intensity = saturate(intensity);

					return output;
				}

				half4 frag(vertOutput output) : COLOR {
					// Convert the intensity into heat using to the heat texture.
					return tex2D(_HeatTex, fixed2(output.intensity, 0.5));
				}
			ENDCG
		}
	}
	Fallback "Diffuse"
}