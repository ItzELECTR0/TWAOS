using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting 
{
    [Version(0, 1, 1)]

    [Title("Frame Step")]
    [Description("Performs a single frame step. It requires the Editor to be paused")]

    [Category("Debug/Frame Step")]

    [Keywords("Debug")]
    [Image(typeof(IconSkipNext), ColorTheme.Type.TextLight)]
    
    [Serializable]
    public class InstructionsCommonDebugStep : Instruction
    {
        public override string Title => "Step Frame";

        protected override Task Run(Args args)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.Step();
            #endif
            
            return DefaultResult;
        }
    }
}