using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Enable Input Map")]
    [Description("Enables an Input Action asset with a Map value which allows reading user input")]

    [Category("Input/Enable Input Map")]

    [Parameter("Input Asset", "The Input Asset reference")]

    [Keywords("Activate", "Active", "Start")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionInputMapAssetEnable : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private InputMapFromAsset m_InputAsset = new InputMapFromAsset();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Enable {this.m_InputAsset}";

        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            InputActionMap inputMap = this.m_InputAsset.InputMap;
            if (inputMap is { enabled: false }) inputMap.Enable();
            
            return DefaultResult;
        }
    }
}
