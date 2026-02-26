Shader "Custom/BlurShader"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _CentreX("CentreX", Range(0,1))=0.5
        _CentreY("CentreY", Range(0,1))=0.5
        _Sample("Sample", Range(1,20))=5
        _BlurStrength("BlurStrength", Range(0,1))=0.5
        _Radius("Radius", Range(0,1))=0.5
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex:POSITION;
                float2 uv: TEXCOORD0;
            };
            struct v2f
            {
                float4 vertex:SV_POSITION;
                float2 uv:TEXCOORD0;
            };
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CentreX;
            float _CentreY;
            float _Sample;
            float _BlurStrength;
            float _Radius;
            v2f vert(appdata i)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;
                return o;
            };
            fixed4 frag(v2f i):SV_TARGET
            {
                fixed4 col = fixed4(1,1,1,1);
                float2 dist = i.uv-float2(_CentreX, _CentreY);
                for(int i = 0; i < _Sample; i++)
                {
                    float blur = 1-_BlurStrength* i/_Sample*saturate(length(dist)/_Radius);
                    col += tex2D(_MainTex, dist*blur+float2(_CentreX, _CentreY));
                }
                col /= _Sample;
                return col;
            }
            ENDCG
        }
    }
}
