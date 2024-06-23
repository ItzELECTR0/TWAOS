
void CTIBillboardFragment_float (
	in float4  		UV,
	in float2       BlendCv,
	in float4  		ScreenPosRaw,

	out float3 		o_Albedo,
	out float  		o_Alpha,
	out float3 		o_NormalTS,
	out float  		o_Smoothness,
	out float 		o_Occlusion,
	out float  		o_Thickness
) {


	float alphaA = SAMPLE_TEXTURE2D(_Alpha, sampler_Alpha, UV.xy);

	float4 sampleA = SAMPLE_TEXTURE2D(_BaseColorMap, sampler_BaseColorMap, UV.xy);
	float4 n_sampleA = SAMPLE_TEXTURE2D(_BumpTex, sampler_BumpTex, UV.xy);

	if (_BlendBB)
	{
		
		float alphaB = SAMPLE_TEXTURE2D(_Alpha, sampler_Alpha, UV.zw);

		float4 sampleB = SAMPLE_TEXTURE2D(_BaseColorMap, sampler_BaseColorMap, UV.zw);
		float4 n_sampleB = SAMPLE_TEXTURE2D(_BumpTex, sampler_BumpTex, UV.zw);

		float blend = saturate(1.0 - BlendCv.x);
	
	//	Dither
		if (_BlendBBDithering)
		{
			uint taaEnabled = _TaaFrameInfo.w;
		    float2 screenPos = floor( (ScreenPosRaw.xy / ScreenPosRaw.w) * _ScreenParams.xy);
		    float alphaClip = InterleavedGradientNoise(float4(screenPos, ScreenPosRaw.zw), (_FrameCount % 8u) * taaEnabled);
			// float alphaClip = GenerateHashedRandomFloat( asuint((int2)screenPos.xy) );
			blend = saturate( (blend - alphaClip) * 1000);
		}
	//	Alpha
		alphaA = alphaA * blend + alphaB * (1.0 - blend);
	//  Base Color and Occlusion
		sampleA = sampleA * blend;
		sampleA += sampleB * (1.0 - blend);
	//	NTS
		n_sampleA = lerp(n_sampleA, n_sampleB, (1.0 - blend));
	}

	o_Alpha = alphaA;
    o_NormalTS = UnpackNormalAG(n_sampleA, _NormalScale);
//	Add Color Variation
	o_Albedo = lerp(sampleA.rgb, (sampleA.rgb + _HueVariation.rgb) * 0.5, (BlendCv.y * _HueVariation.a).xxx );
	o_Occlusion = lerp(1.0, sampleA.a, _Occlusion);
    o_Smoothness = n_sampleA.b * _Smoothness;
    o_Thickness = 1.0 - n_sampleA.r * _ThicknessRemap;
}