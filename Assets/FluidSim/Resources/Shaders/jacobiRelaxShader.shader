// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

Shader "FluidSim/jacobiRelaxShader"
{
	Properties
		{
			pressureTex ("pressureTex", 2D) = "black" {}
			divergenceTex ("divergenceTex", 2D) = "black" {}
			collisionTex ("collisionTex", 2D) = "" {}

			rdx ("rdx", float) = 0.0039
		}
	SubShader {
	
	Pass{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

sampler2D pressureTex;
sampler2D divergenceTex;
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

		float4 frag(v2f i) : color
			{
				float4 xC, xL, xR, xB, xT, bC;
				float2 uv1, uv2, uv3, uv4;

				uv1 = i.uv - float2(rdx, 0);
				uv2 = i.uv + float2(rdx, 0);
				uv3 = i.uv - float2(0, rdx);
				uv4 = i.uv + float2(0, rdx);

				xC = (tex2D(pressureTex, i.uv));
				xL = (tex2D(pressureTex, uv1));
				xR = (tex2D(pressureTex, uv2));
				xB = (tex2D(pressureTex, uv3));
				xT = (tex2D(pressureTex, uv4));
				bC = (tex2D(divergenceTex, i.uv));
				
				//collision checks
				xL = lerp(xC, xL, tex2D(collisionTex, uv1).z);
				xR = lerp(xC, xR, tex2D(collisionTex, uv2).z);
				xB = lerp(xC, xB, tex2D(collisionTex, uv3).z);
				xT = lerp(xC, xT, tex2D(collisionTex, uv4).z);

				return (((xL + xR + xB + xT - bC) / 4));
			}
		ENDCG
		}
	}
}
