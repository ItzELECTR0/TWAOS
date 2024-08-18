using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Input Pressed")]
    [Description("Returns true if the Input Action asset with a button behavior is pressed during this frame")]

    [Category("Input/Is Input Pressed")]
    
    [Parameter("Input", "A reference to the Input Action asset with map and action name")]

    [Keywords("Unity", "Button", "Down", "Input", "Action", "System", "Map")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue, typeof(OverlayArrowLeft))]

    [Serializable]
    public class ConditionInputIsInputAssetPress : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private InputActionFromAsset m_Input = new InputActionFromAsset();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Input} just pressed";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            InputAction input = this.m_Input?.InputAction;
            return input != null && input.WasPressedThisFrame();
        }
    }
}
