Shader "Custom/VRDepthCompensation"
{
    Properties
    {
        _Compensation ("Compensation Strength", Float) = 0.02
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _STEREO_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _Compensation;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                // Object → World
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);

                // Kamera Position (korrekt für VR)
                float3 camPos = GetCameraPositionWS();

                // Distanz berechnen
                float dist = distance(worldPos, camPos);

                // Skalierungsfaktor
                float scale = 1 + dist * _Compensation;

                // Nur X und Y skalieren (Z bleibt)
                float3 dirFromCenter = IN.positionOS.xyz;
                dirFromCenter.xy *= scale;

                // Zurück in World Space
                float3 newWorldPos = TransformObjectToWorld(dirFromCenter);

                // World → Clip
                OUT.positionCS = TransformWorldToHClip(newWorldPos);

                return OUT;
            }

            half4 frag () : SV_Target
            {
                return half4(1,1,1,1);
            }

            ENDHLSL
        }
    }
}