Shader "Appcpi/WaterShadow"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "white" {}
		_WaterTex("Water Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_ShadowOffsetX("Shadow Offset X", float) = 0
		_ShadowOffsetY("Shadow Offset Y", float) = 0
		_ShadowLength("Shadow Length", Range(0, 100)) = 1
		_XRotation("X Rotation", Range(0, 360)) = 26
		_YRotation("Y Rotation", Range(0, 360)) = 65
		_MaxShadowness("Max Shadowness", Range(0, 1)) = 0.6
	}
	SubShader
	{
		LOD 200
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _NoiseTex;
			sampler2D _WaterTex;


			fixed4 _WaterColor;
			fixed _ShadowOffsetX;
			fixed _ShadowOffsetY;
			fixed _ShadowLength;
			fixed _XRotation;
			fixed _YRotation;
			fixed _MaxShadowness;
			fixed2 _waterUVOffset;
			fixed2 _waterUVScale;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(v2f input) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, input.uv);
				fixed4 cWater = tex2D(_WaterTex, fixed2(input.uv.x * _waterUVScale.x, input.uv.y * _waterUVScale.y) + _waterUVOffset);
				if (c.r > 0)
				{
					fixed a = _MaxShadowness;
					return cWater * fixed4(a,a,a,1);
				}

				fixed4 noise = tex2D(_NoiseTex, input.uv + _Time.yy * 0.02);
				fixed2 uv = input.uv;
				uv.y += noise.r * 0.05 + _ShadowOffsetY * _MainTex_TexelSize.y;
				uv.x -= noise.r * 0.05 + _ShadowOffsetX * _MainTex_TexelSize.x;

				fixed degree2Rad = 0.01745329252;// 这个浮点数是 pi / 180，角度转弧度 
				fixed radX = _XRotation * degree2Rad;
				fixed radY = _YRotation * degree2Rad;
				fixed sinX = sin(radX);
				fixed cosX = cos(radX);
				fixed sinY = sin(radY);
				fixed cosY = cos(radY);

				bool shouldInShadow = false;
				_ShadowLength *= noise.r;

				fixed minDistance = _ShadowLength;
				for (int i = -_ShadowLength; i <= _ShadowLength; i++)
				{
					fixed xOffsetByX = i * cosX * _MainTex_TexelSize.x;
					fixed yOffsetByX = i * sinX * _MainTex_TexelSize.y;
					for (int j = -_ShadowLength; j <= _ShadowLength; j++)
					{
						fixed xOffsetByY = -j * sinY * _MainTex_TexelSize.x;
						fixed yOffsetByY = j * cosY * _MainTex_TexelSize.y;

						fixed2 uv1 = uv;
						uv1.x += xOffsetByX;
						uv1.y += yOffsetByX;
						uv1.x += xOffsetByY;
						uv1.y += yOffsetByY;
                        fixed sourceC = tex2Dlod(_MainTex, fixed4(uv1, 0, 0)).r;
						if (sourceC > 0)
						{
							shouldInShadow = true;
							float distance = sqrt(abs(i) * abs(i) + abs(j) * abs(j));
							if (minDistance > distance)
							{
								minDistance = distance;
							}
						}
					}
				}

				if (shouldInShadow)
				{
					fixed p = minDistance * minDistance / _ShadowLength / _ShadowLength * (1 - _MaxShadowness) + _MaxShadowness;
					c = cWater * fixed4(p, p, p, 1);
					return c;
				}

				return cWater;
			}
			ENDCG
		}
	}
}
