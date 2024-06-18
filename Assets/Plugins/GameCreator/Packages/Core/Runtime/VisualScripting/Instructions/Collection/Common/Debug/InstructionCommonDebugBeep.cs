using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting 
{
    [Version(0, 1, 1)]

    [Title("Beep")]
    [Description(
        "Plays the Operative System default 'beep' sound. This is intended for debugging " +
        "purposes and doesn't do anything on a runtime application"
    )]

    [Category("Debug/Beep")]

    [Keywords("Debug")]
    [Image(typeof(IconMusicNote), ColorTheme.Type.TextLight)]
    
    [Serializable]
    public class InstructionCommonDebugBeep : Instruction
    {
        public override string Title => "Play Beep! sound";

        protected override Task Run(Args args)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.Beep();
            #endif
            
            return DefaultResult;
        }
    }
}