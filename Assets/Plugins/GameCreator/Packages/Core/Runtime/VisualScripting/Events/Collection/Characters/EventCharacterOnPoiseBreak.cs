using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Poise Break")]
    [Image(typeof(IconShieldOutline), ColorTheme.Type.Red, typeof(OverlayBolt))]
    
    [Category("Characters/Combat/On Poise Break")]
    [Description("Executed when a character's Poise is broken")]
    
    [Keywords("Resistance", "Combat")]

    [Serializable]
    public class EventCharacterOnPoiseBreak : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Combat.Poise.EventPoiseBreak -= this.OnBreak;
            character.Combat.Poise.EventPoiseBreak += this.OnBreak;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Combat.Poise.EventPoiseBreak -= this.OnBreak;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnBreak()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}