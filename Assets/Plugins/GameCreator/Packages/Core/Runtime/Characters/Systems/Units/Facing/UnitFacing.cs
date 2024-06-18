using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class UnitFacing
    {
        [SerializeReference] private TUnitFacing m_Facing = new UnitFacingPivot();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public TUnitFacing Wrapper => this.m_Facing;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public UnitFacing()
        { }

        public UnitFacing(TUnitFacing unit)
        {
            this.m_Facing = unit;
        }
        
        // OVERRIDES: -----------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_Facing != null ? this.m_Facing.ToString() : "(none)";
        }
    }
}