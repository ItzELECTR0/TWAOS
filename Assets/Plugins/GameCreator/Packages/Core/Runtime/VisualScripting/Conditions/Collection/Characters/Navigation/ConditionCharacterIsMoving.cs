using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Moving")]
    [Description("Returns true if the Character is currently in an active moving phase")]

    [Category("Characters/Navigation/Is Moving")]

    [Keywords("Translate", "Towards", "Destination", "Target", "Follow", "Walk", "Run")]
    
    [Image(typeof(IconCharacterRun), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    [Serializable]
    public class ConditionCharacterIsMoving : TConditionCharacter
    {
        private const float MOVE_THRESHOLD = 0.1f;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Moving {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return false;

            return character.Driver.WorldMoveDirection.magnitude > MOVE_THRESHOLD;
        }
    }
}
