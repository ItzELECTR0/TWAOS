using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public struct SpringLimit
    {
        [SerializeField] private float m_Spring;
        [SerializeField] private float m_Damper;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float Spring
        {
            get => this.m_Spring;
            set => this.m_Spring = value;
        }

        public float Damper
        {
            get => this.m_Damper;
            set => this.m_Damper = value;
        }
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public SpringLimit(float spring, float damper)
        {
            this.m_Spring = spring;
            this.m_Damper = damper;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public SoftJointLimitSpring ToJoint()
        {
            return new SoftJointLimitSpring
            {
                damper = this.Damper,
                spring = this.Spring
            };
        }
    }
}