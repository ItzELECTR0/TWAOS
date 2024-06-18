using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Integer")]
    [Description("Returns true if a comparison between two integer values is satisfied")]

    [Category("Math/Arithmetic/Compare Integer")]
    
    [Parameter("Value", "The integer value that is being compared")]
    [Parameter("Comparison", "The comparison operation performed between both values")]
    [Parameter("Compare To", "The integer value that is compared against")]
    
    [Keywords("Number", "Whole", "Equals", "Different", "Bigger", "Greater", "Larger", "Smaller")]
    [Image(typeof(IconNumber), ColorTheme.Type.Blue)]
    [Serializable]
    public class ConditionMathCompareIntegers : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertyGetInteger m_Value = new PropertyGetInteger(0);

        [SerializeField] 
        private CompareInteger m_CompareTo = new CompareInteger();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Value} {this.m_CompareTo}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            int value = (int) this.m_Value.Get(args);
            return this.m_CompareTo.Match(value, args);
        }
    }
}
