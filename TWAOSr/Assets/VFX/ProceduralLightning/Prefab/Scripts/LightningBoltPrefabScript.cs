//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

#define SHOW_MANUAL_WARNING

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Lightning bolt prefab base script
    /// </summary>
    public abstract class LightningBoltPrefabScriptBase : LightningBoltScript
    {

#if DEBUG && SHOW_MANUAL_WARNING

        private static bool showedManualWarning;

#endif

        private readonly List<LightningBoltParameters> batchParameters = new List<LightningBoltParameters>();
        private readonly System.Random random = new System.Random();

        /// <summary>Reduces the probability that additional bolts from CountRange will actually happen (0 - 1).</summary>
        [Header("Lightning Spawn Properties")]
        [SingleLineClamp("How long to wait before creating another round of lightning bolts in seconds", 0.001, double.MaxValue)]
        public RangeOfFloats IntervalRange = new RangeOfFloats { Minimum = 0.05f, Maximum = 0.1f };

        /// <summary>How many lightning bolts to emit for each interval</summary>
        [SingleLineClamp("How many lightning bolts to emit for each interval", 0.0, 100.0)]
        public RangeOfIntegers CountRange = new RangeOfIntegers { Minimum = 1, Maximum = 1 };

        /// <summary>Reduces the probability that additional bolts from CountRange will actually happen (0 - 1).</summary>
        [Tooltip("Reduces the probability that additional bolts from CountRange will actually happen (0 - 1).")]
        [Range(0.0f, 1.0f)]
        public float CountProbabilityModifier = 1.0f;

        /// <summary>Delay in seconds (range) before each additional lightning bolt in count range is emitted</summary>
        public RangeOfFloats DelayRange = new RangeOfFloats { Minimum = 0.0f, Maximum = 0.0f };

        /// <summary>For each bolt emitted, how long should it stay in seconds</summary>
        [SingleLineClamp("For each bolt emitted, how long should it stay in seconds", 0.01, 10.0)]
        public RangeOfFloats DurationRange = new RangeOfFloats { Minimum = 0.06f, Maximum = 0.12f };

        /// <summary>How long (in seconds) this game object should live before destroying itself. Leave as 0 for infinite.</summary>
        [Header("Lightning Appearance Properties")]
        [SingleLineClamp("The trunk width range in unity units (x = min, y = max)", 0.0001, 100.0)]
        public RangeOfFloats TrunkWidthRange = new RangeOfFloats { Minimum = 0.1f, Maximum = 0.2f };

        /// <summary>How long (in seconds) this game object should live before destroying itself. Leave as 0 for infinite.</summary>
        [Tooltip("How long (in seconds) this game object should live before destroying itself. Leave as 0 for infinite.")]
        [Range(0.0f, 1000.0f)]
        public float LifeTime = 0.0f;

        /// <summary>Generations (1 - 8, higher makes more detailed but more expensive lightning)</summary>
        [Tooltip("Generations (1 - 8, higher makes more detailed but more expensive lightning)")]
        [Range(1, 8)]
        public int Generations = 6;

        /// <summary>The chaos factor that determines how far the lightning main trunk can spread out, higher numbers spread out more. 0 - 1.</summary>
        [Tooltip("The chaos factor that determines how far the lightning main trunk can spread out, higher numbers spread out more. 0 - 1.")]
        [Range(0.0f, 1.0f)]
        public float ChaosFactor = 0.075f;

        /// <summary>The chaos factor that determines how far the forks of the lightning can spread out, higher numbers spread out more. 0 - 1.</summary>
        [Tooltip("The chaos factor that determines how far the forks of the lightning can spread out, higher numbers spread out more. 0 - 1.")]
        [Range(0.0f, 1.0f)]
        public float ChaosFactorForks = 0.095f;

        /// <summary>Intensity of the lightning</summary>
        [Tooltip("Intensity of the lightning")]
        [Range(0.0f, 10.0f)]
        public float Intensity = 1.0f;

        /// <summary>The intensity of the glow</summary>
        [Tooltip("The intensity of the glow")]
        [Range(0.0f, 10.0f)]
        public float GlowIntensity = 0.1f;

        /// <summary>The width multiplier for the glow, 0 - 64</summary>
        [Tooltip("The width multiplier for the glow, 0 - 64")]
        [Range(0.0f, 64.0f)]
        public float GlowWidthMultiplier = 4.0f;

        /// <summary>What percent of time the lightning should fade in and out. For example, 0.15 fades in 15% of the time and fades out 15% of the time, with full visibility 70% of the time.</summary>
        [Tooltip("What percent of time the lightning should fade in and out. For example, 0.15 fades in 15% of the time and fades out 15% of the time, with full visibility 70% of the time.")]
        [Range(0.0f, 0.5f)]
        public float FadePercent = 0.15f;

        /// <summary>Modify the duration of lightning fade in.</summary>
        [Tooltip("Modify the duration of lightning fade in.")]
        [Range(0.0f, 1.0f)]
        public float FadeInMultiplier = 1.0f;

        /// <summary>Modify the duration of fully lit lightning.</summary>
        [Tooltip("Modify the duration of fully lit lightning.")]
        [Range(0.0f, 1.0f)]
        public float FadeFullyLitMultiplier = 1.0f;

        /// <summary>Modify the duration of lightning fade out.</summary>
        [Tooltip("Modify the duration of lightning fade out.")]
        [Range(0.0f, 1.0f)]
        public float FadeOutMultiplier = 1.0f;

        /// <summary>0 - 1, how slowly the lightning should grow. 0 for instant, 1 for slow.</summary>
        [Tooltip("0 - 1, how slowly the lightning should grow. 0 for instant, 1 for slow.")]
        [Range(0.0f, 1.0f)]
        public float GrowthMultiplier;

        /// <summary>How much smaller the lightning should get as it goes towards the end of the bolt. For example, 0.5 will make the end 50% the width of the start.</summary>
        [Tooltip("How much smaller the lightning should get as it goes towards the end of the bolt. For example, 0.5 will make the end 50% the width of the start.")]
        [Range(0.0f, 10.0f)]
        public float EndWidthMultiplier = 0.5f;

        /// <summary>How forked should the lightning be? (0 - 1, 0 for none, 1 for lots of forks)</summary>
        [Tooltip("How forked should the lightning be? (0 - 1, 0 for none, 1 for lots of forks)")]
        [Range(0.0f, 1.0f)]
        public float Forkedness = 0.25f;

        /// <summary>Minimum distance multiplier for forks</summary>
        [Range(0.0f, 10.0f)]
        [Tooltip("Minimum distance multiplier for forks")]
        public float ForkLengthMultiplier = 0.6f;

        /// <summary>Fork distance multiplier variance. Random range of 0 to n that is added to Fork Length Multiplier.</summary>
        [Range(0.0f, 10.0f)]
        [Tooltip("Fork distance multiplier variance. Random range of 0 to n that is added to Fork Length Multiplier.")]
        public float ForkLengthVariance = 0.2f;

        /// <summary>Forks have their EndWidthMultiplier multiplied by this value</summary>
        [Tooltip("Forks have their EndWidthMultiplier multiplied by this value")]
        [Range(0.0f, 10.0f)]
        public float ForkEndWidthMultiplier = 1.0f;

        /// <summary>Light parameters</summary>
        [Header("Lightning Light Properties")]
        [Tooltip("Light parameters")]
        public LightningLightParameters LightParameters;

        /// <summary>Maximum number of lights that can be created per batch of lightning</summary>
        [Tooltip("Maximum number of lights that can be created per batch of lightning")]
        [Range(0, 64)]
        public int MaximumLightsPerBatch = 8;

        /// <summary>Manual or automatic mode. Manual requires that you call the Trigger method in script. Automatic uses the interval to create lightning continuously.</summary>
        [Header("Lightning Trigger Type")]
        [Tooltip("Manual or automatic mode. Manual requires that you call the Trigger method in script. Automatic uses the interval to create lightning continuously.")]
        public bool ManualMode;

        /// <summary>Turns lightning into automatic mode for this number of seconds, then puts it into manual mode.</summary>
        [Tooltip("Turns lightning into automatic mode for this number of seconds, then puts it into manual mode.")]
        [Range(0.0f, 120.0f)]
        public float AutomaticModeSeconds;

        /// <summary>Custom handler to modify the transform of each lightning bolt, useful if it will be alive longer than a few frames and needs to scale and rotate based on the position of other objects.</summary>
        [Header("Lightning custom transform handler")]
        [Tooltip("Custom handler to modify the transform of each lightning bolt, useful if it will be alive longer than a few frames and needs to scale and rotate based " +
            "on the position of other objects.")]
        public LightningCustomTransformDelegate CustomTransformHandler;

        /// <summary>
        /// Override the random generator for the bolts
        /// </summary>
        public System.Random RandomOverride { get; set; }

        private float nextLightningTimestamp;
        private float lifeTimeRemaining;

        private void CalculateNextLightningTimestamp(float offset)
        {
            nextLightningTimestamp = (IntervalRange.Minimum == IntervalRange.Maximum ? IntervalRange.Minimum : offset + IntervalRange.Random());
        }

        private void CustomTransform(LightningCustomTransformStateInfo state)
        {
            if (CustomTransformHandler != null)
            {
                CustomTransformHandler.Invoke(state);
            }
        }

        private void CallLightning()
        {
            CallLightning(null, null);
        }

        private void CallLightning(Vector3? start, Vector3? end)
        {
            System.Random r = (RandomOverride ?? random);
            int count = CountRange.Random(r);
            for (int i = 0; i < count; i++)
            {
                LightningBoltParameters p = CreateParameters();
                if (CountProbabilityModifier >= 0.9999f || i == 0 || (float)p.Random.NextDouble() <= CountProbabilityModifier)
                {
                    p.CustomTransform = (CustomTransformHandler == null ? (System.Action<LightningCustomTransformStateInfo>)null : CustomTransform);
                    CreateLightningBolt(p);
                    if (start != null)
                    {
                        p.Start = start.Value;
                    }
                    if (end != null)
                    {
                        p.End = end.Value;
                    }
                }
                else
                {
                    LightningBoltParameters.ReturnParametersToCache(p);
                }
            }
            CreateLightningBoltsNow();
        }

        /// <summary>
        /// Create lightning bolts immediately
        /// </summary>
        protected void CreateLightningBoltsNow()
        {
            int tmp = LightningBolt.MaximumLightsPerBatch;
            LightningBolt.MaximumLightsPerBatch = MaximumLightsPerBatch;
            CreateLightningBolts(batchParameters);
            LightningBolt.MaximumLightsPerBatch = tmp;
            batchParameters.Clear();
        }

        /// <summary>
        /// Populate lightning bolt parameters from script
        /// </summary>
        /// <param name="parameters">Parameters to populate</param>
        protected override void PopulateParameters(LightningBoltParameters parameters)
        {
            base.PopulateParameters(parameters);

            parameters.RandomOverride = RandomOverride;
            float duration = DurationRange.Random(parameters.Random);
            float trunkWidth = TrunkWidthRange.Random(parameters.Random);

            parameters.Generations = Generations;
            parameters.LifeTime = duration;
            parameters.ChaosFactor = ChaosFactor;
            parameters.ChaosFactorForks = ChaosFactorForks;
            parameters.TrunkWidth = trunkWidth;
            parameters.Intensity = Intensity;
            parameters.GlowIntensity = GlowIntensity;
            parameters.GlowWidthMultiplier = GlowWidthMultiplier;
            parameters.Forkedness = Forkedness;
            parameters.ForkLengthMultiplier = ForkLengthMultiplier;
            parameters.ForkLengthVariance = ForkLengthVariance;
            parameters.FadePercent = FadePercent;
            parameters.FadeInMultiplier = FadeInMultiplier;
            parameters.FadeOutMultiplier = FadeOutMultiplier;
            parameters.FadeFullyLitMultiplier = FadeFullyLitMultiplier;
            parameters.GrowthMultiplier = GrowthMultiplier;
            parameters.EndWidthMultiplier = EndWidthMultiplier;
            parameters.ForkEndWidthMultiplier = ForkEndWidthMultiplier;
            parameters.DelayRange = DelayRange;
            parameters.LightParameters = LightParameters;
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            base.Start();
            CalculateNextLightningTimestamp(0.0f);
            lifeTimeRemaining = (LifeTime <= 0.0f ? float.MaxValue : LifeTime);
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (Time.timeScale <= 0.0f)
            {
                return;
            }
            else if ((lifeTimeRemaining -= LightningBoltScript.DeltaTime) < 0.0f)
            {
                GameObject.Destroy(gameObject);
            }
            if ((nextLightningTimestamp -= LightningBoltScript.DeltaTime) <= 0.0f)
            {
                CalculateNextLightningTimestamp(nextLightningTimestamp);
                if (ManualMode)
                {

#if DEBUG && SHOW_MANUAL_WARNING

                    if (!showedManualWarning)
                    {
                        showedManualWarning = true;
                        Debug.LogWarning("Lightning bolt script is in manual mode. Trigger method must be called.");
                    }

#endif

                }
                else
                {
                    CallLightning();
                }
            }

            if (AutomaticModeSeconds > 0.0f)
            {
                AutomaticModeSeconds = Mathf.Max(0.0f, AutomaticModeSeconds - LightningBoltScript.DeltaTime);
                ManualMode = (AutomaticModeSeconds == 0.0f);
            }
        }

        /// <summary>
        /// OnDrawGizmos
        /// </summary>
        protected virtual void OnDrawGizmos()
        {

#if UNITY_EDITOR

            if (!HideGizmos)
            {
                Gizmos.color = Color.white;
                UnityEditor.Handles.color = Color.white;
            }

#endif

        }

        /// <summary>
        /// Derived classes can override and can call this base class method last to add the lightning bolt parameters to the list of batched lightning bolts
        /// </summary>
        /// <param name="p">Lightning bolt creation parameters</param>
        public override void CreateLightningBolt(LightningBoltParameters p)
        {
            batchParameters.Add(p);
            // do not call the base method, we batch up and use CreateLightningBolts
        }

        /// <summary>
        /// Manually trigger the lightning once
        /// </summary>
        public void Trigger()
        {
            Trigger(-1.0f);
        }

        /// <summary>
        /// Manually trigger lightning
        /// </summary>
        /// <param name="seconds">Number of seconds to turn on automatic lightning for (sets AutomaticModeSeconds).</param>
        public void Trigger(float seconds)
        {
            CallLightning();
            if (seconds >= 0.0f)
            {
                AutomaticModeSeconds = Mathf.Max(0.0f, seconds);
            }
        }

        /// <summary>
        /// Manually trigger lightning
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        public void Trigger(Vector3? start, Vector3? end)
        {
            CallLightning(start, end);
        }
    }

    /// <summary>
    /// Lightning bolt prefab script, base script to create lightning using a prefab
    /// </summary>
    public class LightningBoltPrefabScript : LightningBoltPrefabScriptBase
    {
        /// <summary>The source game object, can be null</summary>
        [Header("Start/end")]
        [Tooltip("The source game object, can be null")]
        public GameObject Source;

        /// <summary>The destination game object, can be null</summary>
        [Tooltip("The destination game object, can be null")]
        public GameObject Destination;

        /// <summary>X, Y and Z for variance from the start point. Use positive values.</summary>
        [Tooltip("X, Y and Z for variance from the start point. Use positive values.")]
        public Vector3 StartVariance;

        /// <summary>X, Y and Z for variance from the end point. Use positive values.</summary>
        [Tooltip("X, Y and Z for variance from the end point. Use positive values.")]
        public Vector3 EndVariance;

#if UNITY_EDITOR

        /// <summary>
        /// OnDrawGizmos
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (HideGizmos)
            {
                return;
            }
            else if (Source != null)
            {
                Gizmos.DrawIcon(Source.transform.position, "LightningPathStart.png");
            }
            if (Destination != null)
            {
                Gizmos.DrawIcon(Destination.transform.position, "LightningPathNext.png");
            }
            if (Source != null && Destination != null)
            {
                Gizmos.DrawLine(Source.transform.position, Destination.transform.position);
                Vector3 direction = (Destination.transform.position - Source.transform.position);
                Vector3 center = (Source.transform.position + Destination.transform.position) * 0.5f;
                float arrowSize = Mathf.Min(2.0f, direction.magnitude) * 2.0f;
                UnityEditor.Handles.ArrowHandleCap(0, center, Quaternion.LookRotation(direction), arrowSize, EventType.Repaint);
            }
        }

#endif

        /// <summary>
        /// Create a lightning bolt
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            parameters.Start = (Source == null ? parameters.Start : Source.transform.position);
            parameters.End = (Destination == null ? parameters.End : Destination.transform.position);
            parameters.StartVariance = StartVariance;
            parameters.EndVariance = EndVariance;

            base.CreateLightningBolt(parameters);
        }
    }
}

