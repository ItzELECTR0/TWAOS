using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Keyboard While Pressing")]
    [Category("Keyboard/Keyboard While Pressing")]
    
    [Description("While the specified keyboard key is being held down")]
    [Image(typeof(IconKey), ColorTheme.Type.Blue, typeof(OverlayDot))]
    
    [Keywords("Key", "Button", "Down", "Held", "Hold")]

    [Serializable]
    public class InputButtonKeyboardWhilePressing : TInputButton
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Key m_Key = Key.Space;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyButton Create(Key key = Key.Space)
        {
            return new InputPropertyButton(
                new InputButtonKeyboardWhilePressing
                {
                    m_Key = key
                }
            );
        }

        // UPDATE METHODS: ------------------------------------------------------------------------
        
        public override void OnUpdate()
        {
            if (Keyboard.current == null) return;
            
            if (Keyboard.current[this.m_Key].wasPressedThisFrame)
            {
                this.ExecuteEventStart();   
            }
            
            if (Keyboard.current[this.m_Key].IsPressed())
            {
                this.ExecuteEventPerform();
            }
        }
    }
}