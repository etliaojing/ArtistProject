// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Inkling/divergenceShader"
{
	Properties
		{
			velocityTex ("velocity", 2D) = "" {}
			collisionTex ("collision", 2D) = "" {}
			rdx ("rdx", float) = 0.0039
		}
	SubShader {
	pass{
		//Pass one of two, this first pass clears the divergence field.
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

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
				return float4(0,0,0,0);
			}
	ENDCG
	}

	Pass{
		//This is the second pass which calculates the new divergence field.
		CGPROGRAM
		#pragma profileoption MaxTexIndirections=8
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

sampler2D velocityTex;
sampler2D collisionTex;

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
				half4 vL, vR, vB, vT;
				float2 uv1, uv2, uv3, uv4;

				uv1 = i.uv - float2(rdx, 0);
				uv2 = i.uv + float2(rdx, 0);
				uv3 = i.uv - float2(0, rdx);
				uv4 = i.uv + float2(0, rdx);

				vL = (tex2D(velocityTex, uv1));
				vR = (tex2D(velocityTex, uv2));
				vB = (tex2D(velocityTex, uv3));
				vT = (tex2D(velocityTex, uv4));

				vL = lerp(float4(0, 0, 0, 0), vL, tex2D(collisionTex, uv1).z);
				vR = lerp(float4(0, 0, 0, 0), vR, tex2D(collisionTex, uv2).z);
				vB = lerp(float4(0, 0, 0, 0), vB, tex2D(collisionTex, uv3).z);
				vT = lerp(float4(0, 0, 0, 0), vT, tex2D(collisionTex, uv4).z);

				return ((((vR.x - vL.x) + (vT.y - vB.y)) * 0.5));
			}
			ENDCG
		}
	}
}
