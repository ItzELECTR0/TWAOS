using System;
using UnityEngine;
using GameCreator.Runtime.Common;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Characters
{
    [Title("At Pointer")]
    [Image(typeof(IconCursor), ColorTheme.Type.Green)]
    
    [Category("Targets/At Pointer")]
    [Description("Rotates towards where the pointer is, relative to the Character")]

    [Serializable]
    public class UnitFacingPointer : TUnitFacing
    {
        private const float MIN_DISTANCE = 0.05f;
        
        private enum Axis
        {
            X,
            Y,
            Z
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Axis m_InPlane;
        
        [SerializeField] private Axonometry m_Axonometry = new Axonometry();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override Axonometry Axonometry
        {
            get => this.m_Axonometry;
            set => this.m_Axonometry = value;
        }
        
        // INITIALIZERS: --------------------------------------------------------------------------

        public UnitFacingPointer()
        {
            this.m_InPlane = Axis.Y;
        }

        protected override Vector3 GetDefaultDirection()
        {
            if (!this.Character.IsPlayer || !this.Character.Player.IsControllable)
            {
                Vector3 noDirection = this.DecideDirection(Vector3.zero);
                return this.m_Axonometry?.ProcessRotation(this, noDirection) ?? noDirection; 
            }
            
            Camera camera = ShortcutMainCamera.Get<Camera>();
            if (camera == null)
            {
                Vector3 noDirection = this.DecideDirection(Vector3.zero);
                return this.m_Axonometry?.ProcessRotation(this, noDirection) ?? noDirection;
            }
            
            Vector2 pointer = Mouse.current?.position.ReadValue() ?? this.Character.Feet;
            Ray ray = camera.ScreenPointToRay(pointer);
            
            Plane ground = new Plane(
                this.m_InPlane switch
                {
                    Axis.X => Vector3.right,
                    Axis.Y => Vector3.up,
                    Axis.Z => Vector3.forward,
                    _ => throw new ArgumentOutOfRangeException()
                },
                this.Character.Feet
            );

            if (!ground.Raycast(ray, out float distance))
            {
                Vector3 noDirection = this.DecideDirection(Vector3.zero);
                return this.m_Axonometry?.ProcessRotation(this, noDirection) ?? noDirection;
            }
            
            Vector3 point = ray.GetPoint(distance);
            Vector3 direction = Vector3.Scale(
                point - this.Character.Feet,
                Vector3Plane.NormalUp
            );
                
            Debug.DrawRay(this.Character.Feet, direction, Color.magenta, 1f);

            Vector3 result = this.DecideDirection(direction.sqrMagnitude > MIN_DISTANCE
                ? direction
                : Vector3.zero
            );
            
            return this.m_Axonometry?.ProcessRotation(this, result) ?? result;
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => "At Pointer";
    }
}