#if UNITY_URP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

#if UNITY_6000_0_OR_NEWER
using System;
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace TND.DLSS
{
    public class DLSSRenderPass : ScriptableRenderPass
    {
        private CommandBuffer cmd;
        private DLSS_URP m_dlssURP;
        private readonly Vector4 flipVector = new Vector4(1, -1, 0, 1);
        private int multipassId = 0;

        public DLSSRenderPass(DLSS_URP _dlssURP)
        {
            renderPassEvent = RenderPassEvent.AfterRendering + 2;
            m_dlssURP = _dlssURP;
        }

        public void OnSetReference(DLSS_URP _dlssURP)
        {
            m_dlssURP = _dlssURP;
        }

#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64
            try
            {
                //Stereo
                if (XRSettings.enabled) {
                    multipassId++;
                    if (multipassId >= 2) {
                        multipassId = 0;
                    }
                }

                m_dlssURP.dlssCMD = cmd = CommandBufferPool.Get();
                m_dlssURP.CameraGraphicsOutput = renderingData.cameraData.cameraTargetDescriptor.graphicsFormat;
                m_dlssURP.state[multipassId].CreateContext(m_dlssURP.dlssData, cmd, true);
                m_dlssURP.state[multipassId].UpdateDispatch(m_dlssURP.m_colorBuffer, m_dlssURP.m_depthBuffer, m_dlssURP.m_motionVectorBuffer, null, m_dlssURP.m_dlssOutput, cmd);

#if UNITY_2022_1_OR_NEWER
                Blitter.BlitCameraTexture(cmd, m_dlssURP.m_dlssOutput, renderingData.cameraData.renderer.cameraColorTargetHandle, flipVector, 0, false);//This is broken on URP Vulkan
#else
                Blit(cmd, m_dlssURP.m_dlssOutput, renderingData.cameraData.renderer.cameraColorTarget);  //This does work on URP Vulkan!!!!!
#endif
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            catch
            {
            }
#endif
        }
    }

    public class DLSSBufferPass : ScriptableRenderPass
    {
        private DLSS_URP m_dlssURP;

#if !UNITY_2022_1_OR_NEWER
        private CommandBuffer cmd;
#endif

        private readonly int depthTexturePropertyID = Shader.PropertyToID("_CameraDepthTexture");
        private readonly int motionTexturePropertyID = Shader.PropertyToID("_MotionVectorTexture");

        public DLSSBufferPass(DLSS_URP _dlssURP)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            ConfigureInput(ScriptableRenderPassInput.Depth);
            m_dlssURP = _dlssURP;
        }

        //2022 and up
        public void Setup()
        {
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (m_dlssURP == null)
            {
                return;
            }

            m_dlssURP.m_depthBuffer = Shader.GetGlobalTexture(depthTexturePropertyID);
            m_dlssURP.m_motionVectorBuffer = Shader.GetGlobalTexture(motionTexturePropertyID);
#endif
        }

        public void OnSetReference(DLSS_URP _dlssURP)
        {
            m_dlssURP = _dlssURP;
        }

#if UNITY_6000_0_OR_NEWER
        [Obsolete]
#endif
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64

#if UNITY_2022_1_OR_NEWER
            m_dlssURP.m_colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
#else
            cmd = CommandBufferPool.Get();

            Blit(cmd, renderingData.cameraData.renderer.cameraColorTarget, m_dlssURP.m_colorBuffer);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            m_dlssURP.m_depthBuffer = Shader.GetGlobalTexture(depthTexturePropertyID);
            m_dlssURP.m_motionVectorBuffer = Shader.GetGlobalTexture(motionTexturePropertyID);
#endif

#endif
        }
    }
}
#endif
