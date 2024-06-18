using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Percentage")]
    [Category("Math/Arithmetic/Percentage")]
    
    [Image(typeof(IconPercent), ColorTheme.Type.TextNormal)]
    [Description("The constant percentage of a value")]

    [Keywords("Float", "Decimal", "Double", "Ratio", "Part", "Percent")]
    
    [Serializable]
    public class GetDecimalMathPercent : PropertyTypeGetDecimal
    {
        [SerializeField] private PropertyGetDecimal m_Number = new PropertyGetDecimal();
        [SerializeField] [Range(0f, 1f)] private float m_Ratio = 0.75f;

        public override double Get(Args args)
        {
            return this.m_Number.Get(args) * this.m_Ratio;
        }

        public static PropertyGetDecimal Create(float percent)
        {
            GetDecimalMathPercent instance = new GetDecimalMathPercent
            {
                m_Number = GetDecimalConstantOne.Create,
                m_Ratio = percent
            };
            
            return new PropertyGetDecimal(instance);
        }

        public override string String
        {
            get
            {
                float percent = this.m_Ratio * 100f;
                return $"({percent:#0}% of {this.m_Number})";
            }
        }
    }
}