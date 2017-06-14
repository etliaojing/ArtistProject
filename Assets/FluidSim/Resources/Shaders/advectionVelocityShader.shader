// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

Shader "FluidSim/advectionVelocityShader"
{
	Properties
		{
			velocityTexSource ("velocitySource", 2D) = "" {}
			targetTex ("targetTex", 2D) = "" {}
			collisionTex ("collisionTex", 2D) = "" {}
			
			timeStep ("timeStep", float) = 0.02
			dissipation ("dissipation", float) = 0.999
			simSpeed ("simSpeed", float) = 1.0
		}
	SubShader {
		Pass{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

sampler2D collisionTex;
sampler2D velocityTexSource;
sampler2D targetTex;

float timeStep;
float dissipation;
float simSpeed;
float tempValue;

float2 pos;
			
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

		half4 frag(v2f i) : COLOR
			{
				pos = i.uv - (timeStep * simSpeed * ((tex2D(velocityTexSource, i.uv))));

				tempValue = (tex2D(collisionTex, pos).z);

				return (((((tex2D(targetTex, pos))) * tempValue) * dissipation));
			}
		ENDCG
		}
	}
}

