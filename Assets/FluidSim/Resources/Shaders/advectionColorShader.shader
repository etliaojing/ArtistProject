// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

Shader "FluidSim/advectionColorShader"
{
	Properties
		{
			velocityTexSource ("velocityTexSource", 2D) = "" {}
			targetTex ("targetTex", 2D) = "" {}
			collisionTex ("collisionTex", 2D) = "" {}
			
			colorDissipateTo ("colorDissipateTo", vector) = (0,0,0,0)
			timestep ("timestep", float) = 0.02
			dissipation ("dissipation", float) = 0.9965
			simSpeed ("simSpeed", float) = 1.0
		}
	SubShader {
		Pass{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

sampler2D velocityTexSource;
sampler2D targetTex;
sampler2D collisionTex;

float4 colorDissipateTo;
float timestep;
float dissipation;
float simSpeed;
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
				pos = i.uv - (timestep * simSpeed * ((tex2D(velocityTexSource, i.uv))));
					
				return clamp(lerp(tex2D(targetTex, pos), colorDissipateTo, dissipation + ((1 - tex2D(collisionTex, pos).b) * 0.01)), 0, 1);
			}
		ENDCG
		}
	}
}

