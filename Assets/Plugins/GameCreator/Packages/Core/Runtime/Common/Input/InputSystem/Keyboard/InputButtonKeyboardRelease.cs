using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Keyboard Release")]
    [Category("Keyboard/Keyboard Release")]
    
    [Description("When a keyboard key is released")]
    [Image(typeof(IconKey), ColorTheme.Type.Yellow, typeof(OverlayArrowUp))]
    
    [Keywords("Key", "Button", "Up")]
    
    [Serializable]
    public class InputButtonKeyboardRelease : TInputButton
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Key m_Key = Key.Space;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        // INITIALIZERS: --------------------------------------------------------------------------

        public static InputPropertyButton Create(Key key = Key.Space)
        {
            return new InputPropertyButton(
                new InputButtonKeyboardRelease
                {
                    m_Key = key
                }
            );
        }

        // UPDATE METHODS: ------------------------------------------------------------------------
        
        public override void OnUpdate()
        {
            if (Keyboard.current == null) return;
            if (!Keyboard.current[this.m_Key].wasReleasedThisFrame) return;
            
            this.ExecuteEventStart();
            this.ExecuteEventPerform();
        }
    }
}