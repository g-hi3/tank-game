Shader "Unlit/PrintShader"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_PrintTex ("Print Texture", 2D) = "black" {}
		_PositionX ("Position X", float) = 0
		_PositionY ("Position Y", float) = 0
		_ScaleX ("Scale X", float) = 0
		_ScaleY ("Scale Y", float) = 0
		_Rotation ("Rotation", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 world_position : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _PrintTex;
			float _PositionX;
			float _PositionY;
			float _ScaleX;
			float _ScaleY;
			float _Rotation;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.world_position = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			float2 translate(const float2 uv)
			{
				return float2(uv.x - _PositionX, uv.y - _PositionY);
			}

			float2 scale(const float2 uv)
			{
				return float2(0.5 + uv.x / _ScaleX, 0.5 + uv.y / _ScaleY);
			}

			float2x2 get_rotation_matrix()
			{
				const float theta = (_Rotation - 1) * 3.14159265;
				const float s = sin(theta);
				const float c = cos(theta);
				return float2x2(c, -s, s, c);
			}

			float2 rotate(const float2 uv)
			{
				return mul(uv - 0.5, get_rotation_matrix()) + 0.5;
			}

			fixed4 print_onto(const fixed4 print_color, const fixed4 main_color)
			{
				const fixed3 color = lerp(main_color.rgb, print_color.rgb, print_color.a);
				return float4(color.r, color.g, color.b, main_color.a);
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				const fixed4 main_texture_color = tex2D(_MainTex, i.uv);

				if (_ScaleX == 0 || _ScaleY == 0)
				{
					return main_texture_color;
				}

				const float2 transformed_uv = rotate(scale(translate(i.uv)));
				const fixed4 print_texture_color = tex2D(_PrintTex, transformed_uv);
				return print_onto(print_texture_color, main_texture_color);
			}
			ENDCG
		}
	}
}