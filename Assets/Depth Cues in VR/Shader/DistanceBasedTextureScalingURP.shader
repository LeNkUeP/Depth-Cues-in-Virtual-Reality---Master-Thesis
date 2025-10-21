Shader "Custom/DistanceBasedTextureScalingURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tiling ("Base Tiling", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _STEREO_MULTIVIEW_ON _STEREO_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float4 positionSS : TEXCOORD1; // screen space pos
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Tiling;

            Varyings vert (Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float3 worldPos = TransformObjectToWorld(v.positionOS);
                o.worldPos = worldPos.xyz;
                o.positionHCS = TransformWorldToHClip(worldPos.xyz);

                // Bildschirmposition (0–1 range)
                o.positionSS = ComputeScreenPos(o.positionHCS);

                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                // Perspective divide
                float2 screenUV = i.positionSS.xy / i.positionSS.w;

                // Tiefe aus Clipspace → größere Tiefe = kleiner
                float depth = i.positionSS.w;

                // Kompensiere perspektivisches Schrumpfen:
                float scale = depth * _Tiling;

                // Zentriere um den Bildschirmmittelpunkt (optional)
                float2 scaledUV = (screenUV - 0.5) * scale + 0.5;

                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, scaledUV);
                return col;
            }
            ENDHLSL
        }
    }
}