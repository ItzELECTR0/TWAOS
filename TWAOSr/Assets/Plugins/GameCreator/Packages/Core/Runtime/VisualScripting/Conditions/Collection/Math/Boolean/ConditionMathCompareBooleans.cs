using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Bool")]
    [Description("Returns true if a comparison between two boolean values is satisfied")]

    [Category("Math/Boolean/Compare Boolean")]
    
    [Parameter("Value", "The boolean value that is being compared")]
    [Parameter("Comparison", "The comparison operation performed between both values")]
    [Parameter("Compare To", "The boolean value that is compared against")]
    
    [Keywords("Boolean")]
    
    [Image(typeof(IconToggleOn), ColorTheme.Type.Yellow)]
    [Serializable]
    public class ConditionMathCompareBooleans : Condition
    {
        private enum Comparison
        {
            Equals,
            Different
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertyGetBool m_Value = new PropertyGetBool(true);
        
        [SerializeField] 
        private Comparison m_Comparison = Comparison.Equals;
        
        [SerializeField] 
        private PropertyGetBool m_CompareTo = GetBoolTrue.Create;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => string.Format(
            "{0} {1} {2}",
            this.m_Value,
            this.m_Comparison switch
            {
                Comparison.Equals => "=",
                Comparison.Different => "≠",
                _ => string.Empty
            }, 
            this.m_CompareTo
        );

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override bool Run(Args args)
        {
            bool a = this.m_Value.Get(args);
            bool b = this.m_CompareTo.Get(args);

            return this.m_Comparison switch
            {
                Comparison.Equals => a == b,
                Comparison.Different => a != b,
                _ => throw new ArgumentOutOfRangeException($"Boolean Comparison '{this.m_Comparison}' not found")
            };
        }
    }
}
