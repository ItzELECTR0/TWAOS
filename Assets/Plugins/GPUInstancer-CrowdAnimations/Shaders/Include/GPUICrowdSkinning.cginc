
#include "./../../../GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc"

#ifndef GPUI_CROWD_SKINNING_INCLUDED
#define GPUI_CROWD_SKINNING_INCLUDED

uniform float totalNumberOfBones;
uniform float4x4 bindPoseOffset;

#ifdef GPUI_CA_TEXTURE
sampler2D gpuiAnimationTexture;
float4 animationTextureSize;
float frameIndex;

float4x4 getBoneMatrixFromTexture(int frameIndex, float boneIndex)
{
	uint textureIndex = frameIndex * totalNumberOfBones + boneIndex;

	float4 texIndex = float4(((textureIndex % animationTextureSize.x) + 0.5) / animationTextureSize.x, 
		((floor(textureIndex / animationTextureSize.x) * 4) + 0.5) / animationTextureSize.y, 0, 0);
	float offset = 1.0f / animationTextureSize.y;

	float4x4 boneMatrix;
	boneMatrix._11_21_31_41 = tex2Dlod(gpuiAnimationTexture, texIndex);
	texIndex.y += offset;
	boneMatrix._12_22_32_42 = tex2Dlod(gpuiAnimationTexture, texIndex);
	texIndex.y += offset;
	boneMatrix._13_23_33_43 = tex2Dlod(gpuiAnimationTexture, texIndex);
	texIndex.y += offset;
	boneMatrix._14_24_34_44 = tex2Dlod(gpuiAnimationTexture, texIndex);
	return mul(boneMatrix, bindPoseOffset);
}

void gpuiCASkinning(float4 boneIndex, float4 boneWeight, float3 vertex, float3 normal, float3 tangent, out float3 out_vertex, out float3 out_normal, out float3 out_tangent)
{    
	uint frameStart = floor(frameIndex);
	uint frameEnd = ceil(frameIndex);
	float progress = frac(frameIndex);
		
	float4x4 boneMatrix = mul(getBoneMatrixFromTexture(frameStart, boneIndex.x), boneWeight.x * (1.0 - progress));
	boneMatrix += mul(getBoneMatrixFromTexture(frameStart, boneIndex.y), boneWeight.y * (1.0 - progress));
	boneMatrix += mul(getBoneMatrixFromTexture(frameStart, boneIndex.z), boneWeight.z * (1.0 - progress));
	boneMatrix += mul(getBoneMatrixFromTexture(frameStart, boneIndex.w), boneWeight.w * (1.0 - progress));

	boneMatrix += mul(getBoneMatrixFromTexture(frameEnd, boneIndex.x), boneWeight.x * progress);
	boneMatrix += mul(getBoneMatrixFromTexture(frameEnd, boneIndex.y), boneWeight.y * progress);
	boneMatrix += mul(getBoneMatrixFromTexture(frameEnd, boneIndex.z), boneWeight.z * progress);
	boneMatrix += mul(getBoneMatrixFromTexture(frameEnd, boneIndex.w), boneWeight.w * progress);

    float3x3 boneMat3 = (float3x3) boneMatrix;
    out_vertex = mul(boneMat3, vertex) +  boneMatrix._14_24_34;
    out_normal = normalize(mul(boneMat3, normal));
    out_tangent = normalize(mul(boneMat3, tangent));
}
#else // GPUI_CA_TEXTURE

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
#if GPUI_MHT_COPY_TEXTURE
    uniform sampler2D gpuiAnimationBufferTexture;
#else
    uniform StructuredBuffer<float4x4> gpuiAnimationBuffer;
#endif

#if GPUI_MHT_COPY_TEXTURE
float4x4 getBoneMatrix(uint index)
{
    uint elementCount = totalNumberOfBones * bufferSize;
    uint textureWidth = min(elementCount, maxTextureSize);
    float indexX = ((index % maxTextureSize) + 0.5) / textureWidth;
    float rowCount = ceil(elementCount / maxTextureSize) * 4.0f;
    float rowIndex = floor(index / maxTextureSize) * 4.0f + 0.5f;

    return float4x4(
        tex2Dlod(gpuiAnimationBufferTexture, float4(indexX, (0.0f + rowIndex) / rowCount, 0.0f, 0.0f)), // row0
        tex2Dlod(gpuiAnimationBufferTexture, float4(indexX, (1.0f + rowIndex) / rowCount, 0.0f, 0.0f)), // row1
        tex2Dlod(gpuiAnimationBufferTexture, float4(indexX, (2.0f + rowIndex) / rowCount, 0.0f, 0.0f)), // row2
        tex2Dlod(gpuiAnimationBufferTexture, float4(indexX, (3.0f + rowIndex) / rowCount, 0.0f, 0.0f))  // row3
    );
}
#else
float4x4 getBoneMatrix(uint index)
{
    return gpuiAnimationBuffer[index];
}
#endif

void gpuiCASkinning(float4 boneIndex, float4 boneWeight, float3 vertex, float3 normal, float3 tangent, out float3 out_vertex, out float3 out_normal, out float3 out_tangent)
{
    uint addition = gpui_InstanceID * totalNumberOfBones;

    // required for some shader compilers like PSSL
    float4x4 weightX = float4x4(
            boneWeight.x, 0, 0, 0,
            0, boneWeight.x, 0, 0,
            0, 0, boneWeight.x, 0,
            0, 0, 0, boneWeight.x
        );
    float4x4 weightY = float4x4(
            boneWeight.y, 0, 0, 0,
            0, boneWeight.y, 0, 0,
            0, 0, boneWeight.y, 0,
            0, 0, 0, boneWeight.y
        );
    float4x4 weightZ = float4x4(
            boneWeight.z, 0, 0, 0,
            0, boneWeight.z, 0, 0,
            0, 0, boneWeight.z, 0,
            0, 0, 0, boneWeight.z
        );
    float4x4 weightW = float4x4(
            boneWeight.w, 0, 0, 0,
            0, boneWeight.w, 0, 0,
            0, 0, boneWeight.w, 0,
            0, 0, 0, boneWeight.w
        );

#ifdef GPUI_CA_BINDPOSEOFFSET
    float4x4 boneMatrix = mul(mul(getBoneMatrix(addition + boneIndex.x), bindPoseOffset), weightX);
    boneMatrix += mul(mul(getBoneMatrix(addition + boneIndex.y), bindPoseOffset), weightY);
    boneMatrix += mul(mul(getBoneMatrix(addition + boneIndex.z), bindPoseOffset), weightZ);
    boneMatrix += mul(mul(getBoneMatrix(addition + boneIndex.w), bindPoseOffset), weightW);
#else
    float4x4 boneMatrix = mul(getBoneMatrix(addition + boneIndex.x), weightX);
    boneMatrix += mul(getBoneMatrix(addition + boneIndex.y), weightY);
    boneMatrix += mul(getBoneMatrix(addition + boneIndex.z), weightZ);
    boneMatrix += mul(getBoneMatrix(addition + boneIndex.w), weightW);	
#endif

    float3x3 boneMat3 = (float3x3) boneMatrix;
    out_vertex = mul(boneMat3, vertex) +  boneMatrix._14_24_34;
    out_normal = normalize(mul(boneMat3, normal));
    out_tangent = normalize(mul(boneMat3, tangent));
}

#else // UNITY_PROCEDURAL_INSTANCING_ENABLED

void gpuiCASkinning(float4 boneIndex, float4 boneWeight, float3 vertex, float3 normal, float3 tangent, out float3 out_vertex, out float3 out_normal, out float3 out_tangent)
{
    out_vertex = vertex;
    out_normal = normal;
    out_tangent = tangent;
}
#endif // UNITY_PROCEDURAL_INSTANCING_ENABLED
#endif // GPUI_CA_TEXTURE

#endif // GPUI_CROWD_SKINNING_INCLUDED