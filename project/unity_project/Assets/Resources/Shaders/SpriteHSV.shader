Shader "Appcpi/HSV For Sprite"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

        [PerRendererData] _Hue("Hue", Range(-180.0, 180.0)) = 0
        [PerRendererData] _Saturation("Saturation", Range(0, 5)) = 1
        [PerRendererData] _Value("Brightness", Range(0, 5)) = 1

        // required for UI.Mask
        [HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector]_ColorMask("Color Mask", Float) = 15
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        // required for UI.Mask
        Stencil
        {
        Ref[_Stencil]
        Comp[_StencilComp]
        Pass[_StencilOp]
        ReadMask[_StencilReadMask]
        WriteMask[_StencilWriteMask]
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #include "UnityCG.cginc"

            float _Hue;
            float _Saturation;
            float _Value;
            sampler2D _MainTex;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float4 color 	: COLOR;
                float2 texcoord : TEXCOORD0;
            };

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }

            float3 RGB2HSV(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 HSV2RGB(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float4 SpriteFrag(v2f IN) : SV_Target
            {
                float4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                float3 colorHSV = RGB2HSV(c.xyz);
                float hue = _Hue / 180.0;
                float saturation = _Saturation;
                float value = _Value;
                colorHSV.x += hue;
                if (colorHSV.x < 0)
                {
                    colorHSV.x += 1;
                }
                else if (colorHSV.x > 1)
                {
                    colorHSV.x -= 1;
                }

                colorHSV.y = saturate(colorHSV.y * saturation);
                colorHSV.z = saturate(colorHSV.z * value);

                c.xyz = HSV2RGB(colorHSV);
                c.xyz *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
