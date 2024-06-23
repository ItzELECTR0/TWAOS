void CTI_ShadowFade_float(
    float alpha,
    out float o_alpha
)
{
    #if (SHADERPASS == SHADERPASS_SHADOWS)
        o_alpha = 1.0;
    #else 
        o_alpha = alpha; 
    #endif
}

void CTI_ColorVariationFrag_float(
    real3       Albedo,
    real4       ColorVariation,

    out real3   o_Albedo
)
{
    float3 TreeWorldPos = UNITY_MATRIX_M._m03_m13_m23;
    #if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
        TreeWorldPos += _WorldSpaceCameraPos;
    #endif
    float ColorVariationStrength = saturate ( ( frac(TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) + frac( (TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) * 3.3 ) ) * 0.5 );
    o_Albedo = lerp(Albedo, (Albedo + ColorVariation.rgb) * 0.5, (ColorVariationStrength * ColorVariation.a).xxx );
}

void CTI_ColorVariation_float(
    half3       Albedo,
    half4       ColorVariation,
    half        ColorVariationStrength,

    out half3   o_Albedo
)
{
    o_Albedo = lerp(Albedo, (Albedo + ColorVariation.rgb) * 0.5, (ColorVariationStrength * ColorVariation.a).xxx );
}

void CTI_UnpackFixNormal_float(
    half4       normalSample,
    half        normalScale,
    float       isFrontFace, 
    out half3   o_normalTS  
)
{
    o_normalTS = UnpackNormalAG(normalSample, normalScale);
    o_normalTS.z *= isFrontFace ? 1 : -1;
}

void CTI_UnpackNormal_float(
    half4       normalSample,
    half        normalScale,
    out half3   o_normalTS  
)
{
    o_normalTS = UnpackNormalAG(normalSample, normalScale);
}

void CTI_UnpackNormalBillboard_float(
    half4       normalSample,
    half        normalScale,
    out half3   o_normalTS  
)
{
    // Up is flipped!
    normalSample.g = 1 - normalSample.g;
    o_normalTS = UnpackNormalAG(normalSample, normalScale);
}

void CTI_AlphaLeak_float(
    half        alphaSample,
    half        leak,
    out half    o_Occlusion 
)
{
    o_Occlusion = (alphaSample <= leak) ? 1 : alphaSample; // Eliminate alpha leaking into ao
}