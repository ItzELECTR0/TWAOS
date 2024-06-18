using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Display Touchstick Right")]
    [Description("Shows or hides the default Touchstick on the right side")]

    [Category("Input/Display Touchstick Right")]

    [Parameter(
        "Show",
        "Shows the touchstick if ticked. Hides the touchstick otherwise"
    )]

    [Keywords("Joystick")]
    [Image(typeof(IconTouchstick), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionInputTouchstickVisibilityRight : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetBool m_Show = GetBoolValue.Create(false);

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Show Right Touchstick: {this.m_Show}";

        // METHODS: -------------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject touchstick = TouchStickRight.INSTANCE;
            if (touchstick == null) return DefaultResult;

            bool active = this.m_Show.Get(args);
            touchstick.SetActive(active);
            
            return DefaultResult;
        }
    }
}
