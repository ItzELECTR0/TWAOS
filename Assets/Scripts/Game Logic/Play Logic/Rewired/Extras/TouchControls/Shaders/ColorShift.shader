// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Rewired/UI/ColorShift"
{
    Properties {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1, 1, 1, 1)
        _Red("Red Shift", Range(-2.0, 2.0)) = 0.0
        _Green("Green Shift", Range(-2.0, 2.0)) = 0.0
        _Blue("Blue Shift", Range(-2.0, 2.0)) = 0.0
        _Brightness("Brightness", Range(0.0, 10.0)) = 1.0
        _Contrast("Contrast", Range(0.0, 10.0)) = 1.0
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
    }
    
    SubShader {
    
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
            
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Fog{ Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass {
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            fixed _Red;
            fixed _Green;
            fixed _Blue;
            fixed _Contrast;
            fixed _Brightness;

            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };

            v2f vert(appdata_t IN) {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
                OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1, 1);
#endif
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half3 AdjustContrast(half3 color, half contrast) {
                return saturate(lerp(half3(0.5, 0.5, 0.5), color, contrast));
            }

            fixed4 frag(v2f IN) : SV_Target {
                fixed4 tex = tex2D(_MainTex, IN.texcoord);
                half3 rgb = AdjustContrast(
                    half3(
                        tex.r - _Red * (2 * tex.r - tex.g - tex.b),
                        tex.g - _Green * (2 * tex.g - tex.r - tex.b),
                        tex.b - _Blue * (2 * tex.b - tex.r - tex.g)
                    ),
                    _Contrast
                );
                return fixed4(rgb.rgb * _Brightness, tex.a) * IN.color;
            }

            ENDCG
        }
    }
}