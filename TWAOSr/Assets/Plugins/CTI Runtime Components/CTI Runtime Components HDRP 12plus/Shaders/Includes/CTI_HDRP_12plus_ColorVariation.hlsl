

// This function always return the absolute position in WS
float3 CTI_GetAbsolutePositionWS_Fragment(float3 positionRWS)
{
#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
    positionRWS += _WorldSpaceCameraPos;
#endif
    return positionRWS;
}
// Return absolute world position of current object
float3 CTI_GetObjectAbsolutePositionWS_Fragment()
{
    float4x4 modelMatrix = UNITY_MATRIX_M;
    return CTI_GetAbsolutePositionWS_Fragment(modelMatrix._m03_m13_m23); // Translation object to world
}

void CTIColorVariation_float(
	out float   colorOut
) {
//  unity_ObjectToWorld does not work... but at least we can use the functions from HDRP!
	const float3 TreeWorldPos = CTI_GetObjectAbsolutePositionWS_Fragment();
//	Store Variation
	colorOut = saturate ( ( frac(TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) + frac( (TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) * 3.3 ) ) * 0.5 );
}