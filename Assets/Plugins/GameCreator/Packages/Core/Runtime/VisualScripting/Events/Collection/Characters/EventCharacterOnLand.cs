using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Land")]
    [Image(typeof(IconLand), ColorTheme.Type.Yellow)]
    
    [Category("Characters/Navigation/On Land")]
    [Description("Executed every time the character lands on the ground")]

    [Serializable]
    public class EventCharacterOnLand : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.EventLand += this.OnStep;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.EventLand -= this.OnStep;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnStep(float velocity)
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}