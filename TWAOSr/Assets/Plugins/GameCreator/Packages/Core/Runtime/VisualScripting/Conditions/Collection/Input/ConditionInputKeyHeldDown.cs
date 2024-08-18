using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Key Held Down")]
    [Description("Returns true if the keyboard key is being held down this frame")]

    [Category("Input/Is Key Held Down")]
    
    [Parameter("Key", "The Keyboard key that is checked")]

    [Keywords("Button", "Active", "Down", "Press")]
    
    [Image(typeof(IconKey), ColorTheme.Type.Blue, typeof(OverlayDot))]
    [Serializable]
    public class ConditionInputKeyHeldDown : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] protected Key m_Key = Key.Space;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Key} held down";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return Keyboard.current[this.m_Key].isPressed;
        }
    }
}
