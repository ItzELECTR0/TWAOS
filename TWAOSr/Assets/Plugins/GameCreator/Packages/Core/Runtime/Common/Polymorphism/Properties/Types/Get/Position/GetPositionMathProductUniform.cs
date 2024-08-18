using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Multiply Decimal")]
    [Category("Math/Multiply Decimal")]
    
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.Green)]
    [Description("Calculates the product of a value to all axis of a position")]

    [Serializable]
    public class GetPositionMathProductUniform : PropertyTypeGetPosition
    {
        [SerializeField] private PropertyGetPosition m_Position = GetPositionSelf.Create();
        [SerializeField] private PropertyGetDecimal m_Scale = GetDecimalDecimal.Create(2);

        public override Vector3 Get(Args args)
        {
            Vector3 position = this.m_Position.Get(args);
            float scale = (float) this.m_Scale.Get(args);
            
            return position * scale;
        }

        public static PropertyGetPosition Create() => new PropertyGetPosition(
            new GetPositionMathProductUniform()
        );

        public override string String => $"({this.m_Position} * {this.m_Scale})";
    }
}