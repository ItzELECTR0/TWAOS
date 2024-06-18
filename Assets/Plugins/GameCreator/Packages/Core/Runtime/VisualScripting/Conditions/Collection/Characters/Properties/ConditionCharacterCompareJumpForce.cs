using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Jump Force")]
    [Description("Returns true if the comparison between a number and the Character's jump force is satisfied")]

    [Category("Characters/Properties/Jump Force")]

    [Keywords("Hop", "Leap")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionCharacterCompareJumpForce : TConditionCharacter
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private CompareDouble m_Comparison = new CompareDouble(8);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Jump Force of {this.m_Character} {this.m_Comparison}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && this.m_Comparison.Match(character.Motion.JumpForce, args);
        }
    }
}
