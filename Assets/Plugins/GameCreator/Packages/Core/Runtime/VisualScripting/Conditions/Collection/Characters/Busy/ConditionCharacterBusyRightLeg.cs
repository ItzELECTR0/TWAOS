using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Right Leg Available")]
    [Description("Returns true if the Character's right leg is available to start a new action")]

    [Category("Characters/Busy/Is Right Leg Available")]

    [Keywords("Occupied", "Available", "Free", "Doing", "Foot", "Feet")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Red)]
    
    [Serializable]
    public class ConditionCharacterBusyRightLeg : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Right Leg Available {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && !character.Busy.IsLegRightBusy;
        }
    }
}
