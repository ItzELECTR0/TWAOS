using System;
using System.Globalization;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Decimal")]
    [Category("Decimal")]
    
    [Image(typeof(IconNumber), ColorTheme.Type.TextNormal)]
    [Description("A constant decimal number")]

    [Keywords("Float", "Decimal", "Double")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetDecimalDecimal : PropertyTypeGetDecimal
    {
        private const string STRING_FMT = "0.##";
        
        [SerializeField] protected double m_Value;

        public override double Get(Args args) => this.m_Value;
        public override double Get(GameObject gameObject) => this.m_Value;

        public override double EditorValue => this.m_Value;

        public GetDecimalDecimal() : base()
        { }
        
        public GetDecimalDecimal(double value) : this()
        {
            this.m_Value = value;
        }

        public GetDecimalDecimal(float value) : this()
        {
            this.m_Value = value;
        }

        public static PropertyGetDecimal Create(float value = 0f) => new PropertyGetDecimal(
            new GetDecimalDecimal(value)
        );
        
        public static PropertyGetDecimal Create(double value = 0) => new PropertyGetDecimal(
            new GetDecimalDecimal(value)
        );

        public override string String => this.m_Value.ToString(
            STRING_FMT, 
            CultureInfo.InvariantCulture
        );
    }
}