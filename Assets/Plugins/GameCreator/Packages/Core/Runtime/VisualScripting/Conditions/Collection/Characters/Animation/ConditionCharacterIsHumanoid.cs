using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Humanoid")]
    [Description("Returns true if the Character has a humanoid model")]

    [Category("Characters/Busy/Is Humanoid")]

    [Keywords("Human", "Biped")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Green)]
    
    [Serializable]
    public class ConditionCharacterIsHumanoid : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Humanoid {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && character.Animim.Animator.isHuman;
        }
    }
}
