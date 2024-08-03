using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

#if UNITY_STANDALONE_WIN && UNITY_64
using UnityEngine.NVIDIA;
#endif

#if DLSS_INSTALLED && UNITY_STANDALONE_WIN && UNITY_64
using NVIDIA = UnityEngine.NVIDIA;
#endif

namespace TND.DLSS
{
    /// <summary>
    /// FSR implementation for the High Definition RenderPipeline
    /// </summary>
    public class DLSS_HDRP : DLSS_UTILS
    {
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64
        private Volume m_postProcessVolume;
        private DLSSRenderPass m_postProcessPass;

        private HDCamera m_camera;
        public HDCamera hdCamera;

        public DlssViewData dlssData;
        public ViewState state;
        public CommandBuffer dlssCMD;

        private Matrix4x4 m_jitterMatrix;
        private Matrix4x4 m_projectionMatrix;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void InitializeDLSS()
        {
            base.InitializeDLSS();

            RenderPipelineManager.beginContextRendering += OnBeginContextRendering;
            RenderPipelineManager.endContextRendering += OnEndContextRendering;

            state = new ViewState(device);

            m_postProcessVolume = gameObject.AddComponent<Volume>();
            m_postProcessVolume.hideFlags = HideFlags.HideInInspector;
            m_postProcessVolume.isGlobal = true;
            m_postProcessPass = m_postProcessVolume.profile.Add<DLSSRenderPass>();
            m_postProcessPass.enable.value = true;
            m_postProcessPass.enable.Override(true);
        }

        private void OnBeginContextRendering(ScriptableRenderContext renderContext, List<Camera> cameras)
        {
            GetHDCamera();
            DynamicResolutionHandler.SetDynamicResScaler(SetDynamicResolutionScale, DynamicResScalePolicyType.ReturnsPercentage);

            if (DLSSQuality == DLSS_Quality.Off)
            {
                DisableDLSS();
                return;
            }

            if (m_previousScaleFactor != m_scaleFactor || m_displayWidth != m_mainCamera.pixelWidth || m_displayHeight != m_mainCamera.pixelHeight || m_previousRenderingPath != m_mainCamera.actualRenderingPath || dlssData.sharpening != sharpening) {
                SetupResolution();
            }

            JitterCameraMatrix();
         
            dlssData.sharpness = sharpness;
            UpdateDlssSettings(ref dlssData, state, DLSSQuality, device);
        }

        private void OnEndContextRendering(ScriptableRenderContext renderContext, List<Camera> cameras)
        {
            m_mainCamera.ResetProjectionMatrix();
        }

        /// <summary>
        /// FSR TAA Jitter
        /// </summary>
        private void JitterCameraMatrix()
        {
            m_jitterMatrix = GetJitteredProjectionMatrix(m_mainCamera.projectionMatrix, m_renderWidth, m_renderHeight, m_antiGhosting, m_mainCamera);
            m_projectionMatrix = m_mainCamera.projectionMatrix;
            m_mainCamera.nonJitteredProjectionMatrix = m_projectionMatrix;
            m_mainCamera.projectionMatrix = m_jitterMatrix;
            m_mainCamera.useJitteredProjectionMatrixForTransparentRendering = true;
        }

        /// <summary>
        /// Gets the HD Camera and sets up things related to the hd camera if the instance cahnged
        /// </summary>
        private void GetHDCamera()
        {
            hdCamera = HDCamera.GetOrCreate(GetComponent<Camera>());

            if (hdCamera != m_camera)
            {
                m_camera = hdCamera;
                m_mainCamera = hdCamera.camera;
                m_camera.tndUpscalerEnabled = true;
                
                //Make sure the camera allows Dynamic Resolution and VolumeMask includes the Layer of the camera.
                HDAdditionalCameraData m_mainCameraAdditional = m_mainCamera.GetComponent<HDAdditionalCameraData>();
                m_mainCameraAdditional.allowDynamicResolution = true;
                m_mainCameraAdditional.volumeLayerMask |= (1 << m_mainCamera.gameObject.layer);
                m_mainCameraAdditional.allowDeepLearningSuperSampling = false;
            }
        }


        /// <summary>
        /// Initializes FSR in the plugin
        /// </summary>
        private void SetupResolution()
        {
         
            m_displayWidth = m_mainCamera.pixelWidth;
            m_displayHeight = m_mainCamera.pixelHeight;

            SetupFrameBuffers();
        }

        /// <summary>
        /// Creates new buffers and sends them to the plugin
        /// </summary>
        private void SetupFrameBuffers()
        {
            m_previousScaleFactor = m_scaleFactor;

            SetupCommandBuffer();

            m_previousRenderingPath = m_mainCamera.actualRenderingPath;
        }

        /// <summary>
        /// Sets up the buffers, initializes the DLSS context, and sets up the command buffer
        /// Must be recalled whenever the display resolution changes
        /// </summary>
        private void SetupCommandBuffer()
        {
            float _upscaleRatio = GetUpscaleRatioFromQualityMode(DLSSQuality) + 0.0001f;
            m_renderWidth = (int)(m_displayWidth / _upscaleRatio);
            m_renderHeight = (int)(m_displayHeight / _upscaleRatio);

            dlssData.inputRes = new Resolution() { width = m_renderWidth, height = m_renderHeight };
            dlssData.outputRes = new Resolution() { width = m_displayWidth, height = m_displayHeight };
            dlssData.sharpening = sharpening;

            SetDynamicResolutionScale();
        }

        public float SetDynamicResolutionScale()
        {
            return 100 / m_scaleFactor;
        }

        protected override void DisableDLSS()
        {
            base.DisableDLSS();
            m_previousScaleFactor = -1;

            RenderPipelineManager.beginContextRendering -= OnBeginContextRendering;
            RenderPipelineManager.endContextRendering -= OnEndContextRendering;

            DynamicResolutionHandler.SetDynamicResScaler(() => { return 100; }, DynamicResScalePolicyType.ReturnsPercentage);

            if (m_mainCamera != null)
            {
                hdCamera.tndUpscalerEnabled = false;

                //Set main camera to null to make sure it's setup again when dlss is initialized
                m_mainCamera = null;
                m_camera = null;
            }

            if (m_postProcessVolume)
            {
                m_postProcessPass.Cleanup();
                Destroy(m_postProcessVolume);
            }

            try
            {
                device.DestroyFeature(dlssCMD, state.DLSSContext);
                state.DLSSContext = null;
                state = null;

            }
            catch { }
        }
#endif
    }
}
