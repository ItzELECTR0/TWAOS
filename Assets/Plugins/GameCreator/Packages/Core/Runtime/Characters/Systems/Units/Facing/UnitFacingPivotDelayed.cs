using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Pivot Delayed")]
    [Image(typeof(IconRotationYaw), ColorTheme.Type.Green, typeof(OverlayDot))]
    
    [Category("Pivot/Pivot Delayed")]
    [Description("Rotates the Character towards the direction it's moving after a delay")]
    
    [Serializable]
    public class UnitFacingPivotDelayed : TUnitFacing
    {
        private enum DirectionFrom
        {
            MotionDirection,
            DriverDirection
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private DirectionFrom m_DirectionFrom = DirectionFrom.MotionDirection;
        
        [SerializeField, Min(0f)] private float m_Delay = 1.75f;
        [SerializeField, Range(0f, 180f)] private float m_DelayAngle = 30f;
        
        [SerializeField] private Axonometry m_Axonometry = new Axonometry();
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private float m_DirectionChangeTime;
        [NonSerialized] private bool m_WasDirectionChanged;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override Axonometry Axonometry
        {
            get => this.m_Axonometry;
            set => this.m_Axonometry = value;
        }
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected override Vector3 GetDefaultDirection()
        {
            Vector3 currentDirection = this.Transform.TransformDirection(Vector3.forward);
            Vector3 driverDirection = Vector3.Scale(
                this.m_DirectionFrom switch
                {
                    DirectionFrom.MotionDirection => this.Character.Motion.MoveDirection,
                    DirectionFrom.DriverDirection => this.Character.Driver.WorldMoveDirection,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Vector3Plane.NormalUp
            );

            if (driverDirection.magnitude > this.Character.Motion.LinearSpeed * 0.25f)
            {
                if (!this.m_WasDirectionChanged)
                {
                    m_DirectionChangeTime = this.Character.Time.Time;
                }
                
                this.m_WasDirectionChanged = true;
            }
            else
            {
                this.m_WasDirectionChanged = false;
                return this.m_Axonometry?.ProcessRotation(this, currentDirection) ?? currentDirection;
            }
            
            if (Vector3.Angle(currentDirection, driverDirection) > this.m_DelayAngle)
            {
                Vector3 direction = this.DecideDirection(
                    this.Character.Time.Time - this.m_DirectionChangeTime < this.m_Delay
                        ? currentDirection
                        : driverDirection
                );
                
                return this.m_Axonometry?.ProcessRotation(this, direction) ?? direction;
            }

            return this.m_Axonometry?.ProcessRotation(this, driverDirection) ?? driverDirection;
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => "Pivot Delayed";
    }
}