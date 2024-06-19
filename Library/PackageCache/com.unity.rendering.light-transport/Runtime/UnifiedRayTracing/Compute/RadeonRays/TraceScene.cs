
namespace UnityEngine.Rendering.RadeonRays
{
    internal class TraceScene
    {
        const uint kGroupSize = 128u;
        const uint kStackSize = 64u;

        private ComputeShader shaderTrace;
        private int kernelTrace;

        public TraceScene(RadeonRaysShaders shaders)
        {
            shaderTrace = shaders.topLevelIntersector;
            kernelTrace = shaderTrace.FindKernel("TraceRays");
        }

        public void Execute(
            CommandBuffer cmd,
            GraphicsBuffer rays, uint rayCount, GraphicsBuffer indirectRayCount,
            RayQueryType queryType, RayQueryOutputType queryOutputType,
            GraphicsBuffer vertexBuffer, int vertexStrideInDWORD, GraphicsBuffer indexBuffer,
            TopLevelAccelStruct accelStruct, GraphicsBuffer scratch,
            GraphicsBuffer hits)
        {
            ConfigureShaderKeywords(cmd, indirectRayCount, queryType, queryOutputType);

            cmd.SetComputeIntParam(shaderTrace, SID.g_constants_ray_count, (int)rayCount);
            cmd.SetComputeIntParam(shaderTrace, SID.g_trace_vertex_stride, vertexStrideInDWORD);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_bvh, accelStruct.topLevelBvh);
            if (indirectRayCount != null)
                cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_ray_count, indirectRayCount);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_rays, rays);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_hits, hits);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_stack, scratch);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_bottom_bvhs, accelStruct.bottomLevelBvhs);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_instance_infos, accelStruct.instanceInfos);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_trace_vertex_buffer, vertexBuffer);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_trace_index_buffer, indexBuffer);

            cmd.DispatchCompute(shaderTrace, kernelTrace, (int)Common.CeilDivide(rayCount, kGroupSize), 1, 1);
        }

        static public ulong GetScratchDataSizeInDwords(uint rayCount)
        {
            return kStackSize * rayCount;
        }
        void ConfigureShaderKeywords(CommandBuffer cmd, GraphicsBuffer indirectRayCount, RayQueryType queryType, RayQueryOutputType queryOutputType)
        {
            Common.EnableKeyword(cmd, shaderTrace, "INDIRECT_TRACE", indirectRayCount != null);
            Common.EnableKeyword(cmd, shaderTrace, "FULL_HIT", queryOutputType == RayQueryOutputType.FullHitData);
            Common.EnableKeyword(cmd, shaderTrace, "ANY_HIT", queryType == RayQueryType.AnyHit);
        }
    }

}
