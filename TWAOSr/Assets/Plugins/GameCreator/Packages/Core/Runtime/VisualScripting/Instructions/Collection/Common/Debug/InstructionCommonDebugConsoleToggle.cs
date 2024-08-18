using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting 
{
    [Version(0, 1, 1)]

    [Title("Toggle Console")]
    [Description("Shows or hides the Console in a standalone development build")]

    [Category("Debug/Toggle Console")]

    [Keywords("Debug", "Terminal")]
    [Image(typeof(IconTerminal), ColorTheme.Type.TextLight)]
    
    [Serializable]
    public class InstructionCommonDebugConsoleToggle : Instruction
    {
        private enum Option
        {
            Show,
            Hide
        }

        [SerializeField] private Option m_Option = Option.Show;
        
        public override string Title => $"{this.m_Option} Console";

        protected override Task Run(Args args)
        {
            Debug.developerConsoleEnabled = true;
            Debug.developerConsoleVisible = this.m_Option == Option.Show;
            return DefaultResult;
        }
    }
}