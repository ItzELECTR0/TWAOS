Shader "Custom/Additive (Soft, Tintable)"
{
	Properties
	{
		_MainTex("Particle Texture", 2D) = "white" {}
		_InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_TintColor("Tint Color (RGBA)", color) = (1, 1, 1, 1)
		_Intensity("Intensity (float)", Range(0.01, 4.0)) = 1.0
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }
		Blend One OneMinusSrcColor
		Cull Off
		Lighting Off
		ZWrite Off
		ColorMask RGB

		CGINCLUDE

		#include "UnityCG.cginc"

		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl_no_auto_normalization
		#pragma multi_compile_particles

		sampler2D _MainTex;
		float4 _MainTex_ST;
		fixed4 _TintColor;
		fixed _Intensity;

#if defined(SOFTPARTICLES_ON)

		float _InvFade;
		sampler2D _CameraDepthTexture;

#endif

		struct appdata_t
		{
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
			// UNITY_FOG_COORDS(1)

#if defined(SOFTPARTICLES_ON)

			float4 projPos : TEXCOORD2;

#endif

		};

		ENDCG

		PASS
		{
			Name "MainPass"
			LOD 100

			CGPROGRAM

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

#if defined(SOFTPARTICLES_ON)

				o.projPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);

#endif

				o.color = v.color * _TintColor * _Intensity;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				// UNITY_TRANSFER_FOG(o, o.vertex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

#if defined(SOFTPARTICLES_ON)

				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate(_InvFade * (sceneZ - partZ));
				i.color.rgb = i.color.rgb * fade;

#endif

				// UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,0)); // fog towards black due to our blend mode
				return i.color * tex2D(_MainTex, i.texcoord);
			}

			ENDCG
		}
	}
}