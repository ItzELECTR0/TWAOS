using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class MotionAcceleration
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private bool m_UseAcceleration;
        [SerializeField] private float m_Acceleration;
        [SerializeField] private float m_Deceleration;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool UseAcceleration
        {
            get => this.m_UseAcceleration;
            set => this.m_UseAcceleration = value;
        }

        public float Acceleration
        {
            get => this.m_Acceleration;
            set => this.m_Acceleration = value;
        }
        
        public float Deceleration
        {
            get => this.m_Deceleration;
            set => this.m_Deceleration = value;
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public MotionAcceleration()
        {
            this.UseAcceleration = true;
            this.m_Acceleration = 10f;
            this.m_Deceleration = 4f;
        }
    }
}