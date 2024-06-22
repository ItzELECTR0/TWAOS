Shader "Custom/LightningBoltShaderMesh"
{
	Properties
	{
		[PerRendererData] _MainTex ("Main Texture (RGBA)", 2D) = "white" {}
		[PerRendererData] _TintColor ("Tint Color (RGB)", Color) = (1, 1, 1, 1)
		[PerRendererData] _InvFade ("Soft Particles Factor", Range(0.01, 100.0)) = 1.0
		[PerRendererData] _JitterMultiplier ("Jitter Multiplier (Float)", Float) = 0.0
		[PerRendererData] _Turbulence ("Turbulence (Float)", Float) = 0.0
		[PerRendererData] _TurbulenceVelocity ("Turbulence Velocity (Vector)", Vector) = (0, 0, 0, 0)
		[PerRendererData] _IntensityFlicker("Intensity flicker (Vector)", Vector) = (0, 0, 0, 0)
		[PerRendererData] _RenderMode("Render Mode - 0 = perspective, 1 = orthoxy, 2 = orthoxz (Int)", Int) = 0
		[PerRendererData] _SrcBlendMode("SrcBlendMode (Source Blend Mode)", Int) = 5 // SrcAlpha
		[PerRendererData] _DstBlendMode("DstBlendMode (Destination Blend Mode)", Int) = 1 // One, change to 10 for alpha blend instead of additive blend
    }

    SubShader
	{
		Tags { "Queue" = "Transparent" }
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest LEqual
		ColorMask RGBA
		Blend [_SrcBlendMode] [_DstBlendMode]

		CGINCLUDE
		
		#include "LightningShader.cginc"

		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma glsl_no_auto_normalization
		#pragma multi_compile_particles
		#pragma multi_compile_instancing

		ENDCG

		// glow pass
		Pass
		{
			Name "GlowPass"
			LOD 400

            CGPROGRAM

            v2f vert(appdata_t v)
            {
                WM_INSTANCE_VERT(v, v2f, o);

				float dirModifier = (v.texcoord.x - 0.5) + (v.texcoord.x - 0.5);
				float absRadius = abs(v.dir.w);
				float lineWidth = absRadius + absRadius;
				float jitter = 1.0 + (rand3(v.vertex) * _JitterMultiplier * 0.05);
				float t = _LightningTime.y;
				float glowWidthMultiplier = v.texcoord.z;
				float glowIntensity = v.texcoord.w;
				float elapsed = (t - v.fadeLifetime.r) / (v.fadeLifetime.a - v.fadeLifetime.r);
				float lineMultiplier = glowWidthMultiplier * lineWidth;
				float turbulence = lerp(0.0f, _Turbulence / max(0.5, absRadius), elapsed);
				float4 turbulenceVelocity = lerp(float4(0, 0, 0, 0), _TurbulenceVelocity, elapsed);
	
				UNITY_BRANCH
				if (_RenderMode == 0)
				{
					float4 turbulenceDirection = turbulenceVelocity + (float4(normalize(v.dir.xyz), 0) * turbulence);
					float3 directionBackwardsNormalized = normalize(v.dir2.xyz);
					float4 directionBackwards = float4(directionBackwardsNormalized * dirModifier * lineMultiplier * 1.5, 0);
					float3 directionToCamera = normalize(_WorldSpaceCameraPos - v.vertex);
					float4 tangent = float4(cross(directionBackwardsNormalized, directionToCamera), 0);
					dirModifier = v.dir.w / absRadius;
					float4 directionSideways = (tangent * lineMultiplier * dirModifier * jitter);
					o.pos = UnityObjectToClipPos(v.vertex + directionBackwards + directionSideways + turbulenceDirection);
				}
				else if (_RenderMode == 1)
				{
					float4 turbulenceDirection = float4(turbulenceVelocity.xy, 0, 0) + (float4(normalize(v.dir).xy, 0, 0) * turbulence);
					float2 directionBackwardsNormalized = normalize(v.dir2.xy);
					float4 directionBackwards = float4(directionBackwardsNormalized * dirModifier * lineMultiplier * 1.5, 0, 0);
					float2 tangent = normalize(float2(-v.dir2.y, v.dir2.x));
					dirModifier = v.dir.w / absRadius;
					float4 directionSideways = float4(tangent * lineMultiplier * dirModifier * jitter, 0, 0);
					o.pos = UnityObjectToClipPos(v.vertex + directionBackwards + directionSideways + turbulenceDirection);
				}
				else
				{
					float2 turbulenceDirection = (turbulenceVelocity.xz + normalize(v.dir.xz)) * turbulence;
					float4 turbulenceDirection4 = float4(turbulenceDirection.x, 0.0f, turbulenceDirection.y, 0.0f);
					float2 directionBackwardsNormalized = normalize(v.dir2.xz);
					float2 directionBackwards = directionBackwardsNormalized * dirModifier * lineMultiplier * 1.5;
					float4 directionBackwards4 = float4(directionBackwards.x, 0.0f, directionBackwards.y, 0.0f);
					float2 tangent = normalize(float2(-v.dir2.z, v.dir2.x));
					dirModifier = v.dir.w / absRadius;
					tangent = tangent * lineMultiplier * dirModifier * jitter;
					float4 directionSideways = float4(tangent.x, 0.0f, tangent.y, 0.0f);
					o.pos = UnityObjectToClipPos(v.vertex + directionBackwards4 + directionSideways + turbulenceDirection4);
				}

				o.color = (lerpColor(v.fadeLifetime) * _TintColor * fixed4(v.color.rgb, 1.0));
				o.color.a *= glowIntensity;
				o.texcoord = v.texcoord.xy;
				
#if defined(SOFTPARTICLES_ON)

                o.projPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);

#endif

                return o;
            }
			
            ENDCG
        }
    }
 
    Fallback Off
}