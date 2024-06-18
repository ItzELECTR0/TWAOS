using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Mass")]
    [Description("Returns true if the comparison between a number and the Character's mass is satisfied")]

    [Category("Characters/Properties/Compare Mass")]

    [Keywords("Weight")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionCharacterCompareMass : TConditionCharacter
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private CompareDouble m_Comparison = new CompareDouble(75f);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Mass of {this.m_Character} {this.m_Comparison}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && this.m_Comparison.Match(character.Motion.Mass, args);
        }
    }
}
