using System;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
    internal class ComputeRayTracingShader : IRayTracingShader
    {
        ComputeShader m_Shader;
        int m_KernelIndex;
        uint3 m_ThreadGroupSizes;

        internal ComputeRayTracingShader(ComputeShader shader, string dispatchFuncName)
        {
            m_Shader = shader;
            m_KernelIndex = m_Shader.FindKernel(dispatchFuncName);

            m_Shader.GetKernelThreadGroupSizes(m_KernelIndex,
                out m_ThreadGroupSizes.x, out m_ThreadGroupSizes.y, out m_ThreadGroupSizes.z);
        }

        public void SetAccelerationStructure(CommandBuffer cmd, string name, IRayTracingAccelStruct accelStruct)
        {
            var computeAccelStruct = accelStruct as ComputeRayTracingAccelStruct;
            Assert.IsNotNull(computeAccelStruct);

            computeAccelStruct.BindGeometryBuffers(cmd, name, this);

            cmd.SetComputeBufferParam(m_Shader, m_KernelIndex, name+"bvh", computeAccelStruct.topLevelBvhBuffer);
            cmd.SetComputeBufferParam(m_Shader, m_KernelIndex, name+"bottomBvhs", computeAccelStruct.bottomLevelBvhBuffer);
            cmd.SetComputeBufferParam(m_Shader, m_KernelIndex, name+"instanceInfos", computeAccelStruct.instanceInfoBuffer);
        }

        public void SetIntParam(CommandBuffer cmd, int nameID, int val)
        {
            cmd.SetComputeIntParam(m_Shader, nameID, val);
        }

        public void SetFloatParam(CommandBuffer cmd, int nameID, float val)
        {
            cmd.SetComputeFloatParam(m_Shader, nameID, val);
        }

        public void SetVectorParam(CommandBuffer cmd, int nameID, Vector4 val)
        {
            cmd.SetComputeVectorParam(m_Shader, nameID, val);
        }

        public void SetMatrixParam(CommandBuffer cmd, int nameID, Matrix4x4 val)
        {
            cmd.SetComputeMatrixParam(m_Shader, nameID, val);
        }

        public void SetTextureParam(CommandBuffer cmd, int nameID, RenderTargetIdentifier rt)
        {
            cmd.SetComputeTextureParam(m_Shader, m_KernelIndex, nameID, rt);
        }

        public void SetBufferParam(CommandBuffer cmd, int nameID, GraphicsBuffer buffer)
        {
            cmd.SetComputeBufferParam(m_Shader, m_KernelIndex, nameID, buffer);
        }
        public void SetBufferParam(CommandBuffer cmd, int nameID, ComputeBuffer buffer)
        {
            cmd.SetComputeBufferParam(m_Shader, m_KernelIndex, nameID, buffer);
        }

        private static uint DivUp(uint x, uint y) => (x + y - 1) / y;

        public void Dispatch(CommandBuffer cmd, GraphicsBuffer scratchBuffer, uint width, uint height, uint depth)
        {
            var requiredScratchSize = GetTraceScratchBufferRequiredSizeInBytes(width, height, depth);
            if (requiredScratchSize > 0 && (scratchBuffer == null || ((ulong)(scratchBuffer.count * scratchBuffer.stride) < requiredScratchSize)))
            {
                throw new System.ArgumentException("scratchBuffer size is too small");
            }

            if (requiredScratchSize > 0 && scratchBuffer.stride != 4)
            {
                throw new System.ArgumentException("scratchBuffer stride must be 4");
            }

            cmd.SetComputeBufferParam(m_Shader, m_KernelIndex, RadeonRays.SID.g_stack, scratchBuffer);

            uint workgroupsX = DivUp(width, m_ThreadGroupSizes.x);
            uint workgroupsY = DivUp(height, m_ThreadGroupSizes.y);
            uint workgroupsZ = DivUp(depth, m_ThreadGroupSizes.z);

            cmd.SetComputeIntParam(m_Shader, "g_DispatchWidth", (int)width);
            cmd.SetComputeIntParam(m_Shader, "g_DispatchHeight", (int)height);
            cmd.SetComputeIntParam(m_Shader, "g_DispatchDepth", (int)depth);
            cmd.DispatchCompute(m_Shader, m_KernelIndex, (int)workgroupsX, (int)workgroupsY, (int)workgroupsZ);
        }

        public ulong GetTraceScratchBufferRequiredSizeInBytes(uint width, uint height, uint depth)
        {
            uint rayCount = width * height * depth;
            return (RadeonRays.RadeonRaysAPI.GetTraceMemoryRequirements(rayCount) * 4);
        }
    }
}


