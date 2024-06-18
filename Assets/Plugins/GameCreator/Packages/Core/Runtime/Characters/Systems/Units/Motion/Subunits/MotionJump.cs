using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class MotionJump
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private bool m_CanJump;
        [SerializeField] private int m_AirJumps;
        [SerializeField] private float m_JumpForce;
        [SerializeField] private float m_JumpCooldown;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool CanJump
        {
            get => this.m_CanJump;
            set => this.m_CanJump = value;
        }
        
        public int AirJumps
        {
            get => this.m_AirJumps;
            set => this.m_AirJumps = value;
        }
        
        public float JumpForce
        {
            get => this.m_JumpForce;
            set => this.m_JumpForce = value;
        }
        
        public float JumpCooldown
        {
            get => this.m_JumpCooldown;
            set => this.m_JumpCooldown = value;
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public MotionJump()
        {
            this.m_CanJump = true;
            this.m_AirJumps = 0;
            
            this.m_JumpForce = 5f;
            this.m_JumpCooldown = 0.5f;
        }
    }
}
