using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Become Player")]
    [Image(typeof(IconPlayer), ColorTheme.Type.Yellow)]
    
    [Category("Characters/On Become Player")]
    [Description("Executed when a character becomes the Player")]

    [Serializable]
    public class EventCharacterOnBecomePlayer : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.EventChangeToPlayer += this.OnChangeToPlayer;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.EventChangeToPlayer -= this.OnChangeToPlayer;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChangeToPlayer()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}