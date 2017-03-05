Shader "Custom/Culling"
{
	Properties
	{
		[NoScaleOffset]
		_DepthTexture("Depth texture", 2D) = "black" {}
		_Ambient("Ambient", Range(0, 1)) = 0.3
		_Diffuse("Diffuse", Range(0, 1)) = 1
		_DiffuseDir("Light direction", Vector) = (0, 1, 0, 0)
	}

	SubShader
	{
		//Tags{ "RenderType" = "Transparent" }
		//Blend SrcAlpha OneMinusSrcAlpha

	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		
		Float _Diffuse, _Ambient;
		float3 _DiffuseDir;
		sampler2D _DepthTexture;

		// vertex shader output
		struct v2f
		{
			float4 position: SV_POSITION;		// position in clip space 
			fixed4 color: SV_Target;			// pixel color
		};

		// vertex shader
		v2f vert(float4 pos : POSITION,			// position in object space
			float3 n : NORMAL,					// normal in object space 
			fixed4 c : COLOR					// vertex color
		)
		{
			v2f o;
			o.position = UnityObjectToClipPos(pos);
			float3 wn = UnityObjectToWorldNormal(n);
			float diffuse = _Diffuse * max(dot(normalize(_DiffuseDir), wn), 0);
			o.color = c * min(1, diffuse + _Ambient);
			return o;
		}

		// fragment shader
		fixed4 frag(v2f i) : SV_Target
		{
			// actual depth
			float depth = i.position.z;

			// get 2D pixel position in range (0,0) top-left to (1,1) bottom-right
			float2 pixelPos = i.position.xy / _ScreenParams.xy;
			pixelPos.y = 1 - pixelPos.y;

			// depth of scanned mesh (from depth texture)
			float scannedMeshDepth = tex2D(_DepthTexture, pixelPos).r;

			// ignore pixel if pixel is behind scanned mesh 
			clip(depth - scannedMeshDepth);

			return i.color;
		}
			ENDCG
		}
	}
}
