using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Reset Vertical Velocity")]
    [Description("Changes the Character's vertical velocity to zero")]

    [Category("Characters/Properties/Reset Vertical Velocity")]

    [Keywords("Fall", "Speed")]
    [Image(typeof(IconFall), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterResetVerticalVelocity : TInstructionCharacterProperty
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Reset {m_Character} Vertical Velocity";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character != null) character.Driver.ResetVerticalVelocity();
            
            return DefaultResult;
        }
    }
}