Shader "MSP/Water" {

    Properties
    {
        _Color ("Main Color", Color) = (0.16,0.34,0.36,1)
        _SpecColor ("Specular Color", Color) = (0.58, 0.75, 0.62, 0)
        _EnvColor ("Environment Color", Color) = (0.8, 0.9, 1.0, 0)
        //_Bathy ("Bathymetric visibility", Range (0.01, 1)) = 0.5
        //_Gloss ("Gloss", Range (0.01, 1)) = 0.9
        //_Shininess ("Shininess", Range (0.01, 1)) = 0.8
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
		_ReflectionTex ("Reflection Map", 2D) = "white" {}
    }
 
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 400
       
        CGPROGRAM
        #pragma surface surf BlinnPhong alpha
        #pragma target 3.0
       
        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _ReflectionTex;
        half3 _Color;
        half3 _EnvColor;
        //fixed _Bathy;
        //half _Gloss;
        //half _Shininess;
       
        struct Input
        {
            float2 uv_MainTex;
			float3 viewDir;
            float2 uv_BumpMap; 
            float4 screenPos;
        };
       
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            o.Normal = normalize(UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap + 0.1 * _Time.x)) + UnpackNormal(tex2D(_BumpMap, 1.1 * IN.uv_BumpMap - 0.09134742 * _Time.x)));
            half rim = pow(1-saturate(dot(normalize(IN.viewDir), o.Normal)),3.0f);
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            half4 refl = tex2D( _ReflectionTex, screenUV );
            o.Alpha = saturate(lerp(1,0.5,tex.a)+rim);
            rim = pow(rim,6.0f);
			o.Emission = refl * rim;
            o.Albedo = _Color * (1-rim);
            o.Gloss = 0.6;
            o.Specular = 1.0;
        }
        ENDCG
       
//        CGPROGRAM
//        #pragma surface surf BlinnPhong finalcolor:final decal:add
//       
//        //sampler2D _BumpMap;
//        half _Gloss;
//        half _Shininess;
//       
//        struct Input
//        {
//            float2 uv_BumpMap;
//        };
//       
//        void surf(Input IN, inout SurfaceOutput o)
//        {
//            o.Gloss = _Gloss;
//            o.Specular = _Shininess;
//            //o.Normal = normalize(UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap + 0.1 * _Time.x)) + UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap - 0.09 * _Time.x)));
//        }
//       
//        void final(Input IN, SurfaceOutput o, inout fixed4 color)
//        {
//            // Need to set alpha here rather than in surf to make this work with all lights
//            color.a = 1;
//        }
//        ENDCG
    }
 
    FallBack "Transparent/VertexLit"
}