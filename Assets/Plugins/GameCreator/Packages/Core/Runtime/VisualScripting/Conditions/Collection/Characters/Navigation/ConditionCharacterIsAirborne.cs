using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Airborne")]
    [Description("Returns true if the Character not touching the ground")]

    [Category("Characters/Navigation/Is Airborne")]

    [Keywords("Fly", "Fall", "Flail", "Jump", "Float", "Suspend")]
    
    [Image(typeof(IconFall), ColorTheme.Type.Yellow, typeof(OverlayArrowDown))]
    [Serializable]
    public class ConditionCharacterIsAirborne : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is On Air {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && !character.Driver.IsGrounded;
        }
    }
}
