Shader "Custom/BoxBlur_Mobile"
{
    Properties
    {
        _Blur ("Blur radius", Range(1, 4)) = 1
        _Scale ("Texel scale", Range(1, 5)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            CBUFFER_START(UnityPerMaterial)
                float _Blur;
                float _Scale;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            // Mobile-friendly separable box blur
            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.screenPos.xy / IN.screenPos.w;

                // texel size
                float2 texel = _Scale / _ScreenParams.xy;

                int radius = max(1, (int)_Blur);
                float4 color = float4(0,0,0,0);

                // Horizontal blur
                for(int x = -radius; x <= radius; x++)
                {
                    color += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(x * texel.x, 0));
                }
                color /= (2*radius + 1);

                // Vertical blur
                float4 finalColor = float4(0,0,0,0);
                for(int y = -radius; y <= radius; y++)
                {
                    finalColor += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + float2(0, y * texel.y));
                }
                finalColor /= (2*radius + 1);

                // Combine horizontal + vertical blur (approximation)
                finalColor.rgb = (finalColor.rgb + color.rgb) * 0.5;

                return float4(finalColor.rgb, 1.0);
            }

            ENDHLSL
        }
    }
}