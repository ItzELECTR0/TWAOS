using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Compare Distance Flat")]
    [Description("Returns true if a comparison of the flat XZ distance between two points is satisfied")]

    [Category("Math/Geometry/Compare Distance Flat")]
    
    [Parameter("Point A", "The first operand that represents a point in space")]
    [Parameter("Point B", "The second operand that represents a point in space")]
    [Parameter("Comparison", "The comparison operation performed between both values")]
    [Parameter("Distance", "The distance value compared against")]
    
    [Keywords("Position", "Vector", "Magnitude", "Length")]
    [Keywords("Equals", "Different", "Greater", "Larger", "Smaller")]
    
    [Image(typeof(IconCompass), ColorTheme.Type.Green, typeof(OverlayBar))]
    [Serializable]
    public class ConditionMathCompareFlatDistance : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetPosition m_PointA = new PropertyGetPosition();
        [SerializeField] private PropertyGetPosition m_PointB = new PropertyGetPosition();

        [SerializeField] private CompareDouble m_Distance = new CompareDouble();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => string.Format(
            "Flat Distance [{0}, {1}] {2}", 
            this.m_PointA,
            this.m_PointB,
            this.m_Distance
        );
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Vector2 a = this.m_PointA.Get(args).XZ();
            Vector2 b = this.m_PointB.Get(args).XZ();

            float distance = Vector2.Distance(a, b);
            return this.m_Distance.Match(distance, args);
        }
    }
}
