Shader "Sprites/AnimatedShadowSprite"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowOffsetX ("Shadow Offset X", Float) = 0.02
        _ShadowOffsetY ("Shadow Offset Y", Float) = -0.02
        _ShadowSize ("Shadow Size", Float) = 0.02

        _AnimSpeed ("Animation Speed", Float) = 0.5
        _AnimAmount ("Animation Amount", Float) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        // Pass 1: Shadow
        Pass
        {
            Name "Shadow"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _ShadowColor;
            float _ShadowOffsetX;
            float _ShadowOffsetY;
            float _ShadowSize;
            float _AnimSpeed;
            float _AnimAmount;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                float offsetY = _ShadowOffsetY + sin(_Time.y * _AnimSpeed) * _AnimAmount;
                float4 pos = IN.vertex;
                pos.xy += float2(_ShadowOffsetX, offsetY);

                // Optional: Slight expansion for soft shadow
                float2 expansion = _ShadowSize;
                pos.xy += (pos.xy > 0) ? expansion : -expansion;

                OUT.vertex = UnityObjectToClipPos(pos);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = _ShadowColor;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, IN.texcoord);
                texCol.rgb = IN.color.rgb;
                texCol.a *= IN.color.a * texCol.a; // preserve alpha shape
                return texCol;
            }
            ENDCG
        }

        // Pass 2: Main sprite
        Pass
        {
            Name "Sprite"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
                return col;
            }
            ENDCG
        }
    }
}
