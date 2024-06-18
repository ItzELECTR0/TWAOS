using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Defense Change")]
    [Image(typeof(IconShieldSolid), ColorTheme.Type.Blue)]
    
    [Category("Characters/Combat/On Defense Change")]
    [Description("Executed when the Character's defense changes")]
    
    [Keywords("Defend", "Block", "Combat")]

    [Serializable]
    public class EventCharacterOnDefenseChange : TEventCharacter
    {
        protected override void WhenEnabled(Trigger trigger, Character character)
        {
            character.Combat.EventDefenseChange -= this.OnChangeDefense;
            character.Combat.EventDefenseChange += this.OnChangeDefense;
        }

        protected override void WhenDisabled(Trigger trigger, Character character)
        {
            character.Combat.EventDefenseChange -= this.OnChangeDefense;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChangeDefense()
        {
            Character character = this.m_Character.Get<Character>(this.m_Trigger.gameObject);
            if (character != null) _ = this.m_Trigger.Execute(character.gameObject);
        }
    }
}