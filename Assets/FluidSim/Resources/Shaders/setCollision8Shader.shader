// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

Shader "FluidSim/setCollision8Shader"
{
	Properties
		{	
			collisionTex ("collisionTex", 2D) = "" {}
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

sampler2D collisionTex;
sampler2D tempStorageBuffer;

float4 tempOutput;
float4 tempCalc;

float texelOffset;

float halfStorageRDX;

int j;

float impulseLocationX;
float impulseLocationY;
float collisionVelocityX;
float collisionVelocityY;
float collisionStrength;
float collisionSize;
float collisionFalloff;

float tempFloat;

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
				tempOutput = tex2D(collisionTex, i.uv);

				for(j = 0; j < 8; j++)
				{
					texelOffset = halfStorageRDX + (j / 16.0);
					
					tempCalc = tex2D(tempStorageBuffer, float2(texelOffset, 0.125));
					impulseLocationX = (DecodeFloatRG(tempCalc.xy) * 2) - 0.5;
					impulseLocationY = (DecodeFloatRG(tempCalc.zw) * 2) - 0.5;
					
					tempCalc = tex2D(tempStorageBuffer, float2(texelOffset, 0.375));
					collisionVelocityX = (((tempCalc.x * 10) - 5) + 0.02);
					collisionVelocityY = (((tempCalc.y * 10) - 5) + 0.02);   //we add 0.02 to fix the pixel velocity bleeding/drift that happens when color and collision size are the same.

					tempCalc = tex2D(tempStorageBuffer, float2(texelOffset, 0.625));
					collisionSize = DecodeFloatRG(tempCalc.xy) * 2;
					collisionFalloff = tempCalc.z * 150;
					collisionStrength = tempCalc.w;
				
					tempFloat = clamp(((distance(i.uv, float2(impulseLocationX, impulseLocationY)) * -1) + collisionSize) * collisionFalloff, 0.0, 1.0) * collisionStrength;
					
					tempOutput =  float4(tempOutput.x + (collisionVelocityX * tempFloat), tempOutput.y + (collisionVelocityY * tempFloat), tempOutput.z * (1 - tempFloat), 0);
				}

				return tempOutput;
			}
		ENDCG
		}
	}
}
