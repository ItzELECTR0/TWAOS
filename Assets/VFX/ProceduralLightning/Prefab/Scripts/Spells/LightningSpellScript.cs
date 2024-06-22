//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;
using System;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Base script for lightning bolt spells
    /// </summary>
    public abstract class LightningSpellScript : MonoBehaviour
    {
        /// <summary>The start point of the spell. Set this to a muzzle end or hand.</summary>
        [Header("Direction and distance")]
        [Tooltip("The start point of the spell. Set this to a muzzle end or hand.")]
        public GameObject SpellStart;

        /// <summary>The end point of the spell. Set this to an empty game object. This will change depending on things like collisions, randomness, etc. Not all spells need an end object, but create this anyway to be sure.</summary>
        [Tooltip("The end point of the spell. Set this to an empty game object. " +
            "This will change depending on things like collisions, randomness, etc. " +
            "Not all spells need an end object, but create this anyway to be sure.")]
        public GameObject SpellEnd;

        /// <summary>The direction of the spell. Should be normalized. Does not change unless explicitly modified.</summary>
        [HideInInspector]
        [Tooltip("The direction of the spell. Should be normalized. Does not change unless explicitly modified.")]
        public Vector3 Direction;

        /// <summary>The maximum distance of the spell</summary>
        [Tooltip("The maximum distance of the spell")]
        public float MaxDistance = 15.0f;

        /// <summary>Whether the collision is an exploision. If not explosion, collision is directional.</summary>
        [Header("Collision")]
        [Tooltip("Whether the collision is an exploision. If not explosion, collision is directional.")]
        public bool CollisionIsExplosion;

        /// <summary>The radius of the collision explosion</summary>
        [Tooltip("The radius of the collision explosion")]
        public float CollisionRadius = 1.0f;

        /// <summary>The force to explode with when there is a collision</summary>
        [Tooltip("The force to explode with when there is a collision")]
        public float CollisionForce = 50.0f;

        /// <summary>Collision force mode</summary>
        [Tooltip("Collision force mode")]
        public ForceMode CollisionForceMode = ForceMode.Impulse;

        /// <summary>The particle system for collisions. For best effects, this should emit particles in bursts at time 0 and not loop.</summary>
        [Tooltip("The particle system for collisions. For best effects, this should emit particles in bursts at time 0 and not loop.")]
        public ParticleSystem CollisionParticleSystem;

        /// <summary>The layers that the spell should collide with</summary>
        [Tooltip("The layers that the spell should collide with")]
        public LayerMask CollisionMask = -1;

        /// <summary>Collision audio source</summary>
        [Tooltip("Collision audio source")]
        public AudioSource CollisionAudioSource;

        /// <summary>Collision audio clips. One will be chosen at random and played one shot with CollisionAudioSource.</summary>
        [Tooltip("Collision audio clips. One will be chosen at random and played one shot with CollisionAudioSource.")]
        public AudioClip[] CollisionAudioClips;

        /// <summary>Collision sound volume range.</summary>
        [Tooltip("Collision sound volume range.")]
        public RangeOfFloats CollisionVolumeRange = new RangeOfFloats { Minimum = 0.4f, Maximum = 0.6f };

        /// <summary>The duration in seconds that the spell will last. Not all spells support a duration. For one shot spells, this is how long the spell cast / emission light, etc. will last.</summary>
        [Header("Duration and Cooldown")]
        [Tooltip("The duration in seconds that the spell will last. Not all spells support a duration. For one shot spells, this is how long the spell cast / emission light, etc. will last.")]
        public float Duration = 0.0f;

        /// <summary>The cooldown in seconds. Once cast, the spell must wait for the cooldown before being cast again.</summary>
        [Tooltip("The cooldown in seconds. Once cast, the spell must wait for the cooldown before being cast again.")]
        public float Cooldown = 0.0f;

        /// <summary>Emission sound</summary>
        [Header("Emission")]
        [Tooltip("Emission sound")]
        public AudioSource EmissionSound;

        /// <summary>Emission particle system. For best results use world space, turn off looping and play on awake.</summary>
        [Tooltip("Emission particle system. For best results use world space, turn off looping and play on awake.")]
        public ParticleSystem EmissionParticleSystem;

        /// <summary>Light to illuminate when spell is cast</summary>
        [Tooltip("Light to illuminate when spell is cast")]
        public Light EmissionLight;

        private int stopToken;

        private IEnumerator StopAfterSecondsCoRoutine(float seconds)
        {
            int token = stopToken;

            yield return WaitForSecondsLightning.WaitForSecondsLightningPooled(seconds);

            if (token == stopToken)
            {
                StopSpell();
            }
        }

        /// <summary>
        /// Duration, in seconds, remaining for the spell
        /// </summary>
        protected float DurationTimer { get; private set; }

        /// <summary>
        /// Cooldown, in seconds, remaining before spell can be cast again
        /// </summary>
        protected float CooldownTimer { get; private set; }

        /// <summary>
        /// Apply collision force at a point
        /// </summary>
        /// <param name="point">Point to apply force at</param>
        protected void ApplyCollisionForce(Vector3 point)
        {
            // apply collision force if needed
            if (CollisionForce > 0.0f && CollisionRadius > 0.0f)
            {
                Collider[] colliders = Physics.OverlapSphere(point, CollisionRadius, CollisionMask);
                foreach (Collider c in colliders)
                {
                    Rigidbody r = c.GetComponent<Rigidbody>();
                    if (r != null)
                    {
                        if (CollisionIsExplosion)
                        {
                            r.AddExplosionForce(CollisionForce, point, CollisionRadius, CollisionForce * 0.02f, CollisionForceMode);
                        }
                        else
                        {
                            r.AddForce(CollisionForce * Direction, CollisionForceMode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Play a collision sound
        /// </summary>
        /// <param name="pos">Location of the sound</param>
        protected void PlayCollisionSound(Vector3 pos)
        {
            if (CollisionAudioSource != null && CollisionAudioClips != null && CollisionAudioClips.Length > 0)
            {
                int index = UnityEngine.Random.Range(0, CollisionAudioClips.Length - 1);
                float volume = UnityEngine.Random.Range(CollisionVolumeRange.Minimum, CollisionVolumeRange.Maximum);
                CollisionAudioSource.transform.position = pos;
                CollisionAudioSource.PlayOneShot(CollisionAudioClips[index], volume);
            }
        }

        /// <summary>
        /// Start. Derived classes should call base class method first.
        /// </summary>
        protected virtual void Start()
        {
            if (EmissionLight != null)
            {
                EmissionLight.enabled = false;
            }
        }

        /// <summary>
        /// Update. Derived classes should call base class method first.
        /// </summary>
        protected virtual void Update()
        {
            CooldownTimer = Mathf.Max(0.0f, CooldownTimer - LightningBoltScript.DeltaTime);
            DurationTimer = Mathf.Max(0.0f, DurationTimer - LightningBoltScript.DeltaTime);
        }

        /// <summary>
        /// Late Update.
        /// </summary>
        protected virtual void LateUpdate()
        {
        }

        /// <summary>
        /// On destroy - derived classes should call base class method first
        /// </summary>
        protected virtual void OnDestroy() { }

        /// <summary>
        /// Start the spell
        /// </summary>
        protected abstract void OnCastSpell();

        /// <summary>
        /// Stop the spell
        /// </summary>
        protected abstract void OnStopSpell();

        /// <summary>
        /// On activated
        /// </summary>
        protected virtual void OnActivated() { }

        /// <summary>
        /// On deactivated
        /// </summary>
        protected virtual void OnDeactivated() { }

        /// <summary>
        /// Cast the spell
        /// </summary>
        /// <returns>True if was able to cast, false if not (i.e. cooldown not met yet)</returns>
        public bool CastSpell()
        {
            if (!CanCastSpell)
            {
                return false;
            }
            Casting = true;
            DurationTimer = Duration;
            CooldownTimer = Cooldown;
            OnCastSpell();
            if (Duration > 0.0f)
            {
                StopAfterSeconds(Duration);
            }
            if (EmissionParticleSystem != null)
            {
                EmissionParticleSystem.Play();
            }
            if (EmissionLight != null)
            {
                EmissionLight.transform.position = SpellStart.transform.position;
                EmissionLight.enabled = true;
            }
            if (EmissionSound != null)
            {
                EmissionSound.Play();
            }
            return true;
        }

        /// <summary>
        /// Stop casting a spell. Some spells are single shot and this method does nothing. Spells
        /// that are continouous for example would stop with this method call.
        /// </summary>
        public void StopSpell()
        {
            if (Casting)
            {
                stopToken++;
                if (EmissionParticleSystem != null)
                {
                    EmissionParticleSystem.Stop();
                }
                if (EmissionLight != null)
                {
                    EmissionLight.enabled = false;
                }
                if (EmissionSound != null && EmissionSound.loop)
                {
                    EmissionSound.Stop();
                }
                DurationTimer = 0.0f;
                Casting = false;
                OnStopSpell();
            }
        }

        /// <summary>
        /// Equip / ready the spell
        /// </summary>
        public void ActivateSpell()
        {
            OnActivated();
        }

        /// <summary>
        /// Unequip the spell
        /// </summary>
        public void DeactivateSpell()
        {
            OnDeactivated();
        }

        /// <summary>
        /// Stop the spell after a certain amount of seconds. If the spell is stopped before seconds elapses, nothing happens.
        /// </summary>
        /// <param name="seconds">Seconds to wait before stopping</param>
        public void StopAfterSeconds(float seconds)
        {
            StartCoroutine(StopAfterSecondsCoRoutine(seconds));
        }

        /// <summary>
        /// Find a game object searching recursively through all children and grand-children, etc.
        /// </summary>
        /// <param name="t">Transform</param>
        /// <param name="name">Name of object to find</param>
        /// <returns>GameObject or null if not found</returns>
        public static GameObject FindChildRecursively(Transform t, string name)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }

            for (int i = 0; i < t.childCount; i++)
            {
                GameObject obj = FindChildRecursively(t.GetChild(i), name);
                if (obj != null)
                {
                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        /// Is the spell currently being cast?
        /// </summary>
        public bool Casting { get; private set; }

        /// <summary>
        /// Determines whether the spell can be cast or not
        /// </summary>
        public bool CanCastSpell { get { return (!Casting && CooldownTimer <= 0.0f); } }
    }
}
