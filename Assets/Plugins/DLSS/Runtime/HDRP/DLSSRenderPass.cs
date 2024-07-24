using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace TND.DLSS
{
    public class DLSSRenderPass : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        [HideInInspector]
        public BoolParameter enable = new BoolParameter(false);
        public bool IsActive() => enable.value;

        public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.BeforePostProcess;

        private readonly int depthTexturePropertyID = Shader.PropertyToID("_CameraDepthTexture");
        private readonly int motionTexturePropertyID = Shader.PropertyToID("_CameraMotionVectorsTexture");

        private DLSS_HDRP m_hdrp;
        private DLSS_Quality currentQuality;

        public override void Setup()
        {
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (!IsActive())
            {
                cmd.Blit(source, destination, 0, 0);
                return;
            }

            if (camera.camera.cameraType != CameraType.Game)
            {
                cmd.Blit(source, destination, 0, 0);
                return;
            }
            if (m_hdrp == null && !camera.camera.TryGetComponent(out m_hdrp))
            {
                cmd.Blit(source, destination, 0, 0);
                return;
            }
            if (currentQuality != m_hdrp.DLSSQuality)
            {
                cmd.Blit(source, destination, 0, 0);
                currentQuality = m_hdrp.DLSSQuality;
                return;
            }

            m_hdrp.dlssCMD = cmd;
            m_hdrp.state.CreateContext(m_hdrp.dlssData, cmd, true);
            m_hdrp.state.UpdateDispatch(source, Shader.GetGlobalTexture(depthTexturePropertyID), Shader.GetGlobalTexture(motionTexturePropertyID), null, destination, cmd);
         
#endif
        }

        //class DLSSColorMaskPassData
        //{
        //    public Material colorMaskMaterial;
        //    public int destWidth;
        //    public int destHeight;
        //}

        //[Reload("Runtime/PostProcessing/Shaders/DLSSBiasColorMask.shader")]
        //public Shader DLSSBiasColorMaskPS;

        //public static readonly int _StencilMask = Shader.PropertyToID("_StencilMask");
        //public static readonly int _StencilRef = Shader.PropertyToID("_StencilRef");

        //TextureHandle DoDLSSColorMaskPass(RenderGraph renderGraph, HDCamera hdCamera, TextureHandle inputDepth)
        //{
        //    TextureHandle output = TextureHandle.nullHandle;
        //    using (var builder = renderGraph.AddRenderPass<DLSSColorMaskPassData>("DLSS Color Mask", out var passData))
        //    {
        //        output = builder.UseColorBuffer(renderGraph.CreateTexture(
        //            new TextureDesc(Vector2.one, true, true)
        //            {
        //                colorFormat = GraphicsFormat.R8G8B8A8_UNorm,
        //                clearBuffer = true,
        //                clearColor = Color.black,
        //                name = "DLSS Color Mask"
        //            }), 0);
        //        builder.UseDepthBuffer(inputDepth, DepthAccess.Read);

        //        Material m_DLSSBiasColorMaskMaterial = CoreUtils.CreateEngineMaterial(DLSSBiasColorMaskPS);

        //        passData.colorMaskMaterial = m_DLSSBiasColorMaskMaterial;

        //        passData.destWidth = hdCamera.actualWidth;
        //        passData.destHeight = hdCamera.actualHeight;

        //        builder.SetRenderFunc(
        //            (DLSSColorMaskPassData data, RenderGraphContext ctx) =>
        //            {
        //                Rect targetViewport = new Rect(0.0f, 0.0f, data.destWidth, data.destHeight);
        //                data.colorMaskMaterial.SetInt(_StencilMask, (int)(1 << 1));
        //                data.colorMaskMaterial.SetInt(_StencilRef, (int)(1 << 1));
        //                ctx.cmd.SetViewport(targetViewport);
        //                ctx.cmd.DrawProcedural(Matrix4x4.identity, data.colorMaskMaterial, 0, MeshTopology.Triangles, 3, 1, null);
        //            });
        //    }

        //    return output;
        //}


        public override void Cleanup()
        {
        }
    }
}
