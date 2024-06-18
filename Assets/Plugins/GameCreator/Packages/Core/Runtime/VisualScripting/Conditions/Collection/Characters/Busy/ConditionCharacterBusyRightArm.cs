using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Right Arm Available")]
    [Description("Returns true if the Character's right arm is available to start a new action")]

    [Category("Characters/Busy/Is Right Arm Available")]

    [Keywords("Occupied", "Available", "Free", "Doing", "Hand", "Finger")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Red)]
    
    [Serializable]
    public class ConditionCharacterBusyRightArm : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Right Arm Available {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && !character.Busy.IsArmRightBusy;
        }
    }
}
