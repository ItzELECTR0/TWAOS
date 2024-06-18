using System;
using GameCreator.Runtime.Cameras;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Input Direction")]
    [Image(typeof(IconGamepadCross), ColorTheme.Type.Yellow)]
    
    [Category("Direction/Input Direction")]
    [Description("Rotates the Character towards the input direction from a camera")]
    
    [Serializable]
    public class UnitFacingInputDirection : TUnitFacing
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private InputPropertyValueVector2 m_Input = InputValueVector2MobileStickRight.Create;
        [SerializeField] private PropertyGetGameObject m_Camera = GetGameObjectCameraMain.Create;
        
        [SerializeField] private Axonometry m_Axonometry = new Axonometry();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private Args m_Args;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override Axonometry Axonometry
        {
            get => this.m_Axonometry;
            set => this.m_Axonometry = value;
        }

        // METHODS: -------------------------------------------------------------------------------

        public override void OnStartup(Character character)
        {
            base.OnStartup(character);
            this.m_Input?.OnStartup();
        }

        public override void OnDispose(Character character)
        {
            base.OnDispose(character);
            this.m_Input?.OnDispose();
        }

        protected override Vector3 GetDefaultDirection()
        {
            if (this.m_Args == null) this.m_Args = new Args(this.Character);

            Vector2 value = this.m_Input?.Read() ?? default;
            Vector3 direction = new Vector3(value.x, 0f, value.y);

            Camera camera = this.m_Camera.Get<Camera>(this.m_Args);
            if (camera != null) direction = camera.transform.TransformDirection(direction);

            Vector3 driverDirection = Vector3.Scale(direction, Vector3Plane.NormalUp);
            
            Vector3 heading = this.DecideDirection(driverDirection);
            return this.m_Axonometry?.ProcessRotation(this, heading) ?? heading;
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString() => "Input Direction";
    }
}