Shader "Hidden/GPUInstancer/Nature/TreeProxy"
{
	SubShader
	{
		Tags 
		{ 
			"Queue"="Transparent" 
			"RenderType"="Transparent"
			//"ForceNoShadowCasting"="True"
			//"IgnoreProjector"="True"
			//"LightMode"="Always" 
		}
		LOD 100
		
		//ZTest Always
        Cull Off
        //ZWrite Off
        //Fog { Mode off }
        //Blend Off

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			struct appdata_t {
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata_base v) 
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			void frag ()
			{
				discard;
			}
			ENDCG
		}
	}
}

