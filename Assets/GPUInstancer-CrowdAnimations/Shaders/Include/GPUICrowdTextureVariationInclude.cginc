#include "./../../../GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc"

#ifndef gpuiTextureVar_INCLUDED
#define gpuiTextureVar_INCLUDED

uniform StructuredBuffer<float4> textureUVBuffer;

void gpuiTextureVar_float(float4 TextureUV, out float4 Float4_Out)
{
    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        Float4_Out = textureUVBuffer[gpui_InstanceID];
    #else
        Float4_Out = TextureUV;
    #endif
}
#endif