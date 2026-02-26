Shader "Custom/MeshSoftBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurStrength ("Blur Strength", Range(0,3)) = 1
        _BlurSamples ("Samples", Range(1,5)) = 2
        _EdgeBlur ("Edge Blur", Range(0,1)) = 0.5
        _FresnelPower ("Fresnel Power", Range(1,5)) = 2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float _BlurStrength;
                int _BlurSamples;
                float _EdgeBlur;
                float _FresnelPower;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.worldPos = mul(unity_ObjectToWorld, float4(IN.positionOS.xyz, 1.0)).xyz;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float4 color = float4(0,0,0,0);
                int samples = max(1, _BlurSamples);

                // --- Textur-Blur (wie vorher) ---
                for(int x=-samples; x<=samples; x++)
                {
                    for(int y=-samples; y<=samples; y++)
                    {
                        float2 offset = float2(x, y) * (_BlurStrength * 0.02);
                        color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + offset);
                    }
                }
                color /= (float)((2*samples+1)*(2*samples+1));

                // --- Edge Blur via Fresnel ---
                float3 viewDir = normalize(_WorldSpaceCameraPos - IN.worldPos);
                float fresnel = pow(1.0 - saturate(dot(IN.worldNormal, viewDir)), _FresnelPower);
                color.rgb = lerp(color.rgb, float3(0,0,0), fresnel * _EdgeBlur);

                return color;
            }

            ENDHLSL
        }
    }
}