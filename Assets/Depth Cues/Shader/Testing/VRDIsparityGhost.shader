Shader "Custom/VRDisparityGhost"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,0.5)
        _IPD ("IPD (meters)", Float) = 0.064
        _NearDistance ("Effect Distance", Float) = 0.3
        _Strength ("Effect Strength", Float) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        float4 _BaseColor;
        float _IPD;
        float _NearDistance;
        float _Strength;

        Varyings vert(Attributes input, float direction)
        {
            Varyings output;

            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
            float3 positionVS = TransformWorldToView(positionWS);

            // Z = Distanz zum Auge
            float z = -positionVS.z;

            // X relativ zum Auge
            float x = positionVS.x;

            // ---------- Nähe Faktor ----------
            float nearFactor = saturate(1.0 - (z / _NearDistance));

            // ---------- Horizontaler Faktor ----------
            float halfIPD = _IPD * 0.5;
            float horizontalFactor = 1.0 - saturate(abs(x) / halfIPD);

            // ---------- Gesamte Disparität ----------
            float disparity = nearFactor * horizontalFactor * _Strength * (_IPD / max(z, 0.01));

            // Offset im View Space
            positionVS.x += direction * disparity;

            output.positionHCS = TransformWViewToHClip(positionVS);

            return output;
        }

        half4 frag(Varyings input) : SV_Target
        {
            return _BaseColor;
        }

        ENDHLSL

        Pass
        {
            Name "Plus"

            HLSLPROGRAM
            #pragma vertex vertPlus
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _STEREO_INSTANCING_ON

            Varyings vertPlus(Attributes input)
            {
                return vert(input, 1.0);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Minus"

            HLSLPROGRAM
            #pragma vertex vertMinus
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _STEREO_INSTANCING_ON

            Varyings vertMinus(Attributes input)
            {
                return vert(input, -1.0);
            }
            ENDHLSL
        }
    }
}