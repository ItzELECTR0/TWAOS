#ifndef __LIGHTNING_SHADER_INCLUDE__
#define __LIGHTNING_SHADER_INCLUDE__

#include "UnityCG.cginc"

#define WM_BASE_VERTEX_INPUT UNITY_VERTEX_INPUT_INSTANCE_ID
#define WM_BASE_VERTEX_TO_FRAG UNITY_VERTEX_INPUT_INSTANCE_ID UNITY_VERTEX_OUTPUT_STEREO
#define WM_INSTANCE_VERT(v, type, o) type o; UNITY_SETUP_INSTANCE_ID(v); UNITY_INITIALIZE_OUTPUT(type, o); UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#define WM_INSTANCE_FRAG(i) UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

uniform fixed4 _TintColor;
uniform float4 _MainTex_ST;
uniform sampler2D _MainTex;

#if defined(SOFTPARTICLES_ON)

uniform float _InvFade;
uniform UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

#endif

uniform float4 _LightningTime;
uniform float _JitterMultiplier;
uniform float _Turbulence;
uniform float4 _TurbulenceVelocity;
uniform float4 _IntensityFlicker;
uniform int _RenderMode;

struct appdata_t
{
	float4 vertex : POSITION;
	float4 dir : TANGENT;
	float4 color : COLOR;
	float3 dir2 : NORMAL;
	float4 texcoord : TEXCOORD0;
	float4 fadeLifetime : TEXCOORD1;
	WM_BASE_VERTEX_INPUT
};

struct v2f
{
	float2 texcoord : TEXCOORD0;
	fixed4 color : COLOR0;
	float4 pos : SV_POSITION;

#if defined(SOFTPARTICLES_ON)

	float4 projPos : TEXCOORD1;

#endif

	WM_BASE_VERTEX_TO_FRAG
};

inline float rand2(float2 pos) { return frac(sin(dot(pos, float2(12.9898, 78.233))) * 43758.5453); }
inline float rand3(float3 pos) { return frac(sin(dot(_LightningTime.xyz * pos, float3(12.9898, 78.233, 45.5432))) * 43758.5453); }
inline float hash(float p) { p = frac(p * 0.011); p *= p + 7.5; p *= p + p; return frac(p); }
inline float noise1d(float x) { float i = floor(x); float f = frac(x); float u = f * f * (3.0 - 2.0 * f); return lerp(hash(i), hash(i + 1.0), u); }

inline float computeFlicker(float offset)
{
	//return saturate(rand2(float2(_LightningTime.y * _IntensityFlicker.y, offset)) * _IntensityFlicker.x);
	return saturate(noise1d((_LightningTime.y * (_IntensityFlicker.y + offset)))) * _IntensityFlicker.x;
}

// float rand(float n) { return frac(sin(n) * 43758.5453123); }
// float noise(float p) { float fl = floor(p); float fc = frac(p); return lerp(rand(fl), rand(fl + 1.0), fc); }

inline float lerpColor(float4 c)
{
	// the vertex will fade in, stay at full color, then fade out
	// r = start time
	// g = peak start time
	// b = peak end time
	// a = end time

	// debug
	// return 1;

	float t = _LightningTime.y;
	float lerpMultiplier = (t < c.g);
	float lerpIn = lerp(0, 1, saturate((t - c.r) / max(0.00001, 0.00001 + c.g - c.r)));
	float lerpOut = lerp(1, 0, saturate((t - c.b) / max(0.00001, c.a - c.b)));
	float flicker = 1.0;

	UNITY_BRANCH
	if (_IntensityFlicker.x > 0.0 && _IntensityFlicker.y > 0.0)
	{
		flicker = computeFlicker(c.a);
	}

	return saturate(((lerpMultiplier * lerpIn) + ((1.0 - lerpMultiplier) * lerpOut))) * flicker;

	/*
	float t = _LightningTime.y;
	if (t < c.g)
	{
		return lerp(0.0, 1.0, ((t - c.r) / (c.g - c.r)));
	}
	return lerp(1.0, 0.0, max(0, ((t - c.b) / (c.a - c.b))));
	*/
}

inline fixed4 fragMethod(sampler2D tex, v2f i)
{
	WM_INSTANCE_FRAG(i);

#if defined(SOFTPARTICLES_ON)

	//float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
	float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
	float partZ = i.projPos.z;
	i.color.a *= saturate(_InvFade * (sceneZ - partZ));

#endif

	fixed4 c = tex2D(tex, i.texcoord);
	return (c * i.color);
}

fixed4 frag(v2f i) : SV_Target
{
	return fragMethod(_MainTex, i);
}

#endif
