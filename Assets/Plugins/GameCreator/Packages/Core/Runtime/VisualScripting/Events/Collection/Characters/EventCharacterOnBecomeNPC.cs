using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Become NPC")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Yellow)]
    
    [Category("Characters/On Become NPC")]
    [Description("Executed when a character that is a Player becomes an NPC")]

    [Serializable]
    public class EventCharacterOnBecomeNPC : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.EventChangeToNPC += this.OnChangeToNPC;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.EventChangeToNPC -= this.OnChangeToNPC;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChangeToNPC()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}