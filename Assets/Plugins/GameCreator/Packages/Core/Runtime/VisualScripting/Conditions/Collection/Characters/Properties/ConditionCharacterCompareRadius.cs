using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Radius")]
    [Description("Returns true if the comparison between a number and the Character's radius is satisfied")]

    [Category("Characters/Properties/Compare Radius")]

    [Keywords("Diameter", "Width", "Fat", "Skin", "Space")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionCharacterCompareRadius : TConditionCharacter
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private CompareDouble m_Comparison = new CompareDouble(0.5f);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Radius of {this.m_Character} {this.m_Comparison}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && this.m_Comparison.Match(character.Motion.Radius, args);
        }
    }
}
