Shader "Custom/SimpleGrabPassBlur" 
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
        _BlurSize ("Blur Size", Range(0,4)) = 1
        _Power ("Distortion Power", Range(0,2)) = 0.5
        _MainTex ("Mask Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            Name "Blur"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _SINGLE_PASS_INSTANCED

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Tint;
                float _BlurSize;
                float _Power;
                float4 _MainTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float4 _CameraOpaqueTexture_TexelSize;

            Varyings vert (Attributes v)
            {
                Varyings o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.positionHCS);

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float2 screenUV = i.screenPos.xy / i.screenPos.w;

                // leichte Power Verzerrung (ersetzt dein ScreenNode + Power Setup)
                float2 center = screenUV - 0.5;
                screenUV = 0.5 + center * (1 + _Power * dot(center, center));

                float2 texel = _CameraOpaqueTexture_TexelSize.xy * _BlurSize;

                half4 col = 0;

                // Mobile-freundlicher 5-Tap Blur (Quest geeignet)
                col += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV) * 0.4;
                col += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + texel * float2(1,0)) * 0.15;
                col += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + texel * float2(-1,0)) * 0.15;
                col += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + texel * float2(0,1)) * 0.15;
                col += SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + texel * float2(0,-1)) * 0.15;

                half4 mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                return col * mask * _Tint;
            }

            ENDHLSL
        }
    }
}