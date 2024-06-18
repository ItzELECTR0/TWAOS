using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Available")]
    [Description("Returns true if the Character is not doing any action and is free to start one")]

    [Category("Characters/Busy/Is Available")]

    [Keywords("Occupied", "Available", "Free", "Doing")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Red)]
    
    [Serializable]
    public class ConditionCharacterIsAvailable : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Available {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && !character.Busy.IsBusy;
        }
    }
}
