#ifndef GPU_INSTANCER_INCLUDED
#define GPU_INSTANCER_INCLUDED

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

uniform uint gpui_InstanceID;
uniform float4x4 gpuiTransformOffset;
uniform float LODLevel; // -1 if crossFade disabled, 0-7 if cross fade enabled
uniform float fadeLevelMultiplier; // 1 no animation
#if GPUI_MHT_COPY_TEXTURE
    uniform sampler2D gpuiTransformationMatrixTexture;
    uniform sampler2D gpuiInstanceLODDataTexture;
    uniform float bufferSize;
    uniform float maxTextureSize;
#elif GPUI_MHT_MATRIX_APPEND
    uniform StructuredBuffer<float4x4> gpuiTransformationMatrix;
#else
    uniform StructuredBuffer<uint> gpuiTransformationMatrix;
    uniform StructuredBuffer<float4x4> gpuiInstanceData;
    uniform StructuredBuffer<uint4> gpuiInstanceLODData;
#endif

#ifdef unity_ObjectToWorld
#undef unity_ObjectToWorld
#endif

#ifdef unity_WorldToObject
#undef unity_WorldToObject
#endif

#endif // UNITY_PROCEDURAL_INSTANCING_ENABLED

void setupGPUI()
{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
#if GPUI_MHT_COPY_TEXTURE
    uint textureWidth = min(bufferSize, maxTextureSize);

    float indexX = ((unity_InstanceID % maxTextureSize) + 0.5f) / textureWidth;
    float rowCount = ceil(bufferSize / maxTextureSize) * 4.0f;
    float rowIndex = floor(unity_InstanceID / maxTextureSize) * 4.0f + 0.5f;

    float4x4 transformData = float4x4(
        tex2Dlod(gpuiTransformationMatrixTexture, float4(indexX, (0.0f + rowIndex) / rowCount, 0.0f, 0.0f)), // row0
        tex2Dlod(gpuiTransformationMatrixTexture, float4(indexX, (1.0f + rowIndex) / rowCount, 0.0f, 0.0f)), // row1
        tex2Dlod(gpuiTransformationMatrixTexture, float4(indexX, (2.0f + rowIndex) / rowCount, 0.0f, 0.0f)), // row2
        tex2Dlod(gpuiTransformationMatrixTexture, float4(indexX, (3.0f + rowIndex) / rowCount, 0.0f, 0.0f))  // row3
    );
    gpui_InstanceID = uint(transformData._44);
    transformData._44 = 1.0f;
    unity_ObjectToWorld = mul(transformData, gpuiTransformOffset);
#elif GPUI_MHT_MATRIX_APPEND
    unity_ObjectToWorld = mul(gpuiTransformationMatrix[unity_InstanceID], gpuiTransformOffset);
#else        
    gpui_InstanceID = gpuiTransformationMatrix[unity_InstanceID];
    unity_ObjectToWorld = mul(gpuiInstanceData[gpui_InstanceID], gpuiTransformOffset);
#endif // SHADER_API
    
#ifdef LOD_FADE_CROSSFADE
    unity_LODFade.x = 1;
    unity_LODFade.y = 16;
    if (LODLevel >= 0)
    {
#ifdef GPUI_MHT_COPY_TEXTURE
        rowCount = ceil(bufferSize / maxTextureSize);
        rowIndex = floor(gpui_InstanceID / maxTextureSize) + 0.5f;
        uint4 lodData = uint4(tex2Dlod(gpuiInstanceLODDataTexture, float4(indexX, rowIndex / rowCount, 0.0f, 0.0f)));
#else
        uint4 lodData = gpuiInstanceLODData[gpui_InstanceID];
#endif
        if (lodData.w > 0)
        {
            float fadeLevel = floor(lodData.w * fadeLevelMultiplier);

            if (lodData.z == uint(LODLevel))
                fadeLevel = - fadeLevel;
            unity_LODFade.x = fadeLevel / 16.0;
            unity_LODFade.y = fadeLevel;
        }
    }
#endif

	// inverse transform matrix
	// taken from richardkettlewell's post on
	// https://forum.unity3d.com/threads/drawmeshinstancedindirect-example-comments-and-questions.446080/

	float3x3 w2oRotation;
	w2oRotation[0] = unity_ObjectToWorld[1].yzx * unity_ObjectToWorld[2].zxy - unity_ObjectToWorld[1].zxy * unity_ObjectToWorld[2].yzx;
	w2oRotation[1] = unity_ObjectToWorld[0].zxy * unity_ObjectToWorld[2].yzx - unity_ObjectToWorld[0].yzx * unity_ObjectToWorld[2].zxy;
	w2oRotation[2] = unity_ObjectToWorld[0].yzx * unity_ObjectToWorld[1].zxy - unity_ObjectToWorld[0].zxy * unity_ObjectToWorld[1].yzx;

	float det = dot(unity_ObjectToWorld[0].xyz, w2oRotation[0]);

	w2oRotation = transpose(w2oRotation);

	w2oRotation *= rcp(det);

	float3 w2oPosition = mul(w2oRotation, -unity_ObjectToWorld._14_24_34);

	unity_WorldToObject._11_21_31_41 = float4(w2oRotation._11_21_31, 0.0f);
	unity_WorldToObject._12_22_32_42 = float4(w2oRotation._12_22_32, 0.0f);
	unity_WorldToObject._13_23_33_43 = float4(w2oRotation._13_23_33, 0.0f);
	unity_WorldToObject._14_24_34_44 = float4(w2oPosition, 1.0f);
    
#if defined(UNITY_PREV_MATRIX_M) && !defined(BUILTIN_TARGET_API)
    unity_MatrixPreviousM = unity_ObjectToWorld;
#endif
#endif // UNITY_PROCEDURAL_INSTANCING_ENABLED
}

#endif // GPU_INSTANCER_INCLUDED
// GPUInstancerInclude.cginc [do not delete needed for IsShaderInstanced check]