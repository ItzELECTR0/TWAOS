//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
//

// uncomment to enable profiling using stopwatch and debug.log
// #define ENABLE_PROFILING

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Lightning bolt script
    /// </summary>
    public class LightningBoltScript : MonoBehaviour
    {
        #region Public variables

        /// <summary>The camera the lightning should be shown in. Defaults to the current camera, or the main camera if current camera is null. If you are using a different camera, you may want to put the lightning in it's own layer and cull that layer out of any other cameras.</summary>
        [Header("Lightning General Properties")]
        [Tooltip("The camera the lightning should be shown in. Defaults to the current camera, or the main camera if current camera is null. If you are using a different " +
            "camera, you may want to put the lightning in it's own layer and cull that layer out of any other cameras.")]
        public Camera Camera;

        /// <summary>Type of camera mode. Auto detects the camera and creates appropriate lightning. Can be overriden to do something more specific regardless of camera.</summary>
        [Tooltip("Type of camera mode. Auto detects the camera and creates appropriate lightning. Can be overriden to do something more specific regardless of camera.")]
        public CameraMode CameraMode = CameraMode.Auto;
        internal CameraMode calculatedCameraMode = CameraMode.Unknown;

        /// <summary>True if you are using world space coordinates for the lightning bolt, false if you are using coordinates relative to the parent game object.</summary>
        [Tooltip("True if you are using world space coordinates for the lightning bolt, false if you are using coordinates relative to the parent game object.")]
        public bool UseWorldSpace = true;

        /// <summary>Whether to compensate for the parent transform. Default is false. If true, rotation, scale and position are altered by the parent transform. Use this to fix scaling, rotation and other offset problems with the lightning.</summary>
        [Tooltip("Whether to compensate for the parent transform. Default is false. If true, rotation, scale and position are altered by the parent transform. " +
            "Use this to fix scaling, rotation and other offset problems with the lightning.")]
        public bool CompensateForParentTransform = false;

        /// <summary>Lightning quality setting. This allows setting limits on generations, lights and shadow casting lights based on the global quality setting.</summary>
        [Tooltip("Lightning quality setting. This allows setting limits on generations, lights and shadow casting lights based on the global quality setting.")]
        public LightningBoltQualitySetting QualitySetting = LightningBoltQualitySetting.UseScript;

        /// <summary>Whether to use multi-threaded generation of lightning. Lightning will be delayed by about 1 frame if this is turned on, but this can significantly improve performance.</summary>
        [Tooltip("Whether to use multi-threaded generation of lightning. Lightning will be delayed by about 1 frame if this is turned on, but this can significantly improve performance.")]
        public bool MultiThreaded;

        /// <summary>If non-zero, the Camera property is used to get distance of lightning from camera. Lightning generations is reduced for each distance from camera. For example, if LevelOfDetailDistance was 100 and the lightning was 200 away from camera, generations would be reduced by 2, to a minimum of 1.</summary>
        [Range(0.0f, 1000.0f)]
        [Tooltip("If non-zero, the Camera property is used to get distance of lightning from camera. Lightning generations is reduced for each distance from camera. For example, " +
            "if LevelOfDetailDistance was 100 and the lightning was 200 away from camera, generations would be reduced by 2, to a minimum of 1.")]
        public float LevelOfDetailDistance;

        /// <summary>True to use game time, false to use real time</summary>
        [Tooltip("True to use game time, false to use real time")]
        public bool UseGameTime;

#if UNITY_EDITOR

        /// <summary>
        /// True to hide gizmos, false otherwise
        /// </summary>
        public bool HideGizmos;

#endif

        /// <summary>Sort layer name</summary>
        [Header("Lightning 2D Settings")]
        [Tooltip("Sort layer name")]
        public string SortLayerName;

        /// <summary>Order in sort layer</summary>
        [Tooltip("Order in sort layer")]
        public int SortOrderInLayer;

        /// <summary>Soft particles factor. 0.01 to 3.0 are typical, 100.0 to disable.</summary>
        [Header("Lightning Rendering Properties")]
        [Tooltip("Soft particles factor. 0.01 to 3.0 are typical, 100.0 to disable.")]
        [Range(0.01f, 100.0f)]
        public float SoftParticlesFactor = 3.0f;

        /// <summary>The render queue for the lightning. -1 for default.</summary>
        [Tooltip("The render queue for the lightning. -1 for default.")]
        public int RenderQueue = -1;

        /// <summary>Lightning material for mesh renderer - glow</summary>
        [Tooltip("Lightning material for mesh renderer - glow")]
        public Material LightningMaterialMesh;

        /// <summary>Lightning material for mesh renderer - bolt</summary>
        [Tooltip("Lightning material for mesh renderer - bolt")]
        public Material LightningMaterialMeshNoGlow;

        /// <summary>The texture to use for the lightning bolts, or null for the material default texture.</summary>
        [Tooltip("The texture to use for the lightning bolts, or null for the material default texture.")]
        public Texture2D LightningTexture;

        /// <summary>The texture to use for the lightning glow, or null for the material default texture.</summary>
        [Tooltip("The texture to use for the lightning glow, or null for the material default texture.")]
        public Texture2D LightningGlowTexture;

        /// <summary>Particle system to play at the point of emission (start). 'Emission rate' particles will be emitted all at once.</summary>
        [Tooltip("Particle system to play at the point of emission (start). 'Emission rate' particles will be emitted all at once.")]
        public ParticleSystem LightningOriginParticleSystem;

        /// <summary>Particle system to play at the point of impact (end). 'Emission rate' particles will be emitted all at once.</summary>
        [Tooltip("Particle system to play at the point of impact (end). 'Emission rate' particles will be emitted all at once.")]
        public ParticleSystem LightningDestinationParticleSystem;

        /// <summary>Tint color for the lightning</summary>
        [Tooltip("Tint color for the lightning")]
        public Color LightningTintColor = Color.white;

        /// <summary>Tint color for the lightning glow</summary>
        [Tooltip("Tint color for the lightning glow")]
        public Color GlowTintColor = new Color(0.1f, 0.2f, 1.0f, 1.0f);

        /// <summary>Allow tinting the main trunk differently than forks.</summary>
        [Tooltip("Allow tintint the main trunk differently than forks.")]
        public Color MainTrunkTintColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        /// <summary>Source blend mode. Default is SrcAlpha.</summary>
        [Tooltip("Source blend mode. Default is SrcAlpha.")]
        public UnityEngine.Rendering.BlendMode SourceBlendMode = UnityEngine.Rendering.BlendMode.SrcAlpha;

        /// <summary>Destination blend mode. Default is One. For additive blend use One. For alpha blend use OneMinusSrcAlpha.</summary>
        [Tooltip("Destination blend mode. Default is One. For additive blend use One. For alpha blend use OneMinusSrcAlpha.")]
        public UnityEngine.Rendering.BlendMode DestinationBlendMode = UnityEngine.Rendering.BlendMode.One;

        /// <summary>Source blend mode glow. Default is SrcAlpha.</summary>
        [Tooltip("Source blend mode. Default is SrcAlpha.")]
        public UnityEngine.Rendering.BlendMode SourceBlendModeGlow = UnityEngine.Rendering.BlendMode.SrcAlpha;

        /// <summary>Destination blend mode glow. Default is One. For additive blend use One. For alpha blend use OneMinusSrcAlpha.</summary>
        [Tooltip("Destination blend mode. Default is One. For additive blend use One. For alpha blend use OneMinusSrcAlpha.")]
        public UnityEngine.Rendering.BlendMode DestinationBlendModeGlow = UnityEngine.Rendering.BlendMode.One;

        /// <summary>Jitter multiplier to randomize lightning size. Jitter depends on trunk width and will make the lightning move rapidly and jaggedly, giving a more lively and sometimes cartoony feel. Jitter may be shared with other bolts depending on materials. If you need different jitters for the same material, create a second script object.</summary>
        [Header("Lightning Movement Properties")]
        [Tooltip("Jitter multiplier to randomize lightning size. Jitter depends on trunk width and will make the lightning move rapidly and jaggedly, " +
            "giving a more lively and sometimes cartoony feel. Jitter may be shared with other bolts depending on materials. If you need different " +
            "jitters for the same material, create a second script object.")]
        public float JitterMultiplier = 0.0f;

        /// <summary>Built in turbulance based on the direction of each segment. Small values usually work better, like 0.2.</summary>
        [Tooltip("Built in turbulance based on the direction of each segment. Small values usually work better, like 0.2.")]
        public float Turbulence = 0.0f;

        /// <summary>Global turbulence velocity for this script</summary>
        [Tooltip("Global turbulence velocity for this script")]
        public Vector3 TurbulenceVelocity = Vector3.zero;

        /// <summary>Flicker intensity, causes lightning to pop in and out rapidly. X = intensity, Y = speed.</summary>
        [Tooltip("Flicker intensity, causes lightning to pop in and out rapidly. X = intensity, Y = speed.")]
        public Vector2 IntensityFlicker = new Vector2(0.0f, 64.0f);

        /// <summary>
        /// Fires when lightning bolt is created (trunk width, start, end)
        /// </summary>
        public Action<LightningBoltParameters, Vector3, Vector3> LightningStartedCallback { get; set; }

        /// <summary>
        /// Fires when lightning bolt is ended (trunk width, start, end)
        /// </summary>
        public Action<LightningBoltParameters, Vector3, Vector3> LightningEndedCallback { get; set; }

        /// <summary>
        /// Fires when a light is added
        /// </summary>
        public Action<Light> LightAddedCallback { get; set; }

        /// <summary>
        /// Fires when a light is removed
        /// </summary>
        public Action<Light> LightRemovedCallback { get; set; }

        /// <summary>
        /// Whether the script has active lightning bolts
        /// </summary>
        public bool HasActiveBolts
        {
            get { return (activeBolts.Count > 0); }
        }

        /// <summary>
        /// The last time value (see unity built in shader variables _Time)
        /// </summary>
        public static Vector4 TimeVectorSinceStart { get; private set; }

        /// <summary>
        /// Lightning time since script start
        /// </summary>
        public static float TimeSinceStart { get; private set; }
        
        /// <summary>
        /// Lightning delta time
        /// </summary>
        public static float DeltaTime { get; private set; }

        /// <summary>
        /// Lightning time scale - as this approaches 0 time slows down
        /// </summary>
        public static float TimeScale = 1.0f;

        private static bool needsTimeUpdate = true;

        #endregion Public variables

        #region Public methods

        /// <summary>
        /// Create a lightning bolt
        /// </summary>
        /// <param name="p">Lightning bolt creation parameters</param>
        public virtual void CreateLightningBolt(LightningBoltParameters p)
        {

#if UNITY_EDITOR

            if (Camera == null)
            {
                UnityEngine.Debug.LogError("Camera not assigned to lightning script. Either set the camera or tag your camera as main camera.");
            }

#endif

            if (p != null && Camera != null)
            {
                UpdateTexture();
                oneParameterArray[0] = p;
                LightningBoltDependencies dependencies = CreateLightningBoltDependencies(oneParameterArray);
                if (dependencies != null)
                {
                    LightningBolt bolt = GetOrCreateLightningBolt();
                    bolt.SetupLightningBolt(dependencies);
                }
            }
        }

        /// <summary>
        /// Create multiple lightning bolts, attempting to batch them into as few draw calls as possible
        /// </summary>
        /// <param name="parameters">Lightning bolt creation parameters</param>
        public void CreateLightningBolts(ICollection<LightningBoltParameters> parameters)
        {

#if UNITY_EDITOR

            if (Camera == null)
            {
                UnityEngine.Debug.LogError("Camera not assigned to lightning script. Either set the camera or tag your camera as main camera.");
            }

#endif

            if (parameters != null && parameters.Count > 0 && Camera != null)
            {
                UpdateTexture();
                LightningBoltDependencies dependencies = CreateLightningBoltDependencies(parameters);
                if (dependencies != null)
                {
                    LightningBolt bolt = GetOrCreateLightningBolt();
                    bolt.SetupLightningBolt(dependencies);
                }
            }
        }

        #endregion Public methods

        #region Protected methods

        /// <summary>
        /// AWake
        /// </summary>
        protected virtual void Awake()
        {
            UpdateShaderIds();

#if UNITY_EDITOR

            if (GetComponents<LightningBoltScript>().Length > 1)
            {
                UnityEngine.Debug.LogError("Having more than one lightning script attached to one game object is not supported.");
            }

#endif

        }

        /// <summary>
        /// Start
        /// </summary>
        protected virtual void Start()
        {
            UpdateCamera();
            UpdateMaterialsForLastTexture();
            UpdateShaderParameters();
            CheckCompensateForParentTransform();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

#if UNITY_WEBGL

            MultiThreaded = false;

#endif

            threadState = new LightningThreadState(MultiThreaded);
        }

        /// <summary>
        /// Update
        /// </summary>
        protected virtual void Update()
        {
            if (Time.timeScale <= 0.0f)
            {
                return;
            }

            if (needsTimeUpdate)
            {
                needsTimeUpdate = false;
                DeltaTime = (UseGameTime ? Time.deltaTime : Time.unscaledDeltaTime) * TimeScale;
                TimeSinceStart += DeltaTime;
                if (TimeSinceStart >= 16384.0f)
                {
                    TimeSinceStart -= 16384.0f;
                }
            }

#if DEBUG

            if (LightningMaterialMesh == null || LightningMaterialMeshNoGlow == null)
            {
                UnityEngine.Debug.LogError("Must assign all lightning materials");
            }

#endif

            if (HasActiveBolts)
            {
                UpdateCamera();
                UpdateShaderParameters();
                CheckCompensateForParentTransform();
                UpdateActiveBolts();
                Shader.SetGlobalVector(shaderId_LightningTime, TimeVectorSinceStart = new Vector4(TimeSinceStart * 0.05f, TimeSinceStart, TimeSinceStart * 2.0f, TimeSinceStart * 3.0f));
            }

            threadState.UpdateMainThreadActions();
        }

        /// <summary>
        /// LateUpdate
        /// </summary>
        protected virtual void LateUpdate()
        {
            needsTimeUpdate = true;
        }

        /// <summary>
        /// Get or create lightning bolt parameters from cache
        /// </summary>
        /// <returns>LightningBoltParameters</returns>
        protected virtual LightningBoltParameters OnCreateParameters()
        {
            return LightningBoltParameters.GetOrCreateParameters();
        }

        /// <summary>
        /// Get or create lightning bolt parameters from cache, and set quality setting
        /// </summary>
        /// <returns>LightningBoltParameters</returns>
        protected LightningBoltParameters CreateParameters()
        {
            LightningBoltParameters p = OnCreateParameters();
            p.quality = QualitySetting;
            PopulateParameters(p);

            return p;
        }

        /// <summary>
        /// Derived class can override to fill in lightning bolt parameters
        /// </summary>
        /// <param name="parameters">Parameters</param>
        protected virtual void PopulateParameters(LightningBoltParameters parameters)
        {
            parameters.MainTrunkTintColor = MainTrunkTintColor;
        }

#endregion Protected methods

#region Private variables

        internal Material lightningMaterialMeshInternal { get; private set; }
        internal Material lightningMaterialMeshNoGlowInternal { get; private set; }
        private Texture2D lastLightningTexture;
        private Texture2D lastLightningGlowTexture;
        private readonly List<LightningBolt> activeBolts = new List<LightningBolt>();
        private readonly LightningBoltParameters[] oneParameterArray = new LightningBoltParameters[1];
        private readonly List<LightningBolt> lightningBoltCache = new List<LightningBolt>();
        private readonly List<LightningBoltDependencies> dependenciesCache = new List<LightningBoltDependencies>();

        private LightningThreadState threadState;

        // shader ids
        private static int shaderId_MainTex = int.MinValue;
        private static int shaderId_TintColor;
        private static int shaderId_JitterMultiplier;
        private static int shaderId_Turbulence;
        private static int shaderId_TurbulenceVelocity;
        private static int shaderId_SrcBlendMode;
        private static int shaderId_DstBlendMode;
        private static int shaderId_InvFade;
        private static int shaderId_LightningTime;
        private static int shaderId_IntensityFlicker;
        private static int shaderId_RenderMode;

#endregion Private variables

#region Private methods

        private Coroutine StartCoroutineWrapper(IEnumerator routine)
        {
            if (isActiveAndEnabled)
            {
                return StartCoroutine(routine);
            }
            return null;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            LightningBolt.ClearCache();
        }

        private LightningBoltDependencies CreateLightningBoltDependencies(ICollection<LightningBoltParameters> parameters)
        {
            if (threadState == null)
            {
                return null;
            }

            LightningBoltDependencies d;
            if (dependenciesCache.Count == 0)
            {
                d = new LightningBoltDependencies();
                d.AddActiveBolt = AddActiveBolt;
                d.LightAdded = OnLightAdded;
                d.LightRemoved = OnLightRemoved;
                d.ReturnToCache = ReturnLightningDependenciesToCache;
                d.StartCoroutine = StartCoroutineWrapper;
                d.Parent = gameObject;
            }
            else
            {
                int i = dependenciesCache.Count - 1;
                d = dependenciesCache[i];
                dependenciesCache.RemoveAt(i);
            }

            d.CameraPos = Camera.transform.position;
            d.CameraIsOrthographic = Camera.orthographic;
            d.CameraMode = calculatedCameraMode;
            d.LevelOfDetailDistance = LevelOfDetailDistance;
            d.DestParticleSystem = LightningDestinationParticleSystem;
            d.LightningMaterialMesh = lightningMaterialMeshInternal;
            d.LightningMaterialMeshNoGlow = lightningMaterialMeshNoGlowInternal;
            d.OriginParticleSystem = LightningOriginParticleSystem;
            d.SortLayerName = SortLayerName;
            d.SortOrderInLayer = SortOrderInLayer;
            d.UseWorldSpace = UseWorldSpace;
            d.ThreadState = threadState;

            // clone parameters list if threading, otherwise just set it
            if (threadState.multiThreaded)
            {
                d.Parameters = new List<LightningBoltParameters>(parameters);
            }
            else
            {
                d.Parameters = parameters;
            }

            d.LightningBoltStarted = LightningStartedCallback;
            d.LightningBoltEnded = LightningEndedCallback;

            return d;
        }

        private void ReturnLightningDependenciesToCache(LightningBoltDependencies d)
        {
            d.Parameters = null;
            d.OriginParticleSystem = null;
            d.DestParticleSystem = null;
            d.LightningMaterialMesh = null;
            d.LightningMaterialMeshNoGlow = null;
            dependenciesCache.Add(d);
        }

        internal void OnLightAdded(Light l)
        {
            if (LightAddedCallback != null)
            {
                LightAddedCallback(l);
            }
        }

        internal void OnLightRemoved(Light l)
        {
            if (LightRemovedCallback != null)
            {
                LightRemovedCallback(l);
            }
        }

        internal void AddActiveBolt(LightningBolt bolt)
        {

#if DEBUG

            if (bolt == null || activeBolts.Contains(bolt))
            {
                UnityEngine.Debug.LogError("Attempted to add null or duplicate active lightning bolt");
                return;
            }

#endif

            // only called from the main thread
            activeBolts.Add(bolt);
        }

        private void UpdateShaderIds()
        {
            if (shaderId_MainTex != int.MinValue)
            {
                return;
            }

            shaderId_MainTex = Shader.PropertyToID("_MainTex");
            shaderId_TintColor = Shader.PropertyToID("_TintColor");
            shaderId_JitterMultiplier = Shader.PropertyToID("_JitterMultiplier");
            shaderId_Turbulence = Shader.PropertyToID("_Turbulence");
            shaderId_TurbulenceVelocity = Shader.PropertyToID("_TurbulenceVelocity");
            shaderId_SrcBlendMode = Shader.PropertyToID("_SrcBlendMode");
            shaderId_DstBlendMode = Shader.PropertyToID("_DstBlendMode");
            shaderId_InvFade = Shader.PropertyToID("_InvFade");
            shaderId_LightningTime = Shader.PropertyToID("_LightningTime");
            shaderId_IntensityFlicker = Shader.PropertyToID("_IntensityFlicker");
            shaderId_RenderMode = Shader.PropertyToID("_RenderMode");
        }

        private void UpdateMaterialsForLastTexture()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            calculatedCameraMode = CameraMode.Unknown;
            lightningMaterialMeshInternal = new Material(LightningMaterialMesh);
            lightningMaterialMeshNoGlowInternal = new Material(LightningMaterialMeshNoGlow);
            if (LightningTexture != null)
            {
                lightningMaterialMeshNoGlowInternal.SetTexture(shaderId_MainTex, LightningTexture);
            }
            if (LightningGlowTexture != null)
            {
                lightningMaterialMeshInternal.SetTexture(shaderId_MainTex, LightningGlowTexture);
            }

            SetupMaterialCamera();
        }

        private void UpdateTexture()
        {
            if (LightningTexture != null && LightningTexture != lastLightningTexture)
            {
                lastLightningTexture = LightningTexture;
                UpdateMaterialsForLastTexture();
            }
            if (LightningGlowTexture != null && LightningGlowTexture != lastLightningGlowTexture)
            {
                lastLightningGlowTexture = LightningGlowTexture;
                UpdateMaterialsForLastTexture();
            }
        }

        private void SetMaterialPerspective()
        {
            if (calculatedCameraMode != CameraMode.Perspective)
            {
                calculatedCameraMode = CameraMode.Perspective;
                lightningMaterialMeshInternal.SetInt(shaderId_RenderMode, 0);
                lightningMaterialMeshNoGlowInternal.SetInt(shaderId_RenderMode, 0);
            }
        }

        private void SetMaterialOrthographicXY()
        {
            if (calculatedCameraMode != CameraMode.OrthographicXY)
            {
                calculatedCameraMode = CameraMode.OrthographicXY;
                lightningMaterialMeshInternal.SetInt(shaderId_RenderMode, 1);
                lightningMaterialMeshNoGlowInternal.SetInt(shaderId_RenderMode, 1);
            }
        }

        private void SetMaterialOrthographicXZ()
        {
            if (calculatedCameraMode != CameraMode.OrthographicXZ)
            {
                calculatedCameraMode = CameraMode.OrthographicXZ;
                lightningMaterialMeshInternal.SetInt(shaderId_RenderMode, 2);
                lightningMaterialMeshNoGlowInternal.SetInt(shaderId_RenderMode, 2);
            }
        }

        private void SetupMaterialCamera()
        {
            if (Camera == null && CameraMode == CameraMode.Auto)
            {
                SetMaterialPerspective();
                return;
            }

            if (CameraMode == CameraMode.Auto)
            {
                if (Camera.orthographic)
                {
                    SetMaterialOrthographicXY();
                }
                else
                {
                    SetMaterialPerspective();
                }
            }
            else if (CameraMode == CameraMode.Perspective)
            {
                SetMaterialPerspective();
            }
            else if (CameraMode == CameraMode.OrthographicXY)
            {
                SetMaterialOrthographicXY();
            }
            else
            {
                SetMaterialOrthographicXZ();
            }
        }

        private void UpdateShaderParameters()
        {
            lightningMaterialMeshInternal.SetColor(shaderId_TintColor, GlowTintColor);
            lightningMaterialMeshInternal.SetFloat(shaderId_JitterMultiplier, JitterMultiplier);
            lightningMaterialMeshInternal.SetFloat(shaderId_Turbulence, Turbulence * LightningBoltParameters.Scale);
            lightningMaterialMeshInternal.SetVector(shaderId_TurbulenceVelocity, TurbulenceVelocity * LightningBoltParameters.Scale);
            lightningMaterialMeshInternal.SetInt(shaderId_SrcBlendMode, (int)SourceBlendModeGlow);
            lightningMaterialMeshInternal.SetInt(shaderId_DstBlendMode, (int)DestinationBlendModeGlow);
            lightningMaterialMeshInternal.renderQueue = RenderQueue;
            lightningMaterialMeshInternal.SetFloat(shaderId_InvFade, SoftParticlesFactor);
            lightningMaterialMeshNoGlowInternal.SetColor(shaderId_TintColor, LightningTintColor);
            lightningMaterialMeshNoGlowInternal.SetFloat(shaderId_JitterMultiplier, JitterMultiplier);
            lightningMaterialMeshNoGlowInternal.SetFloat(shaderId_Turbulence, Turbulence * LightningBoltParameters.Scale);
            lightningMaterialMeshNoGlowInternal.SetVector(shaderId_TurbulenceVelocity, TurbulenceVelocity * LightningBoltParameters.Scale);
            lightningMaterialMeshNoGlowInternal.SetInt(shaderId_SrcBlendMode, (int)SourceBlendMode);
            lightningMaterialMeshNoGlowInternal.SetInt(shaderId_DstBlendMode, (int)DestinationBlendMode);
            lightningMaterialMeshNoGlowInternal.renderQueue = RenderQueue;
            lightningMaterialMeshNoGlowInternal.SetFloat(shaderId_InvFade, SoftParticlesFactor);
            lightningMaterialMeshInternal.SetVector(shaderId_IntensityFlicker, IntensityFlicker);
            lightningMaterialMeshNoGlowInternal.SetVector(shaderId_IntensityFlicker, IntensityFlicker);
            SetupMaterialCamera();
        }

        private void CheckCompensateForParentTransform()
        {
            if (CompensateForParentTransform)
            {
                Transform p = transform.parent;
                if (p != null)
                {
                    transform.position = p.position;
                    transform.localScale = new Vector3(1.0f / p.localScale.x, 1.0f / p.localScale.y, 1.0f / p.localScale.z);
                    transform.rotation = p.rotation;
                }
            }
        }

        private void UpdateCamera()
        {
            Camera = (Camera == null ? (Camera.current == null ? Camera.main : Camera.current) : Camera);
        }

        private LightningBolt GetOrCreateLightningBolt()
        {
            if (lightningBoltCache.Count == 0)
            {

#if ENABLE_PROFILING

                Debug.Log("Lightning cache miss");

#endif

                return new LightningBolt();
            }
            LightningBolt b = lightningBoltCache[lightningBoltCache.Count - 1];
            lightningBoltCache.RemoveAt(lightningBoltCache.Count - 1);

            return b;
        }

        private void UpdateActiveBolts()
        {
            for (int i = activeBolts.Count - 1; i >= 0; i--)
            {
                LightningBolt bolt = activeBolts[i];
                if (!bolt.Update())
                {
                    // bolt is done, remove it and put back in cache
                    activeBolts.RemoveAt(i);
                    bolt.Cleanup();
                    lightningBoltCache.Add(bolt);
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (threadState != null && threadState.multiThreaded)
            {
                threadState.Running = false;
            }
        }

        private void Cleanup()
        {
            // make sure active bolts are destroyed properly and cleaned up
            foreach (LightningBolt bolt in activeBolts)
            {
                bolt.Cleanup();
            }
            activeBolts.Clear();
        }

        private void OnDestroy()
        {
            if (threadState.multiThreaded)
            {
                threadState.TerminateAndWaitForEnd(true);
            }
            if (lightningMaterialMeshInternal != null)
            {
                GameObject.Destroy(lightningMaterialMeshInternal);
            }
            if (lightningMaterialMeshNoGlowInternal != null)
            {
                GameObject.Destroy(lightningMaterialMeshNoGlowInternal);
            }
            Cleanup();
        }

        private void OnDisable()
        {
            Cleanup();
        }

#endregion Private methods
    }

    /// <summary>
    /// Fast coroutine for delays
    /// </summary>
    public class WaitForSecondsLightning : CustomYieldInstruction
    {
        private static readonly List<WaitForSecondsLightning> pool = new List<WaitForSecondsLightning>();

        private float remaining;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="time">Time in seconds to wait</param>
        public WaitForSecondsLightning(float time)
        {
            remaining = time;
        }

        public static WaitForSecondsLightning WaitForSecondsLightningPooled(float time)
        {
            if (pool.Count == 0)
            {
                return new WaitForSecondsLightning(time);
            }
            var idx = pool.Count - 1;
            WaitForSecondsLightning pooledObject = pool[idx];
            pooledObject.remaining = time;
            pooledObject.Reset();
            pool.RemoveAt(idx);
            return pooledObject;
        }

        /// <summary>
        /// Whether to keep waiting
        /// </summary>
        public override bool keepWaiting
        {
            get
            {
                if (remaining <= 0.0f)
                {
                    pool.Add(this);
                    return false;
                }
                remaining -= LightningBoltScript.DeltaTime;
                return true;
            }
        }
    }
}