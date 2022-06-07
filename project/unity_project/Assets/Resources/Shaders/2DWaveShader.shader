Shader "Appcpi/2DWaveShader"
{
	Properties
	{
		_MainTex ("Main Tex", 2D) = "white" {}
		[PerRendererData] _WaveDelay("Wave Delay", Float) = 1
        [PerRendererData] _Rect("Rect", Vector) = (0,0,1,1)
		_WaveRotation("Wave Rotation", Range(0, 90)) = 0
		_WaveSpeed("Wave Speed", Range(0, 5)) = 0.5
		_WaveAmplitude("Wave Amplitude", Range(1, 100)) = 1
		_WaveIndentity("Wave Indentity", Range(1, 20)) = 5
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
		LOD 100

		Pass
		{
			ZWrite Off
            Cull Off
			Blend SrcAlpha OneMinusSrcAlpha 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			half _WaveDelay;
			half _WaveRotation;
			half _WaveSpeed;
			half _WaveAmplitude;
			half _WaveIndentity;
			fixed4 _Rect;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				half time = _Time.y * 6.28 * _WaveSpeed;
				float percentX = (i.uv.x - _Rect.x) / _Rect.z;
				float percentY = (i.uv.y - _Rect.y) / _Rect.w;

				float vX = time + percentX * _WaveIndentity * 6.28 + _WaveDelay * 2.09;
				float vY = time + percentY * _WaveIndentity * 6.28 + _WaveDelay * 2.09;

                i.uv.y = i.uv.y + (sin(vX) + sin(vX / 7) + sin(vX / 13)) * cos(_WaveRotation * 0.0174532924) * _MainTex_TexelSize.y * _WaveAmplitude;
				i.uv.x = i.uv.x + (sin(vX) + sin(vX / 7) + sin(vX / 13)) * sin(_WaveRotation * 0.0174532924) * _MainTex_TexelSize.x * _WaveAmplitude;
                
                fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}

			ENDCG
		}
	}
}
