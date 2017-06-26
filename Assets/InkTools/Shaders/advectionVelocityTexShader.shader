// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Inkling/advectionVelocityTexShader"
{
	Properties
		{
			velocityTexSource ("velocitySource", 2D) = "" {}
			targetTex ("targetTex", 2D) = "" {}
			collisionTex ("collisionTex", 2D) = "" {}
			dissipationTex ("dissipationTex", 2D) = "" {}

			timeStep ("timeStep", float) = 0.02
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
sampler2D dissipationTex;

float timeStep;
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
				pos = i.uv - (timeStep * simSpeed * tex2D(velocityTexSource, i.uv));

				tempValue = tex2D(collisionTex, pos).z;

				return tex2D(targetTex, pos) * tempValue * lerp(0.75, 1.0, tex2D(dissipationTex, pos).x);
			}
		ENDCG
		}
	}
}

