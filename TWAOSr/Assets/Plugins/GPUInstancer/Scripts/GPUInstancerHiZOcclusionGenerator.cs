using UnityEngine;
using UnityEngine.UI;

namespace GPUInstancer
{
    public class GPUInstancerHiZOcclusionGenerator : MonoBehaviour
    {
        public bool debuggerEnabled = false;
        public bool debuggerGUIOnTop = false;
        [Range(0f, 0.1f)]
        public float debuggerOverlay = 0;
        [Range(0, 16)]
        public int debuggerMipLevel;

        [Header("For info only, don't change:")]
        //[HideInInspector]
        public RenderTexture hiZDepthTexture;
        //[HideInInspector]
        public Texture unityDepthTexture;
        //[HideInInspector]
        public Vector2 hiZTextureSize;

        private Camera _mainCamera;
        private bool _isInvalid;
        private int _hiZMipLevels = 0;
        private RenderTexture[] _hiZMipLevelTextures = null;

        private bool _isDepthTex2DArray;

        // Debugger:
        private RawImage _hiZDebugDepthTextureGUIImage;
        private GameObject _hiZOcclusionDebuggerGUI;
#if UNITY_EDITOR
        private bool _debuggerGUIOnTopCached;
        private float _debuggerOverlayCached;
#endif
        private bool _loggedURPWarning;

        #region MonoBehaviour Methods

        private void Awake()
        {
            hiZTextureSize = Vector2.zero;
            GPUInstancerConstants.SetupComputeTextureUtils();
        }

        private void OnEnable()
        {
            if (!GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
                UnityEngine.Rendering.RenderPipelineManager.endCameraRendering += OnEndCameraRenderingSRP;
            else
                Camera.onPostRender += OnEndCameraRendering;
        }

        private void OnDisable()
        {
            if (!GPUInstancerConstants.gpuiSettings.IsStandardRenderPipeline())
                UnityEngine.Rendering.RenderPipelineManager.endCameraRendering -= OnEndCameraRenderingSRP;
            else
                Camera.onPostRender -= OnEndCameraRendering;

            if (hiZDepthTexture != null)
            {
                hiZDepthTexture.Release();
                Destroy(hiZDepthTexture);
                hiZDepthTexture = null;
            }

            if (_hiZMipLevelTextures != null)
            {
                for (int i = 0; i < _hiZMipLevelTextures.Length; i++)
                {
                    if (_hiZMipLevelTextures[i] != null)
                    {
                        _hiZMipLevelTextures[i].Release();
                        Destroy(_hiZMipLevelTextures[i]);
                    }
                }
                _hiZMipLevelTextures = null;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(Camera occlusionCamera = null)
        {
            _isInvalid = false;

            _mainCamera = occlusionCamera != null ? occlusionCamera : gameObject.GetComponent<Camera>();

            if (_mainCamera == null)
            {
                Debug.LogError("GPUI Hi-Z Occlusion Culling Generator failed to initialize: camera not found.");
                _isInvalid = true;
                return;
            }

            if (GPUInstancerConstants.gpuiSettings.isVREnabled)
            {
                if (_mainCamera.stereoTargetEye != StereoTargetEyeMask.Both)
                {
                    Debug.LogError("GPUI Hi-Z Occlusion works only for cameras that render to Both eyes. Disabling Occlusion Culling.");
                    _isInvalid = true;
                    return;
                }
            }

            _mainCamera.depthTextureMode |= DepthTextureMode.Depth;

            CreateHiZDepthTexture();
        }

        #endregion


        #region Private Methods

        // SRP callback signature is different in 2019.1, but we only need the camera. 
        // Using this method to direct the callback to the main method. Unity 2018 has the same signature with Camera.onPostRender, so this is not relevant.
        private void OnEndCameraRenderingSRP(UnityEngine.Rendering.ScriptableRenderContext context, Camera camera)
        {
            OnEndCameraRendering(camera);
        }

        private void OnEndCameraRendering(Camera camera)
        {
            if (_isInvalid || _mainCamera == null || camera != _mainCamera)
                return;

            if (hiZDepthTexture == null)
            {
                //Debug.LogWarning("GPUI HiZ Depth texture is null where it should not be. Recreating it.");
                if (!CreateHiZDepthTexture())
                    return;
            }

            HandleScreenSizeChange();

            if (unityDepthTexture == null)
            {
                unityDepthTexture = Shader.GetGlobalTexture("_CameraDepthTexture");

#if UNITY_2018_3_OR_NEWER
                if (unityDepthTexture != null && unityDepthTexture.dimension == UnityEngine.Rendering.TextureDimension.Tex2DArray)
                    _isDepthTex2DArray = true;
                else
                    _isDepthTex2DArray = false;
#endif
            }

            if (unityDepthTexture != null && hiZDepthTexture != null) // will be null the first time this runs in Unity 2018 SRP since we have to use beginCameraRendering there.
            {
#if GPUI_XR
                if (GPUInstancerConstants.gpuiSettings.isVREnabled)
                {
                    if (UnityEngine.XR.XRSettings.stereoRenderingMode == UnityEngine.XR.XRSettings.StereoRenderingMode.MultiPass)
                    {
                        if (_mainCamera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left || _mainCamera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Mono)
                            UpdateTextureWithComputeShader(0);
                        else if (GPUInstancerConstants.gpuiSettings.IsUseBothEyesVRCulling())
                            UpdateTextureWithComputeShader((int)hiZTextureSize.x / 2);
                        if (_mainCamera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Mono && GPUInstancerConstants.gpuiSettings.IsUseBothEyesVRCulling())
                            UpdateTextureWithComputeShader((int)hiZTextureSize.x / 2);
                    }
                    else if (_isDepthTex2DArray && UnityEngine.XR.XRSettings.stereoRenderingMode == UnityEngine.XR.XRSettings.StereoRenderingMode.SinglePassInstanced)
                    {
                        UpdateTextureWithComputeShader(0);
                        if (GPUInstancerConstants.gpuiSettings.IsUseBothEyesVRCulling())
                            UpdateTextureWithComputeShader((int)hiZTextureSize.x / 2, 1);
                    }
                    else
                        UpdateTextureWithComputeShader(0);
                }
                else
#endif
                    UpdateTextureWithComputeShader(0);
            }

#if UNITY_EDITOR
            HandleHiZDebugger();
#endif
        }

        private void UpdateTextureWithComputeShader(int offset, int textureArrayIndex = 0)
        {
            if (_isDepthTex2DArray)
                GPUInstancerUtility.CopyTextureArrayWithComputeShader(unityDepthTexture, hiZDepthTexture, offset, textureArrayIndex);
            else
                GPUInstancerUtility.CopyTextureWithComputeShader(unityDepthTexture, hiZDepthTexture, offset);

            for (int i = 0; i < _hiZMipLevels - 1; ++i)
            {
                RenderTexture tempRT = _hiZMipLevelTextures[i];

                if (i == 0)
                    GPUInstancerUtility.ReduceTextureWithComputeShader(hiZDepthTexture, tempRT, offset);
                else
                    GPUInstancerUtility.ReduceTextureWithComputeShader(_hiZMipLevelTextures[i - 1], tempRT, offset);

                GPUInstancerUtility.CopyTextureWithComputeShader(tempRT, hiZDepthTexture, offset, 0, i + 1, false);
            }
        }

        private Vector2 GetScreenSize()
        {
            Vector2 screenSize = Vector2.zero;
#if GPUI_XR
            if (GPUInstancerConstants.gpuiSettings.isVREnabled)
            {
                screenSize.x = UnityEngine.XR.XRSettings.eyeTextureWidth;
                screenSize.y = UnityEngine.XR.XRSettings.eyeTextureHeight;
                if (GPUInstancerConstants.gpuiSettings.IsUseBothEyesVRCulling())
                    screenSize.x *= 2;
            }
            else
            {
#endif
                screenSize.x = _mainCamera.pixelWidth;
                screenSize.y = _mainCamera.pixelHeight;

#if GPUI_XR
            }
#endif
#if GPUI_URP
            if (GPUInstancerConstants.gpuiSettings.isURP)
            {
                if (QualitySettings.renderPipeline != null && QualitySettings.renderPipeline is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset urpAsset)
                    screenSize *= urpAsset.renderScale;
                else
                {
                    if (!_loggedURPWarning)
                    {
                        Debug.LogError("GPU Instancer is set up for URP, however it can not find the render pipeline asset." +
                            "\nIf you are using URP, please make sure that render pipeline assets are assigned under Edit->Project Settings->Graphics and Edit->Project Settings->Quality settings." +
                            "\nIf you are not using URP, please remove the URP package from the Package Manager and then remove and reinstall GPU Instancer, so it will be set up for the correct render pipeline when installed.");
                        _loggedURPWarning = true;
                    }
                }
            }
#endif
            return screenSize;
        }

        private bool CreateHiZDepthTexture()
        {
            hiZTextureSize = GetScreenSize();

            _hiZMipLevels = 1 + Mathf.FloorToInt(Mathf.Log(Mathf.Max(hiZTextureSize.x, hiZTextureSize.y), 2f));

            if (hiZTextureSize.x <= 0 || hiZTextureSize.y <= 0 || _hiZMipLevels == 0)
            {
                if (hiZDepthTexture != null)
                {
                    hiZDepthTexture.Release();
                    Destroy(hiZDepthTexture);
                    hiZDepthTexture = null;
                }

                //Debug.LogError("Cannot create GPUI HiZ Depth Texture for occlusion culling: Screen size is too small.");
                return false;
            }

            if (hiZDepthTexture != null)
            {
                hiZDepthTexture.Release();
                Destroy(hiZDepthTexture);
            }

            int width = (int)hiZTextureSize.x;
            int height = (int)hiZTextureSize.y;

            hiZDepthTexture = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
            hiZDepthTexture.name = "GPUIHiZDepthTexture";
            hiZDepthTexture.filterMode = FilterMode.Point;
            hiZDepthTexture.useMipMap = true;
            hiZDepthTexture.autoGenerateMips = false;
            hiZDepthTexture.enableRandomWrite = true;
            hiZDepthTexture.Create();
            hiZDepthTexture.hideFlags = HideFlags.HideAndDontSave;
            hiZDepthTexture.GenerateMips();

            if (_hiZMipLevelTextures != null)
            {
                for (int i = 0; i < _hiZMipLevelTextures.Length; i++)
                {
                    if (_hiZMipLevelTextures[i] != null)
                    {
                        _hiZMipLevelTextures[i].Release();
                        Destroy(_hiZMipLevelTextures[i]);
                    }
                }
            }

            _hiZMipLevelTextures = new RenderTexture[_hiZMipLevels];

            for (int i = 0; i < _hiZMipLevels; ++i)
            {
                width = width >> 1;

                height = height >> 1;

                if (width == 0)
                    width = 1;

                if (height == 0)
                    height = 1;

                _hiZMipLevelTextures[i] = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
                _hiZMipLevelTextures[i].name = "GPUIHiZDepthTexture_Mip_" + i;
                _hiZMipLevelTextures[i].filterMode = FilterMode.Point;
                _hiZMipLevelTextures[i].useMipMap = false;
                _hiZMipLevelTextures[i].autoGenerateMips = false;
                _hiZMipLevelTextures[i].enableRandomWrite = true;
                _hiZMipLevelTextures[i].Create();
                _hiZMipLevelTextures[i].hideFlags = HideFlags.HideAndDontSave;
            }

            return true;
        }

        private void HandleScreenSizeChange()
        {
            Vector2 newScreenSize = GetScreenSize();
            if (newScreenSize != hiZTextureSize)
            {
                CreateHiZDepthTexture();
            }
        }

#if UNITY_EDITOR
        private void HandleHiZDebugger()
        {
            if (debuggerEnabled && _hiZOcclusionDebuggerGUI == null)
                CreateHiZDebuggerCanvas();

            if (!debuggerEnabled && _hiZOcclusionDebuggerGUI != null)
                DestroyImmediate(_hiZOcclusionDebuggerGUI);

            if (!debuggerEnabled)
                return;

            if (debuggerGUIOnTop != _debuggerGUIOnTopCached || debuggerOverlay != _debuggerOverlayCached)
            {
                _hiZOcclusionDebuggerGUI.GetComponent<Canvas>().sortingOrder = debuggerGUIOnTop ? 10000 : -10000;
                _hiZDebugDepthTextureGUIImage.color = new Color(1, 1, 1, 1 - debuggerOverlay);

                _debuggerOverlayCached = debuggerOverlay;
                _debuggerGUIOnTopCached = debuggerGUIOnTop;
            }

            if (_hiZOcclusionDebuggerGUI != null && hiZDepthTexture != null)
            {
                _hiZDebugDepthTextureGUIImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GPUInstancerConstants.gpuiSettings.isVREnabled
                    && GPUInstancerConstants.gpuiSettings.IsUseBothEyesVRCulling() ? hiZTextureSize.x * 0.5f : hiZTextureSize.x);
                _hiZDebugDepthTextureGUIImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, hiZTextureSize.y);
                _hiZDebugDepthTextureGUIImage.texture = debuggerMipLevel == 0 ? hiZDepthTexture : _hiZMipLevelTextures[debuggerMipLevel >= _hiZMipLevels ? _hiZMipLevels - 1 : debuggerMipLevel];
            }
        }

        private void CreateHiZDebuggerCanvas()
        {
            _hiZOcclusionDebuggerGUI = new GameObject("GPUI HiZ Occlusion Culling Debugger");
            _debuggerGUIOnTopCached = debuggerGUIOnTop;
            _debuggerOverlayCached = debuggerOverlay;

            // Add and setup the canvas
            Canvas debuggerCanvas = _hiZOcclusionDebuggerGUI.AddComponent<Canvas>();
            debuggerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            debuggerCanvas.pixelPerfect = true; // no anti-aliasing
            debuggerCanvas.sortingOrder = debuggerGUIOnTop ? 10000 : -10000;
            debuggerCanvas.targetDisplay = 0;

            // Add a raw image to display the HiZ Depth RenderTexture
            GameObject hiZDepthTextureGUI = new GameObject("HiZ Depth Texture");
            hiZDepthTextureGUI.transform.parent = _hiZOcclusionDebuggerGUI.transform;
            _hiZDebugDepthTextureGUIImage = hiZDepthTextureGUI.AddComponent<RawImage>();
            _hiZDebugDepthTextureGUIImage.color = new Color(1, 1, 1, 1 - debuggerOverlay);

            // Setup the image's RectTransform for anchors, pivot and position
            Vector2 bottomRight = new Vector2(0, 0);
            _hiZDebugDepthTextureGUIImage.rectTransform.anchorMin = bottomRight;
            _hiZDebugDepthTextureGUIImage.rectTransform.anchorMax = bottomRight;
            _hiZDebugDepthTextureGUIImage.rectTransform.pivot = bottomRight;
            _hiZDebugDepthTextureGUIImage.rectTransform.position = Vector2.zero;
        }
#endif // UNITY_EDITOR
        #endregion
    }
}