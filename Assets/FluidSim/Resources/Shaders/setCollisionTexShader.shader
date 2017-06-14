// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

Shader "FluidSim/setCollisionTexShader"
{
	Properties
		{	
			collisionTex ("collisionTex", 2D) = "" {}
			collisionTexMask ("collisionTexMask", 2D) = "" {}
			
			collisionStrength ("collisionStrength", float) = 1.0
			
			textureData ("textureData", vector) = (0.0, 0.0, 0.0, 0.0)
		}

	SubShader {
		Pass{
		
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

sampler2D collisionTex;
sampler2D collisionTexMask;

float collisionStrength;

float4 textureData;  //x is location x, y is location y, z is velocity x, w is velocity y

float tempFloat;
float4 tempVector;

float4x4 rotationMatrix;

float2 rotatedUVs;

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
					rotatedUVs = mul(rotationMatrix, i.uv.xyxy - float4((textureData.x * 2) - 0.5, (textureData.y * 2) - 0.5, 0, 0)).xy;
					
					tempFloat = clamp(tex2D(collisionTexMask, rotatedUVs + 0.5).x, 1 - collisionStrength, 1);
					
					tempVector = tex2D(collisionTex, i.uv) * tempFloat * float4(0, 0, 1, 0);
									
					return tempVector + float4(textureData.z * (1 - tempFloat), textureData.w * (1 - tempFloat), 0, 0);
			}
		ENDCG
		}
	}
}
