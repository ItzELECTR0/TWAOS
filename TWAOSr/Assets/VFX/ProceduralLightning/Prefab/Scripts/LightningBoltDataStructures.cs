//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
//

// uncomment to enable profiling using stopwatch and debug.log
// #define ENABLE_PROFILING

#if NETFX_CORE

#define TASK_AVAILABLE

using System.Threading.Tasks;

#endif

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Quality settings for lightning
    /// </summary>
    public enum LightningBoltQualitySetting
    {
        /// <summary>
        /// Use all settings from the script, ignoring the global quality setting
        /// </summary>
        UseScript,

        /// <summary>
        /// Use the global quality setting to determine lightning quality and maximum number of lights and shadowing
        /// </summary>
        LimitToQualitySetting
    }

    /// <summary>
    /// Camera modes
    /// </summary>
    public enum CameraMode
    {
        /// <summary>
        /// Auto detect
        /// </summary>
        Auto,

        /// <summary>
        /// Force perspective camera lightning
        /// </summary>
        Perspective,

        /// <summary>
        /// Force orthographic XY lightning
        /// </summary>
        OrthographicXY,

        /// <summary>
        /// Force orthographic XZ lightning
        /// </summary>
        OrthographicXZ,

        /// <summary>
        /// Unknown camera mode (do not use)
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Lightning custom transfrom state
    /// </summary>
    public enum LightningCustomTransformState
    {
        /// <summary>
        /// Started
        /// </summary>
        Started,

        /// <summary>
        /// Executing
        /// </summary>
        Executing,

        /// <summary>
        /// Ended
        /// </summary>
        Ended
    }

    /// <summary>
    /// Lightning custom transform info
    /// </summary>
    public class LightningCustomTransformStateInfo
    {
        /// <summary>
        /// State
        /// </summary>
        public LightningCustomTransformState State { get; set; }

        /// <summary>
        /// Parameters
        /// </summary>
        public LightningBoltParameters Parameters { get; set; }

        /// <summary>
        /// Lightning bolt start position
        /// </summary>
        public Vector3 BoltStartPosition;

        /// <summary>
        /// Lightning bolt end position
        /// </summary>
        public Vector3 BoltEndPosition;

        /// <summary>
        /// Transform
        /// </summary>
        public Transform Transform;

        /// <summary>
        /// Start position transform
        /// </summary>
        public Transform StartTransform;

        /// <summary>
        /// End position transform
        /// </summary>
        public Transform EndTransform;

        /// <summary>
        /// User defined object
        /// </summary>
        public object UserInfo;

        private static readonly List<LightningCustomTransformStateInfo> cache = new List<LightningCustomTransformStateInfo>();

        /// <summary>
        /// Get or create lightning custom transform state info from cache
        /// </summary>
        /// <returns>LightningCustomTransformStateInfo</returns>
        public static LightningCustomTransformStateInfo GetOrCreateStateInfo()
        {
            if (cache.Count == 0)
            {
                return new LightningCustomTransformStateInfo();
            }
            int idx = cache.Count - 1;
            LightningCustomTransformStateInfo result = cache[idx];
            cache.RemoveAt(idx);
            return result;
        }

        /// <summary>
        /// Put LightningCustomTransformStateInfo back into the cache
        /// </summary>
        /// <param name="info">LightningCustomTransformStateInfo to return to cache</param>
        public static void ReturnStateInfoToCache(LightningCustomTransformStateInfo info)
        {
            if (info != null)
            {
                info.Transform = info.StartTransform = info.EndTransform = null;
                info.UserInfo = null;
                cache.Add(info);
            }
        }
    }

    /// <summary>
    /// Lightning custom transform delegate
    /// </summary>
    [System.Serializable]
    public class LightningCustomTransformDelegate : UnityEngine.Events.UnityEvent<LightningCustomTransformStateInfo> { }

    /// <summary>
    /// Lightning light parameters
    /// </summary>
    [System.Serializable]
    public class LightningLightParameters
    {
        /// <summary>
        /// Light render mode
        /// </summary>
        [Tooltip("Light render mode - leave as auto unless you have special use cases")]
        [HideInInspector]
        public LightRenderMode RenderMode = LightRenderMode.Auto;

        /// <summary>
        /// Color of light
        /// </summary>
        [Tooltip("Color of the light")]
        public Color LightColor = Color.white;

        /// <summary>
        /// What percent of segments should have a light? Keep this pretty low for performance, i.e. 0.05 or lower depending on generations
        /// Set really really low to only have 1 light, i.e. 0.0000001f
        /// For example, at generations 5, the main trunk has 32 segments, 64 at generation 6, etc.
        /// If non-zero, there wil be at least one light in the middle
        /// </summary>
        [Tooltip("What percent of segments should have a light? For performance you may want to keep this small.")]
        [Range(0.0f, 1.0f)]
        public float LightPercent = 0.000001f;

        /// <summary>
        /// What percent of lights created should cast shadows?
        /// </summary>
        [Tooltip("What percent of lights created should cast shadows?")]
        [Range(0.0f, 1.0f)]
        public float LightShadowPercent;

        /// <summary>
        /// Light intensity
        /// </summary>
        [Tooltip("Light intensity")]
        [Range(0.0f, 8.0f)]
        public float LightIntensity = 0.5f;

        /// <summary>
        /// Light multiplier. Can set to a high number (millions) if HDRP (lumens) support is needed.
        /// </summary>
        [Tooltip("Light multiplier. Can set to a high number (millions) if HDRP (lumens) support is needed.")]
        [Range(0.0f, 10000000.0f)]
        public float LightMultiplier = 1.0f;

        /// <summary>
        /// Bounce intensity
        /// </summary>
        [Tooltip("Bounce intensity")]
        [Range(0.0f, 8.0f)]
        public float BounceIntensity;

        /// <summary>
        /// Shadow strength, 0 - 1. 0 means all light, 1 means all shadow
        /// </summary>
        [Tooltip("Shadow strength, 0 means all light, 1 means all shadow")]
        [Range(0.0f, 1.0f)]
        public float ShadowStrength = 1.0f;

        /// <summary>
        /// Shadow bias
        /// </summary>
        [Tooltip("Shadow bias, 0 - 2")]
        [Range(0.0f, 2.0f)]
        public float ShadowBias = 0.05f;

        /// <summary>
        /// Shadow normal bias
        /// </summary>
        [Tooltip("Shadow normal bias, 0 - 3")]
        [Range(0.0f, 3.0f)]
        public float ShadowNormalBias = 0.4f;

        /// <summary>
        /// Light range
        /// </summary>
        [Tooltip("The range of each light created")]
        public float LightRange;

        /// <summary>
        /// Only light up objects that match this layer mask
        /// </summary>
        [Tooltip("Only light objects that match this layer mask")]
        public LayerMask CullingMask = ~0;

        /// <summary>Offset from camera position when in orthographic mode</summary>
        [Tooltip("Offset from camera position when in orthographic mode")]
        [Range(-1000.0f, 1000.0f)]
        public float OrthographicOffset = 0.0f;

        /// <summary>Increase the duration of light fade in compared to the lightning fade.</summary>
        [Tooltip("Increase the duration of light fade in compared to the lightning fade.")]
        [Range(0.0f, 20.0f)]
        public float FadeInMultiplier = 1.0f;

        /// <summary>Increase the duration of light fully lit compared to the lightning fade.</summary>
        [Tooltip("Increase the duration of light fully lit compared to the lightning fade.")]
        [Range(0.0f, 20.0f)]
        public float FadeFullyLitMultiplier = 1.0f;

        /// <summary>Increase the duration of light fade out compared to the lightning fade.</summary>
        [Tooltip("Increase the duration of light fade out compared to the lightning fade.")]
        [Range(0.0f, 20.0f)]
        public float FadeOutMultiplier = 1.0f;

        /// <summary>
        /// Should light be shown for these parameters?
        /// </summary>
        public bool HasLight
        {
            get { return (LightColor.a > 0.0f && LightIntensity >= 0.01f && LightPercent >= 0.0000001f && LightRange > 0.01f); }
        }
    }

    /// <summary>
    /// Parameters that control lightning bolt behavior
    /// </summary>
    [System.Serializable]
    public sealed class LightningBoltParameters
    {
        #region Internal use only

        // INTERNAL USE ONLY!!!
        private static int randomSeed = Environment.TickCount;
        private static readonly List<LightningBoltParameters> cache = new List<LightningBoltParameters>();
        internal int generationWhereForksStop;
        internal int forkednessCalculated;
        internal LightningBoltQualitySetting quality;
        internal float delaySeconds;
        internal int maxLights;
        // END INTERNAL USE ONLY

        #endregion Internal use only

        /// <summary>
        /// Scale all scalar parameters by this value (i.e. trunk width, turbulence, turbulence velocity)
        /// </summary>
        public static float Scale = 1.0f;

        /// <summary>
        /// Contains quality settings for different quality levels. By default, this assumes 6 quality levels, so if you have your own
        /// custom quality setting levels, you may want to clear this dictionary out and re-populate it with your own limits
        /// </summary>
        public static readonly Dictionary<int, LightningQualityMaximum> QualityMaximums = new Dictionary<int, LightningQualityMaximum>();

        static LightningBoltParameters()
        {
            string[] names = QualitySettings.names;
            for (int i = 0; i < names.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 3, MaximumLightPercent = 0, MaximumShadowPercent = 0.0f };
                        break;
                    case 1:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 4, MaximumLightPercent = 0, MaximumShadowPercent = 0.0f };
                        break;
                    case 2:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 5, MaximumLightPercent = 0.1f, MaximumShadowPercent = 0.0f };
                        break;
                    case 3:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 5, MaximumLightPercent = 0.1f, MaximumShadowPercent = 0.0f };
                        break;
                    case 4:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 6, MaximumLightPercent = 0.05f, MaximumShadowPercent = 0.1f };
                        break;
                    case 5:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 7, MaximumLightPercent = 0.025f, MaximumShadowPercent = 0.05f };
                        break;
                    default:
                        QualityMaximums[i] = new LightningQualityMaximum { MaximumGenerations = 8, MaximumLightPercent = 0.025f, MaximumShadowPercent = 0.05f };
                        break;
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public LightningBoltParameters()
        {
            unchecked
            {
                random = currentRandom = new System.Random(randomSeed++);
            }
            Points = new List<Vector3>();
        }

        /// <summary>
        /// Generator to create the lightning bolt from the parameters
        /// </summary>
        public LightningGenerator Generator;

        /// <summary>
        /// Start of the bolt
        /// </summary>
        public Vector3 Start;

        /// <summary>
        /// End of the bolt
        /// </summary>
        public Vector3 End;

        /// <summary>
        /// X, Y and Z radius variance from Start
        /// </summary>
        public Vector3 StartVariance;

        /// <summary>
        /// X, Y and Z radius variance from End
        /// </summary>
        public Vector3 EndVariance;

        /// <summary>
        /// Custom transform action, null if none
        /// </summary>
        public System.Action<LightningCustomTransformStateInfo> CustomTransform;

        private int generations;
        /// <summary>
        /// Number of generations (0 for just a point light, otherwise 1 - 8). Higher generations have lightning with finer detail but more expensive to create.
        /// </summary>
        public int Generations
        {
            get { return generations; }
            set
            {
                int v = Mathf.Clamp(value, 1, 8);

                if (quality == LightningBoltQualitySetting.UseScript)
                {
                    generations = v;
                }
                else
                {
                    LightningQualityMaximum maximum;
                    int level = QualitySettings.GetQualityLevel();
                    if (QualityMaximums.TryGetValue(level, out maximum))
                    {
                        generations = Mathf.Min(maximum.MaximumGenerations, v);
                    }
                    else
                    {
                        generations = v;
                        Debug.LogError("Unable to read lightning quality settings from level " + level.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// How long the bolt should live in seconds
        /// </summary>
        public float LifeTime;

        /// <summary>
        /// Minimum delay
        /// </summary>
        public float Delay;

        /// <summary>
        /// How long to wait in seconds before starting additional lightning bolts
        /// </summary>
        public RangeOfFloats DelayRange;

        /// <summary>
        /// How chaotic is the main trunk of lightning? (0 - 1). Higher numbers create more chaotic lightning.
        /// </summary>
        public float ChaosFactor;

        /// <summary>
        /// How chaotic are the forks of the lightning? (0 - 1). Higher numbers create more chaotic lightning.
        /// </summary>
        public float ChaosFactorForks = -1.0f;

        /// <summary>
        /// The width of the trunk
        /// </summary>
        public float TrunkWidth;

        /// <summary>
        /// The ending width of a segment of lightning
        /// </summary>
        public float EndWidthMultiplier = 0.5f;

        /// <summary>
        /// Intensity of the lightning
        /// </summary>
        public float Intensity = 1.0f;

        /// <summary>
        /// Intensity of the glow
        /// </summary>
        public float GlowIntensity;

        /// <summary>
        /// Glow width multiplier
        /// </summary>
        public float GlowWidthMultiplier;

        /// <summary>
        /// How forked the lightning should be, 0 for none, 1 for LOTS of forks
        /// </summary>
        public float Forkedness;

        /// <summary>
        /// This is subtracted from the initial generations value, and any generation below that cannot have a fork
        /// </summary>
        public int GenerationWhereForksStopSubtractor = 5;

        /// <summary>
        /// Tint color for the lightning, this is applied to both the lightning and the glow. Unlike the script properties for coloring which
        /// are applied per material, this is applied at the mesh level and as such different bolts on the same script can use different color values.
        /// </summary>
        public Color32 Color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

        /// <summary>
        /// Tint color for main trunk of lightning
        /// </summary>
        public Color32 MainTrunkTintColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

        /// <summary>
        /// Used to generate random numbers. Not thread safe.
        /// </summary>
        public System.Random Random
        {
            get { return currentRandom; }
            set
            {
                random = value ?? random;
                currentRandom = (randomOverride ?? random);
            }
        }
        private System.Random random;
        private System.Random currentRandom;

        /// <summary>
        /// Override Random to a different Random. This gets set back to null when the parameters go back to the cache. Great for a one time bolt that looks a certain way.
        /// </summary>
        public System.Random RandomOverride
        {
            get { return randomOverride; }
            set
            {
                randomOverride = value;
                currentRandom = (randomOverride ?? random);
            }
        }
        private System.Random randomOverride;

        /// <summary>
        /// The percent of time the lightning should fade in and out (0 - 1). Example: 0.2 would fade in for 20% of the lifetime and fade out for 20% of the lifetime. Set to 0 for no fade.
        /// </summary>
        public float FadePercent = 0.15f;

        /// <summary>
        /// Modify the fade in time for FadePercent (0 - 1)
        /// </summary>
        public float FadeInMultiplier = 1.0f;

        /// <summary>
        /// Modify the fully lit time for FadePercent (0 - 1)
        /// </summary>
        public float FadeFullyLitMultiplier = 1.0f;

        /// <summary>
        /// Modify the fade out time for FadePercent (0 - 1)
        /// </summary>
        public float FadeOutMultiplier = 1.0f;

        private float growthMultiplier;
        /// <summary>
        /// A value between 0 and 0.999 that determines how fast the lightning should grow over the lifetime. A value of 1 grows slowest, 0 grows instantly
        /// </summary>
        public float GrowthMultiplier
        {
            get { return growthMultiplier; }
            set { growthMultiplier = Mathf.Clamp(value, 0.0f, 0.999f); }
        }

        /// <summary>
        /// Minimum distance multiplier for forks
        /// </summary>
        public float ForkLengthMultiplier = 0.6f;

        /// <summary>
        /// Variance of the fork distance (random range of 0 to n is added to ForkLengthMultiplier)
        /// </summary>
        public float ForkLengthVariance = 0.2f;

        /// <summary>
        /// Forks will have their end widths multiplied by this value
        /// </summary>
        public float ForkEndWidthMultiplier = 1.0f;

        /// <summary>
        /// Light parameters, null for none
        /// </summary>
        public LightningLightParameters LightParameters;

        /// <summary>
        /// Points for the trunk to follow - not all generators support this
        /// </summary>
        public List<Vector3> Points { get; set; }

        /// <summary>
        /// The amount of smoothing applied. For example, if there were 4 original points and smoothing / spline created 32 points, this value would be 8 - not all generators support this
        /// </summary>
        public int SmoothingFactor;

        /// <summary>
        /// Get a multiplier for fork distance
        /// </summary>
        /// <returns>Fork multiplier</returns>
        public float ForkMultiplier()
        {
            return ((float)Random.NextDouble() * ForkLengthVariance) + ForkLengthMultiplier;
        }

        /// <summary>
        /// Apply variance to a vector
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="variance">Variance</param>
        /// <returns>New position</returns>
        public Vector3 ApplyVariance(Vector3 pos, Vector3 variance)
        {
            return new Vector3
            (
                pos.x + (((float)Random.NextDouble() * 2.0f) - 1.0f) * variance.x,
                pos.y + (((float)Random.NextDouble() * 2.0f) - 1.0f) * variance.y,
                pos.z + (((float)Random.NextDouble() * 2.0f) - 1.0f) * variance.z
            );
        }

        /// <summary>
        /// Reset parameters
        /// </summary>
        public void Reset()
        {
            Start = End = Vector3.zero;
            Generator = null;
            SmoothingFactor = 0;
            RandomOverride = null;
            CustomTransform = null;
            if (Points != null)
            {
                Points.Clear();
            }
        }

        /// <summary>
        /// Get or create lightning bolt parameters. If cache has parameters, one is taken, otherwise a new object is created. NOT thread safe.
        /// </summary>
        /// <returns>Lightning bolt parameters</returns>
        public static LightningBoltParameters GetOrCreateParameters()
        {
            LightningBoltParameters p;
            if (cache.Count == 0)
            {
                unchecked
                {
                    p = new LightningBoltParameters();
                }
            }
            else
            {
                int i = cache.Count - 1;
                p = cache[i];
                cache.RemoveAt(i);
            }
            return p;
        }

        /// <summary>
        /// Return parameters to cache. NOT thread safe.
        /// </summary>
        /// <param name="p">Parameters</param>
        public static void ReturnParametersToCache(LightningBoltParameters p)
        {
            if (!cache.Contains(p))
            {
                // reset variables that are state-machine dependant
                p.Reset();
                cache.Add(p);
            }
        }
    }

    /// <summary>
    /// A group of lightning bolt segments, such as the main trunk of the lightning bolt
    /// </summary>
    public class LightningBoltSegmentGroup
    {
        /// <summary>
        /// Width
        /// </summary>
        public float LineWidth;

        /// <summary>
        /// Start index of the segment to render (for performance, some segments are not rendered and only used for calculations)
        /// </summary>
        public int StartIndex;

        /// <summary>
        /// Generation
        /// </summary>
        public int Generation;

        /// <summary>
        /// Delay before rendering should start
        /// </summary>
        public float Delay;

        /// <summary>
        /// Peak start, the segments should be fully visible at this point
        /// </summary>
        public float PeakStart;

        /// <summary>
        /// Peak end, the segments should start to go away after this point
        /// </summary>
        public float PeakEnd;

        /// <summary>
        /// Total life time the group will be alive in seconds
        /// </summary>
        public float LifeTime;

        /// <summary>
        /// The width can be scaled down to the last segment by this amount if desired
        /// </summary>
        public float EndWidthMultiplier;

        /// <summary>
        /// Color for the group
        /// </summary>
        public Color32 Color;

        /// <summary>
        /// Total number of active segments
        /// </summary>
        public int SegmentCount { get { return Segments.Count - StartIndex; } }

        /// <summary>
        /// Segments
        /// </summary>
        public readonly List<LightningBoltSegment> Segments = new List<LightningBoltSegment>();

        /// <summary>
        /// Lights
        /// </summary>
        public readonly List<Light> Lights = new List<Light>();

        /// <summary>
        /// Light parameters
        /// </summary>
        public LightningLightParameters LightParameters;

        /// <summary>
        /// Return the group to its cache if there is one
        /// </summary>
        public void Reset()
        {
            LightParameters = null;
            Segments.Clear();
            Lights.Clear();
            StartIndex = 0;
        }
    }

    /// <summary>
    /// A single segment of a lightning bolt
    /// </summary>
    public struct LightningBoltSegment
    {
        /// <summary>
        /// Segment start
        /// </summary>
        public Vector3 Start;

        /// <summary>
        /// Segment end
        /// </summary>
        public Vector3 End;

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return Start.ToString() + ", " + End.ToString();
        }
    }

    /// <summary>
    /// Contains maximum values for a given quality settings
    /// </summary>
    public class LightningQualityMaximum
    {
        /// <summary>
        /// Maximum generations
        /// </summary>
        public int MaximumGenerations { get; set; }

        /// <summary>
        /// Maximum light percent
        /// </summary>
        public float MaximumLightPercent { get; set; }

        /// <summary>
        /// Maximum light shadow percent
        /// </summary>
        public float MaximumShadowPercent { get; set; }
    }

    /// <summary>
    /// Lightning bolt dependencies
    /// </summary>
    public class LightningBoltDependencies
    {
        /// <summary>
        /// Parent - do not access from threads
        /// </summary>
        public GameObject Parent;

        /// <summary>
        /// Material for glow - do not access from threads
        /// </summary>
        public Material LightningMaterialMesh;

        /// <summary>
        /// Material for bolt - do not access from threads
        /// </summary>
        public Material LightningMaterialMeshNoGlow;

        /// <summary>
        /// Origin particle system - do not access from threads
        /// </summary>
        public ParticleSystem OriginParticleSystem;

        /// <summary>
        /// Dest particle system - do not access from threads
        /// </summary>
        public ParticleSystem DestParticleSystem;

        /// <summary>
        /// Camera position
        /// </summary>
        public Vector3 CameraPos;

        /// <summary>
        /// Is camera 2D?
        /// </summary>
        public bool CameraIsOrthographic;

        /// <summary>
        /// Camera mode
        /// </summary>
        public CameraMode CameraMode;

        /// <summary>
        /// Use world space
        /// </summary>
        public bool UseWorldSpace;

        /// <summary>
        /// Level of detail distance
        /// </summary>
        public float LevelOfDetailDistance;

        /// <summary>
        /// Sort layer name
        /// </summary>
        public string SortLayerName;

        /// <summary>
        /// Order in layer
        /// </summary>
        public int SortOrderInLayer;

        /// <summary>
        /// Parameters
        /// </summary>
        public ICollection<LightningBoltParameters> Parameters;

        /// <summary>
        /// Thread state
        /// </summary>
        public LightningThreadState ThreadState;

        /// <summary>
        /// Method to start co-routines
        /// </summary>
        public Func<IEnumerator, Coroutine> StartCoroutine;

        /// <summary>
        /// Call this when a light is added
        /// </summary>
        public Action<Light> LightAdded;

        /// <summary>
        /// Call this when a light is removed
        /// </summary>
        public Action<Light> LightRemoved;

        /// <summary>
        /// Call this when the bolt becomes active
        /// </summary>
        public Action<LightningBolt> AddActiveBolt;

        /// <summary>
        /// Returns the dependencies to their cache
        /// </summary>
        public Action<LightningBoltDependencies> ReturnToCache;

        /// <summary>
        /// Runs when a lightning bolt is started (parameters, start, end)
        /// </summary>
        public Action<LightningBoltParameters, Vector3, Vector3> LightningBoltStarted;

        /// <summary>
        /// Runs when a lightning bolt is ended (parameters, start, end)
        /// </summary>
        public Action<LightningBoltParameters, Vector3, Vector3> LightningBoltEnded;
    }

    /// <summary>
    /// Lightning bolt
    /// </summary>
    public class LightningBolt
    {
        #region LineRendererMesh

        /// <summary>
        /// Class the encapsulates a game object, and renderer for lightning bolt meshes
        /// </summary>
        public class LineRendererMesh
        {
            #region Public variables

            /// <summary>
            /// Game object
            /// </summary>
            public GameObject GameObject { get; private set; }

            /// <summary>
            /// Material for glow
            /// </summary>
            public Material MaterialGlow
            {
                get { return meshRendererGlow.sharedMaterial; }
                set { meshRendererGlow.sharedMaterial = value; }
            }

            /// <summary>
            /// Material for bolt
            /// </summary>
            public Material MaterialBolt
            {
                get { return meshRendererBolt.sharedMaterial; }
                set { meshRendererBolt.sharedMaterial = value; }
            }

            /// <summary>
            /// Mesh renderer for glow
            /// </summary>
            public MeshRenderer MeshRendererGlow
            {
                get { return meshRendererGlow; }
            }

            /// <summary>
            /// Mesh renderer for bolt
            /// </summary>
            public MeshRenderer MeshRendererBolt
            {
                get { return meshRendererBolt; }
            }

            /// <summary>
            /// User defined int
            /// </summary>
            public int Tag { get; set; }

            #endregion Public variables

            #region Public properties

            /// <summary>
            /// Custom transform, null if none
            /// </summary>
            public System.Action<LightningCustomTransformStateInfo> CustomTransform { get; set; }

            /// <summary>
            /// The transform component
            /// </summary>
            public Transform Transform { get; private set; }

            /// <summary>
            /// Is the line renderer empty?
            /// </summary>
            public bool Empty { get { return vertices.Count == 0; } }

            #endregion Public properties

            #region Public methods

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dependencies">Dependencies</param>
            public LineRendererMesh(LightningBoltDependencies dependencies)
            {
                dependencies.ThreadState.AddActionForMainThread(b =>
                {
                    GameObject = new GameObject("LightningBoltMeshRenderer");
                    GameObject.SetActive(false); // call Begin to activate
                    mesh = new Mesh { name = "ProceduralLightningMesh" };
                    mesh.MarkDynamic();
                    GameObject glowObject = new GameObject("LightningBoltMeshRendererGlow");
                    glowObject.transform.parent = GameObject.transform;
                    GameObject boltObject = new GameObject("LightningBoltMeshRendererBolt");
                    boltObject.transform.parent = GameObject.transform;
                    meshFilterGlow = glowObject.AddComponent<MeshFilter>();
                    meshFilterBolt = boltObject.AddComponent<MeshFilter>();
                    meshFilterGlow.sharedMesh = meshFilterBolt.sharedMesh = mesh;
                    meshRendererGlow = glowObject.AddComponent<MeshRenderer>();
                    meshRendererBolt = boltObject.AddComponent<MeshRenderer>();

#if UNITY_EDITOR

                    GameObject.hideFlags = glowObject.hideFlags = boltObject.hideFlags = HideFlags.HideAndDontSave;

#endif

                    meshRendererGlow.shadowCastingMode = meshRendererBolt.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    meshRendererGlow.reflectionProbeUsage = meshRendererBolt.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                    meshRendererGlow.lightProbeUsage = meshRendererBolt.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                    meshRendererGlow.receiveShadows = meshRendererBolt.receiveShadows = false;
                    Transform = GameObject.GetComponent<Transform>();
                }, true);
            }

            /// <summary>
            /// Apply changes to underlying mesh
            /// </summary>
            public void PopulateMesh()
            {

#if ENABLE_PROFILING

                System.Diagnostics.Stopwatch w = System.Diagnostics.Stopwatch.StartNew();

#endif

                if (vertices.Count == 0)
                {
                    mesh.Clear();
                }
                else
                {
                    PopulateMeshInternal();
                }

#if ENABLE_PROFILING

                Debug.LogFormat("MESH: {0}", w.Elapsed.TotalMilliseconds);

#endif

            }

            /// <summary>
            /// Prepare for lines to be added
            /// </summary>
            /// <param name="lineCount">Lines to add</param>
            /// <returns>True if room for lines, false if full</returns>
            public bool PrepareForLines(int lineCount)
            {
                int vertexCount = lineCount * 4;
                if (vertices.Count + vertexCount > 64999)
                {
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Begin a line
            /// </summary>
            /// <param name="start">Start position</param>
            /// <param name="end">End position</param>
            /// <param name="radius">Radius</param>
            /// <param name="color">Color</param>
            /// <param name="colorIntensity">Color intensity</param>
            /// <param name="fadeLifeTime">Fade lifetime</param>
            /// <param name="glowWidthModifier">Glow width modifier</param>
            /// <param name="glowIntensity">Glow intensity</param>
            public void BeginLine(Vector3 start, Vector3 end, float radius, Color32 color, float colorIntensity, Vector4 fadeLifeTime, float glowWidthModifier, float glowIntensity)
            {
                Vector4 dir = (end - start);
                dir.w = radius;
                AppendLineInternal(ref start, ref end, ref dir, ref dir, ref dir, color, colorIntensity, ref fadeLifeTime, glowWidthModifier, glowIntensity);
            }

            /// <summary>
            /// Append a line, call multiple times after the one BeginLine call
            /// </summary>
            /// <param name="start">Start position</param>
            /// <param name="end">End position</param>
            /// <param name="radius">Radius</param>
            /// <param name="color">Color</param>
            /// <param name="colorIntensity">Color intensity</param>
            /// <param name="fadeLifeTime">Fade lifetime</param>
            /// <param name="glowWidthModifier">Glow width modifier</param>
            /// <param name="glowIntensity">Glow intensity</param>
            public void AppendLine(Vector3 start, Vector3 end, float radius, Color32 color, float colorIntensity, Vector4 fadeLifeTime, float glowWidthModifier, float glowIntensity)
            {
                Vector4 dir = (end - start);
                dir.w = radius;
                Vector4 dirPrev1 = lineDirs[lineDirs.Count - 3];
                Vector4 dirPrev2 = lineDirs[lineDirs.Count - 1];
                AppendLineInternal(ref start, ref end, ref dir, ref dirPrev1, ref dirPrev2, color, colorIntensity, ref fadeLifeTime, glowWidthModifier, glowIntensity);
            }

            /// <summary>
            /// Reset all state
            /// </summary>
            public void Reset()
            {
                CustomTransform = null;
                Tag++;
                GameObject.SetActive(false);
                mesh.Clear();
                indices.Clear();
                vertices.Clear();
                colors.Clear();
                lineDirs.Clear();
                ends.Clear();

#if UNITY_PRE_5_3

				texCoords.Clear();
				glowModifiers.Clear();
				fadeXY.Clear();
				fadeZW.Clear();

#else

                texCoordsAndGlowModifiers.Clear();
                fadeLifetimes.Clear();

#endif

                currentBoundsMaxX = currentBoundsMaxY = currentBoundsMaxZ = int.MinValue + boundsPadder;
                currentBoundsMinX = currentBoundsMinY = currentBoundsMinZ = int.MaxValue - boundsPadder;
            }

            #endregion Public methods

            #region Private variables

            private const int defaultListCapacity = 2048;

            private static readonly Vector2 uv1 = new Vector2(0.0f, 0.0f);
            private static readonly Vector2 uv2 = new Vector2(1.0f, 0.0f);
            private static readonly Vector2 uv3 = new Vector2(0.0f, 1.0f);
            private static readonly Vector2 uv4 = new Vector2(1.0f, 1.0f);

            private readonly List<int> indices = new List<int>(defaultListCapacity);
            private readonly List<Vector3> vertices = new List<Vector3>(defaultListCapacity);
            private readonly List<Vector4> lineDirs = new List<Vector4>(defaultListCapacity);
            private readonly List<Color32> colors = new List<Color32>(defaultListCapacity);
            private readonly List<Vector3> ends = new List<Vector3>(defaultListCapacity);

#if UNITY_PRE_5_3

            private readonly List<Vector2> texCoords = new List<Vector2>(defaultListCapacity);
			private readonly List<Vector2> glowModifiers = new List<Vector2>(defaultListCapacity);
			private readonly List<Vector2> fadeXY = new List<Vector2>(defaultListCapacity);
			private readonly List<Vector2> fadeZW = new List<Vector2>(defaultListCapacity);

#else

            private readonly List<Vector4> texCoordsAndGlowModifiers = new List<Vector4>(defaultListCapacity);
            private readonly List<Vector4> fadeLifetimes = new List<Vector4>(defaultListCapacity);

#endif

            private const int boundsPadder = 1000000000;
            private int currentBoundsMinX = int.MaxValue - boundsPadder;
            private int currentBoundsMinY = int.MaxValue - boundsPadder;
            private int currentBoundsMinZ = int.MaxValue - boundsPadder;
            private int currentBoundsMaxX = int.MinValue + boundsPadder;
            private int currentBoundsMaxY = int.MinValue + boundsPadder;
            private int currentBoundsMaxZ = int.MinValue + boundsPadder;

            private Mesh mesh;
            private MeshFilter meshFilterGlow;
            private MeshFilter meshFilterBolt;
            private MeshRenderer meshRendererGlow;
            private MeshRenderer meshRendererBolt;

            #endregion Private variables

            #region Private methods

            private void PopulateMeshInternal()
            {
                GameObject.SetActive(true);

                mesh.SetVertices(vertices);
                mesh.SetTangents(lineDirs);
                mesh.SetColors(colors);
                mesh.SetUVs(0, texCoordsAndGlowModifiers);
                mesh.SetUVs(1, fadeLifetimes);
                mesh.SetNormals(ends);
                mesh.SetTriangles(indices, 0);

                Bounds b = new Bounds();
                Vector3 min = new Vector3(currentBoundsMinX - 2, currentBoundsMinY - 2, currentBoundsMinZ - 2);
                Vector3 max = new Vector3(currentBoundsMaxX + 2, currentBoundsMaxY + 2, currentBoundsMaxZ + 2);
                b.center = (max + min) * 0.5f;
                b.size = (max - min) * 1.2f;
                mesh.bounds = b;
            }

            private void UpdateBounds(ref Vector3 point1, ref Vector3 point2)
            {
                // r = y + ((x - y) & ((x - y) >> (sizeof(int) * CHAR_BIT - 1))); // min(x, y)
                // r = x - ((x - y) & ((x - y) >> (sizeof(int) * CHAR_BIT - 1))); // max(x, y)

                unchecked
                {
                    {
                        int xCalculation = (int)point1.x - (int)point2.x;
                        xCalculation &= (xCalculation >> 31);
                        int xMin = (int)point2.x + xCalculation;
                        int xMax = (int)point1.x - xCalculation;

                        xCalculation = currentBoundsMinX - xMin;
                        xCalculation &= (xCalculation >> 31);
                        currentBoundsMinX = xMin + xCalculation;

                        xCalculation = currentBoundsMaxX - xMax;
                        xCalculation &= (xCalculation >> 31);
                        currentBoundsMaxX = currentBoundsMaxX - xCalculation;
                    }
                    {
                        int yCalculation = (int)point1.y - (int)point2.y;
                        yCalculation &= (yCalculation >> 31);
                        int yMin = (int)point2.y + yCalculation;
                        int yMax = (int)point1.y - yCalculation;

                        yCalculation = currentBoundsMinY - yMin;
                        yCalculation &= (yCalculation >> 31);
                        currentBoundsMinY = yMin + yCalculation;

                        yCalculation = currentBoundsMaxY - yMax;
                        yCalculation &= (yCalculation >> 31);
                        currentBoundsMaxY = currentBoundsMaxY - yCalculation;
                    }
                    {
                        int zCalculation = (int)point1.z - (int)point2.z;
                        zCalculation &= (zCalculation >> 31);
                        int zMin = (int)point2.z + zCalculation;
                        int zMax = (int)point1.z - zCalculation;

                        zCalculation = currentBoundsMinZ - zMin;
                        zCalculation &= (zCalculation >> 31);
                        currentBoundsMinZ = zMin + zCalculation;

                        zCalculation = currentBoundsMaxZ - zMax;
                        zCalculation &= (zCalculation >> 31);
                        currentBoundsMaxZ = currentBoundsMaxZ - zCalculation;
                    }
                }
            }

            private void AddIndices()
            {
                int vertexIndex = vertices.Count;
                indices.Add(vertexIndex++);
                indices.Add(vertexIndex++);
                indices.Add(vertexIndex);
                indices.Add(vertexIndex--);
                indices.Add(vertexIndex);
                indices.Add(vertexIndex += 2);
            }

            private void AppendLineInternal(ref Vector3 start, ref Vector3 end, ref Vector4 dir, ref Vector4 dirPrev1, ref Vector4 dirPrev2,
                Color32 color, float colorIntensity, ref Vector4 fadeLifeTime, float glowWidthModifier, float glowIntensity)
            {
                AddIndices();
                color.a = (byte)Mathf.Lerp(0.0f, 255.0f, colorIntensity * 0.1f);

                Vector4 texCoord = new Vector4(uv1.x, uv1.y, glowWidthModifier, glowIntensity);

                vertices.Add(start);
                lineDirs.Add(dirPrev1);
                colors.Add(color);
                ends.Add(dir);

                vertices.Add(end);
                lineDirs.Add(dir);
                colors.Add(color);
                ends.Add(dir);

                dir.w = -dir.w;

                vertices.Add(start);
                lineDirs.Add(dirPrev2);
                colors.Add(color);
                ends.Add(dir);

                vertices.Add(end);
                lineDirs.Add(dir);
                colors.Add(color);
                ends.Add(dir);

#if UNITY_PRE_5_3

                texCoords.Add(uv1);
				texCoords.Add(uv2);
				texCoords.Add(uv3);
				texCoords.Add(uv4);
				glowModifiers.Add(new Vector2(texCoord.z, texCoord.w));
				glowModifiers.Add(new Vector2(texCoord.z, texCoord.w));
				glowModifiers.Add(new Vector2(texCoord.z, texCoord.w));
				glowModifiers.Add(new Vector2(texCoord.z, texCoord.w));
				fadeXY.Add(new Vector2(fadeLifeTime.x, fadeLifeTime.y));
				fadeXY.Add(new Vector2(fadeLifeTime.x, fadeLifeTime.y));
				fadeXY.Add(new Vector2(fadeLifeTime.x, fadeLifeTime.y));
				fadeXY.Add(new Vector2(fadeLifeTime.x, fadeLifeTime.y));
				fadeZW.Add(new Vector2(fadeLifeTime.z, fadeLifeTime.w));
				fadeZW.Add(new Vector2(fadeLifeTime.z, fadeLifeTime.w));
				fadeZW.Add(new Vector2(fadeLifeTime.z, fadeLifeTime.w));
				fadeZW.Add(new Vector2(fadeLifeTime.z, fadeLifeTime.w));

#else

                texCoordsAndGlowModifiers.Add(texCoord);
                texCoord.x = uv2.x;
                texCoord.y = uv2.y;
                texCoordsAndGlowModifiers.Add(texCoord);
                texCoord.x = uv3.x;
                texCoord.y = uv3.y;
                texCoordsAndGlowModifiers.Add(texCoord);
                texCoord.x = uv4.x;
                texCoord.y = uv4.y;
                texCoordsAndGlowModifiers.Add(texCoord);
                fadeLifetimes.Add(fadeLifeTime);
                fadeLifetimes.Add(fadeLifeTime);
                fadeLifetimes.Add(fadeLifeTime);
                fadeLifetimes.Add(fadeLifeTime);

#endif

                UpdateBounds(ref start, ref end);
            }

            #endregion Private methods
        }

        #endregion LineRendererMesh

        #region Public variables

        /// <summary>
        /// The maximum number of lights to allow for all lightning
        /// </summary>
        public static int MaximumLightCount = 128;

        /// <summary>
        /// The maximum number of lights to create per batch of lightning emitted
        /// </summary>
        public static int MaximumLightsPerBatch = 8;

        /// <summary>
        /// The current minimum delay until anything will start rendering
        /// </summary>
        public float MinimumDelay { get; private set; }

        /// <summary>
        /// Is there any glow for any of the lightning bolts?
        /// </summary>
        public bool HasGlow { get; private set; }

        /// <summary>
        /// Is this lightning bolt active any more?
        /// </summary>
        public bool IsActive { get { return elapsedTime < lifeTime; } }

        /// <summary>
        /// Camera mode
        /// </summary>
        public CameraMode CameraMode { get; private set; }

        private DateTime startTimeOffset;

        #endregion Public variables

        #region Public methods

        /// <summary>
        /// Default constructor
        /// </summary>
        public LightningBolt()
        {
        }

        /// <summary>
        /// Setup a lightning bolt from dependencies
        /// </summary>
        /// <param name="dependencies">Dependencies</param>
        public void SetupLightningBolt(LightningBoltDependencies dependencies)
        {
            if (dependencies == null || dependencies.Parameters.Count == 0)
            {
                Debug.LogError("Lightning bolt dependencies must not be null");
                return;
            }
            else if (this.dependencies != null)
            {
                Debug.LogError("This lightning bolt is already in use!");
                return;
            }

            this.dependencies = dependencies;
            CameraMode = dependencies.CameraMode;
            timeSinceLevelLoad = LightningBoltScript.TimeSinceStart;
            CheckForGlow(dependencies.Parameters);
            MinimumDelay = float.MaxValue;

            if (dependencies.ThreadState.multiThreaded)
            {
                startTimeOffset = DateTime.UtcNow;
                dependencies.ThreadState.AddActionForBackgroundThread(ProcessAllLightningParameters);
            }
            else
            {
                ProcessAllLightningParameters();
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <returns>True if alive, false if expired</returns>
        public bool Update()
        {
            elapsedTime += LightningBoltScript.DeltaTime;
            if (elapsedTime > maxLifeTime)
            {
                return false;
            }
            else if (hasLight)
            {
                UpdateLights();
            }
            return true;
        }

        /// <summary>
        /// Cleanup all resources
        /// </summary>
        public void Cleanup()
        {
            foreach (LightningBoltSegmentGroup g in segmentGroupsWithLight)
            {
                // cleanup lights
                foreach (Light l in g.Lights)
                {
                    CleanupLight(l);
                }
                g.Lights.Clear();
            }
            lock (groupCache)
            {
                foreach (LightningBoltSegmentGroup g in segmentGroups)
                {
                    groupCache.Add(g);
                }
            }
            hasLight = false;
            elapsedTime = 0.0f;
            lifeTime = 0.0f;
            maxLifeTime = 0.0f;
            if (dependencies != null)
            {
                dependencies.ReturnToCache(dependencies);
                dependencies = null;
            }

            // return all line renderers to cache
            foreach (LineRendererMesh m in activeLineRenderers)
            {
                if (m != null)
                {
                    m.Reset();
                    lineRendererCache.Add(m);
                }
            }
            segmentGroups.Clear();
            segmentGroupsWithLight.Clear();
            activeLineRenderers.Clear();
        }

        /// <summary>
        /// Add a new segment group, or get one from cache
        /// </summary>
        /// <returns>LightningBoltSegmentGroup</returns>
        public LightningBoltSegmentGroup AddGroup()
        {
            LightningBoltSegmentGroup group;
            lock (groupCache)
            {
                if (groupCache.Count == 0)
                {
                    group = new LightningBoltSegmentGroup();
                }
                else
                {
                    int index = groupCache.Count - 1;
                    group = groupCache[index];
                    group.Reset();
                    groupCache.RemoveAt(index);
                }
            }
            segmentGroups.Add(group);
            return group;
        }

        /// <summary>
        /// Clear out all cached objects to free up memory
        /// </summary>
        public static void ClearCache()
        {
            foreach (LineRendererMesh obj in lineRendererCache)
            {
                if (obj != null)
                {
                    GameObject.Destroy(obj.GameObject);
                }
            }
            foreach (Light obj in lightCache)
            {
                if (obj != null)
                {
                    GameObject.Destroy(obj.gameObject);
                }
            }
            lineRendererCache.Clear();
            lightCache.Clear();
            lock (groupCache)
            {
                groupCache.Clear();
            }
        }

        #endregion Public methods

        #region Private variables

        // required dependencies to create lightning bolts
        private LightningBoltDependencies dependencies;

        // how long this bolt has been alive
        private float elapsedTime;

        // total life span of this bolt
        private float lifeTime;

        // either lifeTime or larger depending on if lights are lingering beyond the end of the bolt
        private float maxLifeTime;

        // does this lightning bolt have light?
        private bool hasLight;

        // saved in case of threading
        private float timeSinceLevelLoad;

        private readonly List<LightningBoltSegmentGroup> segmentGroups = new List<LightningBoltSegmentGroup>();
        private readonly List<LightningBoltSegmentGroup> segmentGroupsWithLight = new List<LightningBoltSegmentGroup>();
        private readonly List<LineRendererMesh> activeLineRenderers = new List<LineRendererMesh>();

        private static int lightCount;
        private static readonly List<LineRendererMesh> lineRendererCache = new List<LineRendererMesh>();
        private static readonly List<LightningBoltSegmentGroup> groupCache = new List<LightningBoltSegmentGroup>();
        private static readonly List<Light> lightCache = new List<Light>();

        #endregion Private variables

        #region Private methods

        private void CleanupLight(Light l)
        {
            if (l != null)
            {
                dependencies.LightRemoved(l);
                lightCache.Add(l);
                l.gameObject.SetActive(false);
                lightCount--;
            }
        }

        private void EnableLineRenderer(LineRendererMesh lineRenderer, int tag)
        {
            bool shouldPopulate = (lineRenderer != null && lineRenderer.GameObject != null && lineRenderer.Tag == tag && IsActive);
            if (shouldPopulate)
            {
                lineRenderer.PopulateMesh();
            }
        }

        private IEnumerator EnableLastRendererCoRoutine()
        {
            LineRendererMesh lineRenderer = activeLineRenderers[activeLineRenderers.Count - 1];
            int tag = ++lineRenderer.Tag; // in case it gets cleaned up for later

            yield return WaitForSecondsLightning.WaitForSecondsLightningPooled(MinimumDelay);

            EnableLineRenderer(lineRenderer, tag);
        }

        private LineRendererMesh GetOrCreateLineRenderer()
        {
            LineRendererMesh lineRenderer;

            while (true)
            {
                if (lineRendererCache.Count == 0)
                {
                    lineRenderer = new LineRendererMesh(this.dependencies);
                }
                else
                {
                    int index = lineRendererCache.Count - 1;
                    lineRenderer = lineRendererCache[index];
                    lineRendererCache.RemoveAt(index);
                    if (lineRenderer == null || lineRenderer.Transform == null)
                    {
                        // destroyed by some other means, try again for cache...
                        continue;
                    }
                }
                break;
            }

            dependencies.ThreadState.AddActionForMainThread(b =>
            {
                // clear parent - this ensures that the rotation and scale can be reset before assigning a new parent
                lineRenderer.Transform.parent = null;
                lineRenderer.Transform.rotation = Quaternion.identity;
                lineRenderer.Transform.localScale = Vector3.one;
                lineRenderer.Transform.parent = dependencies.Parent.transform;
                lineRenderer.GameObject.layer = lineRenderer.MeshRendererBolt.gameObject.layer = lineRenderer.MeshRendererGlow.gameObject.layer = dependencies.Parent.layer; // maintain the layer of the parent

                if (dependencies.UseWorldSpace)
                {
                    lineRenderer.GameObject.transform.position = Vector3.zero;
                }
                else
                {
                    lineRenderer.GameObject.transform.localPosition = Vector3.zero;
                }

                lineRenderer.MaterialGlow = dependencies.LightningMaterialMesh;
                lineRenderer.MaterialBolt = dependencies.LightningMaterialMeshNoGlow;
                if (!string.IsNullOrEmpty(dependencies.SortLayerName))
                {
                    lineRenderer.MeshRendererGlow.sortingLayerName = lineRenderer.MeshRendererBolt.sortingLayerName = dependencies.SortLayerName;
                    lineRenderer.MeshRendererGlow.sortingOrder = lineRenderer.MeshRendererBolt.sortingOrder = dependencies.SortOrderInLayer;
                }
                else
                {
                    lineRenderer.MeshRendererGlow.sortingLayerName = lineRenderer.MeshRendererBolt.sortingLayerName = null;
                    lineRenderer.MeshRendererGlow.sortingOrder = lineRenderer.MeshRendererBolt.sortingOrder = 0;
                }
            }, true);

            activeLineRenderers.Add(lineRenderer);

            return lineRenderer;
        }

        private void RenderGroup(LightningBoltSegmentGroup group, LightningBoltParameters p)
        {
            if (group.SegmentCount == 0)
            {
                return;
            }

            float timeOffset = (!dependencies.ThreadState.multiThreaded ? 0.0f : (float)(DateTime.UtcNow - startTimeOffset).TotalSeconds);
            float timeStart = timeSinceLevelLoad + group.Delay + timeOffset;
            Vector4 fadeLifeTime = new Vector4(timeStart, timeStart + group.PeakStart, timeStart + group.PeakEnd, timeStart + group.LifeTime);
            float radius = group.LineWidth * 0.5f * LightningBoltParameters.Scale;
            int lineCount = (group.Segments.Count - group.StartIndex);
            float radiusStep = (radius - (radius * group.EndWidthMultiplier)) / (float)lineCount;

            // growth multiplier
            float timeStep;
            if (p.GrowthMultiplier > 0.0f)
            {
                timeStep = (group.LifeTime / (float)lineCount) * p.GrowthMultiplier;
                timeOffset = 0.0f;
            }
            else
            {
                timeStep = 0.0f;
                timeOffset = 0.0f;
            }

            LineRendererMesh currentLineRenderer = (activeLineRenderers.Count == 0 ? GetOrCreateLineRenderer() : activeLineRenderers[activeLineRenderers.Count - 1]);

            // if we have filled up the mesh, we need to start a new line renderer
            if (!currentLineRenderer.PrepareForLines(lineCount))
            {
                if (currentLineRenderer.CustomTransform != null)
                {
                    // can't create multiple meshes if using a custom transform callback
                    return;
                }

                if (dependencies.ThreadState.multiThreaded)
                {
                    // we need to block until this action is run, Unity objects can only be modified and created on the main thread
                    dependencies.ThreadState.AddActionForMainThread((inDestroy) =>
                    {
                        if (!inDestroy)
                        {
                            EnableCurrentLineRenderer();
                            currentLineRenderer = GetOrCreateLineRenderer();
                        }
                    }, true);
                }
                else
                {
                    EnableCurrentLineRenderer();
                    currentLineRenderer = GetOrCreateLineRenderer();
                }
            }

            currentLineRenderer.BeginLine(group.Segments[group.StartIndex].Start, group.Segments[group.StartIndex].End, radius, group.Color, p.Intensity, fadeLifeTime, p.GlowWidthMultiplier, p.GlowIntensity);
            for (int i = group.StartIndex + 1; i < group.Segments.Count; i++)
            {
                radius -= radiusStep;
                if (p.GrowthMultiplier < 1.0f)
                {
                    timeOffset += timeStep;
                    fadeLifeTime = new Vector4(timeStart + timeOffset, timeStart + group.PeakStart + timeOffset, timeStart + group.PeakEnd, timeStart + group.LifeTime);
                }
                currentLineRenderer.AppendLine(group.Segments[i].Start, group.Segments[i].End, radius, group.Color, p.Intensity, fadeLifeTime, p.GlowWidthMultiplier, p.GlowIntensity);
            }
        }

        private static IEnumerator NotifyBolt(LightningBoltDependencies dependencies, LightningBoltParameters p, Transform transform, Vector3 start, Vector3 end)
        {
            float delay = p.delaySeconds;
            float lifeTime = p.LifeTime;
            yield return WaitForSecondsLightning.WaitForSecondsLightningPooled(delay);
            if (dependencies.LightningBoltStarted != null)
            {
                dependencies.LightningBoltStarted(p, start, end);
            }
            LightningCustomTransformStateInfo state = (p.CustomTransform == null ? null : LightningCustomTransformStateInfo.GetOrCreateStateInfo());
            if (state != null)
            {
                state.Parameters = p;
                state.BoltStartPosition = start;
                state.BoltEndPosition = end;
                state.State = LightningCustomTransformState.Started;
                state.Transform = transform;
                p.CustomTransform(state);
                state.State = LightningCustomTransformState.Executing;
            }

            if (p.CustomTransform == null)
            {
                yield return WaitForSecondsLightning.WaitForSecondsLightningPooled(lifeTime);
            }
            else
            {
                while (lifeTime > 0.0f)
                {
                    p.CustomTransform(state);
                    lifeTime -= LightningBoltScript.DeltaTime;
                    yield return null;
                }
            }

            if (p.CustomTransform != null)
            {
                state.State = LightningCustomTransformState.Ended;
                p.CustomTransform(state);
                LightningCustomTransformStateInfo.ReturnStateInfoToCache(state);
            }
            if (dependencies.LightningBoltEnded != null)
            {
                dependencies.LightningBoltEnded(p, start, end);
            }
            LightningBoltParameters.ReturnParametersToCache(p);
        }

        private void ProcessParameters(LightningBoltParameters p, RangeOfFloats delay, LightningBoltDependencies depends)
        {
            Vector3 start, end;
            MinimumDelay = Mathf.Min(delay.Minimum, MinimumDelay);
            p.delaySeconds = delay.Random(p.Random);

            // apply LOD if specified
            if (depends.LevelOfDetailDistance > Mathf.Epsilon)
            {
                float d;
                if (p.Points.Count > 1)
                {
                    d = Vector3.Distance(depends.CameraPos, p.Points[0]);
                    d = Mathf.Min(Vector3.Distance(depends.CameraPos, p.Points[p.Points.Count - 1]));
                }
                else
                {
                    d = Vector3.Distance(depends.CameraPos, p.Start);
                    d = Mathf.Min(Vector3.Distance(depends.CameraPos, p.End));
                }
                int modifier = Mathf.Min(8, (int)(d / depends.LevelOfDetailDistance));
                p.Generations = Mathf.Max(1, p.Generations - modifier);
                p.GenerationWhereForksStopSubtractor = Mathf.Clamp(p.GenerationWhereForksStopSubtractor - modifier, 0, 8);
            }

            p.generationWhereForksStop = p.Generations - p.GenerationWhereForksStopSubtractor;
            lifeTime = Mathf.Max(p.LifeTime + p.delaySeconds, lifeTime);
            maxLifeTime = Mathf.Max(lifeTime, maxLifeTime);
            p.forkednessCalculated = (int)Mathf.Ceil(p.Forkedness * (float)p.Generations);
            if (p.Generations > 0)
            {
                p.Generator = p.Generator ?? LightningGenerator.GeneratorInstance;
                p.Generator.GenerateLightningBolt(this, p, out start, out end);
                p.Start = start;
                p.End = end;
            }
        }

        private void ProcessAllLightningParameters()
        {
            int maxLightsForEachParameters = MaximumLightsPerBatch / dependencies.Parameters.Count;
            RangeOfFloats delay = new RangeOfFloats();
            List<int> groupIndexes = new List<int>(dependencies.Parameters.Count + 1);
            int i = 0;

#if ENABLE_PROFILING

            System.Diagnostics.Stopwatch w = System.Diagnostics.Stopwatch.StartNew();

#endif

            foreach (LightningBoltParameters parameters in dependencies.Parameters)
            {
                delay.Minimum = parameters.DelayRange.Minimum + parameters.Delay;
                delay.Maximum = parameters.DelayRange.Maximum + parameters.Delay;
                parameters.maxLights = maxLightsForEachParameters;
                groupIndexes.Add(segmentGroups.Count);
                ProcessParameters(parameters, delay, dependencies);
            }
            groupIndexes.Add(segmentGroups.Count);

#if ENABLE_PROFILING

            w.Stop();
            UnityEngine.Debug.LogFormat("GENERATE: {0}", w.Elapsed.TotalMilliseconds);
            w.Reset();
            w.Start();

#endif

            LightningBoltDependencies dependenciesRef = dependencies;
            foreach (LightningBoltParameters parameters in dependenciesRef.Parameters)
            {
                Transform transform = RenderLightningBolt(parameters.quality, parameters.Generations, groupIndexes[i], groupIndexes[++i], parameters);

                if (dependenciesRef.ThreadState.multiThreaded)
                {
                    dependenciesRef.ThreadState.AddActionForMainThread((inDestroy) =>
                    {
                        if (!inDestroy)
                        {
                            dependenciesRef.StartCoroutine(NotifyBolt(dependenciesRef, parameters, transform, parameters.Start, parameters.End));
                        }
                    }, false);
                }
                else
                {
                    dependenciesRef.StartCoroutine(NotifyBolt(dependenciesRef, parameters, transform, parameters.Start, parameters.End));
                }
            }

#if ENABLE_PROFILING

            w.Stop();
            UnityEngine.Debug.LogFormat("RENDER: {0}", w.Elapsed.TotalMilliseconds);

#endif

            if (dependencies.ThreadState.multiThreaded)
            {
                dependencies.ThreadState.AddActionForMainThread(EnableCurrentLineRendererFromThread);
            }
            else
            {
                EnableCurrentLineRenderer();
                dependencies.AddActiveBolt(this);
            }
        }

        private void EnableCurrentLineRendererFromThread(bool inDestroy)
        {
            //try
            //{
            if (inDestroy)
            {
                return;
            }

            EnableCurrentLineRenderer();
            dependencies.AddActiveBolt(this);
            //}
            //finally
            //{
            // clear the thread state, we verify in the Cleanup method that this is nulled out to ensure we are not cleaning up lightning that is still being generated
            //dependencies.ThreadState = null;
            //}
        }

        private void EnableCurrentLineRenderer()
        {
            if (activeLineRenderers.Count == 0)
            {
                return;
            }
            // make sure the last renderer gets enabled at the appropriate time
            else if (MinimumDelay <= 0.0f)
            {
                EnableLineRenderer(activeLineRenderers[activeLineRenderers.Count - 1], activeLineRenderers[activeLineRenderers.Count - 1].Tag);
            }
            else
            {
                dependencies.StartCoroutine(EnableLastRendererCoRoutine());
            }
        }

        private void RenderParticleSystems(Vector3 start, Vector3 end, float trunkWidth, float lifeTime, float delaySeconds)
        {
            // only emit particle systems if we have a trunk - example, cloud lightning should not emit particles
            if (trunkWidth > 0.0f)
            {
                if (dependencies.OriginParticleSystem != null)
                {
                    // we have a strike, create a particle where the lightning is coming from
                    dependencies.StartCoroutine(GenerateParticleCoRoutine(dependencies.OriginParticleSystem, start, delaySeconds));
                }
                if (dependencies.DestParticleSystem != null)
                {
                    dependencies.StartCoroutine(GenerateParticleCoRoutine(dependencies.DestParticleSystem, end, delaySeconds + (lifeTime * 0.8f)));
                }
            }
        }

        private Transform RenderLightningBolt(LightningBoltQualitySetting quality, int generations, int startGroupIndex, int endGroupIndex, LightningBoltParameters parameters)
        {
            if (segmentGroups.Count == 0 || startGroupIndex >= segmentGroups.Count || endGroupIndex > segmentGroups.Count)
            {
                return null;
            }

            Transform transform = null;
            LightningLightParameters lp = parameters.LightParameters;
            if (lp != null)
            {
                if ((hasLight |= lp.HasLight))
                {
                    lp.LightPercent = Mathf.Clamp(lp.LightPercent, Mathf.Epsilon, 1.0f);
                    lp.LightShadowPercent = Mathf.Clamp(lp.LightShadowPercent, 0.0f, 1.0f);
                }
                else
                {
                    lp = null;
                }
            }

            LightningBoltSegmentGroup mainTrunkGroup = segmentGroups[startGroupIndex];
            Vector3 start = mainTrunkGroup.Segments[mainTrunkGroup.StartIndex].Start;
            Vector3 end = mainTrunkGroup.Segments[mainTrunkGroup.StartIndex + mainTrunkGroup.SegmentCount - 1].End;
            parameters.FadePercent = Mathf.Clamp(parameters.FadePercent, 0.0f, 0.5f);

            // create a new line renderer mesh right now if we have a custom transform
            if (parameters.CustomTransform != null)
            {
                LineRendererMesh currentLineRenderer = (activeLineRenderers.Count == 0 || !activeLineRenderers[activeLineRenderers.Count - 1].Empty ? null : activeLineRenderers[activeLineRenderers.Count - 1]);

                if (currentLineRenderer == null)
                {
                    if (dependencies.ThreadState.multiThreaded)
                    {
                        // we need to block until this action is run, Unity objects can only be modified and created on the main thread
                        dependencies.ThreadState.AddActionForMainThread((inDestroy) =>
                        {
                            if (!inDestroy)
                            {
                                EnableCurrentLineRenderer();
                                currentLineRenderer = GetOrCreateLineRenderer();
                            }
                        }, true);
                    }
                    else
                    {
                        EnableCurrentLineRenderer();
                        currentLineRenderer = GetOrCreateLineRenderer();
                    }
                }
                if (currentLineRenderer == null)
                {
                    return null;
                }

                currentLineRenderer.CustomTransform = parameters.CustomTransform;
                transform = currentLineRenderer.Transform;
            }

            for (int i = startGroupIndex; i < endGroupIndex; i++)
            {
                LightningBoltSegmentGroup group = segmentGroups[i];
                group.Delay = parameters.delaySeconds;
                group.LifeTime = parameters.LifeTime;
                group.PeakStart = group.LifeTime * parameters.FadePercent;
                group.PeakEnd = group.LifeTime - group.PeakStart;
                float peakGap = group.PeakEnd - group.PeakStart;
                float fadeOut = group.LifeTime - group.PeakEnd;
                group.PeakStart *= parameters.FadeInMultiplier;
                group.PeakEnd = group.PeakStart + (peakGap * parameters.FadeFullyLitMultiplier);
                group.LifeTime = group.PeakEnd + (fadeOut * parameters.FadeOutMultiplier);
                group.LightParameters = lp;
                RenderGroup(group, parameters);
            }

            if (dependencies.ThreadState.multiThreaded)
            {
                dependencies.ThreadState.AddActionForMainThread((inDestroy) =>
                {
                    if (!inDestroy)
                    {
                        RenderParticleSystems(start, end, parameters.TrunkWidth, parameters.LifeTime, parameters.delaySeconds);

                        // create lights only on the main trunk
                        if (lp != null)
                        {
                            CreateLightsForGroup(segmentGroups[startGroupIndex], lp, quality, parameters.maxLights);
                        }
                    }
                });
            }
            else
            {
                RenderParticleSystems(start, end, parameters.TrunkWidth, parameters.LifeTime, parameters.delaySeconds);

                // create lights only on the main trunk
                if (lp != null)
                {
                    CreateLightsForGroup(segmentGroups[startGroupIndex], lp, quality, parameters.maxLights);
                }
            }

            return transform;
        }

        private void CreateLightsForGroup(LightningBoltSegmentGroup group, LightningLightParameters lp, LightningBoltQualitySetting quality, int maxLights)
        {
            if (lightCount == MaximumLightCount || maxLights <= 0)
            {
                return;
            }

            float fadeOutTime = (lifeTime - group.PeakEnd) * lp.FadeOutMultiplier;
            float peakGap = (group.PeakEnd - group.PeakStart) * lp.FadeFullyLitMultiplier;
            float peakStart = group.PeakStart * lp.FadeInMultiplier;
            float peakEnd = peakStart + peakGap;
            float maxLifeWithLights = peakEnd + fadeOutTime;
            maxLifeTime = Mathf.Max(maxLifeTime, group.Delay + maxLifeWithLights);

            segmentGroupsWithLight.Add(group);

            int segmentCount = group.SegmentCount;
            float lightPercent, lightShadowPercent;
            if (quality == LightningBoltQualitySetting.LimitToQualitySetting)
            {
                int level = QualitySettings.GetQualityLevel();
                LightningQualityMaximum maximum;
                if (LightningBoltParameters.QualityMaximums.TryGetValue(level, out maximum))
                {
                    lightPercent = Mathf.Min(lp.LightPercent, maximum.MaximumLightPercent);
                    lightShadowPercent = Mathf.Min(lp.LightShadowPercent, maximum.MaximumShadowPercent);
                }
                else
                {
                    Debug.LogError("Unable to read lightning quality for level " + level.ToString());
                    lightPercent = lp.LightPercent;
                    lightShadowPercent = lp.LightShadowPercent;
                }
            }
            else
            {
                lightPercent = lp.LightPercent;
                lightShadowPercent = lp.LightShadowPercent;
            }

            maxLights = Mathf.Max(1, Mathf.Min(maxLights, (int)(segmentCount * lightPercent)));
            int nthLight = Mathf.Max(1, (int)((segmentCount / maxLights)));
            int nthShadows = maxLights - (int)((float)maxLights * lightShadowPercent);
            int nthShadowCounter = nthShadows;

            // add lights evenly spaced
            for (int i = group.StartIndex + (int)(nthLight * 0.5f); i < group.Segments.Count; i += nthLight)
            {
                if (AddLightToGroup(group, lp, i, nthLight, nthShadows, ref maxLights, ref nthShadowCounter))
                {
                    return;
                }
            }

            // Debug.Log("Lightning light count: " + lightCount.ToString());
        }

        private bool AddLightToGroup(LightningBoltSegmentGroup group, LightningLightParameters lp, int segmentIndex,
            int nthLight, int nthShadows, ref int maxLights, ref int nthShadowCounter)
        {
            Light light = GetOrCreateLight(lp);
            group.Lights.Add(light);
            Vector3 pos = (group.Segments[segmentIndex].Start + group.Segments[segmentIndex].End) * 0.5f;
            if (dependencies.CameraIsOrthographic)
            {
                if (dependencies.CameraMode == CameraMode.OrthographicXZ)
                {
                    pos.y = dependencies.CameraPos.y + lp.OrthographicOffset;
                }
                else
                {
                    pos.z = dependencies.CameraPos.z + lp.OrthographicOffset;
                }
            }
            if (dependencies.UseWorldSpace)
            {
                light.gameObject.transform.position = pos;
            }
            else
            {
                light.gameObject.transform.localPosition = pos;
            }
            if (lp.LightShadowPercent == 0.0f || ++nthShadowCounter < nthShadows)
            {
                light.shadows = LightShadows.None;
            }
            else
            {
                light.shadows = LightShadows.Soft;
                nthShadowCounter = 0;
            }

            // return true if no more lights possible, false otherwise
            return (++lightCount == MaximumLightCount || --maxLights == 0);
        }

        private Light GetOrCreateLight(LightningLightParameters lp)
        {
            Light light;
            while (true)
            {
                if (lightCache.Count == 0)
                {
                    GameObject lightningLightObject = new GameObject("LightningBoltLight");

#if UNITY_EDITOR

                    lightningLightObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

#endif

                    light = lightningLightObject.AddComponent<Light>();
                    light.type = LightType.Point;
                    break;
                }
                else
                {
                    light = lightCache[lightCache.Count - 1];
                    lightCache.RemoveAt(lightCache.Count - 1);
                    if (light == null)
                    {
                        // may have been disposed or the level re-loaded
                        continue;
                    }
                    break;
                }
            }

            light.bounceIntensity = lp.BounceIntensity;
            light.shadowNormalBias = lp.ShadowNormalBias;
            light.color = lp.LightColor;
            light.renderMode = lp.RenderMode;
            light.range = lp.LightRange;
            light.shadowStrength = lp.ShadowStrength;
            light.shadowBias = lp.ShadowBias;
            light.intensity = 0.0f;
            light.gameObject.transform.parent = dependencies.Parent.transform;
            light.gameObject.SetActive(true);

            dependencies.LightAdded(light);

            return light;
        }

        private void UpdateLight(LightningLightParameters lp, IEnumerable<Light> lights, float delay, float peakStart, float peakEnd, float lifeTime)
        {
            if (elapsedTime < delay)
            {
                return;
            }

            // depending on whether we have hit the mid point of our lifetime, fade the light in or out

            // adjust lights for fade parameters
            float fadeOutTime = (lifeTime - peakEnd) * lp.FadeOutMultiplier;
            float peakGap = (peakEnd - peakStart) * lp.FadeFullyLitMultiplier;
            peakStart *= lp.FadeInMultiplier;
            peakEnd = peakStart + peakGap;
            lifeTime = peakEnd + fadeOutTime;
            float realElapsedTime = elapsedTime - delay;
            if (realElapsedTime >= peakStart)
            {
                if (realElapsedTime <= peakEnd)
                {
                    // fully lit
                    foreach (Light l in lights)
                    {
                        l.intensity = lp.LightIntensity * lp.LightMultiplier;
                    }
                }
                else
                {
                    // fading out
                    float lerp = (realElapsedTime - peakEnd) / (lifeTime - peakEnd);
                    foreach (Light l in lights)
                    {
                        l.intensity = Mathf.Lerp(lp.LightIntensity * lp.LightMultiplier, 0.0f, lerp);
                    }
                }
            }
            else
            {
                // fading in
                float lerp = realElapsedTime / peakStart;
                foreach (Light l in lights)
                {
                    l.intensity = Mathf.Lerp(0.0f, lp.LightIntensity * lp.LightMultiplier, lerp);
                }
            }
        }

        private void UpdateLights()
        {
            foreach (LightningBoltSegmentGroup group in segmentGroupsWithLight)
            {
                UpdateLight(group.LightParameters, group.Lights, group.Delay, group.PeakStart, group.PeakEnd, group.LifeTime);
            }
        }

        private IEnumerator GenerateParticleCoRoutine(ParticleSystem p, Vector3 pos, float delay)
        {
            yield return WaitForSecondsLightning.WaitForSecondsLightningPooled(delay);

            p.transform.position = pos;
            int count;
            if (p.emission.burstCount > 0)
            {
                ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[p.emission.burstCount];
                p.emission.GetBursts(bursts);
                count = UnityEngine.Random.Range(bursts[0].minCount, bursts[0].maxCount + 1);
                p.Emit(count);
            }
            else
            {
                ParticleSystem.MinMaxCurve rate = p.emission.rateOverTime;
                count = (int)((rate.constantMax - rate.constantMin) * 0.5f);
                count = UnityEngine.Random.Range(count, count * 2);
                p.Emit(count);
            }
        }

        private void CheckForGlow(IEnumerable<LightningBoltParameters> parameters)
        {
            // we need to know if there is glow so we can choose the glow or non-glow setting in the renderer
            foreach (LightningBoltParameters p in parameters)
            {
                HasGlow = (p.GlowIntensity >= Mathf.Epsilon && p.GlowWidthMultiplier >= Mathf.Epsilon);

                if (HasGlow)
                {
                    break;
                }
            }
        }

        #endregion Private methods
    }

#if UNITY_WEBGL

    public class LightningThreadState
    {
        internal readonly int mainThreadId = 1;
        internal readonly bool multiThreaded;

        /// <summary>
        /// Running?
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Constructor - starts the thread
        /// </summary>
        /// <param name="multiThreaded">Multi-threaded?</param>
        public LightningThreadState(bool multiThreaded)
        {
            this.multiThreaded = false;
        }

        /// <summary>
        /// Add a main thread action
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="waitForAction">True to wait for completion, false if not</param>
        /// <returns>True if action added, false if in process of terminating the thread</returns>
        public bool AddActionForMainThread(System.Action<bool> action, bool waitForAction = false)
        {
            action(false);
            return true;
        }

        /// <summary>
        /// Terminate and wait for thread end
        /// </summary>
        /// <param name="inDestroy">True if in destroy, false otherwise</param>
        public void TerminateAndWaitForEnd(bool inDestroy)
        {
        }

        /// <summary>
        /// Add a background thread action
        /// </summary>
        /// <param name="action">Action</param>
        /// <returns>True if action added, false if in process of terminating the thread</returns>
        public bool AddActionForBackgroundThread(System.Action action)
        {
            action();
            return true;
        }

        public void UpdateMainThreadActions()
        {
        }
    }

#else

    /// <summary>
    /// Lightning threading state
    /// </summary>
    public class LightningThreadState
    {
        private const int maxTimeoutWaitMainThread = 30000;

        // needs to be thread safe
        private static readonly BlockingCollection<AutoResetEvent> autoResetEventPool = new BlockingCollection<AutoResetEvent>();

        internal readonly int mainThreadId;
        internal readonly bool multiThreaded;

#if TASK_AVAILABLE

        private Task lightningThread;

#else

        /// <summary>
        /// Lightning thread
        /// </summary>
        private Thread lightningThread;

#endif

        /// <summary>
        /// List of background actions
        /// </summary>
        private readonly BlockingCollection<System.Action> actionsForBackgroundThread = new BlockingCollection<System.Action>(new ConcurrentQueue<System.Action>());

        /// <summary>
        /// List of main thread actions and optional events to signal
        /// </summary>
        private readonly BlockingCollection<(System.Action<bool> action, AutoResetEvent evt)> actionsForMainThread = new BlockingCollection<(System.Action<bool>, AutoResetEvent)>(new ConcurrentQueue<(System.Action<bool>, AutoResetEvent)>());

        /// <summary>
        /// Set to false to terminate
        /// </summary>
        public bool Running = true;

        private bool isTerminating;

        private bool UpdateMainThreadActionsOnce(bool inDestroy)
        {
            if (!actionsForMainThread.TryTake(out (System.Action<bool> action, AutoResetEvent evt) item))
            {
                return false;
            }
            try
            {
                item.action(inDestroy);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error in main thread lightning action: " + ex);
            }
            if (item.evt != null)
            {
                item.evt.Set();
                autoResetEventPool.Add(item.evt);
            }
            return true;

        }

        private void BackgroundThreadMethod()
        {
            while (Running)
            {
                if (actionsForBackgroundThread.TryTake(out Action action, 500))
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        actionsForMainThread.Add((inDestroy =>
                        {
                            Debug.LogError("Lightning background thread exception: " + ex);
                        }, null));
                    }
                }
            }
        }

        /// <summary>
        /// Constructor - starts the thread
        /// </summary>
        /// <param name="multiThreaded">Multi-threaded?</param>
        public LightningThreadState(bool multiThreaded)
        {
            this.mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            this.multiThreaded = multiThreaded;

#if TASK_AVAILABLE

            lightningThread = Task.Factory.StartNew(BackgroundThreadMethod);

#else

            lightningThread = new Thread(new ThreadStart(BackgroundThreadMethod))
            {
                IsBackground = true,
                Name = "LightningBoltScriptThread"
            };
            lightningThread.Start();

#endif

        }

        /// <summary>
        /// Terminate and wait for thread end
        /// </summary>
        /// <param name="inDestroy">True if in destroy, false otherwise</param>
        public void TerminateAndWaitForEnd(bool inDestroy)
        {
            DateTime dt = DateTime.UtcNow;
            TimeSpan timeout = TimeSpan.FromSeconds(5.0);
            isTerminating = true;
            while (UpdateMainThreadActionsOnce(inDestroy) || actionsForBackgroundThread.Count > 0)
            {
                Thread.Sleep(20);
                if (DateTime.UtcNow - dt > timeout)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Execute any main thread actions from the main thread
        /// </summary>
        public void UpdateMainThreadActions()
        {
            if (multiThreaded)
            {
                while (UpdateMainThreadActionsOnce(false)) { }
            }
        }

        /// <summary>
        /// Add a main thread action
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="waitForAction">True to wait for completion, false if not</param>
        /// <returns>True if action added, false if in process of terminating the thread</returns>
        public bool AddActionForMainThread(System.Action<bool> action, bool waitForAction = false)
        {
            if (isTerminating)
            {
                return false;
            }
            else if (System.Threading.Thread.CurrentThread.ManagedThreadId == mainThreadId ||
                !multiThreaded)
            {
                action(true);
                return true;
            }
            if (waitForAction)
            {
                if (!autoResetEventPool.TryTake(out AutoResetEvent evt))
                {
                    evt = new AutoResetEvent(false);
                }
                actionsForMainThread.Add((action, evt));
                evt.WaitOne(maxTimeoutWaitMainThread);
            }
            else
            {
                actionsForMainThread.Add((action, null));
            }
            return true;
        }

        /// <summary>
        /// Add a background thread action
        /// </summary>
        /// <param name="action">Action</param>
        /// <returns>True if action added, false if in process of terminating the thread</returns>
        public bool AddActionForBackgroundThread(System.Action action)
        {
            if (isTerminating)
            {
                return false;
            }
            else if (!multiThreaded)
            {
                action();
            }
            else
            {
                actionsForBackgroundThread.Add(action);
            }
            return true;
        }
    }

#endif

}