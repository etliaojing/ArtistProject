// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Inkling/boundaryOpShader"
{
	Properties
		{
			targetTex ("targetTexture", 2D) = "" {}
			setColor ("setColor", vector) = (0,0,0,0)
			rdx ("rdx", float) = 0.016
		}
	SubShader {
	Pass{
		CGPROGRAM

		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

sampler2D targetTex;
float4 setColor;

float rdx;

struct appdata
{
	float4 vertex : POSITION;
	float4 texcoord : TEXCOORD0;
};

struct v2f
{
	float4 pos : POSITION;
	float4 color : COLOR;
	float2 uv : TEXCOORD0;
};

v2f vert (appdata v)
{
	v2f o;
	o.color = float4(0, 0, 0, 1);
	o.pos = UnityObjectToClipPos( v.vertex );
	o.uv = v.texcoord.xy;
	return o;
}

		float4 frag(v2f i) : COLOR
			{
				if(i.uv.x <= rdx || i.uv.x >= 1 - rdx || i.uv.y <= rdx || i.uv.y >= 1 - rdx)
					{
						return setColor;
					}
				else
					{
						return tex2D(targetTex, i.uv);
					}
			}
		ENDCG
		}
	}
}
