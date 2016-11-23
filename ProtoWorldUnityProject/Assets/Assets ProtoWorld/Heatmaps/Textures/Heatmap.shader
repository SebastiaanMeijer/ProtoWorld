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

				uniform int _Points_Length = 0;
				uniform float3 _Points[MAXIMUM_NUMBER_OF_POINTS];		// (x, y, z) = position
				uniform float2 _Properties[MAXIMUM_NUMBER_OF_POINTS];	// x = radius, y = intensity

				sampler2D _HeatTex;

				half4 frag(vertOutput output) : COLOR {
					// Loops over all the points.
					half h = 0;
					for(int i = 0; i < MAXIMUM_NUMBER_OF_POINTS && i < _Points_Length; i++) {
						// Calculates the contribution of each point.
						half di = distance(output.worldPos, _Points[i].xyz);

						half ri = _Properties[i].x;
						half hi = 1 - saturate(di / ri);

						h += hi * _Properties[i].y;
					}

					// Converts (0-1) according to the heat texture.
					h = saturate(h);
					half4 color = tex2D(_HeatTex, fixed2(h, 0.5));

					return color;
				}
			ENDCG
		}
	}
	Fallback "Diffuse"
}