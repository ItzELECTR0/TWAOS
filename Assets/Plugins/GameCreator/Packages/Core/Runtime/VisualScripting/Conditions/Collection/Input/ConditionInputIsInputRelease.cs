using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Input Released")]
    [Description("Returns true if the Input Action asset with a button behavior is released during this frame")]

    [Category("Input/Is Input Released")]
    
    [Parameter("Input", "A reference to the Input Action asset with map and action name")]

    [Keywords("Unity", "Button", "Up", "Input", "Action", "System", "Map")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]

    [Serializable]
    public class ConditionInputIsInputRelease : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private InputActionFromAsset m_Input = new InputActionFromAsset();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Input} just released";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            InputAction input = this.m_Input?.InputAction;
            return input != null && input.WasReleasedThisFrame();
        }
    }
}
