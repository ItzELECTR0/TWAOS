using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Enable Input Action")]
    [Description("Enables an Input Action asset which allows it to start reading user input")]

    [Category("Input/Enable Input Action")]

    [Parameter("Input Asset", "The Input Asset reference")]

    [Keywords("Activate", "Active", "Start")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Green, typeof(OverlayDot))]
    
    [Serializable]
    public class InstructionInputActionAssetEnable : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private InputActionFromAsset m_InputAsset = new InputActionFromAsset();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Enable {this.m_InputAsset}";

        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            InputAction inputAction = this.m_InputAsset.InputAction;
            if (inputAction is { enabled: false }) inputAction.Enable();
            
            return DefaultResult;
        }
    }
}
