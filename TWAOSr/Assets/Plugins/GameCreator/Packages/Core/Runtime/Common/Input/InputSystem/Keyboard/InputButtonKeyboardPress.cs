using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Keyboard Press")]
    [Category("Keyboard/Keyboard Press")]
    
    [Description("When a keyboard key is pressed")]
    [Image(typeof(IconKey), ColorTheme.Type.Yellow, typeof(OverlayArrowDown))]
    
    [Keywords("Key", "Button", "Down")]
    
    [Serializable]
    public class InputButtonKeyboardPress : TInputButton
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Key m_Key = Key.Space;

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyButton Create(Key key = Key.Space)
        {
            return new InputPropertyButton(
                new InputButtonKeyboardPress
                {
                    m_Key = key
                }
            );
        }

        // UPDATE METHODS: ------------------------------------------------------------------------
        
        public override void OnUpdate()
        {
            if (Keyboard.current == null) return;
            if (!Keyboard.current[this.m_Key].wasPressedThisFrame) return;
            
            this.ExecuteEventStart();
            this.ExecuteEventPerform();
        }
    }
}