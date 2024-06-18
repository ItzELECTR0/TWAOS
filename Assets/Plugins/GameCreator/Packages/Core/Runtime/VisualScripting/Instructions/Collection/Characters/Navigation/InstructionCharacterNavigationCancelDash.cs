using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Cancel Dash")]
    [Description("Cancels any existing Dash on the chosen Character")]

    [Category("Characters/Navigation/Cancel Dash")]

    [Keywords("Leap", "Blink", "Roll", "Flash")]
    [Image(typeof(IconCharacterDash), ColorTheme.Type.TextLight, typeof(OverlayCross))]

    [Serializable]
    public class InstructionCharacterNavigationCancelDash : TInstructionCharacterNavigation
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Cancel Dash on {this.m_Character}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            character.Dash.Cancel();
            return DefaultResult;
        }
    }
}