using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class TUnitDriver : TUnit, IUnitDriver
    {
        protected const float COYOTE_TIME = 0.3f;
        protected const int COYOTE_FRAMES = 2;

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Dictionary<int, float> m_GravityInfluence;

        // INTERFACE PROPERTIES: ------------------------------------------------------------------

        public abstract Vector3 WorldMoveDirection { get; }
        public abstract Vector3 LocalMoveDirection { get; }

        public abstract float SkinWidth { get; }
        public abstract bool IsGrounded { get; }
        public abstract Vector3 FloorNormal { get; }

        public float GravityInfluence
        {
            get
            {
                if (this.m_GravityInfluence.Count == 0) return 1f;
                
                float minimum = 1f;
                foreach (KeyValuePair<int, float> entry in this.m_GravityInfluence)
                {
                    if (minimum < entry.Value) continue;
                    minimum = entry.Value;
                }

                return minimum;
            }
        }

        public abstract bool Collision { get; set; }
        
        public abstract Axonometry Axonometry { get; set; }

        // INITIALIZERS: --------------------------------------------------------------------------

        public virtual void OnStartup(Character character)
        {
            this.Character = character;
            this.m_GravityInfluence = new Dictionary<int, float>();
        }

        public virtual void AfterStartup(Character character)
        {
            this.Character = character;
        }

        public virtual void OnDispose(Character character)
        {
            this.Character = character;
        }

        public virtual void OnEnable()
        { }

        public virtual void OnDisable()
        { }

        // METHODS: -------------------------------------------------------------------------------

        public abstract void SetPosition(Vector3 position);
        public abstract void SetRotation(Quaternion rotation);
        public abstract void SetScale(Vector3 scale);

        public abstract void AddPosition(Vector3 amount);
        public abstract void AddRotation(Quaternion amount);
        public abstract void AddScale(Vector3 scale);

        public virtual void OnUpdate()
        { }
        
        public virtual void OnFixedUpdate()
        { }

        public virtual void OnDrawGizmos(Character character)
        { }
        
        // GRAVITY METHODS: -----------------------------------------------------------------------

        public abstract void ResetVerticalVelocity();

        public void SetGravityInfluence(int key, float influence)
        {
            this.m_GravityInfluence[key] = influence;
        }

        public void RemoveGravityInfluence(int key)
        {
            this.m_GravityInfluence.Remove(key);
        }
    }
}