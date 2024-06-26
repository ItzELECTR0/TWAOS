#include "GPUInstancerInclude.cginc"

#ifndef GPUI_F4Var_INCLUDED
#define GPUI_F4Var_INCLUDED

uniform StructuredBuffer<float4> gpuiFloat4Variation;

void gpuiF4Var_float(out float4 Float4_Out)
{
    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        Float4_Out = gpuiFloat4Variation[gpui_InstanceID];
    #else
        Float4_Out = float4(1, 1, 1, 1);
    #endif
}
#endif