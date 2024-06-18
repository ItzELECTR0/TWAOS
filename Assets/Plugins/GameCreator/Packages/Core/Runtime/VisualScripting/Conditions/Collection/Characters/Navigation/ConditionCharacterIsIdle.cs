using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Is Idle")]
    [Description("Returns true if the Character is not moving")]

    [Category("Characters/Navigation/Is Idle")]

    [Keywords("Stay", "Quiet", "Still")]
    
    [Image(typeof(IconCharacterIdle), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionCharacterIsIdle : TConditionCharacter
    {
        private const float MOVE_THRESHOLD = 0.1f;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"is Idle {this.m_Character}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return false;

            return character.Driver.WorldMoveDirection.magnitude <= MOVE_THRESHOLD;
        }
    }
}
