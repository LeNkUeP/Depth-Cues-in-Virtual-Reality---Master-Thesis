Shader "Custom/DepthInvariantVR_Final"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ReferenceDistance ("Reference Distance", Float) = 1.0
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
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float _ReferenceDistance;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                // 1) Objekt → Welt
                float3 worldPos = TransformObjectToWorld(IN.positionOS);

                // 2) Objektursprung in Weltkoordinaten (Anker) – hier Boden des Objekts
                float3 objectOriginWS = TransformObjectToWorld(float3(0,0,0));

                // 3) Distanz entlang Welt-Z Achse (nicht Kamera-Rotation!)
                float depth = worldPos.z - _WorldSpaceCameraPos.z;

                // 4) Skalierungsfaktor für XY
                float scale = depth / _ReferenceDistance;

                // 5) Vertex-Offset relativ zum Anker
                float3 offset = worldPos - objectOriginWS;

                // 6) Tiefen-invariante Skalierung nur in X/Y
                offset.xy *= scale;

                // 7) Neue Weltposition
                worldPos = objectOriginWS + offset;

                // 8) Projektieren
                OUT.positionCS = TransformWorldToHClip(worldPos);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                return tex2D(_MainTex, IN.uv);
            }

            ENDHLSL
        }
    }
}
