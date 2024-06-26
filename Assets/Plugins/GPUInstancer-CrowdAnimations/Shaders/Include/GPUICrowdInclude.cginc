
#include "GPUICrowdSkinning.cginc"

#ifndef GPUI_CROWD_ANIMATIONS_INCLUDED
#define GPUI_CROWD_ANIMATIONS_INCLUDED

#ifndef GPUI_CUSTOM_INPUT
    #define GPUI_STRUCT_NAME appdata_full
    #define GPUI_BONE_INDEX texcoord2
    #define GPUI_BONE_WEIGHT texcoord3
    #define GPUI_VERTEX vertex
    #define GPUI_NORMAL normal
    #define GPUI_TANGENT tangent
#endif

#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED) || defined(GPUI_CA_TEXTURE)

void gpuiAnimate(inout GPUI_STRUCT_NAME v)
{
    gpuiCASkinning(v.GPUI_BONE_INDEX, v.GPUI_BONE_WEIGHT, v.GPUI_VERTEX.xyz, v.GPUI_NORMAL.xyz, v.GPUI_TANGENT.xyz, v.GPUI_VERTEX.xyz, v.GPUI_NORMAL.xyz, v.GPUI_TANGENT.xyz);
}

#define GPUI_CROWD_VERTEX(v) gpuiAnimate(v)

#else // UNITY_PROCEDURAL_INSTANCING_ENABLED

#define GPUI_CROWD_VERTEX(v)

#endif // UNITY_PROCEDURAL_INSTANCING_ENABLED
#endif // GPUI_CROWD_ANIMATIONS_INCLUDED