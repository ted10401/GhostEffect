Shader "FS/Ghost/Rim"
{
	Properties
	{
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower("Rim Power", Range(0, 2)) = 1
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed3 normal : NORMAL;
			};

			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed3 normal : NORMAL;
				fixed3 viewDir : TEXCOORD;
			};

			fixed4 _RimColor;
			fixed _RimPower;
			
			v2f vert (a2v v)
			{
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.viewDir = ObjSpaceViewDir(v.vertex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 normal = normalize(i.normal);
				fixed3 viewDir = normalize(i.viewDir);

				fixed rim = 1.0 - saturate(dot(normal, viewDir));
				return _RimColor * pow(rim, _RimPower);
			}
			ENDCG
		}
	}
}
