Shader "Custom/Depth test visualize"
{
	Properties
	{
		[NoScaleOffset]
		_DepthTexture("Depth texture", 2D) = "black" {}
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
			float4 position: SV_POSITION;	// position in screen space
		};

		// vertex shader
		v2f vert(float4 pos : POSITION			// model space position
		)
		{
			v2f o;
			o.position = UnityObjectToClipPos(pos);
			return o;
		}

		// fragment shader
		fixed4 frag(v2f i) : SV_Target
		{
			// get 2D pixel position in range (0,0) top-left to (1,1) bottom-right
			float2 pixelPos = i.position.xy / _ScreenParams.xy;
			pixelPos.y = 1 - pixelPos.y;

			// get depth from depth texture (stored as red color)
			float depthTextureVal = tex2D(_DepthTexture, pixelPos).r;

			//return fixed4(pixelPos.x, pixelPos.y, 0, 1);
			return fixed4(depthTextureVal, 0, 0, 1);
		}
			ENDCG
		}
	}
}
