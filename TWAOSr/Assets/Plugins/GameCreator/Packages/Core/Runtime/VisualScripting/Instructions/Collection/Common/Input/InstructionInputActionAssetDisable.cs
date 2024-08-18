using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Disable Input Action")]
    [Description("Disables an Input Action asset which stops it from reading user input")]

    [Category("Input/Disable Input Action")]

    [Parameter("Input Asset", "The Input Asset reference")]

    [Keywords("Deactivate", "Inactive")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Red, typeof(OverlayDot))]
    
    [Serializable]
    public class InstructionInputActionAssetDisable : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private InputActionFromAsset m_InputAsset = new InputActionFromAsset();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Disable {this.m_InputAsset}";

        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            InputAction inputAction = this.m_InputAsset.InputAction;
            if (inputAction is { enabled: true }) inputAction.Disable();
            
            return DefaultResult;
        }
    }
}
