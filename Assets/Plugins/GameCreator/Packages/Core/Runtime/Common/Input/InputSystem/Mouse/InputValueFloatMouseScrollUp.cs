using System;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Mouse Scroll-Wheel Up")]
    [Category("Mouse/Mouse Scroll-Wheel Up")]
    
    [Description("The Mouse scroll-Wheel Up component")]
    [Image(typeof(IconScroll), ColorTheme.Type.Yellow, typeof(OverlayArrowUp))]
    
    [Keywords("Cursor", "Location", "Move", "Pan")]
    
    [Serializable]
    public class InputValueFloatMouseScrollUp : TInputValueFloat
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private InputAction m_InputAction;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public InputAction InputAction
        {
            get
            {
                if (this.m_InputAction == null)
                {
                    this.m_InputAction = new InputAction(
                        name: "Scroll-Wheel", 
                        type: InputActionType.Value,
                        binding: "<Mouse>/scroll/up"
                    );
                }

                return this.m_InputAction;
            }
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyValueFloat Create()
        {
            return new InputPropertyValueFloat(
                new InputValueFloatMouseScroll()
            );
        }

        public override void OnStartup()
        {
            this.Enable();
        }

        public override void OnDispose()
        {
            this.Disable();
            this.InputAction?.Dispose();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override float Read()
        {
            return this.InputAction?.ReadValue<float>() ?? 0f;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void Enable()
        {
            this.InputAction?.Enable();
        }

        private void Disable()
        {
            this.InputAction?.Disable();
        }
    }
}