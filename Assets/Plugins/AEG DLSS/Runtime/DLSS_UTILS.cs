using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Math = System.Math;

#if UNITY_STANDALONE_WIN && UNITY_64
using UnityEngine.NVIDIA;
#if DLSS_INSTALLED
using NVIDIA = UnityEngine.NVIDIA;
#endif
#endif


#if !UNITY_STANDALONE_WIN || !UNITY_64 || !DLSS_INSTALLED

#endif

namespace AEG.DLSS
{
    public enum DLSS_Quality
    {
        Off,
        DLAA,
        MaximumQuality,
        Balanced,
        MaximumPerformance,
        UltraPerformance
    }

    public class DLSS_UTILS : MonoBehaviour
    {
        public DLSS_Quality DLSSQuality = DLSS_Quality.Balanced;
        public float m_antiGhosting = 0.1f;
#if AEG_DLSS && UNITY_STANDALONE_WIN && UNITY_64

        protected DLSS_Quality m_previousDLSSQuality;

        public float DLSSScaleFactor;

        protected bool m_dlssInitialized = false;
        protected Camera m_mainCamera;

        protected float m_scaleFactor = 1.5f;
        public int m_renderWidth, m_renderHeight;
        protected int m_displayWidth, m_displayHeight;

        public float sharpening = 0.5f;

        public float mipmapBiasOverride = 0.5f;
        public bool autoTextureUpdate = true;
        public float mipMapUpdateFrequency = 2f;

        private static bool useJitter = true;

        //Mipmap variables
        protected ulong m_previousLength;
        protected float m_mipMapBias;
        protected static float m_prevMipMapBias;
        protected float m_mipMapTimer = float.MaxValue;

        protected float m_previousScaleFactor;
        protected RenderingPath m_previousRenderingPath;

        #region Public API
        /// <summary>
        /// Set DLSS Quality settings.
        /// Quality = 1.5, Balanced = 1.7, Performance = 2, Ultra Performance = 3
        /// </summary>
        public void OnSetQuality(DLSS_Quality value) {
            m_previousDLSSQuality = value;
            DLSSQuality = value;

            m_scaleFactor = GetUpscaleRatioFromQualityMode(value);

            Debug.Log("JOO");
            Initialize();
            DLSSScaleFactor = m_scaleFactor;
        }

        /// <summary>
        /// Checks wether DLSS is compatible using the current build settings 
        /// </summary>
        /// <returns></returns>
        public bool IsSupported() {
            if(device == null) {
                return false;
            }

            if(device.IsFeatureAvailable(GraphicsDeviceFeature.DLSS)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates a single texture to the set MipMap Bias.
        /// Should be called when an object is instantiated, or when the ScaleFactor is changed.
        /// </summary>
        public static void OnMipmapSingleTexture(Texture texture, float _mapmapBias) {
            texture.mipMapBias = _mapmapBias;
        }

        /// <summary>
        /// Updates all textures currently loaded to the set MipMap Bias.
        /// Should be called when a lot of new textures are loaded, or when the ScaleFactor is changed.
        /// </summary>
        public static void OnMipMapAllTextures(float _mapmapBias) {
            Texture[] m_allTextures = Resources.FindObjectsOfTypeAll(typeof(Texture)) as Texture[];
            for(int i = 0; i < m_allTextures.Length; i++) {
                m_allTextures[i].mipMapBias = _mapmapBias;
            }
        }

        /// <summary>
        /// Resets all currently loaded textures to the default mipmap bias. 
        /// </summary>
        public static void OnResetAllMipMaps() {
            m_prevMipMapBias = -1;

            Texture[] m_allTextures = Resources.FindObjectsOfTypeAll(typeof(Texture)) as Texture[];
            for(int i = 0; i < m_allTextures.Length; i++) {
                m_allTextures[i].mipMapBias = 0;
            }
            m_allTextures = null;
        }
        #endregion

        protected GraphicsDevice _device;
        public GraphicsDevice device
        {
            get
            {
                if(_device == null) {
                    SetupDevice();
                }

                return _device;
            }
        }

        protected void SetupDevice() {
            if(!NVUnityPlugin.IsLoaded())
                return;

            if(!SystemInfo.graphicsDeviceVendor.ToLowerInvariant().Contains("nvidia"))
                return;


            _device = GraphicsDevice.CreateGraphicsDevice();
        }

        /// <summary>
        /// Initializes everything in order to run DLSS
        /// </summary>
        protected virtual void Initialize() {
            bool dlssCompatible = IsSupported();

            //Reset mipmap timer so mipmap are instantly updated if automatic mip map is turned on
            m_mipMapTimer = float.MaxValue;

            if(m_dlssInitialized || !Application.isPlaying) {
                return;
            }

            if(dlssCompatible) {
                InitializeDLSS();
                m_dlssInitialized = true;
            } else {
                Debug.LogWarning($"DLSS is not supported");
                enabled = false;
            }
        }

        /// <summary>
        /// Initializes everything in order to run DLSS
        /// </summary>
        protected virtual void InitializeDLSS() {
            m_mainCamera = GetComponent<Camera>();
        }

        protected virtual void OnEnable() {
            OnSetQuality(DLSSQuality);
        }

        protected virtual void Update() {
            Debug.Log(m_previousDLSSQuality);
            if(m_previousDLSSQuality != DLSSQuality) {
                OnSetQuality(DLSSQuality);
            }

            if(!m_dlssInitialized) {
                return;
            }
        }

        protected virtual void OnDisable() {
            DisableDLSS();
        }

        protected virtual void OnDestroy() {
            DisableDLSS();
        }

        /// <summary>
        /// Disables DLSS and cleans up
        /// </summary>
        protected virtual void DisableDLSS() {
            m_dlssInitialized = false;
        }

#if DLSS_INSTALLED

        public static void GetRenderResolutionFromQualityMode(out int renderWidth, out int renderHeight, int displayWidth, int displayHeight, DLSS_Quality qualityMode) {
            float ratio = GetUpscaleRatioFromQualityMode(qualityMode);
            renderWidth = Mathf.RoundToInt(displayWidth / ratio);
            renderHeight = Mathf.RoundToInt(displayHeight / ratio);
        }

        public static float GetUpscaleRatioFromQualityMode(DLSS_Quality qualityMode) {
            switch(qualityMode) {
                case DLSS_Quality.Off:
                    return 1.0f;
                case DLSS_Quality.DLAA:
                    return 1.0f;
                case DLSS_Quality.MaximumQuality:
                    return 1.5f;
                case DLSS_Quality.Balanced:
                    return 1.7f;
                case DLSS_Quality.MaximumPerformance:
                    return 2.0f;
                case DLSS_Quality.UltraPerformance:
                    return 3.0f;
                default:
                    return 1.0f;
            }
        }

        public static void UpdateDlssSettings(ref DlssViewData dlssSettings, ViewState state, DLSS_Quality _dlssQuality, UnityEngine.NVIDIA.GraphicsDevice _device) {

            dlssSettings.jitterX = -taaJitter.x;
            dlssSettings.jitterY = -taaJitter.y;
            if(_dlssQuality == DLSS_Quality.DLAA) {
                dlssSettings.perfQuality = (NVIDIA.DLSSQuality)(-1);
            } else if(_dlssQuality == DLSS_Quality.MaximumQuality) {
                dlssSettings.perfQuality = NVIDIA.DLSSQuality.MaximumQuality;
            } else if(_dlssQuality == DLSS_Quality.Balanced) {
                dlssSettings.perfQuality = NVIDIA.DLSSQuality.Balanced;
            } else if(_dlssQuality == DLSS_Quality.MaximumPerformance) {
                dlssSettings.perfQuality = NVIDIA.DLSSQuality.MaximumPerformance;
            } else if(_dlssQuality == DLSS_Quality.UltraPerformance) {
                dlssSettings.perfQuality = NVIDIA.DLSSQuality.UltraPerformance;
            }



            int outWidth = dlssSettings.outputRes.width;
            int outHeight = dlssSettings.outputRes.height;

            Rect finalViewport = new Rect(0, 0, outWidth, outHeight);

            _device.GetOptimalSettings((uint)outWidth, (uint)outHeight, dlssSettings.perfQuality, out var optSettings);
            state.RequestUseAutomaticSettings(true, dlssSettings.perfQuality, finalViewport, in optSettings);

            dlssSettings.sharpness = optSettings.sharpness;
        }



        //------------------------------------------------------------------------------------------------------------
        //-----------------JITTER-------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------

        protected static float GetHaltonValue(int index, int radix) {
            float result = 0f;
            float fraction = 1f / radix;

            while(index > 0) {
                result += (index % radix) * fraction;

                index /= radix;
                fraction /= radix;
            }

            return result;
        }


        internal static Vector2 taaJitter;

        internal static int taaFrameIndex;
        public static Matrix4x4 GetJitteredProjectionMatrix(Matrix4x4 origProj,
            int width, int height, float antiGhosting, Camera cam) {

            if(!useJitter) {
                taaJitter = Vector4.zero;
                return origProj;
            }
#if UNITY_2021_2_OR_NEWER
            if(UnityEngine.FrameDebugger.enabled) {
                taaJitter = Vector4.zero;
                return origProj;
            }
#endif

            // The variance between 0 and the actual halton sequence values reveals noticeable
            // instability in Unity's shadow maps, so we avoid index 0.
            float jitterX = GetHaltonValue((taaFrameIndex & 1023) + 1, 2) - 0.5f;
            float jitterY = GetHaltonValue((taaFrameIndex & 1023) + 1, 3) - 0.5f;

            jitterX += UnityEngine.Random.Range(-0.1f * antiGhosting, 0.1f * antiGhosting);
            jitterY += UnityEngine.Random.Range(-0.1f * antiGhosting, 0.1f * antiGhosting);

            taaJitter = new Vector2(jitterX, jitterY);

            Matrix4x4 proj;

            if(cam.orthographic) {
                float vertical = cam.orthographicSize;
                float horizontal = vertical * cam.aspect;

                var offset = taaJitter;
                offset.x *= horizontal / (0.5f * width);
                offset.y *= vertical / (0.5f * height);

                float left = offset.x - horizontal;
                float right = offset.x + horizontal;
                float top = offset.y + vertical;
                float bottom = offset.y - vertical;

                proj = Matrix4x4.Ortho(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
            } else {
                var planes = origProj.decomposeProjection;

                float vertFov = Math.Abs(planes.top) + Math.Abs(planes.bottom);
                float horizFov = Math.Abs(planes.left) + Math.Abs(planes.right);

                var planeJitter = new Vector2(jitterX * horizFov / width,
                    jitterY * vertFov / height);

                planes.left += planeJitter.x;
                planes.right += planeJitter.x;
                planes.top += planeJitter.y;
                planes.bottom += planeJitter.y;

                // Reconstruct the far plane for the jittered matrix.
                // For extremely high far clip planes, the decomposed projection zFar evaluates to infinity.
                if(float.IsInfinity(planes.zFar))
                    planes.zFar = cam.farClipPlane;

                proj = Matrix4x4.Frustum(planes);
            }

            const int kMaxSampleCount = 8;
            if(++taaFrameIndex >= kMaxSampleCount)
                taaFrameIndex = 0;

            return proj;
        }

        //----------------------------------------------------------------------------------------------------------
        //-----------------DLSS PASS--------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------



        public struct OptimalSettingsRequest
        {
            public NVIDIA.DLSSQuality quality;
            public Rect viewport;
            public NVIDIA.OptimalDLSSSettingsData optimalSettings;
            public bool CanFit(Resolution rect) {
                return rect.width >= optimalSettings.minWidth && rect.height >= optimalSettings.minHeight
                    && rect.width <= optimalSettings.maxWidth && rect.height <= optimalSettings.maxHeight;
            }
        }
        private static bool IsOptimalSettingsValid(in NVIDIA.OptimalDLSSSettingsData optimalSettings) {
            return optimalSettings.maxHeight > optimalSettings.minHeight
                && optimalSettings.maxWidth > optimalSettings.minWidth
                && optimalSettings.maxWidth != 0
                && optimalSettings.maxHeight != 0
                && optimalSettings.minWidth != 0
                && optimalSettings.minHeight != 0;
        }


        public struct Resolution
        {
            public int width;
            public int height;

            public static bool operator ==(Resolution a, Resolution b) =>
                a.width == b.width && a.height == b.height;

            public static bool operator !=(Resolution a, Resolution b) =>
                !(a == b);
            public override bool Equals(object obj) {
                if(obj is Resolution)
                    return (Resolution)obj == this;
                return false;
            }

            public override int GetHashCode() {
                return (int)(width ^ height);
            }
        }

        //[System.Serializable]
        public struct DlssViewData
        {
            public NVIDIA.DLSSQuality perfQuality;
            public Resolution inputRes;
            public Resolution outputRes;
            public float sharpness;
            public float jitterX;
            public float jitterY;
            public bool reset;
            public bool CanFitInput(in Resolution inputRect) {
                return inputRes.width >= inputRect.width && inputRes.height > inputRect.height;
            }
        }

        public class ViewState
        {
            private NVIDIA.DLSSContext m_DlssContext = null;
            private NVIDIA.GraphicsDevice m_Device;
            private DlssViewData m_Data = new DlssViewData();
            private bool m_UsingOptimalSettings = false;
            private bool m_UseAutomaticSettings = false;
            private Resolution m_BackbufferRes;
            private OptimalSettingsRequest m_OptimalSettingsRequest = new OptimalSettingsRequest();

            public NVIDIA.DLSSContext DLSSContext
            {
                get
                {
                    return m_DlssContext;
                }
                set
                {
                    m_DlssContext = value;
                }
            }
            public bool useAutomaticSettings
            {
                get
                {
                    return m_UseAutomaticSettings;
                }
            }
            public OptimalSettingsRequest OptimalSettingsRequestData
            {
                get
                {
                    return m_OptimalSettingsRequest;
                }
            }

            public ViewState(NVIDIA.GraphicsDevice device) {
                m_Device = device;
                m_DlssContext = null;
            }

            public void RequestUseAutomaticSettings(bool useAutomaticSettings, NVIDIA.DLSSQuality quality, Rect viewport, in NVIDIA.OptimalDLSSSettingsData optimalSettings) {
                m_UseAutomaticSettings = false;// useAutomaticSettings;
                m_OptimalSettingsRequest.quality = quality;
                m_OptimalSettingsRequest.viewport = viewport;
                m_OptimalSettingsRequest.optimalSettings = optimalSettings;
            }

            public void ClearAutomaticSettings() {
                m_UseAutomaticSettings = false;
            }

            private bool ShouldUseAutomaticSettings() {
                if(!m_UseAutomaticSettings || m_DlssContext == null)
                    return false;

                return m_DlssContext.initData.quality == m_OptimalSettingsRequest.quality
                    && m_DlssContext.initData.outputRTHeight == (uint)m_OptimalSettingsRequest.viewport.height
                    && m_DlssContext.initData.outputRTWidth == (uint)m_OptimalSettingsRequest.viewport.width
                    && IsOptimalSettingsValid(m_OptimalSettingsRequest.optimalSettings);
            }

            public void CreateContext(in DlssViewData viewData, CommandBuffer cmdBuffer, bool mvJittered = false) {
                bool shouldUseOptimalSettings = ShouldUseAutomaticSettings();

                bool isNew = false;
                if(viewData.outputRes != m_Data.outputRes ||
                    (viewData.inputRes.width > m_BackbufferRes.width || viewData.inputRes.height > m_BackbufferRes.height) ||
                    (viewData.inputRes != m_BackbufferRes && !m_OptimalSettingsRequest.CanFit(viewData.inputRes)) ||
                    viewData.perfQuality != m_Data.perfQuality ||
                    m_DlssContext == null ||
                    shouldUseOptimalSettings != m_UsingOptimalSettings) {
                    isNew = true;
                    m_BackbufferRes = viewData.inputRes;

                    Cleanup(cmdBuffer);

                    var settings = new UnityEngine.NVIDIA.DLSSCommandInitializationData();
                    settings.SetFlag(NVIDIA.DLSSFeatureFlags.IsHDR, true);
                    settings.SetFlag(NVIDIA.DLSSFeatureFlags.MVLowRes, true);
                    settings.SetFlag(NVIDIA.DLSSFeatureFlags.DepthInverted, true);
                    settings.SetFlag(NVIDIA.DLSSFeatureFlags.DoSharpening, true);
                    settings.SetFlag(NVIDIA.DLSSFeatureFlags.MVJittered, mvJittered);
                    settings.inputRTWidth = (uint)m_BackbufferRes.width;
                    settings.inputRTHeight = (uint)m_BackbufferRes.height;
                    settings.outputRTWidth = (uint)viewData.outputRes.width;
                    settings.outputRTHeight = (uint)viewData.outputRes.height;
                    settings.quality = viewData.perfQuality;
                    m_UsingOptimalSettings = shouldUseOptimalSettings;
                    m_DlssContext = m_Device.CreateFeature(cmdBuffer, settings);
                }
                m_Data = viewData;
                m_Data.reset = isNew || viewData.reset;
            }

            public void UpdateDispatch(Texture source, Texture depth, Texture motionVectors, Texture biasColorMask, Texture output, CommandBuffer cmdBuffer) {
                if(m_DlssContext == null)
                    return;

                m_DlssContext.executeData.sharpness = m_Data.sharpness;
                m_DlssContext.executeData.mvScaleX = -((float)m_Data.inputRes.width);
                m_DlssContext.executeData.mvScaleY = -((float)m_Data.inputRes.height);
                m_DlssContext.executeData.subrectOffsetX = 0;
                m_DlssContext.executeData.subrectOffsetY = 0;
                m_DlssContext.executeData.subrectWidth = (uint)m_Data.inputRes.width;
                m_DlssContext.executeData.subrectHeight = (uint)m_Data.inputRes.height;
                m_DlssContext.executeData.jitterOffsetX = m_Data.jitterX;
                m_DlssContext.executeData.jitterOffsetY = m_Data.jitterY;
                m_DlssContext.executeData.preExposure = 1f;
                m_DlssContext.executeData.invertYAxis = 0u;
                m_DlssContext.executeData.invertXAxis = 0u;
                m_DlssContext.executeData.reset = m_Data.reset ? 1 : 0;

                var textureTable = new NVIDIA.DLSSTextureTable() {
                    colorInput = source,
                    colorOutput = output,
                    depth = depth,
                    motionVectors = motionVectors,
                    biasColorMask = biasColorMask
                };
                m_Device.ExecuteDLSS(cmdBuffer, m_DlssContext, textureTable);
            }

            public void Cleanup(CommandBuffer cmdBuffer) {
                if(m_DlssContext != null) {
                    m_Device.DestroyFeature(cmdBuffer, m_DlssContext);
                    m_DlssContext = null;
                }
            }
        }
#endif
#endif
    }
}
