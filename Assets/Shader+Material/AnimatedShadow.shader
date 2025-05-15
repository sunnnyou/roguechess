Shader "UI/AnimatedShadow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // Shadow properties
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowOffsetX ("Shadow Offset X", Float) = 0.02
        _ShadowOffsetY ("Shadow Offset Y", Float) = -0.02
        _ShadowSize ("Shadow Size", Float) = 0.02
        
        // Animation properties
        _AnimSpeed ("Animation Speed", Float) = 0.5
        _AnimAmount ("Animation Amount", Float) = 0.01
        
        // Required for UI.Mask
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]
        
        // First Pass: Draw the shadow
        Pass
        {
            Name "Shadow"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
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
                float4 worldPosition : TEXCOORD1;
            };
            
            fixed4 _ShadowColor;
            float _ShadowOffsetX;
            float _ShadowOffsetY;
            float _ShadowSize;
            float _AnimSpeed;
            float _AnimAmount;
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _ClipRect;
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                
                // Apply shadow offset with animation
                float animatedOffsetY = _ShadowOffsetY + sin(_Time.y * _AnimSpeed) * _AnimAmount;
                
                // Apply offset to the vertex position for shadow
                float4 offsetVertex = IN.vertex;
                offsetVertex.x += _ShadowOffsetX;
                offsetVertex.y += animatedOffsetY;
                
                // Make shadow slightly bigger
                float2 shadowExpansion = _ShadowSize;
                offsetVertex.x += (offsetVertex.x > 0) ? shadowExpansion : -shadowExpansion;
                offsetVertex.y += (offsetVertex.y > 0) ? shadowExpansion : -shadowExpansion;
                
                OUT.worldPosition = offsetVertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = _ShadowColor;
                
                return OUT;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                // Sample the sprite texture
                fixed4 texColor = tex2D(_MainTex, IN.texcoord);
                
                // Get the shadow color (use the texture's alpha for shape)
                fixed4 shadowColor = IN.color;
                shadowColor.a *= texColor.a;
                
                // Apply UI masking
                shadowColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                return shadowColor;
            }
            ENDCG
        }
        
        // Second Pass: Draw the main sprite
        Pass
        {
            Name "Sprite"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
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
                float4 worldPosition : TEXCOORD1;
            };
            
            fixed4 _Color;
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ClipRect;
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                // Sample the texture
                fixed4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Apply UI masking
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                return color;
            }
            ENDCG
        }
    }
}