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
    /// Lightning bolt cone shape script, draws lightning in a cone coming from inner radius spreading to outter radius at length
    /// </summary>
    public class LightningBoltShapeConeScript : LightningBoltPrefabScriptBase
    {
        /// <summary>Radius at base of cone where lightning can emit from</summary>
        [Header("Lightning Cone Properties")]
        [Tooltip("Radius at base of cone where lightning can emit from")]
        public float InnerRadius = 0.1f;

        /// <summary>Radius at outer part of the cone where lightning emits to</summary>
        [Tooltip("Radius at outer part of the cone where lightning emits to")]
        public float OuterRadius = 4.0f;

        /// <summary>The length of the cone from the center of the inner and outer circle</summary>
        [Tooltip("The length of the cone from the center of the inner and outer circle")]
        public float Length = 4.0f;

#if UNITY_EDITOR

        /// <summary>
        /// OnDrawGizmos
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, InnerRadius);
            UnityEditor.Handles.DrawWireDisc(transform.position + (transform.forward * Length), transform.forward, OuterRadius);

            UnityEditor.Handles.DrawLine(transform.position + (transform.rotation * Vector3.right * InnerRadius),
                transform.position + (transform.rotation * Vector3.right * OuterRadius) + (transform.forward * Length));

            UnityEditor.Handles.DrawLine(transform.position + (transform.rotation * -Vector3.right * InnerRadius),
                transform.position + (transform.rotation * -Vector3.right * OuterRadius) + (transform.forward * Length));

            UnityEditor.Handles.DrawLine(transform.position + (transform.rotation * Vector3.up * InnerRadius),
                transform.position + (transform.rotation * Vector3.up * OuterRadius) + (transform.forward * Length));

            UnityEditor.Handles.DrawLine(transform.position + (transform.rotation * -Vector3.up * InnerRadius),
                transform.position + (transform.rotation * -Vector3.up * OuterRadius) + (transform.forward * Length));
        }

#endif

        /// <summary>
        /// Create a lightning bolt
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            Vector2 circle1 = UnityEngine.Random.insideUnitCircle * InnerRadius;
            Vector3 start = transform.rotation * new Vector3(circle1.x, circle1.y, 0.0f);
            Vector2 circle2 = UnityEngine.Random.insideUnitCircle * OuterRadius;
            Vector3 end = (transform.rotation * new Vector3(circle2.x, circle2.y, 0.0f)) + (transform.forward * Length);

            parameters.Start = start;
            parameters.End = end;

            base.CreateLightningBolt(parameters);
        }
    }
}