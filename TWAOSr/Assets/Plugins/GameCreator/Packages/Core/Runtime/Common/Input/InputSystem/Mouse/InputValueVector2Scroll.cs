using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Mouse Scroll")]
    [Category("Mouse/Mouse Scroll")]
    
    [Description("Every time the scroll wheel is used")]
    [Image(typeof(IconScroll), ColorTheme.Type.Yellow)]
    
    [Keywords("Cursor", "Button", "Up")]
    
    [Serializable]
    public class InputValueVector2Scroll : TInputValueVector2
    {
        private enum Direction
        {
            Both,
            Up,
            Down
        }
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Direction m_Direction = Direction.Both;
        
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
                        name: "Zoom", 
                        type: InputActionType.Value,
                        binding: "<Mouse>/scroll"
                    );
                }

                return this.m_InputAction;
            }
        }

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyValueVector2 Create()
        {
            return new InputPropertyValueVector2(
                new InputValueVector2Scroll()
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
            Vector2 scroll = this.InputAction?.ReadValue<Vector2>() ?? Vector2.zero;
            return this.m_Direction switch
            {
                Direction.Both => scroll,
                Direction.Up => new Vector2(Math.Max(scroll.x, 0f), Math.Max(scroll.y, 0f)),
                Direction.Down => new Vector2(Math.Min(scroll.x, 0f), Math.Min(scroll.y, 0f)),
                _ => throw new ArgumentOutOfRangeException()
            };
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