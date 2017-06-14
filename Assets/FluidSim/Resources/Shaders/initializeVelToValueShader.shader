// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Created by Phillip Heckinger, 2012.
//Distributed only on the Unity Asset Store.
//If purchased or downloaded from any other source,
//then these files were not obtained legally.
//Please support the Indie community and only use these
//files if you have obtained them legally.
//Thanks

Shader "FluidSim/initializeVelToValueShader"
{
	Properties
		{
			initialValue ("initialValue", vector) = (0.0, 0.0, 0.0, 0.0)
		}
	SubShader {
	Pass{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

float4 initialValue;
			
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
				return initialValue;
			}
		ENDCG
		}
	}
}
