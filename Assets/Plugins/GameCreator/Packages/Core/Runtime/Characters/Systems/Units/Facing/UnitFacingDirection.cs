using System;
using UnityEngine;
using GameCreator.Runtime.Common;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.Characters
{
    [Title("Towards Direction")]
    [Image(typeof(IconArrowCircleRight), ColorTheme.Type.Yellow)]
    
    [Category("Direction/Towards Direction")]
    [Description("Rotates the Character towards a specific world-space direction")]
    
    [Serializable]
    public class UnitFacingDirection : TUnitFacing
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private PropertyGetRotation m_Direction = GetRotationTowardsDirection.CreateForward;
        
        [SerializeField] private Axonometry m_Axonometry = new Axonometry();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private Args args;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override Axonometry Axonometry
        {
            get => this.m_Axonometry;
            set => this.m_Axonometry = value;
        }

        // METHODS: -------------------------------------------------------------------------------
        
        protected override Vector3 GetDefaultDirection()
        {
            if (this.args == null) this.args = new Args(this.Character);
            
            Vector3 driverDirection = Vector3.Scale(
                this.m_Direction.Get(this.args) * Vector3.forward,
                Vector3Plane.NormalUp
            );

            Vector3 heading = this.DecideDirection(driverDirection);
            return this.m_Axonometry?.ProcessRotation(this, heading) ?? heading;
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => "Towards Direction";
    }
}