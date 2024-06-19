Shader "RayTracing/StandardMaterial"
{
    SubShader
    {
        Pass
        {
            Name "RayTracing"

            HLSLPROGRAM

            #define RAYTRACING_BACKEND_HARDWARE
            #include "Packages/com.unity.rendering.light-transport/Runtime/UnifiedRayTracing/Bindings.hlsl"

            #pragma raytracing test

            struct AttributeData
            {
                float2 barycentrics;
            };

            [shader("closesthit")]
            void ClosestHitMain(inout UnifiedRT::Hit payload : SV_RayPayload, AttributeData attribs : SV_IntersectionAttributes)
            {
                payload.instanceIndex = InstanceID();
                payload.primitiveIndex = PrimitiveIndex();
                payload.uvBarycentrics = attribs.barycentrics;
                payload.hitDistance = RayTCurrent();
                payload.isFrontFace = (HitKind() == HIT_KIND_TRIANGLE_FRONT_FACE);
            }

            ENDHLSL
        }
    }
}
