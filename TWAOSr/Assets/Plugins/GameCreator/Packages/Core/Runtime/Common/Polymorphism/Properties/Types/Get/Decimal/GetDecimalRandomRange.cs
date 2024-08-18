using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Random Range")]
    [Category("Random/Random Range")]
    
    [Image(typeof(IconDice), ColorTheme.Type.TextNormal)]
    [Description("A random decimal number between two values (range is inclusive)")]
    
    [Parameter("Min Value", "The smallest value the random operation returns")]
    [Parameter("Max Value", "The largest value the random operation returns")]

    [Keywords("Float", "Decimal", "Double")]
    [Serializable]
    public class GetDecimalRandomRange : PropertyTypeGetDecimal
    {
        [SerializeField] private PropertyGetDecimal m_MinValue;
        [SerializeField] private PropertyGetDecimal m_MaxValue;
        
        public override double Get(Args args)
        {
            float minValue = (float) this.m_MinValue.Get(args);
            float maxValue = (float) this.m_MaxValue.Get(args);

            return UnityEngine.Random.Range(minValue, maxValue);
        }

        public GetDecimalRandomRange()
        {
            this.m_MinValue = GetDecimalConstantZero.Create;
            this.m_MaxValue = GetDecimalConstantTwo.Create;
        }
        
        public GetDecimalRandomRange(PropertyGetDecimal min, PropertyGetDecimal max)
        {
            this.m_MinValue = min;
            this.m_MaxValue = max;
        }
        
        public static PropertyGetDecimal Create()
        {
            return new PropertyGetDecimal(new GetDecimalRandomRange());
        }
        
        public static PropertyGetDecimal Create(PropertyGetDecimal min, PropertyGetDecimal max)
        {
            return new PropertyGetDecimal(new GetDecimalRandomRange(min, max));
        }

        public override string String => $"Random({this.m_MinValue}, {this.m_MaxValue})";
    }
}