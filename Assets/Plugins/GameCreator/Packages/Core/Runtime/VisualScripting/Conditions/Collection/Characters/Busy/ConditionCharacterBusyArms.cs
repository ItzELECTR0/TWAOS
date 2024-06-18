using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Are Arms Available")]
    [Description("Returns true if the Character's arms are available to start a new action")]

    [Category("Characters/Busy/Are Arms Available")]

    [Keywords("Occupied", "Available", "Free", "Doing", "Hand", "Finger")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Red)]
    
    [Serializable]
    public class ConditionCharacterBusyArms : TConditionCharacter
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Arms available {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && !character.Busy.AreArmsBusy;
        }
    }
}
