using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    public struct DriverAdditionalTranslation
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private Vector3 m_Value;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Add(Vector3 amount)
        {
            this.m_Value += amount;
        }

        public Vector3 Consume()
        {
            Vector3 value = this.m_Value;
            this.m_Value = Vector3.zero;

            return value;
        }
    }
}