using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Key Pressed")]
    [Description("Returns true if the keyboard key is pressed during this frame")]

    [Category("Input/Is Key Pressed")]
    
    [Parameter("Key", "The Keyboard key that is checked")]

    [Keywords("Button", "Down")]
    
    [Image(typeof(IconKey), ColorTheme.Type.Yellow, typeof(OverlayArrowLeft))]
    [Serializable]
    public class ConditionInputIsKeyPress : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] protected Key m_Key = Key.Space;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Key} just pressed";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return Keyboard.current[this.m_Key].wasPressedThisFrame;
        }
    }
}
