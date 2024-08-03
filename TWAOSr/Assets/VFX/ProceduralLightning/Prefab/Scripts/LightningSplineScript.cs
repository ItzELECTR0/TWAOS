//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Lightning bolt spline script, helps lightning curve around path points
    /// </summary>
    public class LightningSplineScript : LightningBoltPathScriptBase
    {
        /// <summary>
        /// For performance, cap generations
        /// </summary>
        public const int MaxSplineGenerations = 5;

        /// <summary>The distance hint for each spline segment. Set to &lt;= 0 to use the generations to determine how many spline segments to use. If &gt; 0, it will be divided by Generations before being applied. This value is a guideline and is approximate, and not uniform on the spline.</summary>
        [Header("Lightning Spline Properties")]
        [Tooltip("The distance hint for each spline segment. Set to <= 0 to use the generations to determine how many spline segments to use. " +
            "If > 0, it will be divided by Generations before being applied. This value is a guideline and is approximate, and not uniform on the spline.")]
        public float DistancePerSegmentHint = 0.0f;

        private readonly List<Vector3> prevSourcePoints = new List<Vector3>(new Vector3[] { Vector3.zero });
        private readonly List<Vector3> sourcePoints = new List<Vector3>();
        private List<Vector3> savedSplinePoints = new List<Vector3>();

        private int previousGenerations = -1;
        private float previousDistancePerSegment = -1.0f;

        private bool SourceChanged()
        {
            if (sourcePoints.Count != prevSourcePoints.Count)
            {
                return true;
            }
            for (int i = 0; i < sourcePoints.Count; i++)
            {
                if (sourcePoints[i] != prevSourcePoints[i])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Create a lightning bolt
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            if (LightningPath == null)
            {
                return;
            }

            sourcePoints.Clear();
            try
            {
                foreach (GameObject obj in LightningPath)
                {
                    if (obj != null)
                    {
                        sourcePoints.Add(obj.transform.position);
                    }
                }
            }
            catch (System.NullReferenceException)
            {
                return;
            }

            if (sourcePoints.Count < PathGenerator.MinPointsForSpline)
            {
                Debug.LogError("To create spline lightning, you need a lightning path with at least " + PathGenerator.MinPointsForSpline + " points.");
            }
            else
            {
                Generations = parameters.Generations = Mathf.Clamp(Generations, 1, MaxSplineGenerations);
                parameters.Points.Clear();
                if (previousGenerations != Generations || previousDistancePerSegment != DistancePerSegmentHint || SourceChanged())
                {
                    previousGenerations = Generations;
                    previousDistancePerSegment = DistancePerSegmentHint;
                    PopulateSpline(parameters.Points, sourcePoints, Generations, DistancePerSegmentHint, Camera);
                    prevSourcePoints.Clear();
                    prevSourcePoints.AddRange(sourcePoints);
                    savedSplinePoints.Clear();
                    savedSplinePoints.AddRange(parameters.Points);
                }
                else
                {
                    parameters.Points.AddRange(savedSplinePoints);
                }

                parameters.SmoothingFactor = (parameters.Points.Count - 1) / sourcePoints.Count;

                base.CreateLightningBolt(parameters);
            }
        }

        /// <summary>
        /// Create a new lightning bolt parameters instance or get from cache
        /// </summary>
        /// <returns>LightningBoltParameters</returns>
        protected override LightningBoltParameters OnCreateParameters()
        {
            LightningBoltParameters p = LightningBoltParameters.GetOrCreateParameters();
            p.Generator = LightningGeneratorPath.PathGeneratorInstance;
            return p;
        }

        /// <summary>
        /// Triggers lightning that follows a set of points, rather than the standard lightning bolt that goes between two points.
        /// </summary>
        /// <param name="points">Points to follow</param>
        /// <param name="spline">Whether to spline the lightning through the points or not</param>
        public void Trigger(List<Vector3> points, bool spline)
        {
            if (points.Count < 2)
            {
                return;
            }
            Generations = Mathf.Clamp(Generations, 1, MaxSplineGenerations);
            LightningBoltParameters parameters = CreateParameters();
            parameters.Points.Clear();
            if (spline && points.Count > 3)
            {
                LightningSplineScript.PopulateSpline(parameters.Points, points, Generations, DistancePerSegmentHint, Camera);
                parameters.SmoothingFactor = (parameters.Points.Count - 1) / points.Count;
            }
            else
            {
                parameters.Points.AddRange(points);
                parameters.SmoothingFactor = 1;
            }
            base.CreateLightningBolt(parameters);
            CreateLightningBoltsNow();
        }

        /// <summary>
        /// Populate a list of spline points from source points
        /// </summary>
        /// <param name="splinePoints">List to fill with spline points</param>
        /// <param name="sourcePoints">Source points</param>
        /// <param name="generations">Generations</param>
        /// <param name="distancePerSegmentHit">Distance per segment hint - if non-zero, attempts to maintain this distance between spline points.</param>
        /// <param name="camera">Optional camera</param>
        public static void PopulateSpline(List<Vector3> splinePoints, List<Vector3> sourcePoints, int generations, float distancePerSegmentHit, Camera camera)
        {
            splinePoints.Clear();
            PathGenerator.Is2D = (camera != null && camera.orthographic);
            if (distancePerSegmentHit > 0.0f)
            {
                PathGenerator.CreateSplineWithSegmentDistance(splinePoints, sourcePoints, distancePerSegmentHit / generations, false);
            }
            else
            {
                PathGenerator.CreateSpline(splinePoints, sourcePoints, sourcePoints.Count * generations * generations, false);
            }
        }
    }
}