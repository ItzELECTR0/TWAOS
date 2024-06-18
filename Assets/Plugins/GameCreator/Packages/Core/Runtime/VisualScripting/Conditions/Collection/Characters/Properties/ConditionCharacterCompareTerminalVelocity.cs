using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Terminal Velocity")]
    [Description("Returns true if the comparison between a number and the Character's terminal velocity is satisfied")]

    [Category("Characters/Properties/Terminal Velocity")]

    [Keywords("Max", "Fall", "Vertical", "Down")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionCharacterCompareTerminalVelocity : TConditionCharacter
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private CompareDouble m_Comparison = new CompareDouble(-52);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Terminal Velocity of {this.m_Character} {this.m_Comparison}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return false;

            return this.m_Comparison.Match(character.Motion.TerminalVelocity, args);
        }
    }
}
