using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Jump")]
    [Image(typeof(IconCharacterJump), ColorTheme.Type.Yellow)]
    
    [Category("Characters/Navigation/On Jump")]
    [Description("Executed every time the character performs a jump")]

    [Serializable]
    public class EventCharacterOnJump : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.EventJump += this.OnJump;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.EventJump -= this.OnJump;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnJump(float velocity)
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}