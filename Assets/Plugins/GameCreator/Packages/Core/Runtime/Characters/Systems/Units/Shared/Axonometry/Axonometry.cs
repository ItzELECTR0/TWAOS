using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Axonometry : ICloneable
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeReference] private TAxonometry m_Axonometry = new AxonometryNone();
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector3 ProcessTranslation(TUnitDriver driver, Vector3 movement)
        {
            return this.m_Axonometry?.ProcessTranslation(driver, movement) ?? movement;
        }

        public void ProcessPosition(TUnitDriver driver, Vector3 position)
        {
            this.m_Axonometry?.ProcessPosition(driver, position);
        }
        
        public Vector3 ProcessRotation(TUnitFacing facing, Vector3 direction)
        {
            return this.m_Axonometry?.ProcessRotation(facing, direction) ?? direction;
        }
        
        // CLONE: ---------------------------------------------------------------------------------
        
        public object Clone() => new Axonometry
        {
            m_Axonometry = this.m_Axonometry.Clone() as TAxonometry
        };

        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => this.m_Axonometry.ToString();
    }
}