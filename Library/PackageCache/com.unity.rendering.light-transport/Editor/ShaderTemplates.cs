using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace UnityEditor.Rendering.UnifiedRayTracing
{
    internal class ShaderTemplates
    {
        // TODO: Uncomment when API is made public
        //[MenuItem("Assets/Create/Shader/Unified RayTracing Shader", false, 1)]
        internal static void CreateNewUnifiedRayTracingShader()
        {
            var action = ScriptableObject.CreateInstance<DoCreateUnifiedRayTracingShaders>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, "NewRayTracingShader.hlsl", null, null);
        }

        internal static Object CreateScriptAssetWithContent(string pathName, string templateContent)
        {
            string fullPath = Path.GetFullPath(pathName);
            File.WriteAllText(fullPath, templateContent);
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }

        internal class DoCreateUnifiedRayTracingShaders : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var includeName = Path.GetFileNameWithoutExtension(pathName);
                Object o = CreateScriptAssetWithContent(pathName, shaderContent);
                CreateScriptAssetWithContent(Path.ChangeExtension(pathName, ".compute"), computeShaderContent.Replace("SHADERNAME", includeName));
                CreateScriptAssetWithContent(Path.ChangeExtension(pathName, ".raytrace"), raytracingShaderContent.Replace("SHADERNAME", includeName));
                ProjectWindowUtil.ShowCreatedAsset(o);
            }
        }

const string computeShaderContent =
@"#define RAYTRACING_BACKEND_COMPUTE
#define GROUP_SIZE_X 16
#define GROUP_SIZE_Y 8
#define RAYTRACING_GROUP_SIZE GROUP_SIZE_X*GROUP_SIZE_Y
#include ""SHADERNAME.hlsl""

int g_DispatchWidth;
int g_DispatchHeight;
int g_DispatchDepth;

#pragma kernel MainRayGenShader
[numthreads(GROUP_SIZE_X, GROUP_SIZE_Y, 1)]
void MainRayGenShader(
    in uint3 gidx: SV_DispatchThreadID,
    in uint lidx : SV_GroupIndex)
{
    if (gidx.x >= uint(g_DispatchWidth) || gidx.y >= uint(g_DispatchHeight) || gidx.z >= uint(g_DispatchDepth))
        return;

    UnifiedRT::DispatchInfo dispatchInfo;
    dispatchInfo.dispatchThreadID = gidx;
    dispatchInfo.dispatchDimensionsInThreads = int3(g_DispatchWidth, g_DispatchHeight, g_DispatchDepth);
    dispatchInfo.localThreadIndex = lidx;
    dispatchInfo.globalThreadIndex = gidx.x + gidx.y * g_DispatchWidth + gidx.z * (g_DispatchWidth* g_DispatchHeight);

    RayGenExecute(dispatchInfo);
}
";

const string raytracingShaderContent =
@"#define RAYTRACING_BACKEND_HARDWARE
#include ""SHADERNAME.hlsl""

#pragma max_recursion_depth 1

[shader(""raygeneration"")]
void MainRayGenShader()
{
    UnifiedRT::DispatchInfo dispatchInfo;
    dispatchInfo.dispatchThreadID = DispatchRaysIndex();
    dispatchInfo.dispatchDimensionsInThreads = DispatchRaysDimensions();
    dispatchInfo.localThreadIndex = 0;
    dispatchInfo.globalThreadIndex = DispatchRaysIndex().x + DispatchRaysIndex().y * DispatchRaysDimensions().x + DispatchRaysIndex().z * (DispatchRaysDimensions().x * DispatchRaysDimensions().y);

    RayGenExecute(dispatchInfo);
}

[shader(""miss"")]
void MainMissShader0(inout UnifiedRT::Hit hit : SV_RayPayload)
{
    hit.instanceID = -1;
}
";

const string shaderContent =
@"#include ""Packages/com.unity.rendering.light-transport/Runtime/UnifiedRayTracing/FetchGeometry.hlsl""
#include ""Packages/com.unity.rendering.light-transport/Runtime/UnifiedRayTracing/TraceRay.hlsl""

UNITY_DECLARE_RT_ACCEL_STRUCT(_AccelStruct);

void RayGenExecute(UnifiedRT::DispatchInfo dispatchInfo)
{
    // Example code:
    UnifiedRT::Ray ray;
    ray.origin = 0;
    ray.direction = float3(0, 0, 1);
    ray.tMin = 0;
    ray.tMax = 1000.0f;
    UnifiedRT::RayTracingAccelStruct accelStruct = UNITY_GET_RT_ACCEL_STRUCT(_AccelStruct);
    UnifiedRT::Hit hitResult = UnifiedRT::TraceRayClosestHit(dispatchInfo, accelStruct, 0xFFFFFFFF, ray, 0);
    if (hitResult.IsValid())
    {
        UnifiedRT::HitGeomAttributes attributes = UnifiedRT::FetchHitGeomAttributes(accelStruct, hitResult);
    }

}
";
    }
}


