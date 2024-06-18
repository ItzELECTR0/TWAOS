using System;
using System.Globalization;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Number Decimal")]
    [Category("Values/Number Decimal")]
    
    [Image(typeof(IconNumber), ColorTheme.Type.Blue)]
    [Description("A numeric value")]

    [Keywords("String", "Value", "Number", "Decimal", "Float", "Double")]
    
    [Serializable]
    public class GetStringValueDecimal : PropertyTypeGetString
    {
        [SerializeField] private PropertyGetDecimal m_Value = GetDecimalDecimal.Create(0f);
        [SerializeField] private string m_Format = string.Empty; 

        public override string Get(Args args)
        {
            double value = this.m_Value.Get(args);
            return string.IsNullOrEmpty(this.m_Format)
                ? value.ToString(CultureInfo.InvariantCulture)
                : value.ToString(this.m_Format);
        }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringValueDecimal()
        );

        public override string String => this.m_Value.ToString();
        
        public override string EditorValue => this.m_Value.EditorValue.ToString(CultureInfo.InvariantCulture);
    }
}