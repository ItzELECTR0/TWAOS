using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Dodge")]
    [Image(typeof(IconCharacterDash), ColorTheme.Type.Green)]
    
    [Category("Characters/Combat/On Dodge")]
    [Description("Executed every time the character evades an attack")]

    [Serializable]
    public class EventCharacterOnDodge : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Dash.EventDodge += this.OnDodge;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Dash.EventDodge -= this.OnDodge;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnDodge()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}