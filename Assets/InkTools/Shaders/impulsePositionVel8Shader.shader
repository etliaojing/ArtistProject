// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Inkling/impulsePositionVel8Shader"
{
	Properties
		{
			velTex ("velTex", 2D) = "" {}
			tempStorageBuffer ("tempStorageBuffer", 2D) = "" {}

			halfStorageRDX ("halfStorageRDX", float) = 0.015265
		}
	SubShader {
	Pass{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

sampler2D velTex;
sampler2D tempStorageBuffer;

float4 tempOutput;
float4 tempCalc;

float texelOffset;

float halfStorageRDX;

int j;

float impulsePositionX;
float impulsePositionY;
float impulseSize;
float velocityFalloff;
float velocityStrengthX;
float velocityStrengthY;

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
				for(j = 0; j < 8; j++)
				{
					texelOffset = halfStorageRDX + (j / 16.0);

					tempCalc = tex2D(tempStorageBuffer, float2(texelOffset, 0.125));
					impulsePositionX = (DecodeFloatRG(tempCalc.xy) * 2) - 0.5;
					impulsePositionY = (DecodeFloatRG(tempCalc.zw) * 2) - 0.5;

					tempCalc = tex2D(tempStorageBuffer, float2(texelOffset, 0.375));
					velocityStrengthX = ((tempCalc.z * 10) - 5.0);
					velocityStrengthY = ((tempCalc.w * 10) - 5.0);

					tempCalc.x = DecodeFloatRG(tempCalc.xy);
					velocityStrengthX = velocityStrengthX * tempCalc.x;
					velocityStrengthY = velocityStrengthY * tempCalc.x;

					tempCalc = tex2D(tempStorageBuffer, float2(texelOffset, 0.625));
					impulseSize = DecodeFloatRG(tempCalc.xy) * 2;
					velocityFalloff = tempCalc.z * 150;

					tempOutput += float4(velocityStrengthX, velocityStrengthY, 0, 0) * clamp((distance(i.uv, float2(impulsePositionX, impulsePositionY)) - impulseSize) * -velocityFalloff, 0.0, 1.0);
				}

				tempOutput = tempOutput + tex2D(velTex, i.uv);

				return tempOutput;
			}
		ENDCG
		}
	}
}
