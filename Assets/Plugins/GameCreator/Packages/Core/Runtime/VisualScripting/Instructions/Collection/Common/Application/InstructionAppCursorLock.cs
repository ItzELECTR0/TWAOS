using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Lock Cursor")]
    [Description("Determines if the hardware pointer is locked to the center of the view or not")]

    [Category("Application/Cursor/Lock Cursor")]
    
    [Parameter("Lock Mode", "The behavior of the cursor. The default value is None")]

    [Keywords("Mouse", "State", "FPS", "Center", "Confine")]
    [Image(typeof(IconCursor), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionAppCursorLock : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private CursorLockMode m_LockMode = CursorLockMode.Locked;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Set Cursor to {0}",
            TextUtils.Humanize(this.m_LockMode.ToString())
        );

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Cursor.lockState = this.m_LockMode;
            return DefaultResult;
        }
    }
}