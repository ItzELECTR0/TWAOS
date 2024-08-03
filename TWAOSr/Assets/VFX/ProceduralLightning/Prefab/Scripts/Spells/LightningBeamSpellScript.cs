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
    /// Lightning bolt beam spell script, like a phasor or laser
    /// </summary>
    public class LightningBeamSpellScript : LightningSpellScript
    {
        /// <summary>The lightning path script creating the beam of lightning</summary>
        [Header("Beam")]
        [Tooltip("The lightning path script creating the beam of lightning")]
        public LightningBoltPathScriptBase LightningPathScript;

        /// <summary>Give the end point some randomization</summary>
        [Tooltip("Give the end point some randomization")]
        public float EndPointRandomization = 1.5f;

        /// <summary>
        /// Callback for collision events
        /// </summary>
        [HideInInspector]
        public System.Action<RaycastHit> CollisionCallback;

        private void CheckCollision()
        {
            RaycastHit hit;

            // send out a ray to see what gets hit
            if (Physics.Raycast(SpellStart.transform.position, Direction, out hit, MaxDistance, CollisionMask))
            {
                // we hit something, set the end object position
                SpellEnd.transform.position = hit.point;

                // additional randomization of end point
                SpellEnd.transform.position += (UnityEngine.Random.insideUnitSphere * EndPointRandomization);

                // play collision sound
                PlayCollisionSound(SpellEnd.transform.position);

                // play the collision particle system
                if (CollisionParticleSystem != null)
                {
                    CollisionParticleSystem.transform.position = hit.point;
                    CollisionParticleSystem.Play();
                }

                ApplyCollisionForce(hit.point);

                // notify listeners of collisions
                if (CollisionCallback != null)
                {
                    CollisionCallback(hit);
                }
            }
            else
            {
                // stop collision particle system
                if (CollisionParticleSystem != null)
                {
                    CollisionParticleSystem.Stop();
                }

                // extend beam to max length
                SpellEnd.transform.position = SpellStart.transform.position + (Direction * MaxDistance);

                // randomize end point a bit
                SpellEnd.transform.position += (UnityEngine.Random.insideUnitSphere * EndPointRandomization);
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Start()
        {
            base.Start();

            LightningPathScript.ManualMode = true;
        }

        /// <summary>
        /// Update
        /// </summary>
        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (!Casting)
            {
                return;
            }

            CheckCollision();
        }

        /// <summary>
        /// OnCastSpell
        /// </summary>
        protected override void OnCastSpell()
        {
            LightningPathScript.ManualMode = false;
        }

        /// <summary>
        /// OnStopSpell
        /// </summary>
        protected override void OnStopSpell()
        {
            LightningPathScript.ManualMode = true;
        }
    }
}