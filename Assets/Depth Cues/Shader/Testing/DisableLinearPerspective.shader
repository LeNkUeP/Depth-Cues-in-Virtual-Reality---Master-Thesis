Shader "Custom/DisableLinearPerspective"
{
    Properties
    {
        _BaseColor ("Color", Color) = (1,1,1,1)
        _SizeFactor ("Size Factor", Float) = 0.05
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "LightMode"="UniversalForward" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _STEREO_INSTANCING_ON
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 positionWS  : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _BaseColor;
            float _SizeFactor;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);

                float3 camPos = GetCameraPositionWS();
                float distanceToCam = distance(worldPos, camPos);

                float scale = distanceToCam * _SizeFactor;

                float3 centerWS = TransformObjectToWorld(float3(0,0,0));
                float3 offset = worldPos - centerWS;
                worldPos = centerWS + offset * scale;

                OUT.positionWS = worldPos;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

                OUT.positionHCS = TransformWorldToHClip(worldPos);

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                Light mainLight = GetMainLight();

                float3 normal = normalize(IN.normalWS);
                float NdotL = saturate(dot(normal, mainLight.direction));

                float3 diffuse = _BaseColor.rgb * mainLight.color * NdotL;

                return half4(diffuse, 1);
            }

            ENDHLSL
        }
    }
}
