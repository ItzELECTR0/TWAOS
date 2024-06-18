using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Invincible")]
    [Description("Returns true if the Character is Invincible")]

    [Category("Characters/Combat/Is Invincible")]

    [Keywords("Invincibility", "Combat")]
    
    [Image(typeof(IconDiamondSolid), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class ConditionCharacterIsInvincible : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Invincible {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && character.Combat.Invincibility.IsInvincible;
        }
    }
}
