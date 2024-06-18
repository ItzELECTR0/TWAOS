using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Constant Motion")]
    [Category("Usage/Constant Motion")]
    
    [Description("Keeps returning the last value after releasing the input until changed")]
    [Image(typeof(IconGamepadCross), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]

    [Serializable]
    public class InputValueVector2MotionConstant : TInputValueVector2
    {
        private const float MIN_MAGNITUDE = 0.2f;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private float m_X = 0f;
        [SerializeField] private float m_Y = 1f;
        
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
                        "Constant Motion",
                        InputActionType.Value
                    );
                }

                return this.m_InputAction;
            }
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyValueVector2 Create()
        {
            return new InputPropertyValueVector2(
                new InputValueVector2MotionConstant()
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

        public override Vector2 Read()
        {
            Vector2 value = new Vector2(this.m_X, this.m_Y);
            return value.magnitude < MIN_MAGNITUDE ? Vector2.zero : value;
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