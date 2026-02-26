Shader "Custom/SelectiveBlur"
{
    Properties
    {
        _BlurSize ("Blur Size", Float) = 2
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }

        Pass
        {
            Name "SelectiveBlur"

            Stencil
            {
                Ref 1
                Comp NotEqual
            }

            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fragment _ _XR_SINGLE_PASS_INSTANCED

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            float _BlurSize;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 offset = _BlurSize / _ScreenParams.xy;

                half4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv) * 0.4;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv + offset) * 0.15;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv - offset) * 0.15;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv + float2(offset.x, -offset.y)) * 0.15;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv + float2(-offset.x, offset.y)) * 0.15;

                return col;
            }

            ENDHLSL
        }
    }
}