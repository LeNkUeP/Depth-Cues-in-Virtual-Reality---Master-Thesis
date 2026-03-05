Shader "Custom/VRDepthCompensation"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Tiling ("Tiling", Vector) = (1,1,0,0)
        _StretchFactor ("Stretch Factor", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _BaseColor;
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _Tiling;
            float _StretchFactor;

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

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // Weltposition
                float3 worldPos = TransformObjectToWorld(IN.positionOS).xyz;

                // Entfernung vom Ursprung (XZ-Ebene)
                float dist = length(worldPos.xz);

                // Horizontale Streckung
                float scaleX = 1.0 + _StretchFactor * dist;
                float3 pos = IN.positionOS.xyz;
                pos.x *= scaleX;
                OUT.positionCS = TransformObjectToHClip(float4(pos, 1.0));

                // UVs basierend auf gestreckter X-Position
                float worldX = pos.x; // gestreckte Position
                OUT.uv = float2(0.5 + worldX * 0.5 * _Tiling.x, IN.uv.y * _Tiling.y);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return tex * _BaseColor;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}