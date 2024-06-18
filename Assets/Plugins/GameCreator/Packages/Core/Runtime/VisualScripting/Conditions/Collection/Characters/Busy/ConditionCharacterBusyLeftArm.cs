using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Left Arm Available")]
    [Description("Returns true if the Character's left arm is available to start a new action")]

    [Category("Characters/Busy/Is Left Arm Available")]

    [Keywords("Occupied", "Available", "Free", "Doing", "Hand", "Finger")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Red)]
    
    [Serializable]
    public class ConditionCharacterBusyLeftArm : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Left Arm Available {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && !character.Busy.IsArmLeftBusy;
        }
    }
}
