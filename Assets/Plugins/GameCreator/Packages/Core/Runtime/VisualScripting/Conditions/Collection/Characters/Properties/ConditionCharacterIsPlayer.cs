using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Player")]
    [Description("Returns true if the Character is marked as a Player")]

    [Category("Characters/Properties/Is Player")]

    [Keywords("Control", "Character")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionCharacterIsPlayer : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Player {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && character.IsPlayer;
        }
    }
}
