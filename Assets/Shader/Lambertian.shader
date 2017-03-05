Shader "Custom/Lambertian"
{
	Properties
	{
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
			#pragma target 3.0
			#include "UnityCG.cginc"

			// vertex shader output
			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : SV_Target;
			};

			// color from the material
			Float _Diffuse, _Ambient;
			float3 _DiffuseDir;

			v2f vert(float4 pos : POSITION, 
				float3 n : NORMAL, 
				fixed4 c : COLOR
				)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(pos);
				float3 wn = UnityObjectToWorldNormal(n);
				float diffuse = _Diffuse * max(dot(normalize(_DiffuseDir), wn), 0);
				o.color = c * min(1, diffuse + _Ambient);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}
