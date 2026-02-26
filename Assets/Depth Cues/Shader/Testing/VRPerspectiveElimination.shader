Shader "Custom/PerspectiveCompensation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _CompensationStrength ("Compensation Strength", Range(0, 1)) = 1.0
        _ReferenceDistance ("Reference Distance", Float) = 10.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _CompensationStrength;
                float _ReferenceDistance;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                // Transform to world space
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                
                // Calculate camera-relative position
                float3 cameraPos = _WorldSpaceCameraPos;
                float3 toCameraDir = cameraPos - worldPos;
                float distanceToCamera = length(toCameraDir);
                
                // Normalize direction
                toCameraDir = normalize(toCameraDir);
                
                // Calculate scale factor to compensate for perspective
                float scaleFactor = distanceToCamera / _ReferenceDistance;
                scaleFactor = lerp(1.0, scaleFactor, _CompensationStrength);
                
                // Get object center in world space
                float3 objectCenter = TransformObjectToWorld(float3(0,0,0));
                
                // Scale vertex position relative to object center
                float3 offsetFromCenter = worldPos - objectCenter;
                float3 scaledOffset = offsetFromCenter * scaleFactor;
                float3 newWorldPos = objectCenter + scaledOffset;
                
                // Transform to clip space
                output.positionCS = TransformWorldToHClip(newWorldPos);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionWS = newWorldPos;
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Basic lighting
                Light mainLight = GetMainLight();
                float3 normalWS = normalize(input.normalWS);
                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float3 lighting = mainLight.color * NdotL + unity_AmbientSky.rgb;
                
                return half4(texColor.rgb * _Color.rgb * lighting, 1.0);
            }
            ENDHLSL
        }
    }
}
