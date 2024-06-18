using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Stop Following")]
    [Description("Instructs a Character to stop following a game object")]

    [Category("Characters/Navigation/Stop Following")]

    [Keywords("Cancel", "Lead", "Pursue", "Chase")]
    [Image(typeof(IconCharacterIdle), ColorTheme.Type.Red)]

    [Serializable]
    public class InstructionCharacterNavigationFollowStop : TInstructionCharacterNavigation
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"{this.m_Character} Stop Following";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;
            
            character.Motion.StopFollowingTarget();
            return DefaultResult;
        }
    }
}