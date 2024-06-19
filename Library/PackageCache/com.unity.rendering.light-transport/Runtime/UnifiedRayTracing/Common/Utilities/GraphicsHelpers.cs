
namespace UnityEngine.Rendering.UnifiedRayTracing
{
    internal class GraphicsHelpers
    {
        static public void CopyBuffer(ComputeShader copyShader, CommandBuffer cmd, GraphicsBuffer src, int srcOffsetInDWords, GraphicsBuffer dst, int dstOffsetInDwords, int sizeInDwords)
        {
            cmd.SetComputeBufferParam(copyShader, 0, "_SrcBuffer", src);
            cmd.SetComputeIntParam(copyShader, "_SrcOffset", srcOffsetInDWords);
            cmd.SetComputeBufferParam(copyShader, 0, "_DstBuffer", dst);
            cmd.SetComputeIntParam(copyShader, "_DstOffset", dstOffsetInDwords);
            cmd.SetComputeIntParam(copyShader, "_Size", sizeInDwords);

            cmd.DispatchCompute(copyShader, 0, DivUp(sizeInDwords, 8*256), 1, 1);
        }

        static public void CopyBuffer(ComputeShader copyShader, GraphicsBuffer src, int srcOffsetInDWords, GraphicsBuffer dst, int dstOffsetInDwords, int sizeInDwords)
        {
            CommandBuffer cmd = new CommandBuffer();
            CopyBuffer(copyShader, cmd, src, srcOffsetInDWords, dst, dstOffsetInDwords, sizeInDwords);
            Graphics.ExecuteCommandBuffer(cmd);
        }

        static public int DivUp(int x, uint y) => (x + (int)y - 1) / (int)y;
    }
}

