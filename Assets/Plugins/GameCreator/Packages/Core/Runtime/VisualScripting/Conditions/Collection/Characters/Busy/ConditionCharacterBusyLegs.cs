using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Are Legs Available")]
    [Description("Returns true if the Character's legs are available to start a new action")]

    [Category("Characters/Busy/Are Legs Available")]

    [Keywords("Occupied", "Available", "Free", "Doing", "Foot", "Feet")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Red)]
    
    [Serializable]
    public class ConditionCharacterBusyLegs : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Legs Available {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && !character.Busy.AreLegsBusy;
        }
    }
}
