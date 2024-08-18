using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Quit Application")]
    [Description(
        "Closes the application and exits the program. This instruction is ignored in the " +
        "Unity Editor or WebGL platforms"
    )]

    [Category("Application/Quit Application")]

    [Keywords("Exit", "Close", "Shutdown", "Turn")]
    [Image(typeof(IconExit), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionAppQuit : Instruction
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => "Quit Application";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Application.Quit();
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
            #endif
            
            return DefaultResult;
        }
    }
}