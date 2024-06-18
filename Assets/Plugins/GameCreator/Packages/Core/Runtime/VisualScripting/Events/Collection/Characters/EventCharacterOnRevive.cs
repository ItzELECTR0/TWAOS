using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Revive")]
    [Image(typeof(IconSkull), ColorTheme.Type.Green)]
    
    [Category("Characters/On Revive")]
    [Description("Executed when a dead character revives")]
    
    [Keywords("Resurrect", "Respawn")]
    
    [Serializable]
    public class EventCharacterOnRevive : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.EventRevive += this.OnRevive;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.EventRevive -= this.OnRevive;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnRevive()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}