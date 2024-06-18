using System;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Block
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private bool m_IsBlocking;

        [NonSerialized] private Character m_Character;
        [NonSerialized] private IShield m_Shield;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool IsBlocking => this.m_IsBlocking;

        public IShield Shield => this.m_Shield;
        
        [field: NonSerialized] public float RaiseStartTime { get; set; }
        [field: NonSerialized] public float BlockHitTime { get; set; }
        
        // INTERNAL METHODS: ----------------------------------------------------------------------

        internal void OnEnable(Character character)
        {
            this.m_Character = character;
            this.m_IsBlocking = false;
            
            this.RaiseStartTime = -9999f;
            this.BlockHitTime = -9999f;
        }

        internal void OnDisable(Character character)
        {
            this.LowerGuard();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void RaiseGuard()
        {
            if (this.m_IsBlocking) return;
            if (this.m_Character.Busy.IsBusy) return;

            this.RaiseStartTime = this.m_Character.Time.Time;
            
            this.m_IsBlocking = true;
            this.m_Shield?.OnRaise(this.m_Character);
        }

        public void LowerGuard()
        {
            if (!this.m_IsBlocking) return;
            
            this.m_IsBlocking = false;
            this.m_Shield?.OnLower(this.m_Character);
        }
        
        public void SetShield(IShield shield)
        {
            this.m_Shield = shield;
            if (this.m_Shield == null) this.LowerGuard();
        }
    }
}