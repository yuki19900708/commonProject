// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Appcpi/Grey+OutlineShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}

		_OutlineThickness ("Outline Thickness", Float) = 1.0
        _OutlineColor ("Outline Color", Color) = (0,1,0,1)
		_FillColor("_Fill Color", Color) = (0,0,0,1)

		// required for UI.Mask
        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector]_ColorMask ("Color Mask", Float) = 15
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
		// 源rgba*源a + 背景rgba*(1-源A值)   
		Blend SrcAlpha OneMinusSrcAlpha

		// required for UI.Mask
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert     
			#pragma fragment frag     
			#include "UnityCG.cginc"     

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				half2 texcoord  : TEXCOORD0;
			};

			sampler2D _MainTex;
			float _OutlineThickness;  
            fixed4 _OutlineColor;
			fixed4 _FillColor;
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				return OUT;
			}

			fixed getIsStrokeWithAngel(half angel, fixed2 texcoord)  
			{  
			    int stroke = 0;  
			    float rad = angel * 0.01745329252; // 这个浮点数是 pi / 180，角度转弧度  
			    // 这句比较难懂，outlineSize * cos(rad)可以理解为在x轴上投影，除以textureSize.x是因为texture2D接收的是一个0~1的纹理坐标，而不是像素坐标  
			    float a = tex2D(_MainTex, fixed2(texcoord.x + _OutlineThickness * cos(rad) / 1024, texcoord.y + _OutlineThickness * sin(rad) / 1024)).a;
			    if (a <= 0.5)// 我把alpha值大于0.5都视为不透明，小于0.5都视为透明  
			    {
			        stroke = 1;  
			    }
			    return stroke;  
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, IN.texcoord);
				if (c.a > 0.5)
				{
					int strokeCount = 0;
					strokeCount += getIsStrokeWithAngel(0.0, IN.texcoord);
					strokeCount += getIsStrokeWithAngel(45.0, IN.texcoord);
					strokeCount += getIsStrokeWithAngel(90.0, IN.texcoord);
					strokeCount += getIsStrokeWithAngel(135.0, IN.texcoord);
					strokeCount += getIsStrokeWithAngel(180.0, IN.texcoord);
					strokeCount += getIsStrokeWithAngel(225.0, IN.texcoord);
					strokeCount += getIsStrokeWithAngel(270.0, IN.texcoord);
					strokeCount += getIsStrokeWithAngel(315.0, IN.texcoord);

					if (strokeCount > 0) // 四周围至少有一个点是不透明的，这个点要设成描边颜色  
					{
			    		c = _OutlineColor;
						return c;
					}
					return _FillColor;
				}
				return c;
			}
			ENDCG
		}
	}
}