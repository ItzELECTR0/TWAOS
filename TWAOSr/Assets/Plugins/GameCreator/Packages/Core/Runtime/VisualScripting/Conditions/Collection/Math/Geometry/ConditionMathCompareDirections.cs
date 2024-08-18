using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Direction")]
    [Description("Returns true if a comparison between two direction values is satisfied")]

    [Category("Math/Geometry/Compare Direction")]
    
    [Parameter("Value", "The direction value that is being compared")]
    [Parameter("Comparison", "The comparison operation performed between both values")]
    [Parameter("Compare To", "The direction value that is compared against")]
    
    [Keywords("Towards", "Vector", "Magnitude", "Length")]
    [Keywords("Equals", "Different", "Greater", "Larger", "Smaller")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    [Serializable]
    public class ConditionMathCompareDirections : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertyGetDirection m_Value = new PropertyGetDirection();

        [SerializeField] 
        private CompareDirection m_CompareTo = new CompareDirection();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Value} {this.m_CompareTo}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Vector3 value = this.m_Value.Get(args);
            return this.m_CompareTo.Match(value, args);
        }
    }
}
