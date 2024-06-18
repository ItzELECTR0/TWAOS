using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Left Leg Available")]
    [Description("Returns true if the Character's left leg is available to start a new action")]

    [Category("Characters/Busy/Is Left Leg Available")]

    [Keywords("Occupied", "Available", "Free", "Doing", "Foot", "Feet")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Red)]
    
    [Serializable]
    public class ConditionCharacterBusyLeftLeg : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Left Leg Available {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && !character.Busy.IsLegLeftBusy;
        }
    }
}
