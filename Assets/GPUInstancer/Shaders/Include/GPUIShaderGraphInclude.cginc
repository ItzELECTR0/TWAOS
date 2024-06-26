#include "GPUInstancerInclude.cginc"

#ifndef GPUI_SG_INCLUDED
#define GPUI_SG_INCLUDED
void gpuiDummy_float(float3 Input, out float3 Output)
{
    Output = Input;
}

void gpuiDummy_half(half3 Input, out half3 Output)
{
    Output = Input;
}
#endif