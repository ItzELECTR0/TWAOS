using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Poise Change")]
    [Image(typeof(IconShieldOutline), ColorTheme.Type.Yellow)]
    
    [Category("Characters/Combat/On Poise Change")]
    [Description("Executed every time the character's combat Poise changes")]
    
    [Keywords("Resistance", "Combat")]

    [Serializable]
    public class EventCharacterOnPoiseChange : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Combat.Poise.EventChange -= this.OnChange;
            character.Combat.Poise.EventChange += this.OnChange;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Combat.Poise.EventChange -= this.OnChange;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChange()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}