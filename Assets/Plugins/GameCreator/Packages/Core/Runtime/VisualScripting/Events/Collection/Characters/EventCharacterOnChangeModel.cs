using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Change Model")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Blue)]
    
    [Category("Characters/On Change Model")]
    [Description("Executed when a character changes its model")]

    [Serializable]
    public class EventCharacterOnChangeModel : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.EventAfterChangeModel += this.OnChangeModel;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.EventAfterChangeModel -= this.OnChangeModel;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChangeModel()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}