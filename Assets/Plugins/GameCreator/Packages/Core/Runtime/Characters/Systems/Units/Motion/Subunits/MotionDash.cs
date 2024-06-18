using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class MotionDash
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private int m_InSuccession;
        [SerializeField] private bool m_DashInAir;
        [SerializeField] private float m_Cooldown;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public int InSuccession
        {
            get => this.m_InSuccession;
            set => this.m_InSuccession = value;
        }
        
        public float Cooldown
        {
            get => this.m_Cooldown;
            set => this.m_Cooldown = value;
        }
        
        
        public bool DashInAir
        {
            get => this.m_DashInAir;
            set => this.m_DashInAir = value;
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public MotionDash()
        {
            this.m_InSuccession = 0;
            this.m_DashInAir = false;
            this.m_Cooldown = 1f;
        }
    }
}
