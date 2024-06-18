using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public struct TetherLimit
    {
        [SerializeField] private float m_Limit;
        [SerializeField] private float m_Bounciness;
        [SerializeField] private float m_ContactDistance;

        // PROPERTIES: ----------------------------------------------------------------------------

        public float Limit
        {
            get => this.m_Limit;
            set => this.m_Limit = value;
        }
        
        public float Bounciness
        {
            get => this.m_Bounciness;
            set => this.m_Bounciness = value;
        }

        public float ContactDistance
        {
            get => this.m_ContactDistance;
            set => this.m_ContactDistance = value;
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public TetherLimit(float limit, float bounciness, float contactDistance)
        {
            this.m_Limit = limit;
            this.m_Bounciness = bounciness;
            this.m_ContactDistance = contactDistance;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public SoftJointLimit ToJoint()
        {
            return new SoftJointLimit
            {
                limit = this.Limit,
                bounciness = this.Bounciness,
                contactDistance = this.ContactDistance
            };
        }
    }
}