using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Height")]
    [Description("Returns true if the comparison between a number and the Character's height is satisfied")]

    [Category("Characters/Properties/Compare Height")]

    [Keywords("Length", "Long")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionCharacterCompareHeight : TConditionCharacter
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private CompareDouble m_Comparison = new CompareDouble(-9.81f);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"Height of {this.m_Character} {this.m_Comparison}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null && this.m_Comparison.Match(character.Motion.Height, args);
        }
    }
}
