#if UNITY_URP
using UnityEngine.Rendering.Universal;
using UnityEngine;

namespace TND.DLSS
{
    public class DLSSScriptableRenderFeature : ScriptableRendererFeature
    {
        [HideInInspector]
        public bool IsEnabled = false;

        private DLSS_URP m_dlssURP;

        private DLSSBufferPass dlssBufferPass;
        private DLSSRenderPass dlssRenderPass;

        private CameraData cameraData;

        public void OnSetReference(DLSS_URP _dlssURP) {
            m_dlssURP = _dlssURP;
            dlssBufferPass.OnSetReference(m_dlssURP);
            dlssRenderPass.OnSetReference(m_dlssURP);
        }

        public override void Create() {
            name = "DLSSScriptableRenderFeature";

            // Pass the settings as a parameter to the constructor of the pass.
            dlssBufferPass = new DLSSBufferPass(m_dlssURP);
            dlssRenderPass = new DLSSRenderPass(m_dlssURP);

            dlssBufferPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
        }

        public void OnDispose() {
        }

#if UNITY_2022_1_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData) {
            dlssBufferPass.Setup();
        }
#endif

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
#if UNITY_EDITOR
            if(!Application.isPlaying) {
                return;
            }
#endif
            if(!IsEnabled) {
                return;
            }
            if(m_dlssURP == null) {
                return;
            }

            cameraData = renderingData.cameraData;
            if(cameraData.cameraType != CameraType.Game) {
                return;
            }
            if(cameraData.camera.GetComponent<DLSS_URP>() == null) {
                return;
            }
            if(!cameraData.resolveFinalTarget) {
                return;
            }
            // Here you can queue up multiple passes after each other.
            renderer.EnqueuePass(dlssBufferPass);
            renderer.EnqueuePass(dlssRenderPass);
        }
    }
}
#endif