using System;

namespace GameCreator.Runtime.Characters
{
    public abstract class TStance : IStance
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private bool m_IsBlocking;
        [NonSerialized] private float m_BlockStartTime;
        
        [NonSerialized] private float m_Defense;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public abstract int Id { get; }
        public abstract Character Character { get; set; }

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public abstract void OnEnable(Character character);
        public abstract void OnDisable(Character character);
        public abstract void OnUpdate();
    }
}