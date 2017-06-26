// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Inkling/gradientShader"
{
	Properties
		{
			collisionTex ("collisionTex", 2D) = "" {}
			velocityTex ("velocity", 2D) = "black" {}
			pressureTex ("pressure", 2D) = "black" {}

			timestep ("timestep", float) = 0.02
			dissipation ("dissipation", float) = 0.9999
			rdx ("rdx", float) = 0.0039

			impulsePositionX ("impulsePositionX", float) = 0.5
			impulsePositionY ("impulsePositionY", float) = 0.5
			impulseColor ("impulseColor", float) = 1
		}
	SubShader {
	Pass{
		CGPROGRAM

		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

sampler2D collisionTex;
sampler2D velocityTex;
sampler2D pressureTex;

float timestep;
float dissipation;

float rdx;

float addImpulse;
float impulseColor;
float impulsePositionX;
float impulsePositionY;

float4 uNew;

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
		  		float2 uv1, uv2, uv3, uv4;
		  		float v1, v2, v3, v4;
		  		float vL, vR, vT, vB;
		  		float pL, pR, pB, pT, pC;

				float4 vMask = float4(1.0, 1.0, 1.0, 0.0);
				float4 obstV = float4(0.0, 0.0, 0.0, 0.0);

				uv1 = i.uv - float2(rdx, 0);
				uv2 = i.uv + float2(rdx, 0);
				uv3 = i.uv - float2(0, rdx);
				uv4 = i.uv + float2(0, rdx);

				v1 = (tex2D(velocityTex, uv1).x);
				v2 = (tex2D(velocityTex, uv2).x);
				v3 = (tex2D(velocityTex, uv3).y);
				v4 = (tex2D(velocityTex, uv4).y);

				vL = tex2D(collisionTex, uv1).x;
				vR = tex2D(collisionTex, uv2).x;
				vT = tex2D(collisionTex, uv3).y;
				vB = tex2D(collisionTex, uv4).y;

				pC = (tex2D(pressureTex, i.uv).x);
				pL = (tex2D(pressureTex, uv1).x);
				pR = (tex2D(pressureTex, uv2).x);
				pB = (tex2D(pressureTex, uv3).x);
				pT = (tex2D(pressureTex, uv4).x);

				if(tex2D(collisionTex, uv1).z < 0.5)
				{
					pL = pC;
					obstV.x = vL;
					vMask.x = 0;
				}
				if(tex2D(collisionTex, uv2).z < 0.5)
				{
					pR = pC;
					obstV.x = vR;
					vMask.x = 0;
				}
				if(tex2D(collisionTex, uv3).z < 0.5)
				{
					pB = pC;
					obstV.y = vT;
					vMask.y = 0;
				}
				if(tex2D(collisionTex, uv4).z < 0.5)
				{
					pT = pC;
					obstV.y = vB;
					vMask.y = 0;
				}

				uNew.xy = (tex2D(velocityTex, i.uv).xy) - float2(pR - pL, pT - pB);

				return ((vMask * uNew) + obstV);
			}
				ENDCG
		}
	}
}
