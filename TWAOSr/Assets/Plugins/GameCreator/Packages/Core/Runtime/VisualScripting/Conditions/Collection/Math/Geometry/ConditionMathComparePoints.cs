using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Point")]
    [Description("Returns true if a comparison between two points in space is satisfied")]

    [Category("Math/Geometry/Compare Point")]
    
    [Parameter("Value", "The point in space that is being compared")]
    [Parameter("Comparison", "The comparison operation performed between both values")]
    [Parameter("Compare To", "The point in space that is compared against")]
    
    [Keywords("Position", "Vector", "Magnitude", "Length")]
    [Keywords("Equals", "Different", "Greater", "Larger", "Smaller")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionMathComparePoints : Condition
    {
        private enum Comparison
        {
            Equals,
        }
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private PropertyGetPosition m_Value = new PropertyGetPosition();

        [SerializeField] 
        private Comparison m_Comparison = Comparison.Equals;
        
        [SerializeField] 
        private PropertyGetPosition m_CompareTo = new PropertyGetPosition();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => string.Format(
            "{0} {1} {2}", 
            this.m_Value,
            this.m_Comparison switch
            {
                Comparison.Equals => "=",
                _ => string.Empty
            }, 
            this.m_CompareTo
        );
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Vector3 a = this.m_Value.Get(args);
            Vector3 b = this.m_CompareTo.Get(args);

            return this.m_Comparison switch
            {
                Comparison.Equals => a == b,
                _ => throw new ArgumentOutOfRangeException($"Point Comparison '{this.m_Comparison}' not found")
            };
        }
    }
}
