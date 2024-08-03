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
    /// Lightning bolt sphere shape script, creates lightning inside a sphere
    /// </summary>
    public class LightningBoltShapeSphereScript : LightningBoltPrefabScriptBase
    {
        /// <summary>Radius inside the sphere where lightning can emit from</summary>
        [Header("Lightning Sphere Properties")]
        [Tooltip("Radius inside the sphere where lightning can emit from")]
        public float InnerRadius = 0.1f;

        /// <summary>Radius of the sphere</summary>
        [Tooltip("Radius of the sphere")]
        public float Radius = 4.0f;

#if UNITY_EDITOR

        /// <summary>
        /// OnDrawGizmos
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            Gizmos.DrawWireSphere(transform.position, InnerRadius);
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

#endif

        /// <summary>
        /// Create a lightning bolt
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            Vector3 start = UnityEngine.Random.insideUnitSphere * InnerRadius;
            Vector3 end = UnityEngine.Random.onUnitSphere * Radius;

            parameters.Start = start;
            parameters.End = end;

            base.CreateLightningBolt(parameters);
        }
    }
}