//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections.Generic;

using UnityEngine;

namespace DigitalRuby.ThunderAndLightning
{
    /// <summary>
    /// Collision handler interface
    /// </summary>
    public interface ICollisionHandler
    {
        /// <summary>
        /// Handle collision
        /// </summary>
        /// <param name="obj">Game object</param>
        /// <param name="collision">Collision</param>
        /// <param name="collisionCount">Collision count</param>
        void HandleCollision(GameObject obj, List<ParticleCollisionEvent> collision, int collisionCount);
    }

    /// <summary>
    /// This script simply allows forwarding collision events for the objects that collide with something. This
    /// allows you to have a generic collision handler and attach a collision forwarder to your child objects.
    /// In addition, you also get access to the game object that is colliding, along with the object being
    /// collided into, which is helpful.
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class LightningParticleCollisionForwarder : MonoBehaviour
    {
        /// <summary>The script to forward the collision to. Must implement ICollisionHandler.</summary>
        [Tooltip("The script to forward the collision to. Must implement ICollisionHandler.")]
        public MonoBehaviour CollisionHandler;

        private ParticleSystem _particleSystem;
        private readonly List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void OnParticleCollision(GameObject other)
        {
            ICollisionHandler i = CollisionHandler as ICollisionHandler;
            if (i != null)
            {
                int numCollisionEvents = _particleSystem.GetCollisionEvents(other, collisionEvents);
                if (numCollisionEvents > 0)
                {
                    i.HandleCollision(other, collisionEvents, numCollisionEvents);
                }
            }
        }

        /*
        public void OnCollisionEnter(Collision col)
        {
            ICollisionHandler i = CollisionHandler as ICollisionHandler;
            if (i != null)
            {
                i.HandleCollision(gameObject, col);
            }
        }
        */
    }
}