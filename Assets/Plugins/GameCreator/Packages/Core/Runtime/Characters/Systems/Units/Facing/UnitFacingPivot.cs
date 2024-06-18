using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Pivot")]
    [Image(typeof(IconRotationYaw), ColorTheme.Type.Green)]
    
    [Category("Pivot/Pivot")]
    [Description("Rotates the Character towards the direction it moves")]

    [Serializable]
    public class UnitFacingPivot : TUnitFacing
    {
        private enum DirectionFrom
        {
            MotionDirection,
            DriverDirection
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private DirectionFrom m_DirectionFrom = DirectionFrom.MotionDirection;
        
        [SerializeField] private Axonometry m_Axonometry = new Axonometry();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override Axonometry Axonometry
        {
            get => this.m_Axonometry;
            set => this.m_Axonometry = value;
        }
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override Vector3 GetDefaultDirection()
        {
            Vector3 driverDirection = Vector3.Scale(
                this.m_DirectionFrom switch
                {
                    DirectionFrom.MotionDirection => this.Character.Motion.MoveDirection,
                    DirectionFrom.DriverDirection => this.Character.Driver.WorldMoveDirection,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Vector3Plane.NormalUp
            );
            
            Vector3 direction = this.DecideDirection(driverDirection);
            return this.m_Axonometry?.ProcessRotation(this, direction) ?? direction;
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => "Pivot";
    }
}