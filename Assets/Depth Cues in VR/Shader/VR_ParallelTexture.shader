Shader "Custom/VR_ParallelTexture"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ScaleFactor("Distance Scale Factor", Float) = 0.1
        _Enabled("Depth Cue Enabled", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _ScaleFactor;
            float _Enabled;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS).xyz;
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float3 camPos = _WorldSpaceCameraPos.xyz;
                float dist = length(IN.worldPos - camPos);

                float2 uv = IN.uv;

                // Tiefenhinweis aus: Textur abstandsabhängig skalieren
                if (_Enabled < 0.5)
                {
                    uv = uv * (1 + dist * _ScaleFactor);
                }

                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                return col;
            }
            ENDHLSL
        }
    }
}
