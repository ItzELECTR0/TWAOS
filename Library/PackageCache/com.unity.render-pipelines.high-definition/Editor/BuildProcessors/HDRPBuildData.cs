using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
    internal class HDRPBuildData : IDisposable
    {
        static HDRPBuildData m_Instance = null;
        public static HDRPBuildData instance => m_Instance ??= new(EditorUserBuildSettings.activeBuildTarget, Debug.isDebugBuild);

        public bool buildingPlayerForHDRenderPipeline { get; private set; }

        public List<HDRenderPipelineAsset> renderPipelineAssets { get; private set; } = new List<HDRenderPipelineAsset>();
        public bool playerNeedRaytracing { get; private set; }
        public bool stripDebugVariants { get; private set; } = true;
        public Dictionary<int, ComputeShader> rayTracingComputeShaderCache { get; private set; } = new();
        public Dictionary<int, ComputeShader> computeShaderCache { get; private set; } = new();

        public HDRPBuildData()
        {

        }

        public HDRPBuildData(BuildTarget buildTarget, bool isDevelopmentBuild)
        {
            buildingPlayerForHDRenderPipeline = false;

            if (buildTarget.TryGetRenderPipelineAssets<HDRenderPipelineAsset>(renderPipelineAssets))
            {
                buildingPlayerForHDRenderPipeline = true;

                foreach (var hdrpAsset in renderPipelineAssets)
                {
                    if (hdrpAsset.currentPlatformRenderPipelineSettings.supportRayTracing)
                    {
                        playerNeedRaytracing = true;
                        break;
                    }
                }

                var hdrpGlobalSettingsInstance = HDRenderPipelineGlobalSettings.Ensure();
                if (hdrpGlobalSettingsInstance != null)
                {
                    GraphicsSettings.GetRenderPipelineSettings<HDRPRayTracingResources>()
                        .ForEachFieldOfType<ComputeShader>(computeShader => rayTracingComputeShaderCache.Add(computeShader.GetInstanceID(), computeShader));

                    var runtimeShaders = GraphicsSettings.GetRenderPipelineSettings<HDRenderPipelineRuntimeShaders>();
                    runtimeShaders?.ForEachFieldOfType<ComputeShader>(computeShader => computeShaderCache.Add(computeShader.GetInstanceID(), computeShader));

                    stripDebugVariants = !isDevelopmentBuild || GraphicsSettings.GetRenderPipelineSettings<ShaderStrippingSetting>().stripRuntimeDebugShaders;
                }
            }

            m_Instance = this;
        }

        public void Dispose()
        {
            renderPipelineAssets?.Clear();
            rayTracingComputeShaderCache?.Clear();
            computeShaderCache?.Clear();
            playerNeedRaytracing = false;
            stripDebugVariants = true;
            buildingPlayerForHDRenderPipeline = false;
            m_Instance = null;
        }
    }
}
