using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class UnitPlayer
    {
        [SerializeReference] private TUnitPlayer m_Player = new UnitPlayerDirectional();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public TUnitPlayer Wrapper => this.m_Player;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public UnitPlayer()
        { }

        public UnitPlayer(TUnitPlayer unit)
        {
            this.m_Player = unit;
        }
        
        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_Player != null ? this.m_Player.ToString() : "(none)";
        }
    }
}