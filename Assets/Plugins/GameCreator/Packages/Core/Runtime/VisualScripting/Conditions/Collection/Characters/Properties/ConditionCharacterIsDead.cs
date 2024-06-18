using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Dead")]
    [Description("Returns true if the character has been killed")]

    [Category("Characters/Properties/Is Dead")]

    [Keywords("Kill", "Kaput")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionCharacterIsDead : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Dead {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && character.IsDead;
        }
    }
}
