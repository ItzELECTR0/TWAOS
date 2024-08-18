using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Cursor Visibility")]
    [Description("Determines if the hardware cursor is visible or not")]

    [Category("Application/Cursor/Cursor Visibility")]
    
    [Parameter("Is Visible", "If true the cursor is visible, unless it is set as Locked")]

    [Keywords("Mouse", "FPS", "Crosshair")]
    [Image(typeof(IconCursor), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionCursorVisibility : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetBool m_IsVisible = new PropertyGetBool(true);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set Cursor Visibility to {this.m_IsVisible}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Cursor.visible = this.m_IsVisible.Get(args);
            return DefaultResult;
        }
    }
}