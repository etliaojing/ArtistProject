// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Inkling/impulsePositionVelTexShader"
{
	Properties
		{
			velTex ("velTex", 2D) = "" {}
			velocityTexMask ("velocityTexMask", 2D) = "" {}

			textureData ("textureData", vector) = (0.0, 0.0, 0.0, 0.0)
		}
	SubShader {
	Pass{
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag

sampler2D velTex;
sampler2D velocityTexMask;

float4 textureData;  //x is position x, y is position y, z is velocity x, w is velocity y

float4x4 rotationMatrix;

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
				float2 rotatedUVs = mul(rotationMatrix, i.uv.xyxy - float4((textureData.x * 2) - 0.5, (textureData.y * 2) - 0.5, 0, 0)).xy;

				return lerp(tex2D(velTex, i.uv), float4(textureData.z, textureData.w, 0.0, 0.0) + tex2D(velTex, i.uv), tex2D(velocityTexMask, rotatedUVs + 0.5));
			}
		ENDCG
		}
	}
}
