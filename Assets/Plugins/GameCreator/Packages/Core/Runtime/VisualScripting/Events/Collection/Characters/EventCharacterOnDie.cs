using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Die")]
    [Image(typeof(IconSkull), ColorTheme.Type.Red)]
    
    [Category("Characters/On Die")]
    [Description("Executed when the character dies")]
    
    [Serializable]
    public class EventCharacterOnDie : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.EventDie += this.OnDie;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.EventDie -= this.OnDie;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnDie()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}