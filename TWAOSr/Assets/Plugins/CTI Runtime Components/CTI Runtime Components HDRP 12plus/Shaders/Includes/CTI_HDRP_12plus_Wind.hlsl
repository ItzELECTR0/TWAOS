float4 _CTI_SRP_Wind;
float  _CTI_SRP_Turbulence;

float3x3 GetRotationMatrix(float3 axis, float angle)
{
	//axis = normalize(axis); // moved to calling function
	float s = sin(angle);
	float c = cos(angle);
	float oc = 1.0 - c;

	return float3x3	(oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s,
		oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s,
		oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c);
}

float4 SmoothCurve(float4 x) {
	return x * x * (3.0 - 2.0 * x);
}
float4 TriangleWave(float4 x) {
	return abs(frac(x + 0.5) * 2.0 - 1.0);
}
float4 SmoothTriangleWave(float4 x) {
	return SmoothCurve(TriangleWave(x));
}

// Overloads for single float 
float SmoothCurve(float x) {
	return x * x * (3.0 - 2.0 * x);
}
float TriangleWave(float x) {
	return abs(frac(x + 0.5) * 2.0 - 1.0);
}
float SmoothTriangleWave(float x) {
	return SmoothCurve(TriangleWave(x));
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

//	////////////////////
//	Fast math
#define IEEE_INT_SQRT_CONST_NR0 0x1FBD1DF5 // 0-1, 1.42% error
float FastSqrt(float inX) {
	int x = asint(inX);
	x = IEEE_INT_SQRT_CONST_NR0 + (x >> 1);
	return asfloat(x);
}
inline float3 FastSign(float3 x) {
	#define FLT_MAX 3.402823466e+38 // Maximum representable floating-point number
	return saturate(x * FLT_MAX + 0.5) * 2.0 - 1.0;
}

void CTI_AnimateVertexSG_float(
	float3 	PositionOS,
	float4	VertexColor,
	float2	UV2,
	float3 	UV3,
	float3 	NormalOS,

	float 	tumbleStrength,
	float   tumbleFrequency,
	float 	leafTurbulence,
	float 	leafNoise,
	float3  windMultipliers,

	bool    enableNormalRotation,
	
	bool    enableAdvancedEdgeBending,
	float2  advancedEdgeBending,

	float3 	timeParams,

	bool 	isBark,

	out float3 	o_positionOS,
	out float3  o_normalOS,
	out float   o_occlusion
) {
	const float fDetailAmp = 0.1f;
	const float fBranchAmp = 0.3f;
	
	#define Phase VertexColor.r
	#define Flutter VertexColor.g
	#define BranchBending UV2.y
	#define MainBending UV2.x

//	Init output
	o_positionOS = PositionOS;
	o_normalOS = NormalOS;
//	Store ambient occlusion
	o_occlusion = VertexColor.a;

	const float3 TreeWorldPos = CTI_GetObjectAbsolutePositionWS();

	float sinuswave = sin(timeParams.x * 0.5);
	float shiftedsinuswave = (sinuswave + timeParams.y) * 0.5;

	float4 vOscillations = SmoothTriangleWave(float4(TreeWorldPos.x + sinuswave, TreeWorldPos.z + sinuswave * 0.7, TreeWorldPos.x + shiftedsinuswave, TreeWorldPos.z + shiftedsinuswave * 0.8));
	// x used for main wind bending / y used for tumbling
	float2 fOsc = vOscillations.xz + (vOscillations.yw * vOscillations.yw);
	fOsc = 0.75 + (fOsc + 3.33) * 0.33;

	// float fObjPhase = abs(frac((TreeWorldPos.x + TreeWorldPos.z) * 0.5) * 2 - 1);
	// NOTE: We have to limit (frac) fObjPhase in case we use fBranchPhase in tumbling or turbulence
	float fObjPhase = dot(TreeWorldPos, 1);
	float fBranchPhase = fObjPhase + Phase;
	float fVtxPhase = dot(o_positionOS, Flutter + fBranchPhase);

	// x is used for edge fluttering / y is used for branch bending
	float2 vWavesIn = timeParams.xx + float2(fVtxPhase, fBranchPhase);

	// 1.975, 0.793, 0.375, 0.193 are good frequencies
	float4 vWaves = (frac(vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193)) * 2.0 - 1.0);
	vWaves = SmoothTriangleWave(vWaves);
	float2 vWavesSum = vWaves.xz + vWaves.yw;

//	Get local wind
	#define absWindStrength _CTI_SRP_Wind.w

	float turbulence = _CTI_SRP_Turbulence;
	float3 windDir = mul((float3x3)GetWorldToObjectMatrix(), _CTI_SRP_Wind.xyz);
	float4 Wind = float4(windDir, absWindStrength);

//	Leaf specific bending
	if (!isBark)
	{
		float3 pivot;
		
	//  Decode UV3
		// 15bit compression 2 components only, important: sign of y
		pivot.xz = (frac(float2(1.0, 32768.0) * UV3.xx) * 2.0) - 1.0;
		pivot.y = sqrt(1.0 - saturate(dot(pivot.xz, pivot.xz)));
		pivot *= UV3.y;

		float tumbleInfluence = frac(VertexColor.b * 2.0);
	
	//	Move point to 0,0,0
		o_positionOS -= pivot;

		#if defined(_LEAFTUMBLING) || defined (_LEAFTURBULENCE)
			
			float3 fracs = frac(pivot * 33.3);

			float offset = fracs.x + fracs.y + fracs.z	/* this adds a lot of noise, so we use * 0.1 */ + (BranchBending  + Phase) * leafNoise;
			float tFrequency = tumbleFrequency * (timeParams.x/* new */ + fObjPhase * 10);
			float4 vWaves1 = SmoothTriangleWave(float4((tFrequency + offset) * (1.0 + offset * 0.25), tFrequency * 0.75 + offset, tFrequency * 0.5 + offset, tFrequency * 1.5 + offset));

			float3 windTangent = float3(-windDir.z, windDir.y, windDir.x);
			float twigPhase = vWaves1.x + vWaves1.y + (vWaves1.z * vWaves1.z);

			// This was the root of the fern issue: branchAxes slightly varied on different LODs!
			#if defined(_BAKEDBRANCHAXIS)
            	float3 branchAxis = frac( UV3.z * float3(1.0, 256.0, 65536.0) );
            	branchAxis = branchAxis * 2.0 - 1.0;
            	branchAxis = normalize(branchAxis);
			#else
				// Just rotation around y up seems to be fine
				float3 branchAxis = cross(windTangent, float3(-1, 0, 1));
			#endif
            
            // We could do better in case we have the baked branch main axis
			// float facingWind = dot(normalize(float3(position.x, 0, position.z)), windDir);
            // Can we do better in case we have the baked branch main axis?
            float facingWind = (dot(branchAxis, windDir));

			float localWindStrength = absWindStrength * tumbleInfluence; // * (1.35 - facingWind);

		//  tumbling
			#if defined(_LEAFTUMBLING)
				// float angleTumble = 
				// 	(twigPhase + fBranchPhase * 0.25 + BranchBending)
				// 	 * localWindStrength * tumbleStrength
				// 	 * fOsc.y
				// ;
			//	Let's keep it simple
				float angleTumble = (twigPhase + BranchBending) * localWindStrength * tumbleStrength;
				
				float3x3 tumbleRot = GetRotationMatrix(windTangent, angleTumble);
				o_positionOS = mul(tumbleRot, o_positionOS);
				if(enableNormalRotation)
				{
					o_normalOS = mul(tumbleRot, o_normalOS);
				}
			#endif


		//  turbulence
	        #if defined (_LEAFTURBULENCE)
				// float angleTurbulence =
				// 	// center rotation so the leaves rotate leftwards as well as rightwards according to the incoming waves
				// 	((twigPhase + vWaves1.w) * 0.25 - 0.5)
				// 	// make rotation strength depend on absWindStrength and all other inputs
				// 	* localWindStrength * leafTurbulence * saturate(lerp(1.0, Flutter * 8.0, _EdgeFlutterInfluence))
				// 	* fOsc.x
				// ;
			//	Let's keep it simple
				float angleTurbulence =
					((twigPhase + vWaves1.w) * 0.25 - 0.5)
					* localWindStrength * leafTurbulence * saturate(lerp(1.0, Flutter * 8.0, _EdgeFlutterInfluence))
				;
				
				float3x3 turbulenceRot = GetRotationMatrix( -branchAxis, angleTurbulence);
				o_positionOS = mul(turbulenceRot, o_positionOS);
				if(enableNormalRotation)
				{
					o_normalOS = mul(turbulenceRot, o_normalOS);	
				}
				
			#endif

		#endif

	//	fade in/out leave planes
			// float lodfade = ceil(pos.w - 0.51);
	//  asset store
			// float lodfade = (pos.w > 0.5) ? 1 : 0;
	//  latest
		if (unity_LODFade.x < 1.0)
		{
			float lodfade = (VertexColor.b > (1.0f / 255.0f * 126.0f) ) ? 1 : 0; // Make sure that the 1st vertex is taken into account
			o_positionOS.xyz *= 1.0 - unity_LODFade.x * lodfade;
		}

	//	Move point back to origin
		o_positionOS.xyz += pivot;

	//  Advanced edge bending (has to be outside preserve length)
        if (enableAdvancedEdgeBending)
        {
            o_positionOS.xyz += o_normalOS.xyz * SmoothTriangleWave( tumbleInfluence * timeParams.x * advancedEdgeBending.y + Phase ) * advancedEdgeBending.x * Flutter * absWindStrength;
        }
	}

//	Preserve Length
	float origLength = length(o_positionOS);

//  Apply secondary bending and edge flutter
	float3 bend = Flutter * fDetailAmp * o_normalOS * FastSign(o_normalOS) * windMultipliers.z;
	bend.y = BranchBending * fBranchAmp * windMultipliers.y;
	o_positionOS += vWavesSum.xyx * bend * absWindStrength + vWavesSum.y * BranchBending * turbulence * windMultipliers.y;

//	Primary bending: Displace position
	o_positionOS += MainBending * Wind.xyz * fOsc.x * absWindStrength * windMultipliers.x;

//	Preserve Length
	o_positionOS = normalize(o_positionOS) * origLength;

//	Store Variation 
    //colorOut = saturate((frac(TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) + frac((TreeWorldPos.x + TreeWorldPos.y + TreeWorldPos.z) * 3.3)) * 0.5);
}