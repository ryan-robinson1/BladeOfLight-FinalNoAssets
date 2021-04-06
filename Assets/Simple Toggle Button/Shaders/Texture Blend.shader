Shader "Custom/Simple Toggle Button/Texture Blend"
{
	Properties
	{
		_Color1("Color1", Color) = (1, 1, 1, 1)
		_Color2("Color2", Color) = (1, 1, 1, 1)
		_Blend("Texture Blend", Range(0, 1)) = 0
		_MainTex1("Texture1", 2D) = "white" {}
		_MainTex2("Texture2", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite off
		Cull off

		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex1;
			sampler2D _MainTex2;
			float4 _MainTex1_ST;

			fixed4 _Color1;
			fixed4 _Color2;
			half _Blend;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert(appdata data)
			{
				v2f updatedData;
				
				updatedData.position = UnityObjectToClipPos(data.vertex);
				updatedData.uv = TRANSFORM_TEX(data.uv, _MainTex1);
				updatedData.color = data.color;

				return updatedData;
			}

			fixed4 frag(v2f input) : SV_TARGET
			{
				fixed4 col = lerp(
					tex2D(_MainTex1, input.uv),
					tex2D(_MainTex2, input.uv),
					_Blend);
			
				col *= lerp(_Color1, _Color2, _Blend);
				col *= input.color;

				return col;
			}

			ENDCG
		}
	}
}
