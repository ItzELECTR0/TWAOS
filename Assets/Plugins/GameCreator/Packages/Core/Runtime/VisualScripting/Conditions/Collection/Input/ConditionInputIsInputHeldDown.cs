using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Input Held Down")]
    [Description("Returns true while the Input Action asset with a button behavior is being pressed")]

    [Category("Input/Is Input Held Down")]
    
    [Parameter("Input", "A reference to the Input Action asset with map and action name")]

    [Keywords("Unity", "Button", "While", "Hold", "Press", "Input", "Action", "System", "Map")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue, typeof(OverlayDot))]

    [Serializable]
    public class ConditionInputIsInputHeldDown : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private InputActionFromAsset m_Input = new InputActionFromAsset();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Input} held down";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            InputAction input = this.m_Input?.InputAction;
            return input != null && input.IsPressed();
        }
    }
}
