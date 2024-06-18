using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Dash")]
    [Image(typeof(IconCharacterDash), ColorTheme.Type.Yellow)]
    
    [Category("Characters/Navigation/On Dash")]
    [Description("Executed every time the character performs a dash")]

    [Serializable]
    public class EventCharacterOnDash : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Dash.EventDashStart += this.OnDash;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Dash.EventDashStart -= this.OnDash;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnDash()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}