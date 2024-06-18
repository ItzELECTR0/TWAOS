using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Speed")]
    [Description("Returns true if the comparison between a number and the Character's speed is satisfied")]

    [Category("Characters/Properties/Compare Speed")]

    [Keywords("Velocity", "Travel", "Movement", "Walk", "Run", "Step")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionCharacterCompareSpeed : TConditionCharacter
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private CompareDouble m_Comparison = new CompareDouble(4f);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Speed of {this.m_Character} {this.m_Comparison}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && this.m_Comparison.Match(character.Motion.LinearSpeed, args);
        }
    }
}
