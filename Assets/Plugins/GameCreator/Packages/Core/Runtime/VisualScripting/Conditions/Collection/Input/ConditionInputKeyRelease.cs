using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Key Released")]
    [Description("Returns true if the keyboard key is released during this frame")]

    [Category("Input/Is Key Released")]
    
    [Parameter("Key", "The Keyboard key that is checked")]

    [Keywords("Button", "Up")]
    
    [Image(typeof(IconKey), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    [Serializable]
    public class ConditionInputKeyRelease : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] protected Key m_Key = Key.Space;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Key} just released";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return Keyboard.current[this.m_Key].wasReleasedThisFrame;
        }
    }
}
