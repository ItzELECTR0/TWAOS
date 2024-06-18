using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Decimal")]
    [Description("Returns true if a comparison between two decimal values is satisfied")]

    [Category("Math/Arithmetic/Compare Decimal")]
    
    [Parameter("Value", "The decimal value that is being compared")]
    [Parameter("Comparison", "The comparison operation performed between both values")]
    [Parameter("Compare To", "The decimal value that is compared against")]
    
    [Keywords("Number", "Float", "Comma", "Equals", "Different", "Bigger", "Greater", "Larger", "Smaller")]
    [Image(typeof(IconNumber), ColorTheme.Type.Blue)]
    [Serializable]
    public class ConditionMathCompareDecimals : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertyGetDecimal m_Value = new PropertyGetDecimal(0f);

        [SerializeField] 
        private CompareDouble m_CompareTo = new CompareDouble();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Value} {this.m_CompareTo}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            double value = this.m_Value.Get(args);
            return this.m_CompareTo.Match(value, args);
        }
    }
}
