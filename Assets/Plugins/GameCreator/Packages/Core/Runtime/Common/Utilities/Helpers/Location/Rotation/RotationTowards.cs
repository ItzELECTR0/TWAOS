using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct RotationTowards : IRotation
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private readonly Transform m_Transform;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public RotationTowards(Transform transform)
        {
            this.m_Transform = transform;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool HasRotation(GameObject source) => source != null && this.m_Transform != null;
        
        public Quaternion GetRotation(GameObject source)
        {
            Vector3 direction = this.m_Transform.position - source.transform.position;
            if (source.Get<Character>() != null)
            {
                direction = Vector3.Scale(direction, Vector3Plane.NormalUp);
            }

            return Quaternion.LookRotation(direction);
        }
    }
}