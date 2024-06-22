//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Lightning generator base class
    /// </summary>
    public class LightningGenerator
    {
        internal const float oneOver255 = 1.0f / 255.0f;
        internal const float mainTrunkMultiplier = 255.0f * oneOver255 * oneOver255;

        private void GetPerpendicularVector(ref Vector3 directionNormalized, out Vector3 side)
        {
            if (directionNormalized == Vector3.zero)
            {
                side = Vector3.right;
            }
            else
            {
                // use cross product to find any perpendicular vector around directionNormalized:
                // 0 = x * px + y * py + z * pz
                // => pz = -(x * px + y * py) / z
                // for computational stability use the component farthest from 0 to divide by
                float x = directionNormalized.x;
                float y = directionNormalized.y;
                float z = directionNormalized.z;
                float px, py, pz;
                float ax = Mathf.Abs(x), ay = Mathf.Abs(y), az = Mathf.Abs(z);
                if (ax >= ay && ay >= az)
                {
                    // x is the max, so we can pick (py, pz) arbitrarily at (1, 1):
                    py = 1.0f;
                    pz = 1.0f;
                    px = -(y * py + z * pz) / x;
                }
                else if (ay >= az)
                {
                    // y is the max, so we can pick (px, pz) arbitrarily at (1, 1):
                    px = 1.0f;
                    pz = 1.0f;
                    py = -(x * px + z * pz) / y;
                }
                else
                {
                    // z is the max, so we can pick (px, py) arbitrarily at (1, 1):
                    px = 1.0f;
                    py = 1.0f;
                    pz = -(x * px + y * py) / z;
                }
                side = new Vector3(px, py, pz).normalized;
            }
        }

        /// <summary>
        /// Fires when a lightning bolt needs to be generated
        /// </summary>
        /// <param name="bolt">Lightning bolt</param>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        /// <param name="parameters">Parameters</param>
        protected virtual void OnGenerateLightningBolt(LightningBolt bolt, Vector3 start, Vector3 end, LightningBoltParameters parameters)
        {
            GenerateLightningBoltStandard(bolt, start, end, parameters.Generations, parameters.Generations, 0.0f, parameters);
        }

        /// <summary>
        /// Determines if a fork should be created
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <param name="generation">Generation</param>
        /// <param name="totalGenerations">Max generation</param>
        /// <returns>True if fork should be created, false otherwise</returns>
        public bool ShouldCreateFork(LightningBoltParameters parameters, int generation, int totalGenerations)
        {
            return (generation > parameters.generationWhereForksStop && generation >= totalGenerations - parameters.forkednessCalculated && (float)parameters.Random.NextDouble() < parameters.Forkedness);
        }

        /// <summary>
        /// Create a fork in a lightning bolt
        /// </summary>
        /// <param name="bolt">Lightning bolt</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="generation">Generation</param>
        /// <param name="totalGenerations">Max generation</param>
        /// <param name="start">Start position</param>
        /// <param name="midPoint">Middle position</param>
        public void CreateFork(LightningBolt bolt, LightningBoltParameters parameters, int generation, int totalGenerations, Vector3 start, Vector3 midPoint)
        {
            if (ShouldCreateFork(parameters, generation, totalGenerations))
            {
                Vector3 branchVector = (midPoint - start) * parameters.ForkMultiplier();
                Vector3 splitEnd = midPoint + branchVector;
                GenerateLightningBoltStandard(bolt, midPoint, splitEnd, generation, totalGenerations, 0.0f, parameters);
            }
        }

        /// <summary>
        /// Generate a normal/standard lightning bolt
        /// </summary>
        /// <param name="bolt">Lightning bolt</param>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        /// <param name="generation">Generation</param>
        /// <param name="totalGenerations">Max generation</param>
        /// <param name="offsetAmount">Offset amount for variance</param>
        /// <param name="parameters">Parameters</param>
        public void GenerateLightningBoltStandard(LightningBolt bolt, Vector3 start, Vector3 end, int generation, int totalGenerations, float offsetAmount, LightningBoltParameters parameters)
        {
            if (generation < 1)
            {
                return;
            }

            LightningBoltSegmentGroup group = bolt.AddGroup();
            group.Segments.Add(new LightningBoltSegment { Start = start, End = end });

            // every generation, get the percentage we have gone down and square it, this makes lines thinner
            float widthMultiplier = (float)generation / (float)totalGenerations;
            widthMultiplier *= widthMultiplier;

            Vector3 randomVector;
            group.LineWidth = parameters.TrunkWidth * widthMultiplier;
            group.Generation = generation;
            group.Color = parameters.Color;
            if (generation == parameters.Generations &&
                (parameters.MainTrunkTintColor.r != 255 || parameters.MainTrunkTintColor.g != 255 || parameters.MainTrunkTintColor.b != 255 || parameters.MainTrunkTintColor.a != 255))
            {
                group.Color.r = (byte)(mainTrunkMultiplier * (float)group.Color.r * (float)parameters.MainTrunkTintColor.r);
                group.Color.g = (byte)(mainTrunkMultiplier * (float)group.Color.g * (float)parameters.MainTrunkTintColor.g);
                group.Color.b = (byte)(mainTrunkMultiplier * (float)group.Color.b * (float)parameters.MainTrunkTintColor.b);
                group.Color.a = (byte)(mainTrunkMultiplier * (float)group.Color.a * (float)parameters.MainTrunkTintColor.a);
            }
            group.Color.a = (byte)(255.0f * widthMultiplier);
            group.EndWidthMultiplier = parameters.EndWidthMultiplier * parameters.ForkEndWidthMultiplier;
            if (offsetAmount <= 0.0f)
            {
                offsetAmount = (end - start).magnitude * (generation == totalGenerations ? parameters.ChaosFactor : parameters.ChaosFactorForks);
            }

            while (generation-- > 0)
            {
                int previousStartIndex = group.StartIndex;
                group.StartIndex = group.Segments.Count;
                for (int i = previousStartIndex; i < group.StartIndex; i++)
                {
                    start = group.Segments[i].Start;
                    end = group.Segments[i].End;

                    // determine a new direction for the split
                    Vector3 midPoint = (start + end) * 0.5f;

                    // adjust the mid point to be the new location
                    RandomVector(bolt, ref start, ref end, offsetAmount, parameters.Random, out randomVector);
                    midPoint += randomVector;

                    // add two new segments
                    group.Segments.Add(new LightningBoltSegment { Start = start, End = midPoint });
                    group.Segments.Add(new LightningBoltSegment { Start = midPoint, End = end });

                    CreateFork(bolt, parameters, generation, totalGenerations, start, midPoint);
                }

                // halve the distance the lightning can deviate for each generation down
                offsetAmount *= 0.5f;
            }
        }

        /// <summary>
        /// Get a random 3D direction
        /// </summary>
        /// <param name="random">Random</param>
        /// <returns>Random 3D direction vector</returns>
        public Vector3 RandomDirection3D(System.Random random)
        {
            float z = (2.0f * (float)random.NextDouble()) - 1.0f; // z is in the range [-1,1]
            Vector3 planar = RandomDirection2D(random) * Mathf.Sqrt(1.0f - (z * z));
            planar.z = z;

            return planar;
        }

        /// <summary>
        /// Get random 2D direction in XY plane
        /// </summary>
        /// <param name="random">Random</param>
        /// <returns>Random 2D direction</returns>
        public Vector3 RandomDirection2D(System.Random random)
        {
            float azimuth = (float)random.NextDouble() * 2.0f * Mathf.PI;
            return new Vector3(Mathf.Cos(azimuth), Mathf.Sin(azimuth), 0.0f);
        }

        /// <summary>
        /// Get random 2D direction in XZ plane
        /// </summary>
        /// <param name="random">Random</param>
        /// <returns>Random 2D direction</returns>
        public Vector3 RandomDirection2DXZ(System.Random random)
        {
            float azimuth = (float)random.NextDouble() * 2.0f * Mathf.PI;
            return new Vector3(Mathf.Cos(azimuth), 0.0f, Mathf.Sin(azimuth));
        }

        /// <summary>
        /// Generate a random vector
        /// </summary>
        /// <param name="bolt">Lightning bolt</param>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        /// <param name="offsetAmount">Offset amount for variance</param>
        /// <param name="random">Random instance</param>
        /// <param name="result">Receives random vector</param>
        public void RandomVector(LightningBolt bolt, ref Vector3 start, ref Vector3 end, float offsetAmount, System.Random random, out Vector3 result)
        {
            if (bolt.CameraMode == CameraMode.Perspective)
            {
                Vector3 direction = (end - start).normalized;
                Vector3 side = Vector3.Cross(start, end);
                if (side == Vector3.zero)
                {
                    // slow path, rarely hit unless cross product is zero
                    GetPerpendicularVector(ref direction, out side);
                }
                else
                {
                    side.Normalize();
                }

                // generate random distance and angle
                float distance = (((float)random.NextDouble() + 0.1f) * offsetAmount);

#if DEBUG

                float rotationAngle = ((float)random.NextDouble() * 360.0f);
                result = Quaternion.AngleAxis(rotationAngle, direction) * side * distance;

#else

                // optimized path for RELEASE mode, skips two normalize and two multiplies in Quaternion.AngleAxis
                float rotationAngle = ((float)random.NextDouble() * Mathf.PI);
                direction *= (float)System.Math.Sin(rotationAngle);
                Quaternion rotation;
                rotation.x = direction.x;
                rotation.y = direction.y;
                rotation.z = direction.z;
                rotation.w =  (float)System.Math.Cos(rotationAngle);
                result = rotation * side * distance;

#endif

            }
            else if (bolt.CameraMode == CameraMode.OrthographicXY)
            {
                // XY plane
                end.z = start.z;
                Vector3 directionNormalized = (end - start).normalized;
                Vector3 side = new Vector3(-directionNormalized.y, directionNormalized.x, 0.0f);
                float distance = ((float)random.NextDouble() * offsetAmount * 2.0f) - offsetAmount;
                result = side * distance;
            }
            else
            {
                // XZ plane
                end.y = start.y;
                Vector3 directionNormalized = (end - start).normalized;
                Vector3 side = new Vector3(-directionNormalized.z, 0.0f, directionNormalized.x);
                float distance = ((float)random.NextDouble() * offsetAmount * 2.0f) - offsetAmount;
                result = side * distance;
            }
        }

        /// <summary>
        /// Generate a lightning bolt
        /// </summary>
        /// <param name="bolt">Lightning bolt</param>
        /// <param name="parameters">Parameters</param>
        public void GenerateLightningBolt(LightningBolt bolt, LightningBoltParameters parameters)
        {
            Vector3 start, end;
            GenerateLightningBolt(bolt, parameters, out start, out end);
        }

        /// <summary>
        /// Generate a lightning bolt
        /// </summary>
        /// <param name="bolt">Lightning bolt</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        public void GenerateLightningBolt(LightningBolt bolt, LightningBoltParameters parameters, out Vector3 start, out Vector3 end)
        {
            start = parameters.ApplyVariance(parameters.Start, parameters.StartVariance);
            end = parameters.ApplyVariance(parameters.End, parameters.EndVariance);
            OnGenerateLightningBolt(bolt, start, end, parameters);
        }

        /// <summary>
        /// Singleton lightning generator instance
        /// </summary>
        public static readonly LightningGenerator GeneratorInstance = new LightningGenerator();
    }

    /// <summary>
    /// Generates lightning that follows a path
    /// </summary>
    public class LightningGeneratorPath : LightningGenerator
    {
        /// <summary>
        /// Singleton path generator
        /// </summary>
        public static readonly LightningGeneratorPath PathGeneratorInstance = new LightningGeneratorPath();

        /// <summary>
        /// Generate lightning bolt path
        /// </summary>
        /// <param name="bolt">Lightning bolt</param>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="parameters">Parameters</param>
        public void GenerateLightningBoltPath(LightningBolt bolt, Vector3 start, Vector3 end, LightningBoltParameters parameters)
        {
            if (parameters.Points.Count < 2)
            {
                Debug.LogError("Lightning path should have at least two points");
                return;
            }

            int generation = parameters.Generations;
            int totalGenerations = generation;
            float offsetAmount, d;
            float chaosFactor = (generation == parameters.Generations ? parameters.ChaosFactor : parameters.ChaosFactorForks);
            int smoothingFactor = parameters.SmoothingFactor - 1;
            Vector3 distance, randomVector;
            LightningBoltSegmentGroup group = bolt.AddGroup();
            group.LineWidth = parameters.TrunkWidth;
            group.Generation = generation--;
            group.EndWidthMultiplier = parameters.EndWidthMultiplier;
            group.Color = parameters.Color;
            if (generation == parameters.Generations &&
                (parameters.MainTrunkTintColor.r != 255 || parameters.MainTrunkTintColor.g != 255 || parameters.MainTrunkTintColor.b != 255 || parameters.MainTrunkTintColor.a != 255))
            {
                group.Color.r = (byte)(mainTrunkMultiplier * (float)group.Color.r * (float)parameters.MainTrunkTintColor.r);
                group.Color.g = (byte)(mainTrunkMultiplier * (float)group.Color.g * (float)parameters.MainTrunkTintColor.g);
                group.Color.b = (byte)(mainTrunkMultiplier * (float)group.Color.b * (float)parameters.MainTrunkTintColor.b);
                group.Color.a = (byte)(mainTrunkMultiplier * (float)group.Color.a * (float)parameters.MainTrunkTintColor.a);
            }

            parameters.Start = parameters.Points[0] + start;
            parameters.End = parameters.Points[parameters.Points.Count - 1] + end;
            end = parameters.Start;

            for (int i = 1; i < parameters.Points.Count; i++)
            {
                start = end;
                end = parameters.Points[i];
                distance = (end - start);
                d = PathGenerator.SquareRoot(distance.sqrMagnitude);
                if (chaosFactor > 0.0f)
                {
                    if (bolt.CameraMode == CameraMode.Perspective)
                    {
                        end += (d * chaosFactor * RandomDirection3D(parameters.Random));
                    }
                    else if (bolt.CameraMode == CameraMode.OrthographicXY)
                    {
                        end += (d * chaosFactor * RandomDirection2D(parameters.Random));
                    }
                    else
                    {
                        end += (d * chaosFactor * RandomDirection2DXZ(parameters.Random));
                    }
                    distance = (end - start);
                }
                group.Segments.Add(new LightningBoltSegment { Start = start, End = end });

                offsetAmount = d * chaosFactor;
                RandomVector(bolt, ref start, ref end, offsetAmount, parameters.Random, out randomVector);

                if (ShouldCreateFork(parameters, generation, totalGenerations))
                {
                    Vector3 branchVector = distance * parameters.ForkMultiplier() * smoothingFactor * 0.5f;
                    Vector3 forkEnd = end + branchVector + randomVector;
                    GenerateLightningBoltStandard(bolt, start, forkEnd, generation, totalGenerations, 0.0f, parameters);
                }

                if (--smoothingFactor == 0)
                {
                    smoothingFactor = parameters.SmoothingFactor - 1;
                }
            }
        }

        /// <summary>
        /// Fires when lightning bolt needs to be generated
        /// </summary>
        /// <param name="bolt">Lightning bolt</param>
        /// <param name="start">Start position</param>
        /// <param name="end">End position</param>
        /// <param name="parameters">Parameters</param>
        protected override void OnGenerateLightningBolt(LightningBolt bolt, Vector3 start, Vector3 end, LightningBoltParameters parameters)
        {
            GenerateLightningBoltPath(bolt, start, end, parameters);
        }
    }
}
