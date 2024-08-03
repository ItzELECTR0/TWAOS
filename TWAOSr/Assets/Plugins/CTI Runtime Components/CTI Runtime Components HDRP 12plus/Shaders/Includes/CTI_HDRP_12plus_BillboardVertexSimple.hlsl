CBUFFER_START(UnityBillboardPerCamera)
	float3 unity_BillboardNormal;
	float3 unity_BillboardTangent;
	float4 unity_BillboardCameraParams;
	#define unity_BillboardCameraPosition (unity_BillboardCameraParams.xyz)
	#define unity_BillboardCameraXZAngle (unity_BillboardCameraParams.w)
CBUFFER_END

CBUFFER_START(UnityBillboardPerBatch)
	float3 unity_BillboardSize;
CBUFFER_END

#define unity_BillboardCameraPosition (unity_BillboardCameraParams.xyz)
float4 _CTI_SRP_Wind;

float2 SmoothCurve(float2 x) {
	return x * x * (3.0 - 2.0 * x);
}

float2 TriangleWave(float2 x) {
	return abs(frac(x + 0.5) * 2.0 - 1.0);
}

float2 SmoothTriangleWave(float2 x) {
	return (SmoothCurve(TriangleWave(x)) - 0.5) * 2.0;
}

// This function always return the absolute position in WS
float3 CTI_GetAbsolutePositionWS(float3 positionRWS)
{
#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
    positionRWS += _WorldSpaceCameraPos;
#endif
    return positionRWS;
}
// Return absolute world position of current object
float3 CTI_GetObjectAbsolutePositionWS()
{
    float4x4 modelMatrix = UNITY_MATRIX_M;
    return CTI_GetAbsolutePositionWS(modelMatrix._m03_m13_m23); // Translation object to world
}

//	Billboard Vertex Function

//	IMPORTANT: Only use original mesh attributes
//	colorVAriation has to be written to uv1!? otherwise it gets evaluated twice?!


// #if defined(LOD_FADE_CROSSFADE)
//  	#undef LOD_FADE_CROSSFADE
//  	#define CUSTOM_LODFADE
// #endif


void CTIBillboard_float (
	in float3  positionOS,
	in float2  uv,
	in float3  uv1,

	in float   windStrength,
	in float   windPower,

	in float3  timeParams,

	out float3 o_positionOS,
	out float3 o_normalOS,
	out float3 o_tangentOS,
	out float2 o_uv,
	out float o_cv
) {

	float3 position = positionOS;
	float3 positionWS = positionOS + CTI_GetObjectAbsolutePositionWS();
//	Get Color Variation
	o_cv = saturate ( ( frac(positionWS.x + positionWS.y + positionWS.z) + frac( (positionWS.x + positionWS.y + positionWS.z) * 3.3 ) ) * 0.5 );

// 	////////////////////////////////////
//	Set vertex position
	float3 positionRWS = TransformObjectToWorld(positionOS);
//	This does not work!
	//float3 eyeVec = normalize(unity_BillboardCameraPosition - positionWS);
	float3 eyeVec = GetWorldSpaceNormalizeViewDir(positionRWS);

//	NOTE: We have incorrect triangle winding...
	float3 billboardTangent = normalize(float3(-eyeVec.z, 0, eyeVec.x));
	float3 billboardNormal = -float3(billboardTangent.z, 0, -billboardTangent.x); // -float3! due to false triangle winding

	float2 percent = uv.xy;
	float3 billboardPos;
	billboardPos.xz = (percent.x - 0.5) * unity_BillboardSize.x * uv1.x * billboardTangent.xz;
	billboardPos.y = (percent.y * unity_BillboardSize.y + unity_BillboardSize.z) * uv1.y;
	
// 	////////////////////////////////////
//	Wind

	if (windStrength > 0) {
		//positionWS.xyz = abs(positionWS.xyz * 0.125f);
		float origLength = length(billboardPos);
		
		float sinuswave = sin(timeParams.x * 0.5); 						// _SinTime.z;
		float shiftedsinuswave = (sinuswave + timeParams.y) * 0.5; 		// (_SinTime.z + _SinTime.w) * 0.5;
		float2 vOscillations = SmoothTriangleWave( float2(positionWS.x + sinuswave, positionWS.z + sinuswave * 0.7) );
		float fOsc = vOscillations.x + (vOscillations.y * vOscillations.y);
		fOsc = 0.75 + (fOsc + 3.33) * 0.33;
		float percentage = pow(saturate(percent.y), windPower); // saturate added to stop warning on dx11.
		billboardPos += _CTI_SRP_Wind.xyz * ( _CTI_SRP_Wind.w * windStrength * fOsc * percentage);
		
		billboardPos = normalize(billboardPos) * origLength;
	}

//	Now bring it to the proper position
	position.xyz += billboardPos;
	o_positionOS = position.xyz;

// 	////////////////////////////////////
//	Get billboard texture coords

// 	Here we need the billboard Tangent and Normal in absolute WS?
	float3 viewVecAbsWS;
//	We have to distinguish between shadow caster and regular passes
	// #if(defined(SHADERPASS) && (SHADERPASS == SHADERPASS_SHADOWS))
	// 	viewVecAbsWS = normalize( GetCurrentViewPosition() - positionWS); // UNITY_MATRIX_I_V[2].xyz;
	// #else
	// 	viewVecAbsWS = normalize(_WorldSpaceCameraPos - positionWS);
	// #endif
	// float3 billboardTangentAbsWS = normalize(float3(-viewVecAbsWS.z, 0, viewVecAbsWS.x));
	// float3 billboardNormalAbsWS = float3(billboardTangentAbsWS.z, 0, -billboardTangentAbsWS.x); // no: -float !
	// float angle = atan2(billboardNormalAbsWS.z, billboardNormalAbsWS.x); // signed angle between billboardNormal to {0,0,1}

//	Actually this is more correct?
//	NOTE: billboardNormal is -billboardNormal
	float angle = atan2(-billboardNormal.z, -billboardNormal.x); // signed angle between billboardNormal to {0,0,1}
	angle += angle < 0 ? 2 * PI : 0;										
//	Set Rotation
	angle += uv1.z;
//	Write final billboard texture coords
	const float invDelta = 1.0 / (45.0 * ((PI * 2.0) / 360.0));
	float imageIndex = fmod(floor(angle * invDelta + 0.5f), 8.0);
	float2 column_row;
// 	We do not care about the horizontal coord that much as our billboard texture tiles
	column_row.x = imageIndex * 0.25; 
	column_row.y = saturate(4.0 - imageIndex) * 0.5;
	o_uv.xy = column_row + uv.xy * float2(0.25, 0.5);

// 	////////////////////////////////////
//	Set Normal and Tangent
	o_normalOS = billboardNormal.xyz;
	o_tangentOS = billboardTangent.xyz;

}