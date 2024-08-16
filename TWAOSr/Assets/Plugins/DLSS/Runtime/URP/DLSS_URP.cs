using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#if UNITY_STANDALONE_WIN && UNITY_64
using UnityEngine.NVIDIA;
using UnityEngine.XR;

#endif

#if DLSS_INSTALLED && UNITY_STANDALONE_WIN && UNITY_64
using NVIDIA = UnityEngine.NVIDIA;
#endif

namespace TND.DLSS
{
    [RequireComponent(typeof(Camera))]
    public class DLSS_URP : DLSS_UTILS
    {
#if TND_DLSS && UNITY_STANDALONE_WIN && UNITY_64

        public RTHandleSystem RTHandleS;
        public RTHandle m_dlssOutput;
        public RTHandle m_colorBuffer;
        public Texture m_depthBuffer;
        public Texture m_motionVectorBuffer;
        public GraphicsFormat CameraGraphicsOutput = GraphicsFormat.B10G11R11_UFloatPack32;
        private GraphicsFormat prevCameraGraphicsFormat;

        private int actualDisplayWidth, actualDisplayHeight;

        //UniversalRenderPipelineAsset
        private List<DLSSScriptableRenderFeature> dlssScriptableRenderFeature;
        private bool containsRenderFeature = false;
        private UniversalRenderPipelineAsset UniversalRenderPipelineAsset;
        private UniversalAdditionalCameraData m_cameraData;
        private bool m_usePhysicalProperties;

        public DlssViewData dlssData;
        public ViewState[] state;
        public CommandBuffer dlssCMD;

        //Camera Stacking
        public bool m_cameraStacking = false;
        public Camera m_topCamera;
        private int m_prevCameraStackCount;
        private bool m_isBaseCamera;
        private List<DLSS_URP> m_prevCameraStack = new List<DLSS_URP>();
        private DLSS_Quality m_prevStackQuality = (DLSS_Quality)(-2);



        protected override void InitializeDLSS()
        {
            base.InitializeDLSS();
            m_mainCamera.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.MotionVectors;

            actualDisplayWidth = m_mainCamera.pixelWidth;
            actualDisplayHeight = m_mainCamera.pixelHeight;
            SetupResolution();

            if (!m_dlssInitialized)
            {
                RenderPipelineManager.beginCameraRendering += PreRenderCamera;
                RenderPipelineManager.endCameraRendering += PostRenderCamera;
            }

            if (XRSettings.enabled)
            {
                state = new ViewState[2];
            }
            else
            {
                state = new ViewState[1];
            }
            for (int i = 0; i < state.Length; i++)
            {
                state[i] = new ViewState(device);
            }

            if (m_cameraData == null)
            {
                m_cameraData = m_mainCamera.GetUniversalAdditionalCameraData();
                if (m_cameraData != null)
                {
                    if (m_cameraData.renderType == CameraRenderType.Base)
                    {
                        m_isBaseCamera = true;
                        SetupCameraStacking();
                    }
                }
            }
        }

        /// <summary>
        /// Sets up the buffers, initializes the DLSS context, and sets up the command buffer
        /// Must be recalled whenever the display resolution changes
        /// </summary>
        private void SetupCommandBuffer()
        {
            if (m_dlssOutput != null)
            {
                m_dlssOutput.Release();
            }
            if (m_colorBuffer != null)
            {
                m_colorBuffer.Release();
            }

            if (dlssScriptableRenderFeature != null)
            {
                for (int i = 0; i < dlssScriptableRenderFeature.Count; i++)
                {
                    dlssScriptableRenderFeature[i].OnDispose();
                }
            }
            else
            {
                containsRenderFeature = GetRenderFeature();
            }

            float _upscaleRatio = GetUpscaleRatioFromQualityMode(DLSSQuality) + 0.0001f;

            m_renderWidth = (int)(m_displayWidth / _upscaleRatio);
            m_renderHeight = (int)(m_displayHeight / _upscaleRatio);

            m_dlssOutput = RTHandleS.Alloc(m_displayWidth, m_displayHeight, enableRandomWrite: true, colorFormat: CameraGraphicsOutput, msaaSamples: MSAASamples.None, name: "DLSS OUTPUT");

#if !UNITY_2022_1_OR_NEWER
            m_colorBuffer = RTHandleS.Alloc(m_renderWidth, m_renderHeight, enableRandomWrite: false, colorFormat: CameraGraphicsOutput, msaaSamples: MSAASamples.None, name: "DLSS INPUT");
#endif

            dlssData.inputRes = new Resolution() { width = m_renderWidth, height = m_renderHeight };
            dlssData.outputRes = new Resolution() { width = m_displayWidth, height = m_displayHeight };


            SetDynamicResolution(_upscaleRatio);

            if (!containsRenderFeature)
            {
                Debug.LogError("Current Universal Render Data is missing the 'DLSS Scriptable Render Pass' Rendering Feature");
            }
            else
            {
                for (int i = 0; i < dlssScriptableRenderFeature.Count; i++)
                {
                    dlssScriptableRenderFeature[i].OnSetReference(this);
                }
            }

            for (int i = 0; i < dlssScriptableRenderFeature.Count; i++)
            {
                dlssScriptableRenderFeature[i].IsEnabled = true;
            }
        }

        private bool GetRenderFeature()
        {
            dlssScriptableRenderFeature = new List<DLSSScriptableRenderFeature>();

            UniversalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            bool dlssScriptableRenderFeatureFound = false;
            if (UniversalRenderPipelineAsset != null)
            {
                UniversalRenderPipelineAsset.upscalingFilter = UpscalingFilterSelection.Linear;

                var type = UniversalRenderPipelineAsset.GetType();
                var propertyInfo = type.GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

                if (propertyInfo != null)
                {
                    var scriptableRenderData = (ScriptableRendererData[])propertyInfo.GetValue(UniversalRenderPipelineAsset);

                    if (scriptableRenderData != null && scriptableRenderData.Length > 0)
                    {
                        foreach (var renderData in scriptableRenderData)
                        {

                            foreach (var renderFeature in renderData.rendererFeatures)
                            {
                                DLSSScriptableRenderFeature dlssFeature = renderFeature as DLSSScriptableRenderFeature;
                                if (dlssFeature != null)
                                {
                                    dlssScriptableRenderFeature.Add(renderFeature as DLSSScriptableRenderFeature);
                                    dlssScriptableRenderFeatureFound = true;

                                    //Stop looping the current renderer, we only allow 1 instance per renderer 
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("DLSS 2: Can't find UniversalRenderPipelineAsset");
            }
            return dlssScriptableRenderFeatureFound;
        }

        private void PreRenderCamera(ScriptableRenderContext context, Camera cameras)
        {
            if (cameras != m_mainCamera)
            {
                return;
            }

            if (m_mainCamera.stereoEnabled)
            {
                actualDisplayWidth = (int)(XRSettings.eyeTextureWidth / XRSettings.eyeTextureResolutionScale);
                actualDisplayHeight = (int)(XRSettings.eyeTextureHeight / XRSettings.eyeTextureResolutionScale);
            }
            else
            {
                actualDisplayWidth = m_mainCamera.pixelWidth;
                actualDisplayHeight = m_mainCamera.pixelHeight;
            }

            //Check if display resolution has changed
            if (m_displayWidth != actualDisplayWidth || m_displayHeight != actualDisplayHeight)
            {
                SetupResolution();
            }

            if (m_previousScaleFactor != m_scaleFactor || m_previousRenderingPath != m_mainCamera.actualRenderingPath || prevCameraGraphicsFormat != CameraGraphicsOutput)
            {
                SetupFrameBuffers();
            }

            if (UniversalRenderPipelineAsset != GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset)
            {
                for (int i = 0; i < dlssScriptableRenderFeature.Count; i++)
                {
                    dlssScriptableRenderFeature[i].OnDispose();
                }
                dlssScriptableRenderFeature = null;
                OnSetQuality(DLSSQuality);
                SetupCommandBuffer();
            }
            if (DLSSQuality == DLSS_Quality.Off)
            {
                DisableDLSS();
                return;
            }
            JitterCameraMatrix();
            dlssData.sharpening = sharpening;
            dlssData.sharpness = sharpness;
            for (int i = 0; i < state.Length; i++)
            {
                UpdateDlssSettings(ref dlssData, state[i], DLSSQuality, device);
            }

            //Camera Stacking
            if (m_isBaseCamera)
            {
                if (m_cameraData != null)
                {
                    if (m_cameraStacking)
                    {
                        try
                        {
                            if (m_topCamera != m_cameraData.cameraStack[m_cameraData.cameraStack.Count - 1] || m_prevCameraStackCount != m_cameraData.cameraStack.Count || m_prevStackQuality != DLSSQuality)
                            {
                                SetupCameraStacking();
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        private void PostRenderCamera(ScriptableRenderContext context, Camera cameras)
        {
            if (cameras != m_mainCamera)
            {
                return;
            }
            m_mainCamera.usePhysicalProperties = m_usePhysicalProperties;
            if (!m_mainCamera.usePhysicalProperties)
                m_mainCamera.ResetProjectionMatrix();
        }

        /// <summary>
        /// DLSS TAA Jitter
        /// </summary>
        private void JitterCameraMatrix()
        {
            //Only jitter the top camera
            if (m_cameraStacking && m_isBaseCamera)
            {
                return;
            }
            if (dlssScriptableRenderFeature == null)
            {
                return;
            }
            else if (!dlssScriptableRenderFeature[0].IsEnabled)
            {
                return;
            }

            m_usePhysicalProperties = m_mainCamera.usePhysicalProperties;
            var m_jitterMatrix = GetJitteredProjectionMatrix(m_mainCamera.projectionMatrix, m_renderWidth, m_renderHeight, m_antiGhosting, m_mainCamera);
            var m_projectionMatrix = m_mainCamera.projectionMatrix;
            m_mainCamera.nonJitteredProjectionMatrix = m_projectionMatrix;
            m_mainCamera.projectionMatrix = m_jitterMatrix;
            m_mainCamera.useJitteredProjectionMatrixForTransparentRendering = true;

        }

        /// <summary>
        /// Handle Dynamic Scaling
        /// </summary>
        /// <param name="_value"></param>
        private void SetDynamicResolution(float _value)
        {
            if (UniversalRenderPipelineAsset != null)
            {
                UniversalRenderPipelineAsset.renderScale = (1 / _value);
            }
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
        /// Creates new buffers, sends them to the plugin, and reintilized DLSS to adjust the display size
        /// </summary>
        private void SetupResolution()
        {
            m_displayWidth = actualDisplayWidth;
            m_displayHeight = actualDisplayHeight;

            RTHandleS = new RTHandleSystem();
            RTHandleS.Initialize(m_displayWidth, m_displayHeight);

            SetupFrameBuffers();
        }

        /// <summary>
        /// Automatically Setup camera stacking
        /// </summary>
        private void SetupCameraStacking()
        {
            m_prevCameraStackCount = m_cameraData.cameraStack.Count;
            if (m_cameraData.renderType == CameraRenderType.Base)
            {
                m_isBaseCamera = true;

                m_cameraStacking = m_cameraData.cameraStack.Count > 0;
                if (m_cameraStacking)
                {
                    CleanupOverlayCameras();
                    m_prevStackQuality = DLSSQuality;

                    m_topCamera = m_cameraData.cameraStack[m_cameraData.cameraStack.Count - 1];

                    for (int i = 0; i < m_cameraData.cameraStack.Count; i++)
                    {
                        DLSS_URP stackedCamera = m_cameraData.cameraStack[i].gameObject.GetComponent<DLSS_URP>();
                        if (stackedCamera == null)
                        {
                            stackedCamera = m_cameraData.cameraStack[i].gameObject.AddComponent<DLSS_URP>();
                        }
                        m_prevCameraStack.Add(m_cameraData.cameraStack[i].gameObject.GetComponent<DLSS_URP>());

                        //stackedCamera.hideFlags = HideFlags.HideInInspector;
                        stackedCamera.m_cameraStacking = true;
                        stackedCamera.m_topCamera = m_topCamera;

                        stackedCamera.OnSetQuality(DLSSQuality);

                        stackedCamera.sharpening = sharpening;
                        stackedCamera.m_antiGhosting = m_antiGhosting;
                    }
                }
            }
        }

        private void CleanupOverlayCameras()
        {
            for (int i = 0; i < m_prevCameraStack.Count; i++)
            {
                if (!m_prevCameraStack[i].m_isBaseCamera)
                    DestroyImmediate(m_prevCameraStack[i]);
            }
            m_prevCameraStack = new List<DLSS_URP>();
        }

        protected override void DisableDLSS()
        {
            base.DisableDLSS();

            RenderPipelineManager.beginCameraRendering -= PreRenderCamera;
            RenderPipelineManager.endCameraRendering -= PostRenderCamera;

            SetDynamicResolution(1);
            if (dlssScriptableRenderFeature != null)
            {
                for (int i = 0; i < dlssScriptableRenderFeature.Count; i++)
                {
                    dlssScriptableRenderFeature[i].IsEnabled = false;
                }
            }
            CleanupOverlayCameras();
            m_previousScaleFactor = -1;
            m_prevStackQuality = (DLSS_Quality)(-2);

            m_depthBuffer = null;
            m_motionVectorBuffer = null;

            if (m_dlssOutput != null)
            {
                m_dlssOutput.Release();
                m_dlssOutput = null;
            }
            if (m_colorBuffer != null)
            {
                m_colorBuffer.Release();
                m_colorBuffer = null;
            }

            try
            {
                for (int i = 0; i < state.Length; i++)
                {
                    device.DestroyFeature(dlssCMD, state[i].DLSSContext);
                    state[i].DLSSContext = null;
                    state[i] = null;
                }
            }
            catch { }
        }
#endif
    }
}
