using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Subtract Positions")]
    [Category("Math/Subtract Positions")]
    
    [Image(typeof(IconMinusCircle), ColorTheme.Type.Green)]
    [Description("Calculates the subtraction of two positions")]

    [Serializable]
    public class GetPositionMathSubtract : PropertyTypeGetPosition
    {
        [SerializeField] private PropertyGetPosition m_Position1 = GetPositionSelf.Create();
        [SerializeField] private PropertyGetPosition m_Position2 = GetPositionTarget.Create();

        public override Vector3 Get(Args args)
        {
            Vector3 position1 = this.m_Position1.Get(args);
            Vector3 position2 = this.m_Position2.Get(args);
            
            return position1 - position2;
        }

        public static PropertyGetPosition Create() => new PropertyGetPosition(
            new GetPositionMathSubtract()
        );

        public override string String => $"({this.m_Position1} - {this.m_Position2})";
    }
}