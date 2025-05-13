Shader "Custom/BlurShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _BlurSize ("Blur Size", Float) = 1.0
        _Randomness ("Randomness", Float) = 0.5
        _Opacity ("Opacity", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _BlurSize;
            float _Randomness;
            float _Opacity;

            float4 _MainTex_TexelSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 offset = _Randomness * _MainTex_TexelSize.xy;
                float4 color = tex2D(_MainTex, i.uv);
                
                color += tex2D(_MainTex, i.uv + float2(offset.x, 0));
                color += tex2D(_MainTex, i.uv - float2(offset.x, 0));
                color += tex2D(_MainTex, i.uv + float2(0, offset.y));
                color += tex2D(_MainTex, i.uv - float2(0, offset.y));

                return color * 0.25 * _Opacity; // 4 samples average, adjust opacity
            }
            ENDCG
        }
    }
}
