using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Multiply Positions")]
    [Category("Math/Multiply Positions")]
    
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.Green)]
    [Description("Calculates the product of two positions component by component")]

    [Serializable]
    public class GetPositionMathProductScale : PropertyTypeGetPosition
    {
        [SerializeField] private PropertyGetPosition m_Position1 = GetPositionSelf.Create();
        [SerializeField] private PropertyGetPosition m_Position2 = GetPositionTarget.Create();

        public override Vector3 Get(Args args)
        {
            Vector3 position1 = this.m_Position1.Get(args);
            Vector3 position2 = this.m_Position2.Get(args);
            
            return Vector3.Scale(position1, position2);
        }

        public static PropertyGetPosition Create() => new PropertyGetPosition(
            new GetPositionMathProductScale()
        );

        public override string String => $"({this.m_Position1} * {this.m_Position2})";
    }
}