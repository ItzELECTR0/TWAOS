//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Lightning field script, creates lightning in a cube
    /// </summary>
    public class LightningFieldScript : LightningBoltPrefabScriptBase
    {
        /// <summary>The minimum length for a field segment</summary>
        [Header("Lightning Field Properties")]
        [Tooltip("The minimum length for a field segment")]
        public float MinimumLength = 0.01f;
        private float minimumLengthSquared;

        /// <summary>The bounds to put the field in.</summary>
        [Tooltip("The bounds to put the field in.")]
        public Bounds FieldBounds;

        /// <summary>Optional light for the lightning field to emit</summary>
        [Tooltip("Optional light for the lightning field to emit")]
        public Light Light;

        private Vector3 RandomPointInBounds()
        {
            float x = UnityEngine.Random.Range(FieldBounds.min.x, FieldBounds.max.x);
            float y = UnityEngine.Random.Range(FieldBounds.min.y, FieldBounds.max.y);
            float z = UnityEngine.Random.Range(FieldBounds.min.z, FieldBounds.max.z);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (Light != null)
            {
                Light.enabled = false;
            }
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
            else if (Light != null)
            {
                Light.transform.position = FieldBounds.center;
                Light.intensity = UnityEngine.Random.Range(2.8f, 3.2f);
            }
        }

        /// <summary>
        /// Create a lightning bolt
        /// </summary>
        /// <param name="parameters">Parameters</param>
        public override void CreateLightningBolt(LightningBoltParameters parameters)
        {
            minimumLengthSquared = MinimumLength * MinimumLength;

            for (int i = 0; i < 16; i++)
            {
                // get two random points in the bounds
                parameters.Start = RandomPointInBounds();
                parameters.End = RandomPointInBounds();
                if ((parameters.End - parameters.Start).sqrMagnitude >= minimumLengthSquared)
                {
                    break;
                }
            }

            if (Light != null)
            {
                Light.enabled = true;
            }

            base.CreateLightningBolt(parameters);
        }
    }
}
