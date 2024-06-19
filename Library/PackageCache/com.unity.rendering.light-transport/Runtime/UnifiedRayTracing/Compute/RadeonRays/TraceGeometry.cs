
namespace UnityEngine.Rendering.RadeonRays
{
    internal class TraceGeometry
    {
        const uint kGroupSize = 64u;
        const uint kStackSize = 64u;

        private ComputeShader shaderTrace;
        private int kernelTrace;

        public TraceGeometry(RadeonRaysShaders shaders)
        {
            shaderTrace = shaders.intersector;
            kernelTrace = shaderTrace.FindKernel("TraceRays");
        }

        public void Execute(
            CommandBuffer cmd,
            GraphicsBuffer rays, uint rayCount, GraphicsBuffer indirectRayCount,
            RayQueryType queryType, RayQueryOutputType queryOutputType,
            GraphicsBuffer vertexBuffer, GraphicsBuffer indexBuffer,
            GraphicsBuffer bvh, uint bvhOffset, GraphicsBuffer scratch,
            GraphicsBuffer hits)
        {
            ConfigureShaderKeywords(cmd, indirectRayCount, queryType, queryOutputType);

            cmd.SetComputeIntParam(shaderTrace, SID.g_constants_ray_count, (int)rayCount);
            cmd.SetComputeIntParam(shaderTrace, SID.g_bvh_offset, (int)bvhOffset);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_bvh, bvh);
            if (indirectRayCount != null)
                cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_ray_count, indirectRayCount);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_rays, rays);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_hits, hits);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, SID.g_stack, scratch);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, Shader.PropertyToID("g_trace_index_buffer"), indexBuffer);
            cmd.SetComputeBufferParam(shaderTrace, kernelTrace, Shader.PropertyToID("g_trace_vertex_buffer"), vertexBuffer);
            cmd.SetComputeIntParam(shaderTrace, Shader.PropertyToID("g_trace_vertex_stride"), 3);

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
